using System.Collections.Generic;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    public PuzzlePiece puzzlePiecePF;
    public Transform puzzleContent;
    public List<Sprite> sprite;

    private void Start()
    {
        SetSprite();
    }

    [ContextMenu("SetSprite")]
    public void SetSprite()
    {
        for (int i = 0; i < sprite.Count; i++)
        {
            PuzzlePiece puzzlePieceImage = Instantiate(puzzlePiecePF, puzzleContent);
            puzzlePieceImage.name = sprite[i].name;
            puzzlePieceImage.pieceID = int.Parse(sprite[i].name);
            puzzlePieceImage.image.sprite = sprite[i];

            PuzzleManager.Instance.allPieces[i] = puzzlePieceImage;
        }

    }


}
