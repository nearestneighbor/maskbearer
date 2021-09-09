using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AnimationData", menuName = "Maskbearer/AnimationData", order = 1)]
public class AnimationData : ScriptableObject
{
    public UnityEngine.Object folder;
    public List<Sprite> sprites;

    private void OnValidate()
    {
        #if UNITY_EDITOR

        if (folder != null)
        {
            var path = AssetDatabase.GetAssetPath(folder);
            var pathIsValid = AssetDatabase.IsValidFolder(path);
            if (pathIsValid != true)
            {
                folder = null;
                Debug.LogWarning($"Asset isn't a valid folder");
            }
        }

        #endif
    }

    [Button]
    public void Load()
    {
        #if UNITY_EDITOR

        if (folder == null)
        {
            sprites.Clear();
        }
        else
        {
            sprites = AssetDatabase
                .FindAssets("t: Sprite", new [] { AssetDatabase.GetAssetPath(folder) })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(x => AssetDatabase.LoadAssetAtPath<Sprite>(x))
                .ToList();
        }

        #endif
    }
}
