using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenu : UIManager.UIBehaviour
{
    [SerializeField] private GameObject _play;
    [SerializeField] private GameObject _quit;
    [SerializeField] private TextMeshProUGUI _version;

    private void Start()
    {
        _version.text = Application.version;
    }

    protected override IEnumerator OnShow()
    {
        Select(_play);
        yield break;
    }

    private void Update()
    {
        var eventSystem = EventSystem.current;
        if (eventSystem.currentSelectedGameObject == null || !eventSystem.currentSelectedGameObject.activeInHierarchy)
            Select(_play);
    }

    public void OnPlayClick()
    {
        StartCoroutine(OnPlayClickCoroutine());
    }

    private IEnumerator OnPlayClickCoroutine()
    {
        yield return Get<UICurtain>().ShowAndWait();
        yield return Main.Level.Load("Test_01");

        Get<UICurtain>().Hide();
        Get<HUD>().Show();
        Hide();
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    private void Select(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
