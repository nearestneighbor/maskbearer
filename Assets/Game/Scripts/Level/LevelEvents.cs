using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private void OnLevelEvent()
    {
        SendMessageUpwards(TransitionMessage.Name, new TransitionMessage());
    }
}