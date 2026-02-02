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

    public Button NextBtnPopup;
    public Button resetBtnPopup;
    public Button homeBtnPopup;

    public GameObject NextBtnGo;
    public GameObject resetBtnGo;
    public GameObject homeBtnGo;

    public Image targetImg;

    public GameObject userImageGO;
    public GameObject realImageGO;
    public Image userImage;
    public Image realImage;
    private float similarity;
    public Sprite targetSprite;
    public Sprite bgSprite;
    public Sprite withoutGb;
    public Image bgImg;

    public float winpercentage;
    private LevelSetup levelSetup;
    private ContentSizeFitter contentSizeFitter;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        targetImg.sprite = targetSprite;
        //bgImg.sprite = bgSprite;    
        submitBtn.onClick.AddListener(Submit);
        resetBtn.onClick.AddListener(ResetPuzzle);
       
        resetBtnPopup.onClick.AddListener(ResetPuzzle);

        NextBtnPopup.onClick.AddListener(NextLevel);
        resetBtnPopup.onClick.AddListener(ResetBtn);
        homeBtnPopup.onClick.AddListener(MenuBtn);
        realImage.sprite = withoutGb;
        realImageGO.SetActive(false);

        NextBtnGo.SetActive(false);
        resetBtnGo.SetActive(false);
        homeBtnGo.SetActive(false);
    }
    private void Start()
    {
        levelSetup = GetComponent<LevelSetup>();
        contentSizeFitter = levelSetup.puzzleContent.GetComponent<ContentSizeFitter>();
        
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
        contentSizeFitter.enabled = false;
        history.Clear();
        foreach (var piece in allPieces)
        {
            piece.ResetToStart();
        }
        contentSizeFitter.enabled = true;

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
                LevelManager.Instance.NextLevel();
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

        StartCoroutine( ScreenPixelComparer.Instance.CaptureFirstScreen());
        yield return null;
        userImageGO.SetActive(false);
        realImageGO.SetActive(true);

        StartCoroutine( ScreenPixelComparer.Instance.CaptureSecondScreen());
        yield return null;
        similarity = ScreenPixelComparer.Instance.CompareScreens();
        realImageGO.SetActive(false);
        userImageGO.SetActive(true);

        if (similarity >= winpercentage)
        {

            if (LevelManager.Instance.currentLevelIndex == 4)
            {
                NextpopupSetActive(false);
                MenupopupSetActive(true);
                //LevelManager.Instance.NextLevel();
                Debug.Log("Puzzle Completed!");
            }
            else
            {
                NextpopupSetActive(true);
                //LevelManager.Instance.NextLevel();
                Debug.Log("Puzzle Completed!");
            }

        }
        else
        {
            Debug.Log("Puzzle not Completed!");
            ResetpopupSetActive(true);
            //ResetPuzzle();
        }


    //PuzzleManager.Instance.CheckWinCondition();
    }

    private void NextLevel()
    {
        NextpopupSetActive(false);
        LevelManager.Instance.NextLevel();
    }

    private void ResetBtn()
    {
        ResetpopupSetActive(false);

    }
    private void MenuBtn()
    {
        MenupopupSetActive(false);
        LevelManager.Instance.NextLevel();

    }


    private void NextpopupSetActive(bool var)
    {
        NextBtnGo.SetActive(var);
    }
    private void ResetpopupSetActive(bool var)
    {
        resetBtnGo.SetActive(var);
    }
    private void MenupopupSetActive(bool var)
    {
        homeBtnGo.SetActive(var);
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
