using UnityEngine;
using System.Collections;

public class ChessPieceGUI : MonoBehaviour
{
  public ChessPiece chessPiece;
  public ChessBoardGUI boardGUI;

  public void Setup(ChessPiece chessPiece, ChessBoardGUI boardGUI, GameObject icon)
  {
    this.chessPiece = chessPiece;
    this.boardGUI = boardGUI;
  }

  void Update()
  {
    if (boardGUI.board.IsPieceInPlay(chessPiece))
    {
      transform.SetParent(boardGUI.boardTilesOrigin, true);
    }
    else if (boardGUI.board.eliminatedBlackPieces.Contains(chessPiece))
    {
      transform.SetParent(boardGUI.eliminatedBlackOrigin, true);
    }
    else if (boardGUI.board.eliminatedWhitePieces.Contains(chessPiece))
    {
      transform.SetParent(boardGUI.eliminatedWhiteOrigin, true);
    }
    else
    {
      transform.SetParent(null);
    }
    transform.localPosition = boardGUI.GetLocalCoordinates(chessPiece);
  }

  void OnTriggerEnter(Collider collider)
  {
//    Debug.LogError("HI" + collider.gameObject);
  }
}
