using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public List<PuzzlePiece> allPieces = new List<PuzzlePiece>();
    private Stack<ICommand> history = new Stack<ICommand>();

    public GameObject winPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterPiece(PuzzlePiece piece)
    {
        if (!allPieces.Contains(piece))
            allPieces.Add(piece);
    }

    public void RecordMove(PuzzlePiece piece, Vector3 oldPos, Transform oldParent, bool wasLocked)
    {
        ICommand move = new MoveCommand(piece, oldPos, oldParent, wasLocked);
        history.Push(move);
    }

    public void Undo()
    {
        if (history.Count > 0)
        {
            ICommand lastCommand = history.Pop();
            lastCommand.Undo();
        }
    }

    public void ResetPuzzle()
    {
        history.Clear();
        foreach (var piece in allPieces)
        {
            piece.ResetToStart();
        }
    }

    public void CheckWinCondition()
    {
        bool allCorrect = true;
        foreach (var piece in allPieces)
        {
            if (!piece.IsLocked)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect && winPanel != null)
        {
            winPanel.SetActive(true);
            Debug.Log("Puzzle Completed!");
        }
    }
}

public interface ICommand
{
    void Undo();
}

public class MoveCommand : ICommand
{
    private PuzzlePiece piece;
    private Vector3 previousPosition;
    private Transform previousParent;
    private bool wasLocked;

    public MoveCommand(PuzzlePiece p, Vector3 pos, Transform parent, bool locked)
    {
        piece = p;
        previousPosition = pos;
        previousParent = parent;
        wasLocked = locked;
    }

    public void Undo()
    {
        piece.ForceMove(previousPosition, previousParent, wasLocked);
    }
}
