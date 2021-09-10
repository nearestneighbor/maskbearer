using System.Collections;
using UnityEngine;

public class LevelInputController : MonoBehaviour
{
    void OnCancel()
    {
        Quit();
    }

    public void Quit()
    {
        StartCoroutine(QuitCoroutine());
    }

    private IEnumerator QuitCoroutine()
    {
        var room = FindObjectOfType<LevelManager>();
        var ui = FindObjectOfType<UIManager>();

        var curtain = ui.Get<UICurtain>();
        var menu = ui.Get<UIMenu>();
        var hud = ui.Get<HUD>();

        yield return curtain.ShowAndWait();
        yield return room.Unload();

        curtain.Hide();
        hud.Hide();

        menu.Show();
    }
}