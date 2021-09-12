using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawnerSetting : ScriptableObject
{
    [Serializable]
    public struct LevelSpawnerItem 
    {
        public LevelSpawnerType Type;
        public GameObject Prefab;
        public Sprite Icon;
    }

    [SerializeField]
    private List<LevelSpawnerItem> _setting;

    public Sprite GetIcon(LevelSpawnerType type)
    {
        return _setting.Find(x => x.Type == type).Icon;
    }

    public GameObject GetPrefab(LevelSpawnerType type)
    {
        return _setting.Find(x => x.Type == type).Prefab;
    }
}

public enum LevelSpawnerType
{
    Player,
    Crawler
}