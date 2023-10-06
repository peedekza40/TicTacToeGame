using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnTitle : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void SetText(Component sender, object data)
    {
        var currentPlayer = (PlayerType)data;
        Text.text = $"Turn : Player {currentPlayer}";
    }
}
