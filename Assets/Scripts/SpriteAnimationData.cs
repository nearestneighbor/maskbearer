using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Maskbearer/Create Sprite Animation", order = 1)]
public class SpriteAnimationData : ScriptableObject
{
    public string animationName;
    public int fps;
    public string spritesPath;

    public List<Sprite> sprites;
    public void Load()
    {
        if (spritesPath.Length == 0) return;

        var loaded = Resources.LoadAll(spritesPath, typeof(Sprite));
        sprites = new List<Sprite>();
        foreach (var sprite in loaded)
        {
            sprites.Add(sprite as Sprite);
        }
    }
}
