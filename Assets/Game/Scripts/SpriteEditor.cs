using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SpriteAnimation))]
public class SpriteAnimationEditor : Editor
{
    public string spritesPath;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var anim = (SpriteAnimation)target;

        anim.loopStart = EditorGUILayout.IntField("Loop Start", anim.loopStart.HasValue ? anim.loopStart.Value : 0);

        spritesPath = EditorGUILayout.TextField("Path of Sprites to Load", spritesPath);

        if (GUILayout.Button("Load Sprites", GUILayout.Height(40)))
        {
            if (spritesPath.Length == 0) return;
            var sprites = Resources.LoadAll(spritesPath, typeof(Sprite)).ToList().Select(obj => (Sprite)obj).ToArray();
            if (sprites != null) anim.frames = sprites;
        }
    }
}