using System;
using UnityEngine;

[Serializable]
public class HealthSegment
{
    /// <summary>
    /// The maxmimum size of the segment.
    /// </summary>
    [SerializeField]
    public float maxSegmentSize = 100;
    /// <summary>
    /// The current size of the segment.
    /// </summary>
    [SerializeField]
    private float _currentSegmentSize = 100;
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
    public float DrainRate = 1;
    /// <summary>
    /// Whether this health segment is full/not draining.
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
    public float RestoreRate = 1;
    /// <summary>
    /// Whether this health segment is empty.
    /// </summary>
    public bool Empty => _currentSegmentSize <= 0;

    public void UpdateSegment(float deltaTime)
    {
        if (_draining && !Empty)
            _currentSegmentSize -= DrainRate;
        else if (_restoring && !Full)
            _currentSegmentSize += RestoreRate;
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
        _draining = false;
        _restoring = false;
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
        _draining = false;
        _restoring = false;
        _currentSegmentSize = maxSegmentSize;
    }
}
