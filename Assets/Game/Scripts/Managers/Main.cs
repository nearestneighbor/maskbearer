using UnityEngine;

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
        UI.Get<UIMenu>().Show();
    }
}