using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CustomGameEvent : UnityEvent<Component, object> {}

public class GameEventListener : MonoBehaviour
{
    public GameEvent GameEvent;

    public CustomGameEvent Response;

    private void OnEnable() 
    {
        GameEvent.RegisterListener(this);
    }

    private void OnDisable() 
    {
        GameEvent.UnregisterListener(this);
    }

    public void OnEventRaised(Component sender, object data)
    {
        Response.Invoke(sender, data);
    }
}
