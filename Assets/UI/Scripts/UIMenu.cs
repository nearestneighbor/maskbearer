using System.Collections;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIMenu : UIManager.UIBehaviour
{
    [Header("Parts")]
    [SerializeField] private GameObject _play;
    [SerializeField] private GameObject _quit;
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _optionsLevel;
    [SerializeField] private GameObject _optionsLevelName;
    [SerializeField] private TextMeshProUGUI _version;
    
    [Header("Blocks")]
    [SerializeField] private GameObject _bMain;
    [SerializeField] private GameObject _bOptions;

    [Header("Data")]
    [Scene]
    [SerializeField] private string _levelName;

    private GameObject _selection;

    private void Start()
    {
        _version.text = Application.version;
        _selection = _play;
        _levelName = PlayerPrefs.GetString("_levelName", null);

        var levelNameExists = GetSceneNames().IndexOf(_levelName) != -1;
        if (levelNameExists == false)
            _levelName = GetSceneNames()[0];

        _optionsLevelName.GetComponent<TextMeshProUGUI>().text = _levelName;

        _bMain.SetActive(true);
        _bOptions.SetActive(false);
    }

    protected override IEnumerator OnShow()
    {
        Select(_play);
        yield break;
    }

    private void Update()
    {
        if (_selection != null)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem.currentSelectedGameObject != _selection && _selection.activeInHierarchy)
            {
                Select(_selection);
                _selection = null;
            }
        }
    }

    //
    // Main
    //
    public void OnPlayClick()
    {
        StartCoroutine(OnPlayClickCoroutine());
    }

    private IEnumerator OnPlayClickCoroutine()
    {
        yield return Get<UICurtain>().ShowAndWait();

        Fader.instance.FadeIn();

        Hide();
        Main.Game.StartUp(_levelName);
    }

    public void OnOptionsClick()
    {
        _bMain.SetActive(false);
        _bOptions.SetActive(true);
        Select(_optionsLevel);
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    //
    // Options
    //

    public void OnOptionsLevelClick()
    {
        var sceneNames = GetSceneNames();
        var sceneIndex = sceneNames.IndexOf(_levelName);
        var nextSceneIndex = (sceneIndex + 1) % sceneNames.Count;
        var nextSceneName = sceneNames[nextSceneIndex];

        _levelName = nextSceneName;
        _optionsLevelName.GetComponent<TextMeshProUGUI>().text = _levelName;
        PlayerPrefs.SetString("_levelName", _levelName);
        PlayerPrefs.Save();
    }

    public void OnOptionsBackClick()
    {
        _bMain.SetActive(true);
        _bOptions.SetActive(false);
        Select(_options);
    }

    //
    // Tools
    //

    private List<string> GetSceneNames()
    {
        var sceneNames = new List<string>();
        for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var name = Path.GetFileNameWithoutExtension(path);
            if (name != gameObject.scene.name)
                sceneNames.Add(name);
        }

        return sceneNames;
    }

    private void Select(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
