using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour
{
    private static Fader _instance;
    private static string _faderPath = "Fader";
    private Animator _animator;

    public Action<bool> FadedIn;

    private void Start()
    {
        SceneManager.sceneLoaded += FadeOut;
    }

    public static Fader instance
    {
        get
        {
            if (_instance == null)
            {
                var prefab = Resources.Load<GameObject>(_faderPath);
                _instance = Instantiate(prefab).GetComponent<Fader>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    public bool isFading { get; private set; }

    public void FadeIn()
    {
        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        _animator.SetBool("Faded", true);
    }

    public void FadeOut(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.SetBool("Faded", false);
    }

    public void FadeInOver()
    {
        FadedIn.Invoke(true);
        FadedIn = null;
    }
}
