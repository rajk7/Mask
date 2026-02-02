using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class LevelSetup : MonoBehaviour
{
    public PuzzlePiece puzzlePiecePF;
    public Transform puzzleContent;
    //public Sprite targetSprite;
    //public Image targetImg;
    public List<Sprite> sprite;

  

    private void Awake()
    {
        SetSprite();
    }

    [ContextMenu("SetSprite")]
    public void SetSprite()
    {
        Shuffle(sprite);

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



    public  void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }


}
