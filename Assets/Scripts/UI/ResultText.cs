using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void SetText(Component sender, object data)
    {
        var textTemplate = "Player {0} Win !!!";
        var gameResult = (GameResult)data;
        Color color = Color.black;

        switch(gameResult)
        {
            case GameResult.XWin:
                ColorUtility.TryParseHtmlString("#FF6C71", out color);
                Text.text = string.Format(textTemplate, PlayerType.X);
                Text.color = color;
                break;
            case GameResult.OWin:
                ColorUtility.TryParseHtmlString("#A0D6FF", out color);
                Text.text = string.Format(textTemplate, PlayerType.O);
                Text.color = color;
                break;
            case GameResult.Draw:
                Text.text = "It's a draw";
                Text.color = Color.white;
                break;
        }
    }
}
