using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Models;
using SqlCipher4Unity3D;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayContext : MonoBehaviour
{
    public int Size;
    public bool EnableAI = false;
    public bool EnableReplayMode = false;

    [Header("Events")]
    public GameEvent OnInitialBoard;
    public GameEvent OnSwitchPlayer;
    public GameEvent OnAutoCellClick;
    public GameEvent OnUnTicCell;
    public GameEvent OnGameEnded;

    private int Turn;
    private bool GameEnded;
    private float TimeSinceStartTurn = 0;
    private PlayerType[,] Board;
    public PlayerType CurrentPlayer { get; private set; }
    public PlayerType HumanPlayer { get; private set; }
    public PlayerType AIPlayer { get; private set; }

    private SQLiteConnection Connection;
    private GameHistory GameHistory = new GameHistory();
    private List<GameHistoryMove> GameHistoryMoves = new List<GameHistoryMove>();
    private List<GameHistoryMove> ReplayHistoryMoves = new List<GameHistoryMove>();

    public static GameplayContext instance;

    private void Awake() 
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);    
    }

    private void Start() 
    {
        Connection = new DbContextBuilder().Connection;
    }

    private void Update() 
    {
        if(!GameEnded && Board != null)
        {
            if(EnableReplayMode && TimeSinceStartTurn >= 1)
            {
                var move = ReplayHistoryMoves.FirstOrDefault(x => x.Turn == Turn);
                OnAutoCellClick.Raise(this, move);
                TimeSinceStartTurn = 0;
            }
            else if(EnableAI && TimeSinceStartTurn >= 0.8 && CurrentPlayer == AIPlayer)
            {
                var bestMove = MiniMax(Board, CurrentPlayer, Mathf.Min(Size, 4), int.MinValue, int.MaxValue);
                var move = new GameHistoryMove(){ Row = bestMove.BestMoveRow, Column = bestMove.BestMoveColumn };
                OnAutoCellClick.Raise(this, move);
                TimeSinceStartTurn = 0;
            }

        }

        TimeSinceStartTurn += Time.deltaTime;
    }

    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
        if(scene.name == "Game")
        {
            InitialBoard();
        }
    }

    public void InitialBoard()
    {
        if(EnableReplayMode)
        {
            ReplayHistoryMoves = Connection.Table<GameHistoryMove>().Where(x => x.GameHistoryID == GameHistory.ID).ToList();
            SetStartPlayer(ReplayHistoryMoves.FirstOrDefault()?.Player == nameof(PlayerType.X) ? PlayerType.X : PlayerType.O); 
            Size = GameHistory.Size;
        }
        else 
        {
            GameHistory = new GameHistory();
            GameHistoryMoves = new List<GameHistoryMove>();
            GameHistory.Mode = EnableAI ? "PVE" : "PVP";
            GameHistory.Size = Size;
            SetStartPlayer(PlayerType.X);
        }

        Turn = 1;
        GameEnded = false;
        AIPlayer = GetSwitchPlayer(HumanPlayer);

        //create board
        Board = new PlayerType[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Board[i, j] = PlayerType.None;
            }
        }

        //raise event
        OnInitialBoard.Raise(this, Size);
        OnSwitchPlayer.Raise(this, CurrentPlayer);
    }

    public void SetStartPlayer(PlayerType player)
    {
        CurrentPlayer = player;
    }

    public void OnStartGame(Component sender, object data)
    {
        var isPVP = (bool)data;
        EnableAI = isPVP == false;
        EnableReplayMode = false;
    }

    public void OnSetPlayer(Component sender, object data)
    {
        var player = (PlayerType)data;
        HumanPlayer = player;
    }

    public void OnSetSize(Component sender, object data)
    {
        Size = (int)data;
    }

    public void OnCellClick(Component sender, object data)
    {
        var position = (Vector2)data;
        var row = (int)position.x;
        var col = (int)position.y;
        TicOnBoardAndCheckGame(row, col);
    }

    public void OnHistoryRowClick(Component sender, object data)
    {
        var gameHistory = (GameHistory)data;
        GameHistory = gameHistory;
        EnableReplayMode = true;
    }

    public void OnReplayControlAction(Component sender, object data)
    {
        var action = (ReplayControlAction)data;
        switch(action)
        {
            case ReplayControlAction.Play:
                Time.timeScale = 1;
                break;
            case ReplayControlAction.Pause:
                Time.timeScale = 0;
                break;
            case ReplayControlAction.Backward:
                TimeSinceStartTurn = 0;
                UnTicOnBoard();
                break;
            case ReplayControlAction.Forward:
                TimeSinceStartTurn = 0;
                var move = ReplayHistoryMoves.FirstOrDefault(x => x.Turn == Turn);
                if(move != null)
                {
                    OnAutoCellClick.Raise(this, move);
                }
                break;
        }
    }

    public bool IsAvailableCell(int row, int col)
    {
        return Board[row, col] == PlayerType.None;
    }

    private void TicOnBoardAndCheckGame(int row, int col)
    {
        var gameResult = GameResult.Draw;
        if (IsAvailableCell(row, col) && !GameEnded)
        {
            Board[row, col] = CurrentPlayer;
            GameHistoryMoves.Add(new GameHistoryMove()
            {
                Player = CurrentPlayer.ToString(),
                Turn = Turn,
                Row = row,
                Column = col
            });

            if (IsWinner(Board, CurrentPlayer))
            {
                gameResult = (CurrentPlayer == PlayerType.X) ? GameResult.XWin : GameResult.OWin;
                GameEnded = true;
            }
            else if (IsBoardFull(Board))
            {
                gameResult = GameResult.Draw;
                GameEnded = true;
            }
            else
            {
                CurrentPlayer = GetSwitchPlayer(CurrentPlayer);
                OnSwitchPlayer.Raise(this, CurrentPlayer);
            }

            Turn++;
        }

        if(GameEnded)
        {
            if(EnableReplayMode == false)
            {
                //save to db
                GameHistory.GameResult = gameResult.ToString();
                GameHistory.DateTime = DateTime.Now.ToString("dd MMM yyyy h:mm tt");
                Connection.Insert(GameHistory);
                foreach(var move in GameHistoryMoves)
                {
                    move.GameHistoryID = GameHistory.ID;
                }
                Connection.InsertAll(GameHistoryMoves);
            }
            
            //raise event
            OnGameEnded.Raise(this, gameResult);
        }

    }

    private void UnTicOnBoard()
    {
        var lastMove = GameHistoryMoves.LastOrDefault();
        if(lastMove != null)
        {
            Board[lastMove.Row, lastMove.Column] = PlayerType.None;
            CurrentPlayer = lastMove.Player == nameof(PlayerType.X) ? PlayerType.X : PlayerType.O;
            GameEnded = false;
            Turn = lastMove.Turn;
            GameHistoryMoves.Remove(lastMove);

            //raise
            OnUnTicCell.Raise(this, new Vector2(lastMove.Row, lastMove.Column));
            OnSwitchPlayer.Raise(this, CurrentPlayer);
        }
    }

    private PlayerType GetSwitchPlayer(PlayerType currentPlayer)
    {
        return (currentPlayer == PlayerType.X) ? PlayerType.O : PlayerType.X;
    }

    private MiniMaxResult MiniMax(PlayerType[,] currentBoard, PlayerType currentPlayer, int depth, int alpha, int beta)
    {
        // StringBuilder sb = new StringBuilder();
        // for(int i=0; i < currentBoard.GetLength(1); i++)
        // {
        //     for(int j=0; j < currentBoard.GetLength(0); j++)
        //     {
        //         var move = currentBoard[i,j];
        //         sb.Append('|' + (move == PlayerType.None ? "N" : move.ToString()) + '|');
        //     }
        //     sb.AppendLine();
        // }

        // Check for terminal states (win, lose, draw) or reached maximum depth
        if (IsWinner(currentBoard, AIPlayer))
        {
            // Debug.Log($"[AI WIN]" +
            //     "\ndepth : " + depth + 
            //     "\nPlayer : " + currentPlayer.ToString() + 
            //     "\ncurrentBoard : \n" + sb +
            //     "\nalpha : " + alpha +
            //     "\nbeta : " + beta);
            return new MiniMaxResult { Score = 10 - depth, BestMoveRow = 0, BestMoveColumn = 0 }; // AI wins
        }
        else if (IsWinner(currentBoard, HumanPlayer))
        {
            return new MiniMaxResult { Score = depth - 10, BestMoveRow = 0, BestMoveColumn = 0 }; // Human wins
        }
        else if (IsBoardFull(currentBoard) || depth == 0)
        {
            return new MiniMaxResult { Score = 0, BestMoveRow = 0, BestMoveColumn = 0 }; // It's a draw or reached maximum depth
        }

        var bestMove = new MiniMaxResult { Score = (currentPlayer == AIPlayer) ? int.MinValue : int.MaxValue, BestMoveRow = -1, BestMoveColumn = -1 };

        // Loop through all empty spots
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (currentBoard[i, j] == PlayerType.None)
                {
                    // Make the move
                    currentBoard[i, j] = currentPlayer == AIPlayer ? AIPlayer : HumanPlayer;

                    // Recursive call to evaluate the move
                    var result = MiniMax(currentBoard, GetSwitchPlayer(currentPlayer), depth - 1, alpha, beta);

                    // Undo the move
                    currentBoard[i, j] = PlayerType.None;

                    // if(result.IsAIWin)
                    // {
                    //     Debug.Log($"[Best move]" +
                    //             "\ndepth : " + depth + 
                    //             "\nPlayer : " + currentPlayer.ToString() + 
                    //             "\ncurrentBoard : \n" + sb +
                    //             "\nrow : " + result.BestMoveRow +
                    //             "\ncol : " + result.BestMoveColumn +
                    //             "\nScore : " + result.Score +
                    //             "\nalpha : " + alpha +
                    //             "\nbeta : " + beta);
                    // }

                    // Update alpha and beta based on the result
                    if (currentPlayer == AIPlayer)
                    {
                        alpha = Mathf.Max(alpha, bestMove.Score);
                    }
                    else
                    {
                        beta = Mathf.Min(beta, bestMove.Score);
                    }

                    // Alpha-beta pruning
                    if (beta <= alpha)
                    {
                        break;
                    }

                    // Update the best move if needed
                    if ((currentPlayer == AIPlayer && result.Score > bestMove.Score) 
                        || (currentPlayer == HumanPlayer && result.Score < bestMove.Score))
                    {
                        bestMove.Score = result.Score;
                        bestMove.BestMoveRow = i;
                        bestMove.BestMoveColumn = j;
                    }
                    
                }
            }
        }
        return bestMove;
    }

    private bool IsWinner(PlayerType[,] currentBoard, PlayerType player)
    {
        return CheckRows(currentBoard, player) || CheckColumns(currentBoard, player) || CheckDiagonals(currentBoard, player);
    }

    private bool CheckRows(PlayerType[,] currentBoard, PlayerType player)
    {
        for (int i = 0; i < currentBoard.GetLength(0); i++)
        {
            if (CheckLine(currentBoard, i, 0, 0, 1, player))
                return true;
        }

        return false;
    }

    private bool CheckColumns(PlayerType[,] currentBoard, PlayerType player)
    {
        for (int i = 0; i < currentBoard.GetLength(0); i++)
        {
            if (CheckLine(currentBoard, 0, i, 1, 0, player))
                return true;
        }

        return false;
    }

    private bool CheckDiagonals(PlayerType[,] currentBoard, PlayerType player)
    {
        return CheckDiagonalForward(currentBoard, player) || CheckDiagonalBackward(currentBoard, player);
    }

    private bool CheckDiagonalForward(PlayerType[,] currentBoard, PlayerType player)
    {
        for (int i = 0; i < currentBoard.GetLength(0); i++)
        {
            if (CheckLine(currentBoard, 0, i, 1, 1, player))
                return true;
        }

        return false;
    }

    private bool CheckDiagonalBackward(PlayerType[,] currentBoard, PlayerType player)
    {
        for (int i = 0; i < currentBoard.GetLength(0); i++)
        {
            if (CheckLine(currentBoard, 0, currentBoard.GetLength(1) - 1 - i, 1, -1, player))
                return true;
        }

        return false;
    }

    private bool CheckLine(PlayerType[,] currentBoard, int startRow, int startCol, int rowInc, int colInc, PlayerType player)
    {
        for (int i = 0; i < currentBoard.GetLength(0); i++)
        {
            if (startRow < 0 || startRow >= currentBoard.GetLength(0) || startCol < 0 || startCol >= currentBoard.GetLength(1))
            {
                return false;  // Check if the indices are within bounds
            }

            if (currentBoard[startRow, startCol] != player)
            {
                return false;
            }

            startRow += rowInc;
            startCol += colInc;
        }

        return true;
    }

    private bool IsBoardFull(PlayerType[,] currentBoard)
    {
        for (int i = 0; i < currentBoard.GetLength(0); i++)
        {
            for (int j = 0; j < currentBoard.GetLength(1); j++)
            {
                if (currentBoard[i, j] == PlayerType.None)
                    return false;
            }
        }
        return true;
    }
}
