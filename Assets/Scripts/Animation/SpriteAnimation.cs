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
    public int? loopStart = null;
    [SerializeField]
    public int fps;
    [SerializeField]
    public Sprite[] frames;

    private int _currentFrameNumber;
    public int CurrentFrame { get => _currentFrameNumber; }

    private bool _playing;
    public bool Playing { get => _playing; }

    private bool _backwards;

    public delegate void UpdatedFrame(int frameNumber);
    public event UpdatedFrame FrameUpdated;
    protected virtual void OnFrameUpdate() => FrameUpdated?.Invoke(_currentFrameNumber);

    public delegate void FinishedAnimation(string animName);
    public event FinishedAnimation AnimationFinished;
    protected virtual void OnAnimationFinish() => AnimationFinished?.Invoke(animationName);

    public float Duration
    {
        get => 1.0f / fps * frames.Length;
    }

    private float _frameTime;

    public SpriteAnimation(string animationName, bool looping, int loopStart, int fps, Sprite[] frames)
    {
        this.animationName = animationName;
        this.looping = looping;
        this.loopStart = loopStart;
        this.fps = fps;
        this.frames = frames;
    }

    public void Update()
    {
        if (!_playing) return;

        _frameTime += Time.deltaTime;
        if (_frameTime >= 1.0f / fps)
            UpdateFrame();
    }

    private void UpdateFrame()
    {
        OnFrameUpdate();
        _frameTime = 0;
        _currentFrameNumber += _backwards ? -1 : 1;
        if (!_backwards)
        {
            if (_currentFrameNumber >= frames.Length)
            {
                if (looping) _currentFrameNumber = loopStart.HasValue ? loopStart.Value : 0;
                OnAnimationFinish();
            }
        }
        else
        {
            if (_currentFrameNumber < 0)
            {
                if (looping) _currentFrameNumber = loopStart.HasValue ? loopStart.Value : frames.Length - 1;
                OnAnimationFinish();
            }
        }
    }

    public void Play()
    {
        PlayInternal();
    }

    public void PlayBackwards()
    {
        PlayInternal(0, true);
    }

    public void PlayFromFrame(int startFrame)
    {
        PlayInternal(startFrame);
    }

    public void PlayFromFrameBackwards(int startFrame)
    {
        PlayInternal(startFrame, true);
    }

    private void PlayInternal(int startFrame = 0, bool backwards = false)
    {
        _backwards = backwards;
        _playing = true;
        _currentFrameNumber = startFrame;
    }

    public void Stop()
    {
        _playing = false;
    }
}
