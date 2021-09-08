using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Maskbearer/AnimationData", order = 1)]
public class AnimationData : ScriptableObject
{
    public string path;

    public List<Sprite> sprites;


    public void Load(){
        if (path.Length == 0) return;
        var loaded = Resources.LoadAll(path, typeof(Sprite));
        sprites = new List<Sprite>();
        foreach(var sprite in loaded){
            sprites.Add(sprite as Sprite);
        }

    }
}
