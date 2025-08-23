using System;
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
    public class SpriteAnimation
    {
        public readonly Sprite[] Frames;
        public readonly float[] FramesPerSecond;

        public SpriteAnimation(Sprite[] frames, float[] framesPerSeconds)
        {
            Frames = frames;
            FramesPerSecond = framesPerSeconds;
        }

        public SpriteAnimation(Sprite[] frames, float framesPerSecond)
        {
            Frames = frames;
            FramesPerSecond = new float[frames.Length];
            Array.Fill(FramesPerSecond, framesPerSecond);
        }
    }
}