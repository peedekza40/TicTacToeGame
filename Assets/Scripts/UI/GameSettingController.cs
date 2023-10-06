using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingController : MonoBehaviour
{
    [Header("Events")]
    public GameEvent OnStartGame;

    public void StartGame(bool isPVP)
    {
        OnStartGame.Raise(this, isPVP);
    }
}
