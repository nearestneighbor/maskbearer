using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class LevelSpawner : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private LevelSpawnerSetting _settings;

    [SerializeField]
    private LevelSpawnerType _type;

    private SpriteRenderer _sprite;

    private void Awake()
    {
        if (Application.isPlaying)
            return;

        _sprite = GetComponent<SpriteRenderer>();
        _sprite.hideFlags = HideFlags.HideInInspector | HideFlags.DontSaveInBuild;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        name = "Spawner_" + _type.ToString();

        if (_sprite != null)
            _sprite.sprite = _settings.GetIcon(_type);
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        var prefab = _settings.GetPrefab(_type);
        var instance = Instantiate(prefab, transform.position, Quaternion.identity);
        instance.name = prefab.name;
        SceneManager.MoveGameObjectToScene(instance, gameObject.scene);
        Destroy(gameObject);
    }
}