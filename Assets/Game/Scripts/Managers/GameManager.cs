using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private PlayerInputActions _input;

    public void Start()
    {
        _input = new PlayerInputActions();
        _input.UI.Cancel.performed += OnUICancel;
        _input.UI.Enable();
    }

    //
    // Public API
    //
    public void StartUp(string levelName)
    {
        Open(new OpenArgs() {
            name = levelName
        });
    }

    //
    // Message Handlers
    //

    private void OnLevelTransitionMessage(LevelTransitionMessage message)
    {
        Open(new OpenArgs() {
            name = message.LevelName,
            transitionName = message.TransitionName,
            transitionDirection = message.TransitionDirection
        });
    }

    private void OnUICancel(InputAction.CallbackContext context)
    {
        Close();
    }

    //
    // ...
    //
    private void Open(OpenArgs options) => StartCoroutine(OpenCoroutine(options));
    private IEnumerator OpenCoroutine(OpenArgs options)
    {
        yield return Main.UI.Get<UICurtain>().ShowAndWait();
        yield return Main.Level.Load(options.name);

        // ... initialize level ...
        FindObjectOfType<LevelMessageBus>().Listener = this;

        // ... initialize player ...
        if (options.transitionName != null)
        {
            var transition = FindObjectsOfType<LevelTransition>().FirstOrDefault(x => x.name == options.transitionName);
            if (transition == null)
                throw new Exception();

            transition.Place(
                FindObjectOfType<Player>(),
                options.transitionDirection
            );

            FindObjectOfType<LevelCameraController>().Setup(
                Camera.main,
                FindObjectOfType<Player>(),
                FindObjectOfType<LevelBounds>()
            );
        }

        _input.Player.Enable();
        FindObjectOfType<Player>().SetPlayerInputActions(
            _input
        );

        yield return Main.UI.Get<UICurtain>().HideAndWait();
    }

    private struct OpenArgs
    {
        public string name;

        public string transitionName;
        public LevelTransitionMessage.Direction transitionDirection; 
    }

    private void Close() => StartCoroutine(CloseCoroutine());
    private IEnumerator CloseCoroutine()
    {
        yield return Main.UI.Get<UICurtain>().ShowAndWait();
        yield return Main.Level.Unload();
        Main.UI.Get<UIMenu>().Show();
        yield return Main.UI.Get<UICurtain>().HideAndWait();
    }
}
