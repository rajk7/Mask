using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public List<PuzzlePiece> allPieces = new List<PuzzlePiece>();
    private Stack<ICommand> history = new Stack<ICommand>();

    public GameObject winPanel;
    public Button submitBtn;
    public Button resetBtn;

    public Image targetImg;

    public GameObject userImageGO;
    public GameObject realImageGO;
    public Image userImage;
    public Image realImage;
    private float similarity;
    public Sprite targetSprite;
    public Sprite bgSprite;
    public Image bgImg;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        targetImg.sprite = targetSprite;
        //bgImg.sprite = bgSprite;    
        submitBtn.onClick.AddListener(Submit);
        resetBtn.onClick.AddListener(ResetPuzzle);
        realImage.sprite = targetSprite;
        realImageGO.SetActive(false);
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
            if (similarity > 98)
            {
               // winPanel.SetActive(true);
            }
            Debug.Log("Puzzle Completed!");
        }
    }


    public void Submit()
    {
        StartCoroutine(comparescreens());

    }

    IEnumerator comparescreens()
    {
        userImageGO.SetActive(true);
        realImageGO.SetActive(false);

        ScreenPixelComparer.Instance.CaptureFirstScreen();
        yield return new WaitForSeconds (1);
        userImageGO.SetActive(false);
        realImageGO.SetActive(true);

        ScreenPixelComparer.Instance.CaptureSecondScreen();
        yield return new WaitForSeconds(1);
        similarity = ScreenPixelComparer.Instance.CompareScreens();
        realImageGO.SetActive(false);
        userImageGO.SetActive(true);

        if (similarity > 98)
        {
            //LevelManager.Instance.NextLevel();
            Debug.Log("Puzzle Completed!");

        }
        else
        {
            Debug.Log("Puzzle not Completed!");

        }


    //PuzzleManager.Instance.CheckWinCondition();
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
