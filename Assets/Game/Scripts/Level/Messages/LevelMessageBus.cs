using System;
using UnityEngine;

// This cass is used to broadcast level events to out world
public class LevelMessageBus : MonoBehaviour
{
    public MonoBehaviour Listener { get; set; }

    private void OnLevelTransitionMessage(LevelTransitionMessage message)
        => Listener.SendMessage(LevelTransitionMessage.Name, message);
}