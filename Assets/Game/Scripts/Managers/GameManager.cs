using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // gameObject.SendMessageUpwards()
        // SendMessageUpwards()
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    // ...
    //

    private void OnLevelTransitionEnter()
    {
        Transit("level_01", "entrance");
    }

    //
    // 
    //

    private void Transit(string levelName, string transitionName) => StartCoroutine(TransitCoroutine(levelName, transitionName));
    private IEnumerator TransitCoroutine(string levelName, string transitionName)
    {
        yield return Main.UI.Get<UICurtain>().ShowAndWait();
        yield return Main.Level.Load(levelName);

        // ... initialize player ...

        yield return Main.UI.Get<UICurtain>().HideAndWait();
    }
}
