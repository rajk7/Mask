using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSetup : MonoBehaviour
{
    public PuzzlePiece puzzlePiecePF;
    public Transform puzzleContent;
    //public Sprite targetSprite;
    //public Image targetImg;
    public List<Sprite> sprite;

  

    private void Start()
    {
        SetSprite();
    }

    [ContextMenu("SetSprite")]
    public void SetSprite()
    {
        //targetImg.sprite = targetSprite;
        for (int i = 0; i < sprite.Count; i++)
        {
            PuzzlePiece puzzlePieceImage = Instantiate(puzzlePiecePF, puzzleContent);
            puzzlePieceImage.name = sprite[i].name;
            puzzlePieceImage.pieceID = int.Parse(sprite[i].name);
            puzzlePieceImage.image.sprite = sprite[i];

            PuzzleManager.Instance.allPieces.Add(puzzlePieceImage);
            //PuzzleManager.Instance.allPieces[i] = puzzlePieceImage;
        }

    }




}
