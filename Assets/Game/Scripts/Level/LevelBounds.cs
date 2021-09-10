using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelBounds : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _size;
    public Vector2Int Size => _size;

    [SerializeField] private GameObject _top;
    [SerializeField] private GameObject _right;
    [SerializeField] private GameObject _bottom;
    [SerializeField] private GameObject _left;

    // [SerializeField] private Wall _wallTop;
    // [SerializeField] private Wall _wallRight;
    // [SerializeField] private Wall _wallBottom;
    // [SerializeField] private Wall _wallLeft;

    private void OnEnable()
    {
        // _top = transform.Find("Top").gameObject;
        // _right = transform.Find("Right").gameObject;
        // _bottom = transform.Find("Bottom").gameObject;
        // _left = transform.Find("Left").gameObject;

        // _top.hideFlags = _right.hideFlags = _bottom.hideFlags = _left.hideFlags = HideFlags.None;
    }

    private void OnValidate()
    {
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        _size.x = Mathf.Max(1, _size.x);
        _size.y = Mathf.Max(1, _size.y);

        _top.transform.localScale = _bottom.transform.localScale = new Vector3(_size.x+1, 1, 1);
        _left.transform.localScale = _right.transform.localScale = new Vector3(1, _size.y+1, 1);

        var half = (_size + Vector2.one) / 2f;
        var offset = 1/2f;

        _top.transform.localPosition = new Vector3(-offset, half.y, 0);
        _right.transform.localPosition = new Vector3(half.x, offset, 0);
        _bottom.transform.localPosition = new Vector3(offset, -half.y, 0);
        _left.transform.localPosition = new Vector3(-half.x, -offset, 0);
    }

    [Serializable]
    private struct Wall
    {
        public WallMode Mode;
    }

    private enum WallMode
    {
        Obstacle,
        Damage,
        Transit
    }
}
