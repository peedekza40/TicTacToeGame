using System.Collections;
using System.Collections.Generic;
using Infrastructure.Entities;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
    public Button Button;
    public Image SignImage;

    [Header("Events")]
    public GameEvent OnCellClick;

    private int Row;
    private int Column;

    private void Start() 
    {
        Button.interactable = !GameplayContext.instance.EnableReplayMode;
    }

    public void TicCell()
    {
        if(GameplayContext.instance.IsAvailableCell(Row, Column))
        {
            var spritePath = string.Empty;
            Color color = Color.black;
            var currentPlayer =  GameplayContext.instance.CurrentPlayer;
            if(currentPlayer == PlayerType.X)
            {
                spritePath = "Sprites/UX/Icons/Flat Icons [Free]/Free Flat Button Blank Cross Icon";
                ColorUtility.TryParseHtmlString("#C63439", out color);
            }
            else if(currentPlayer == PlayerType.O)
            {
                spritePath = "Sprites/UX/Icons/Flat Icons [Free]/Free Flat Button Blank Circle Icon";
                ColorUtility.TryParseHtmlString("#2C47B0", out color);
            }

            SignImage.gameObject.SetActive(true);
            SignImage.color = color;
            SignImage.sprite = Resources.Load<Sprite>(spritePath);
            OnCellClick.Raise(this, new Vector2(Row, Column));
        }
    }

    public void AutoTicCell(Component sender, object data)
    {
        var move = (GameHistoryMove)data;
        if(Row == move.Row && Column == move.Column)
        {
            TicCell();
        }
    }

    public void UnTicCell(Component sender, object data)
    {
        var position = (Vector2)data;
        var row = (int)position.x;
        var col = (int)position.y;
        if(Row == row && Column == col)
        {
            SignImage.gameObject.SetActive(false);
        }
    }

    public void CheckIsAITurn(Component sender, object data)
    {
        if(GameplayContext.instance.EnableAI)
        {
            var currentPlayer = (PlayerType)data;
            Button.interactable = currentPlayer != GameplayContext.instance.AIPlayer;
        }
    }

    public void SetRowColumn(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
