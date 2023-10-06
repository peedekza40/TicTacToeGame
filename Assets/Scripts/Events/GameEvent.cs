using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    public List<GameEventListener> Listeners = new List<GameEventListener>();

    // Raise event through different methods signatures
    public void Raise(Component sender, object data)
    {
        foreach(var listener in Listeners)
        {
            listener.OnEventRaised(sender, data);
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if(Listeners.Contains(listener) == false)
        {
            Listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if(Listeners.Contains(listener))
        {
            Listeners.Remove(listener);
        }
    }
}
