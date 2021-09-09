using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public SpriteAnimation[] animations;
    private SpriteAnimation _currentAnimation;
    public SpriteAnimation CurrentAnimation { get => _currentAnimation; }

    public bool IsPlaying { get => _currentAnimation.IsPlaying; }

    public delegate void FinishedAnimation(string animName);
    public event FinishedAnimation AnimationFinished;
    protected virtual void OnAnimationFinish() => AnimationFinished?.Invoke(_currentAnimation.animationName);

    private SpriteRenderer _spriteRend;
    private void Awake()
    {
        _spriteRend ??= GetComponent<SpriteRenderer>();
        animations = animations?.Select(animation => animation = Instantiate(animation)).ToArray();
    }

    private void Start()
    {
        Play(animations.First()?.animationName);
    }

    private void Update()
    {
        if (!IsPlaying) return;

        _currentAnimation?.UpdateTime(Time.deltaTime);
    }

    private void OnDisable()
    {
        _currentAnimation.Stop();
    }
    public void Play(string animName, int startFrame = 0, float timeScale = 1)
    {
        var animation = animations.First(anim => anim.animationName == animName);
        if (animation != null)
        {
            if (animation == _currentAnimation) return;
            if (_currentAnimation != null)
            {
                _currentAnimation.AnimationFinished -= CurrentAnimationFinished;
                _currentAnimation.FrameUpdated -= OnFrameUpdate;
            }
            _currentAnimation = animation;
            _currentAnimation.AnimationFinished += CurrentAnimationFinished;
            _currentAnimation.FrameUpdated += OnFrameUpdate;
            _currentAnimation.Play(startFrame, timeScale);
        }
        else
            Debug.LogError($"Animation {animName} does not exist in this animator.");
    }

    private void CurrentAnimationFinished(string animName) => OnAnimationFinish();

    public void Stop()
    {
        _currentAnimation.AnimationFinished -= CurrentAnimationFinished;
        _currentAnimation.FrameUpdated -= OnFrameUpdate;
        _currentAnimation.Stop();
        _currentAnimation = null;
    }

    private void OnFrameUpdate(int frameNumber)
    {
        _spriteRend.sprite = _currentAnimation.frames[frameNumber];
    }
}