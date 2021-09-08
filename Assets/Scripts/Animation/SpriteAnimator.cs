using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public SpriteAnimation[] animations;
    private SpriteAnimation _currentAnimation;
    public SpriteAnimation CurrentAnimation
    {
        get => _currentAnimation;
    }
    
    public bool Playing { get => _currentAnimation.Playing; }

    public delegate void FinishedAnimation(string animName);
    public event FinishedAnimation AnimationFinished;
    protected virtual void OnAnimationFinish() => AnimationFinished?.Invoke(_currentAnimation.animationName);

    private SpriteRenderer _spriteRend;
    private void Awake()
    {
        _spriteRend ??= GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Play(animations.First().animationName);
    }

    private void Update()
    {
        if (_currentAnimation != null && Playing) _currentAnimation.Update();
    }

    public void Play(string animName)
    {
        var animation = animations.First(anim => anim.animationName == animName);
        if (animation != null)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.AnimationFinished -= CurrentAnimationFinished;
                _currentAnimation.FrameUpdated -= OnFrameUpdate;
            }
            _currentAnimation = animation;
            _currentAnimation.AnimationFinished += CurrentAnimationFinished;
            _currentAnimation.FrameUpdated += OnFrameUpdate;
            _currentAnimation.Play();
        }
        else
            Debug.LogError($"Animation {animName} does not exist in this animator.");
    }

    public void PlayBackwards(string animName)
    {
        var animation = animations.First(anim => anim.animationName == animName);
        if (animation != null)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.AnimationFinished -= CurrentAnimationFinished;
                _currentAnimation.FrameUpdated -= OnFrameUpdate;
            }
            _currentAnimation = animation;
            _currentAnimation.AnimationFinished += CurrentAnimationFinished;
            _currentAnimation.FrameUpdated += OnFrameUpdate;
            _currentAnimation.PlayBackwards();
        }
        else
            Debug.LogError($"Animation {animName} does not exist in this animator.");
    }

    public void PlayFromFrame(string animName, int startFrame)
    {
        var animation = animations.First(anim => anim.animationName == animName);
        if (animation != null)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.AnimationFinished -= CurrentAnimationFinished;
                _currentAnimation.FrameUpdated -= OnFrameUpdate;
            }
            _currentAnimation = animation;
            _currentAnimation.AnimationFinished += CurrentAnimationFinished;
            _currentAnimation.FrameUpdated += OnFrameUpdate;
            _currentAnimation.PlayFromFrame(startFrame);
        }
        else
            Debug.LogError($"Animation {animName} does not exist in this animator.");
    }

    public void PlayFromFrameBackwards(string animName, int startFrame)
    {
        var animation = animations.First(anim => anim.animationName == animName);
        if (animation != null)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.AnimationFinished -= CurrentAnimationFinished;
                _currentAnimation.FrameUpdated -= OnFrameUpdate;
            }
            _currentAnimation = animation;
            _currentAnimation.AnimationFinished += CurrentAnimationFinished;
            _currentAnimation.FrameUpdated += OnFrameUpdate;
            _currentAnimation.PlayFromFrameBackwards(startFrame);
        }
        else
            Debug.LogError($"Animation {animName} does not exist in this animator.");
    }

    private void CurrentAnimationFinished(string animName) => OnAnimationFinish();

    public void Stop()
    {
        _currentAnimation.FrameUpdated -= OnFrameUpdate;
        _currentAnimation.Stop();
        _currentAnimation = null;
    }

    private void OnFrameUpdate(int frameNumber)
    {
        _spriteRend.sprite = _currentAnimation.frames[frameNumber];
    }
}