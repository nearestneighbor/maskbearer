using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private AsyncOperation _async;
    private string _name;

    public CustomYieldInstruction Load(string name)
    {
        StartCoroutine(LoadCoroutine(name));
        return new WaitWhile(IsLoading);
    }

    public CustomYieldInstruction Unload()
    {
        StartCoroutine(UnloadCoroutine());
        return new WaitWhile(IsLoading);
    }

    private IEnumerator LoadCoroutine(string name)
    {
        if (_async != null)
            throw new InvalidOperationException("Can't Load() while IsLoading");

        if (_name != null)
            yield return Unload();

        _async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        yield return _async;

        _name = name;
        _async = null;
    }

    private IEnumerator UnloadCoroutine()
    {
        if (_async != null)
            throw new InvalidOperationException("Can't Unload() while IsLoading");
        
        if (_name == null)
            throw new InvalidOperationException("There isn't loaded scene");

        _async = SceneManager.UnloadSceneAsync(_name);
        yield return _async;

        _name = null;
        _async = null;
    }

    private bool IsLoading()
    {
        return _async != null && _async.isDone == false;
    }
}