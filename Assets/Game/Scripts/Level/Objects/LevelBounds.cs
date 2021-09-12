using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class LevelBounds : MonoBehaviour
{
    public Vector2Int Size => _size;

    [SerializeField] private Vector2Int _size;
    [SerializeField] [HideInInspector] private Sprite _sprite;
    
    private GameObject _container;
    private GameObject _top;
    private GameObject _right;
    private GameObject _bottom;
    private GameObject _left;

    private void Start()
    {
        _top = InstantiateWall("Top");
        _right = InstantiateWall("Right");
        _bottom = InstantiateWall("Bottom");
        _left = InstantiateWall("Left");

        OnValidate();
    }

    private void OnDestroy()
    {
        if (_container != null)
        {
            if (Application.isPlaying)
                Destroy(_container);
            else
                DestroyImmediate(_container);
        }
    }

    private GameObject InstantiateWall(string name)
    {
        if (_container == null)
        {
            _container = new GameObject("Bounds");
            _container.transform.SetParent(transform, false);
            _container.transform.SetSiblingIndex(0);
            _container.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
        }

        var wall = new GameObject(name);
        wall.transform.SetParent(_container.transform, false);
        wall.hideFlags = _container.hideFlags;
        wall.AddComponent<SpriteRenderer>().sprite = _sprite;
        wall.AddComponent<BoxCollider2D>();
        wall.layer = 3;

        return wall;
    }

    private void DestroyWall(GameObject wall)
    {
    }




    // Validation .............



    private void Update()
    {
        if (Application.isPlaying)
            return;

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    private void OnValidate()
    {
        if (_top == null)
            return;


        _size.x = Mathf.Max(1, _size.x);
        _size.y = Mathf.Max(1, _size.y);

        _top.GetComponent<SpriteRenderer>().sprite =
        _right.GetComponent<SpriteRenderer>().sprite =
        _bottom.GetComponent<SpriteRenderer>().sprite =
        _left.GetComponent<SpriteRenderer>().sprite = _sprite;

        _top.transform.localScale = _bottom.transform.localScale = new Vector3(_size.x+1, 1, 1);
        _left.transform.localScale = _right.transform.localScale = new Vector3(1, _size.y+1, 1);

        var half = (_size + Vector2.one) / 2f;
        var offset = 1/2f;

        _top.transform.localPosition = new Vector3(-offset, half.y, 0);
        _right.transform.localPosition = new Vector3(half.x, offset, 0);
        _bottom.transform.localPosition = new Vector3(offset, -half.y, 0);
        _left.transform.localPosition = new Vector3(-half.x, -offset, 0);
    }
}
