using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UCGUI
{
    /// <summary>
    /// Object containing all individual frames, as well as the time every frame will show for. <i>Assuming <see cref="SpriteAnimator.Speed"/> is 1</i>.
    /// <seealso cref="SpriteAnimator"/>
    /// </summary>
    /// <example>
    /// <code>
    /// // Every frame will appear for (1 / fps) seconds => here 0.1s each (assuming speed is 1).
    /// SpriteAnimation myAnimation1 = new SpriteAnimation(frameArray, 10);
    /// // Every frame potentially has its individual frame time, allowing for different pacing.
    /// SpriteAnimation myAnimation2 = new SpriteAnimation(frameArray, frameTimeArray);
    /// </code>
    /// </example>
    [System.Serializable]
    public partial class TextAnimation 
    {
        public readonly string[] Frames;
        public readonly float[] FramesPerSecond;

        public enum Mode
        {
            Word, Letter, Sentence
        }

        public TextAnimation(string[] frames, float[] framesPerSeconds)
        {
            Frames = frames;
            FramesPerSecond = framesPerSeconds;
        }

        public TextAnimation(string text, Mode mode, float framesPerSecond)
        {
            string pattern = mode.GetPattern();
            var matches = Regex.Matches(text, pattern);
            Frames = new string[matches.Count];

            for (int index = 0; index < matches.Count; index++)
            {
                Frames[index] = matches[index].Value;
            }
            
            FramesPerSecond = new float[Frames.Length];
            Array.Fill(FramesPerSecond, framesPerSecond);
        }
    }

    public static class ModeExtension
    {
        public static string GetPattern(this TextAnimation.Mode mode)
        {
            return mode switch
            {
                TextAnimation.Mode.Letter => @"\S|\s",
                TextAnimation.Mode.Word => @"\S+|\s",
                TextAnimation.Mode.Sentence => @"[^.!?]+[.!?]*\s*",
                _ => @""
            };
        }
    }
}