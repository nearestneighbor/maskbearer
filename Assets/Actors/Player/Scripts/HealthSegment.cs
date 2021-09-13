using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class HealthSegment
{
    public delegate void SegmentDepleted(HealthSegment healthSegment);
    public event SegmentDepleted Depleted;
    protected virtual void OnDeplete() => Depleted?.Invoke(this);

    public delegate void SegmentFilled(HealthSegment healthSegment);
    public event SegmentFilled Filled;
    protected virtual void OnFill() => Filled?.Invoke(this);

    /// <summary>
    /// The maxmimum size of the segment.
    /// </summary>
    [SerializeField]
    public float maxSegmentSize = 200;
    /// <summary>
    /// The current size of the segment.
    /// </summary>
    [SerializeField]
    private float _currentSegmentSize = 200;
    public float CurrentSegmentSize => _currentSegmentSize;
    /// <summary>
    /// The index of this health segment in the player's total health.
    /// </summary>
    [SerializeField]
    public int segmentIndex;
    /// <summary>
    /// Whether this health segment is currently draining.
    /// </summary>
    [SerializeField]
    private bool _draining;
    public bool Draining => _draining;
    /// <summary>
    /// The rate at which the segment drains.
    /// </summary>
    [SerializeField]
    public float DrainRate = 10;
    public bool AllDraining => PlayerData.Instance.HealthSegments.All(segment => segment.Draining || segment.Empty);
    /// <summary>
    /// The rate at which the segment drains when all other segments are also draining.
    /// </summary>
    [SerializeField]
    public float DrainRateAll = 20;
    /// <summary>
    /// Whether this health segment is full.
    /// </summary>
    public bool Full => _currentSegmentSize >= maxSegmentSize;
    /// <summary>
    /// Whether this health segment is being restored.
    /// </summary>
    private bool _restoring;
    public bool Restoring => _restoring;
    /// <summary>
    /// The rate at which the segment is restored.
    /// </summary>
    [SerializeField]
    public float RestoreRate = 25;
    /// <summary>
    /// Whether this health segment is empty.
    /// </summary>
    public bool Empty => _currentSegmentSize <= 0;

    public void UpdateSegment(float deltaTime)
    {
        if (_draining)
        {
            _currentSegmentSize -= (AllDraining ? DrainRateAll : DrainRate) * deltaTime;
            if (Empty)
            {
                OnDeplete();
                _currentSegmentSize = 0;
                _draining = false;
            }
        }
        else if (_restoring)
        {
            _currentSegmentSize += RestoreRate * deltaTime;
            if (Full)
            {
                OnFill();
                _currentSegmentSize = maxSegmentSize;
                _restoring = false;
            }
        }
    }

    /// <summary>
    /// Start draining this health segment.
    /// </summary>
    public void StartDraining()
    {
        _draining = true;
        _restoring = false;
    }

    /// <summary>
    /// Instantly and completely drain this health segment.
    /// </summary>
    public void InstantlyDrain()
    {
        _draining = _restoring = false;
        _currentSegmentSize = 0;
    }

    /// <summary>
    /// Start restoring this health segment.
    /// </summary>
    public void StartRestoring()
    {
        _draining = false;
        _restoring = true;
    }

    /// <summary>
    /// Instantly and completely restore this health segment.
    /// </summary>
    public void InstantlyRestore()
    {
        _draining = _restoring = false;
        _currentSegmentSize = maxSegmentSize;
    }

    /// <summary>
    /// Stop draining and restoring this health segment.
    /// </summary>
    public void StopDrainingAndRestoring()
    {
        _draining = _restoring = false;
    }

    /// <summary>
    /// Increase the size of this health segment.
    /// </summary>
    /// <param name="growAmount">The amount to increase the health segment size by.</param>
    public void GrowSegment(float growAmount)
    {
        maxSegmentSize += growAmount;
        InstantlyRestore();
    }

    /// <summary>
    /// Decrease the size of this health segment.
    /// </summary>
    /// <param name="shrinkAmount">The amount to decrease the health segment size by.</param>
    public void ShrinkSegment(float shrinkAmount)
    {
        maxSegmentSize -= shrinkAmount;
    }
}
