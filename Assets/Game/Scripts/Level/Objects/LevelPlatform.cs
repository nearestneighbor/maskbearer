using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
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
        if (_collider == null)
            _collider = gameObject.AddComponent<BoxCollider2D>();

        _collider.hideFlags = HideFlags.HideInInspector;
        _collider.isTrigger = false;

        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
            _renderer = gameObject.AddComponent<SpriteRenderer>();;

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

        transform.localPosition = new Vector3(
            Mathf.RoundToInt(transform.localPosition.x * 4) / 4f,
            Mathf.RoundToInt(transform.localPosition.y * 4) / 4f,
            0
        );

        transform.localScale = new Vector3(
            Mathf.RoundToInt(transform.localScale.x * 4) / 4f,
            Mathf.RoundToInt(transform.localScale.y * 4) / 4f,
            1
        );
    }
}
