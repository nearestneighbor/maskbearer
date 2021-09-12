using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public static Main Instance { get; private set; }
    public static UIManager UI { get; private set; }
    public static LevelManager Level { get; private set; }
    public static GameManager Game { get; private set; }

    private void Awake()
    {
        Instance = this;
        UI = GetComponent<UIManager>();
        Level = GetComponent<LevelManager>();
        Game = GetComponent<GameManager>();
    }

    private void Start()
    {
        UnloadAllScenesButThis();
        
        UI.Get<UIMenu>().Show();
    }

    private void UnloadAllScenesButThis()
    {
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene != gameObject.scene)
                SceneManager.UnloadSceneAsync(scene);
        }
    }
}