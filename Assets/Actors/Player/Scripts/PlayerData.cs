using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private static PlayerData _instance;
    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateNewPlayerData();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Contains all the health segments that the player currently has.
    /// </summary>
    [SerializeField]
    private List<HealthSegment> _healthSegments = new List<HealthSegment>();
    public HealthSegment[] HealthSegments => _healthSegments.ToArray();
    /// <summary>
    /// The current total number of health segments that the player has.
    /// </summary>
    public int NumMaxHealthSegments => _healthSegments.Count();
    /// <summary>
    /// The number of health segments that are draining.
    /// </summary>
    public int DrainingHealthSegments => _healthSegments.Count(segment => segment.Draining);
    /// <summary>
    /// The number of health segments that are empty.
    /// </summary>
    public int EmptyHealthSegments => _healthSegments.Count(segment => segment.Empty);
    /// <summary>
    /// The number of segments that are full.
    /// </summary>
    public int FullHealthSegments => _healthSegments.Count(segment => segment.Full);
    /// <summary>
    /// Whether all health segments are draining.
    /// </summary>
    public bool AllHealthSegmentsDraining => DrainingHealthSegments >= NumMaxHealthSegments;
    /// <summary>
    /// Whether the player is at full health with no segments draining.
    /// </summary>
    public bool FullHealth => FullHealthSegments >= NumMaxHealthSegments;
    /// <summary>
    /// Whether the player has no health remaining.
    /// </summary>
    public bool NoHealth => EmptyHealthSegments >= NumMaxHealthSegments;
    /// <summary>
    /// The current size of each segment.
    /// </summary>
    public float MaxHealthSegmentSize => _healthSegments.FirstOrDefault().maxSegmentSize;

    public static PlayerData CreateNewPlayerData()
    {
        Debug.Log("Creating new PlayerData instance.");
        _instance = new PlayerData();
        for (int i = 0; i < 5; i++)
        {
            var newSegment = new HealthSegment();
            newSegment.Filled += _instance.OnHealthSegmentFill;
            _instance._healthSegments.Add(newSegment);
        }
        return _instance;
    }

    /// <summary>
    /// Start draining health.
    /// </summary>
    /// <param name="damageAmount">The number of health segments to start draining.</param>
    public void StartDrainingHealth(int damageAmount)
    {
        if (AllHealthSegmentsDraining ||
            NoHealth)
            return;

        var lastFullSegment = _healthSegments.LastOrDefault(segment => segment.Full || segment.Restoring);
        if (lastFullSegment == null) return;
        var indexOfLastSegment = _healthSegments.IndexOf(lastFullSegment);
        _healthSegments.GetRange(indexOfLastSegment - damageAmount + 1, damageAmount).ForEach(segment => segment.StartDraining());
    }

    /// <summary>
    /// Instantly and completely drain an amount of health.
    /// </summary>
    /// <param name="damageAmount">The number of health segments to instantly drain.</param>
    public void InstantlyDrainHealth(int damageAmount)
    {
        if (AllHealthSegmentsDraining ||
            NoHealth)
            return;

        var lastFullSegment = _healthSegments.LastOrDefault(segment => segment.Full || segment.Draining);
        if (lastFullSegment == null) return;
        var indexOfLastSegment = _healthSegments.IndexOf(lastFullSegment);
        _healthSegments.GetRange(indexOfLastSegment - damageAmount + 1, damageAmount).ForEach(segment => segment.InstantlyDrain());
    }

    /// <summary>
    /// Start draining all health segments.
    /// </summary>
    public void StartDrainingAllHealth()
    {
        if (AllHealthSegmentsDraining ||
            NoHealth) 
            return;

        _healthSegments.ForEach(segment => { if (segment.Full) segment.StartDraining(); });
    }

    /// <summary>
    /// Instantly drain all health segments.
    /// </summary>
    public void InstantlyDrainAllHealth()
    {
        if (NoHealth) return;

        _healthSegments.ForEach(segment => { if (segment.Full) segment.InstantlyDrain(); });
    }

    /// <summary>
    /// Start restoring health.
    /// </summary>
    /// <param name="restoreAmount">The number of health segments to start restoring.</param>
    public void StartRestoringHealth(int restoreAmount)
    {
        if (FullHealth) return;

        var firstDrainingOrEmptySegment = _healthSegments.FirstOrDefault(segment => segment.Draining || segment.Empty);
        var indexOfDrainingOrEmptySegment = _healthSegments.IndexOf(firstDrainingOrEmptySegment);
        _healthSegments.GetRange(indexOfDrainingOrEmptySegment, restoreAmount).ForEach(segment => segment.StartRestoring());
    }

    /// <summary>
    /// Start restoring health to maximum.
    /// </summary>
    public void StartRestoringAllHealth()
    {
        if (FullHealth) return;

        _healthSegments.ForEach(segment => { if (segment.Draining || segment.Empty) segment.StartRestoring(); });
    }

    /// <summary>
    /// Stop restoring health.
    /// </summary>
    public void StopRestoringHealth()
    {
        var restoringSegment = _healthSegments.FirstOrDefault(segment => segment.Restoring);
        if (restoringSegment != null)
            restoringSegment.StartDraining();
    }

    /// <summary>
    /// Instantly restore health to maximum.
    /// </summary>
    public void InstantlyRestoreAllHealth()
    {
        if (FullHealth) return;

        _healthSegments.ForEach(segment => { if (segment.Draining || segment.Empty) segment.InstantlyRestore(); });
    }

    /// <summary>
    /// Instantly and completely restore an amount of health.
    /// </summary>
    /// <param name="restoreAmount">The number of health segments to instantly restore.</param>
    public void InstantlyRestoreHealth(int restoreAmount)
    {
        if (FullHealth) return;

        var lastDrainingOrEmptySegment = _healthSegments.LastOrDefault(segment => segment.Draining || segment.Empty);
        if (lastDrainingOrEmptySegment == null) return;
        var indexOfDrainingOrEmptySegment = _healthSegments.IndexOf(lastDrainingOrEmptySegment);
        _healthSegments.GetRange(indexOfDrainingOrEmptySegment, restoreAmount).ForEach(segment => segment.InstantlyRestore());
    }

    /// <summary>
    /// Add new health segment(s) to the player's total health.
    /// </summary>
    /// <param name="numSegments">The number of segments to add.</param>
    public void AddHealthSegment(int numSegments)
    {
        _healthSegments.ForEach(segment => segment.InstantlyRestore());
        for (int i = 0; i < numSegments; i++)
        {
            var referenceSegment = _healthSegments[0];
            var newSegment = new HealthSegment();
            newSegment.maxSegmentSize = referenceSegment.maxSegmentSize;
            newSegment.DrainRate = referenceSegment.DrainRate;
            newSegment.RestoreRate = referenceSegment.RestoreRate;
            newSegment.segmentIndex = _healthSegments.Count();
            newSegment.Filled += OnHealthSegmentFill;
            _healthSegments.Add(newSegment);
        }
    }

    /// <summary>
    /// Handles when a health segment is filled while the player is still focusing.
    /// </summary>
    /// <param name="healthSegment">The health segment that has just been filled.</param>
    private void OnHealthSegmentFill(HealthSegment healthSegment)
    {
        var segmentIndex = _healthSegments.IndexOf(healthSegment);
        if (segmentIndex >= _healthSegments.Count - 1) return;
        var nextSegment = _healthSegments[segmentIndex + 1];
        nextSegment.StopDrainingAndRestoring();
        nextSegment.StartRestoring();
    }

    /// <summary>
    /// Remove health segment(s) from the player's total health.
    /// </summary>
    /// <param name="numSegments">The number of segments to remove.</param>
    public void RemoveHealthSegment(int numSegments)
    {
        for (int i = _healthSegments.Count - numSegments - 1; i <= _healthSegments.Count; i++)
        {
            var segmentToRemove = _healthSegments[i];
            segmentToRemove.Filled -= OnHealthSegmentFill;
            _healthSegments.Remove(segmentToRemove);
        }
    }

    /// <summary>
    /// Increase the size of all health segments by an amount.
    /// </summary>
    /// <param name="growAmount">The amount to increase the size of all health segments by.</param>
    public void GrowHealthSegments(float growAmount)
    {
        _healthSegments.ForEach(segment => segment.GrowSegment(growAmount));
    }

    /// <summary>
    /// Decrease the size of all health segments by an amount.
    /// </summary>
    /// <param name="shrinkAmount">The amount to decrease the size of all health segments by.</param>
    public void ShrinkHealthSegments(float shrinkAmount)
    {
        _healthSegments.ForEach(segment => segment.ShrinkSegment(shrinkAmount));
    }
}
