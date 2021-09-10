using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public static UIManager UI { get; private set; }
    public static LevelManager Level { get; private set; }

    private void Awake()
    {
        UI = GetComponent<UIManager>();
        Level = GetComponent<LevelManager>();
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