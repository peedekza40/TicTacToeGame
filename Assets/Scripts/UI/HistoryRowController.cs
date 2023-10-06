using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Infrastructure.Entities;
using TMPro;
using UnityEngine;

public class HistoryRowController : MonoBehaviour
{
    public TextMeshProUGUI ModeText;
    public TextMeshProUGUI ResultText;
    public TextMeshProUGUI DimensionText;
    public TextMeshProUGUI DateTimeText;
    public GameHistory History { get; private set; }

    [Header("Events")]
    public GameEvent OnClickHistoryRow;

    public void SetRow(GameHistory history)
    {
        History = history;

        switch(History.GameResult)
        {
            case nameof(GameResult.XWin):
                ResultText.text = "Player X Win";
                break;
            case nameof(GameResult.OWin):
                ResultText.text = "Player O Win";
                break;
            case nameof(GameResult.Draw):
                ResultText.text = "Draw";
                break;
        }
        ModeText.text = History.Mode;
        DimensionText.text = $"{History.Size} X {History.Size}";
        DateTimeText.text = History.DateTime;
    }

    public void OnClick()
    {
        OnClickHistoryRow.Raise(this, History);
    }
}
