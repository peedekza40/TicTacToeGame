using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRadio : MonoBehaviour
{
    public Toggle ToggleX;
    public Toggle ToggleO;

    [Header("Events")]
    public GameEvent OnSetPlayer;

    private void Awake()
    {
        if(PlayerType.X == GameplayContext.instance.HumanPlayer || PlayerType.None == GameplayContext.instance.HumanPlayer)
        {
            ToggleX.isOn = true;
        }
        else
        {
            ToggleO.isOn = true;
        }
        SetPlayer();
    }
     
    public void SetPlayer()
    {
        var player = ToggleX.isOn ? PlayerType.X : PlayerType.O;
        OnSetPlayer.Raise(this, player);
    }
}
