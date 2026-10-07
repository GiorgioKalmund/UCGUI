using TMPro;
using UCGUI.Support;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UCGUI
{
    /// <summary>
    /// Highly customizable Animator for any component that uses an <see cref="TextComponent"/>.
    /// Variables:
    /// <list type="bullet">
    /// <item><description><see cref="currentAnimation"/> - The current <see cref="SpriteAnimation"/>.</description></item>
    /// <item><description><see cref="currentState"/> - The current <see cref="State"/> the animator is in.</description></item>
    /// <item><description><see cref="speed"/> - The playback speed. Default value is 1f.</description></item>
    /// <item><description><see cref="animationType"/> - The <see cref="Type"/> of the animator.</description></item>
    /// </list>
    /// Functions:
    /// <list type="bullet">
    /// <item><description><see cref="Play"/> - Sets the <see cref="currentState"/> to <see cref="State.Running"/>.<i>If the animation is already <see cref="State.Completed"/>, will simply return.</i></description></item>
    /// <item><description><see cref="Pause"/> - Sets the <see cref="currentState"/> to <see cref="State.Paused"/>.</description></item>
    /// <item><description><see cref="ResetAnimation"/> - Resets the current animation by setting the state to <see cref="State.None"/>.</description></item>
    /// <item><description><see cref="RestartAnimation"/> - Resets and then plays the current animation.</description></item>
    /// </list>
    /// Events:
    /// <list type="bullet">
    /// <item><description><see cref="OnPing"/> - Event fired when <see cref="animationType"/> is set to <see cref="Type.PingPong"/> and the <see cref="PingPongPhase.Ping"/> is reached. <i>Does <b>NOT</b> fire at the initial "Ping" when the animation starts.</i></description></item>
    /// <item><description><see cref="OnPong"/> - Event fired when <see cref="animationType"/> is set to <see cref="Type.PingPong"/> and the <see cref="PingPongPhase.Pong"/> is reached.</description></item>
    /// </list>
    /// <seealso cref="TextAnimation"/>
    /// <seealso cref="TextComponent.AddAnimator"/>
    /// <para>
    /// Also implements <see cref="ICopyable{T}"/> which allows <see cref="ICopyable{T}.CopyFrom"/> and <see cref="ICopyable{T}.Copy"/>.
    /// </para>
    /// </summary>
    [RequireComponent(typeof(TextComponent), typeof(TextMeshProUGUI))]
    public class TextAnimator : BaseComponent, ICopyable<TextAnimator>
    {
        public enum State
        {
            None,
            Running,
            Paused,
            Completed
        }

        public enum Type
        {
            Loop,
            Once,
            PingPong
        }

        private enum PingPongPhase
        {
            Ping,
            Pong
        }
        
        public TextAnimation currentAnimation;
        public State currentState = State.None;
        public Type animationType = Type.Loop;
        private PingPongPhase _currentPingPongPhase = PingPongPhase.Ping;
        public float speed = 1f;
        public float ElapsedTime { get; private set; }
        public int currentFrame;
        public int AnimationLength => currentAnimation.frames.Length;

        private UnityEvent _onPing;
        public UnityEvent OnPing {
            get
            {
                _onPing ??= new UnityEvent();
                return _onPing;
            }
        }
        private UnityEvent _onPong;
        public UnityEvent OnPong
        {
            get
            {
                _onPong ??= new UnityEvent();
                return _onPong;
            }
        }

        public TextComponent Text { get; private set; }

        public void Play()
        {
            if (currentState == State.Completed)
                return;
            
            if (AnimationLength <= 1)
            {
                UCGUILogger.LogWarning(DisplayName + ": Animation has length 1. Will not start playing.");
                return;
            }
            
            currentState = State.Running;
        }

        public void Pause()
        {
            if (currentState == State.Completed)
                return;
            
            currentState = State.Paused;
        }

        private void Start()
        {
            Text = GetComponent<TextComponent>();
            SetFrame();
        }

        private void Update()
        {
            if (currentState != State.Running || currentAnimation == null)
                return;
            
            ElapsedTime += Time.deltaTime;
            
            float frameDuration = GetFrameTime();
            if (ElapsedTime >= frameDuration)
            {
                NextFrame();
            }
        }

        public TextAnimator CreateAnimation(TextAnimation anim, Type animationType, float speed = 1f)
        {
            currentAnimation = anim;
            this.animationType = animationType;
            this.speed = speed;
            return this;
        }

        public void NextFrame()
        {
            switch (animationType)
            {
                case Type.Loop:
                {
                    currentFrame++;
                    if (currentFrame == AnimationLength)
                        Text.Clear();
                    currentFrame %= AnimationLength;
                    SetFrame();
                    break;   
                }
                case Type.Once:
                {
                    currentFrame++;
                    if (currentFrame < AnimationLength)
                        SetFrame();
                    else
                    {
                        currentFrame = AnimationLength - 1;
                        currentState = State.Completed;
                    }
                    break;
                }
                case Type.PingPong:
                {
                    if (_currentPingPongPhase == PingPongPhase.Ping)
                    {
                        if (currentFrame == AnimationLength - 1)
                        {
                            _currentPingPongPhase = PingPongPhase.Pong;
                            _onPong?.Invoke();
                            PingPongPhaseBackFrame();
                            break;
                        }
                        currentFrame++;
                        SetFrame();
                    }
                    else
                    {
                        if (currentFrame == 0)
                        {
                            _currentPingPongPhase = PingPongPhase.Ping;
                            _onPing?.Invoke();
                            SetFrame();
                            break;
                        }
                        currentFrame--;
                        PingPongPhaseBackFrame();
                    }
                    break;
                }
            }
            
            ElapsedTime = 0;
        }

        public void ResetAnimation()
        {
            ClearText();
            currentFrame = 0;
            currentState = State.None;
            ElapsedTime = 0;
            _currentPingPongPhase = PingPongPhase.Ping;
            SetFrame();
        }

        public void ClearText()
        {
            Text.Clear();
        }

        public void Clear()
        {
            ResetAnimation();
            currentAnimation = null;
        }
        
        public void RestartAnimation()
        {
            ResetAnimation();
            Play();
        }

        private void SetFrame()
        {
            Text.GetTextMesh().text += currentAnimation.frames[currentFrame];
        }
        private void PingPongPhaseBackFrame()
        {
            string frameText = currentAnimation.frames[currentFrame];
            var textMesh = Text.GetTextMesh();
            textMesh.text = textMesh.text.Substring(0, textMesh.text.Length - frameText.Length);
        }

        float GetFrameTime()
        {
            if (currentFrame < 0 || currentFrame >= currentAnimation.framesPerSecond.Length)
                UCGUILogger.LogError(animationType + ": "+ currentAnimation.framesPerSecond.Length + $">> ({currentFrame})", this);
            return 1f / (currentAnimation.framesPerSecond[currentFrame] * speed);
        }

        public TextAnimator Configure(Type? type = null, float? speed = null)
        {
            if (type.HasValue)
                animationType = type.Value;
            if (speed.HasValue)
                this.speed = speed.Value;
            return this;
        }

        public new TextAnimator Copy(bool fullyCopyRect = true)
        {
            TextAnimator copyAnimator = this.BaseCopy(this);
            return copyAnimator.CopyFrom(this, fullyCopyRect);
        }

        public TextAnimator CopyFrom(TextAnimator other, bool fullyCopyRect = true)
        {
            base.CopyFrom(other, fullyCopyRect);
            DisplayName = other.DisplayName + " (Copy)";
            
            speed = other.speed;
            currentAnimation = other.currentAnimation;
            currentState = other.currentState;
            currentFrame = other.currentFrame;

            return this;
        }

        #if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (debugOptions.HasFlag(DebugOptions.TextOnly))
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.red;
                style.fontSize = 14; 
                
                Handles.Label(transform.position, $"Type: {animationType}\nState: {currentState}\nFrame: {currentFrame}\nSpeed: {speed}\nFrames: {currentAnimation.frames.Length}", style);
            }
        }
        #endif
    }
}