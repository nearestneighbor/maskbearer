using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class LevelPlatform : MonoBehaviour
{
    [HideInInspector] [SerializeField] private Sprite _sprite;
    [HideInInspector] [SerializeField] private BoxCollider2D _collider;
    [HideInInspector] [SerializeField] private SpriteRenderer _renderer;

    private enum Type { Solid, Hazard }
    [SerializeField] private Type _type;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.hideFlags = HideFlags.HideInInspector;
        _collider.isTrigger = false;

        _renderer = GetComponent<SpriteRenderer>();
        _renderer.hideFlags = HideFlags.HideInInspector;
        _renderer.sprite = _sprite;

        OnValidate();
    }

    private void OnValidate()
    {
        if (_renderer == null)
            return;

        switch (_type)
        {
            case Type.Solid:
                _renderer.color = Color.white;
                break;
            
            case Type.Hazard:
                _renderer.color = Color.red;
                break;
        }
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;

        transform.localScale = new Vector3(
            Round(transform.localScale.x),
            Round(transform.localScale.y),
            1
        );

        transform.localPosition = new Vector3(
            Round(transform.localPosition.x, 8),
            Round(transform.localPosition.y, 8),
            0
        );
    }

    private float Round(float value, float k = 4f)
    {
        return Mathf.RoundToInt(value * k) / k;
    }
}
