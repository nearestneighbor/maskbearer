using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteAnimation", menuName = "Maskbearer/Create Sprite Animation", order = 1)]
[Serializable]
public class SpriteAnimation : ScriptableObject
{
    [SerializeField]
    public string animationName;
    [SerializeField]
    public bool looping;
    [SerializeField]
    public int loopStart;
    [SerializeField]
    public int fps;
    [SerializeField]
    public Sprite[] frames;

    private int _currentFrameNumber;
    public int CurrentFrame { get => _currentFrameNumber; }

    private bool _playing;
    public bool IsPlaying { get => _playing; }

    private float _timeScale;

    public delegate void UpdatedFrame(int frameNumber);
    public event UpdatedFrame FrameUpdated;
    protected virtual void OnFrameUpdate() => FrameUpdated?.Invoke(_currentFrameNumber);

    public delegate void FinishedAnimation(string animName);
    public event FinishedAnimation AnimationFinished;
    protected virtual void OnAnimationFinish() => AnimationFinished?.Invoke(animationName);

    public float Duration => 1.0f / fps * frames.Length;

    private float _frameTime;

    public SpriteAnimation(string animationName, bool looping, int loopStart, int fps, Sprite[] frames)
    {
        this.animationName = animationName;
        this.looping = looping;
        this.loopStart = loopStart;
        this.fps = fps;
        this.frames = frames;
    }

    public void UpdateTime(float deltaTime)
    {
        if (!_playing) return;

        _frameTime += deltaTime;
        if (_frameTime >= 1 / (fps * Mathf.Abs(_timeScale)))
            UpdateFrame();
    }

    private void UpdateFrame()
    {
        OnFrameUpdate();
        _frameTime = 0;
        if (_timeScale > 0)
        {
            _currentFrameNumber++;
            if (_currentFrameNumber >= frames.Length)
            {
                OnAnimationFinish();
                if (looping)
                {
                    _currentFrameNumber = Mathf.Clamp(loopStart, 0, frames.Length - 1);
                }
                else
                {
                    _currentFrameNumber = frames.Length - 1;
                    _playing = false;
                }
            }
        }
        else
        {
            _currentFrameNumber--;
            if (_currentFrameNumber < 0)
            {
                OnAnimationFinish();
                if (looping)
                {
                    _currentFrameNumber = Mathf.Clamp(loopStart, 0, frames.Length - 1);
                }
                else
                {
                    _currentFrameNumber = 0;
                    _playing = false;
                }
            }
        }
    }

    public void Play(int startFrame = 0, float timeScale = 1)
    {
        _currentFrameNumber = startFrame;
        _frameTime = 0;
        _playing = true;
        _timeScale = timeScale;
        OnFrameUpdate();
    }

    public void Stop()
    {
        _playing = false;
    }
}
