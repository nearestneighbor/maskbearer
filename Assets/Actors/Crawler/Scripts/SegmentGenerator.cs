using PathCreation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject segmentPrefab;
    public int numberOfSegments = 8;
    public float pathFollowSpeed = 3;
    public float segmentOffset = 0.65f;
    private Transform _pathsParent;
    private GameObject _pathPrefab;
    private Transform _segmentsParent;
    private List<List<Segment>> _segmentGroups = new List<List<Segment>>();

    private void Awake()
    {
        _pathsParent = transform.Find("Paths");
        _pathPrefab = _pathsParent.GetChild(0).gameObject;
        _segmentsParent = transform.Find("Segments");
    }

    private void Start()
    {
        PreInstantiatePaths();
        PreInstantiateSegments();
    }

    private void PreInstantiatePaths()
    {
        for (int i = 0; i < numberOfSegments - 1; i++)
            Instantiate(_pathPrefab, _pathsParent);
    }

    private void PreInstantiateSegments()
    {
        var startSegments = new List<Segment>();
        for (int i = 0; i < numberOfSegments; i++)
        {
            var segmentObj = Instantiate(segmentPrefab, _segmentsParent);
            var segment = segmentObj.GetComponent<Segment>();
            segment.pathCreator = _pathsParent.GetChild(i).GetComponent<PathCreator>();
            segment.speed = pathFollowSpeed;
            segment.offset = i * segmentOffset;
            segment.transform.Find("Sprite").GetComponent<SpriteAnimator>().Play((i % 2 == 0) ? "CrawlA" : "CrawlB");
            var deathManager = segment.GetComponentInChildren<DeathManager>(true);
            // deathManager.Death += CheckSegments;
            startSegments.Add(segment);
        }

        _segmentGroups.Add(startSegments);
    }

    private void CheckSegments()
    {
        if (_segmentsParent.childCount <= 0) Destroy(gameObject);
        for (int i = 0; i < _segmentGroups.Count; i++)
        {
            var segmentGroup = _segmentGroups[i];
            for (int j = 0; j < segmentGroup.Count; j++)
            {
                var segment = segmentGroup[j];
                if (segment.GetComponent<HealthManager>().IsDead)
                    SplitSegmentGroup(i, j);
            }
        }
    }

    private void SplitSegmentGroup(int segmentGroupIndex, int segmentIndex)
    {
        var segmentGroup = _segmentGroups[segmentGroupIndex];

        if (segmentGroup.Count > 0 && segmentIndex > 0 && segmentIndex < _segmentGroups[segmentGroupIndex].Count)
        {
            Debug.Log($"Splitting segment group at ({segmentGroupIndex}, {segmentIndex})");

            List<Segment> segmentGroup1 = segmentGroup.GetRange(0, segmentIndex);
            List<Segment> segmentGroup2 = segmentGroup.GetRange(segmentIndex + 1, segmentGroup.Count - (segmentIndex + 1));

            if (segmentGroup1.Count > 0)
            {
                _segmentGroups.Add(segmentGroup1);
                UpdateSegmentGroupPath(segmentGroup1);
            }

            if (segmentGroup2.Count > 0)
            {
                _segmentGroups.Add(segmentGroup2);
                UpdateSegmentGroupPath(segmentGroup2);
            }
        }

        _segmentGroups.Remove(segmentGroup);
    }

    private void UpdateSegmentGroupPath(List<Segment> segmentGroup)
    {
        var points = segmentGroup.Select(segment => segment.transform.position - transform.position).ToList();
        var offset = segmentGroup.Count * 0.25f;
        var x = Mathf.Clamp(Random.Range(-offset, offset), 1, 5);
        var y = Mathf.Clamp(Random.Range(-offset, offset), 1, 5);
        points.Add(points.Last() + new Vector3(x, y, 0));
        var newBezier = new BezierPath(points, true, PathSpace.xy);
        foreach (Segment segment in segmentGroup)
        {
            segment.pathCreator.bezierPath = newBezier;
            segment.pathCreator.TriggerPathUpdate();
        }
    }
}
