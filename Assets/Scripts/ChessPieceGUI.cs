using UnityEngine;
using System.Collections;

public class ChessPieceGUI : MonoBehaviour
{
  public ChessPiece chessPiece;
  public ChessBoardGUI boardGUI;

  void Start()
  {
    GetComponent<Renderer>().enabled = false;
  }

  void Update()
  {
    if (boardGUI.board.IsPieceInPlay(chessPiece))
    {
      transform.SetParent(boardGUI.boardTilesOrigin, true);
    }
    else if (boardGUI.board.eliminatedBlackPieces.Contains(chessPiece))
    {
      transform.SetParent(boardGUI.eliminatedBlackOrigin.parent, true);
    }
    else if (boardGUI.board.eliminatedWhitePieces.Contains(chessPiece))
    {
      transform.SetParent(boardGUI.eliminatedWhiteOrigin.parent, true);
    }
    else
    {
      GetComponent<Renderer>().enabled = false;
      return;
    }
    GetComponent<Renderer>().enabled = true;
    transform.localPosition = boardGUI.GetLocalCoordinates(chessPiece);
  }

  public void Setup(ChessPiece chessPiece, ChessBoardGUI boardGUI, GameObject icon)
  {
    this.chessPiece = chessPiece;
    this.boardGUI = boardGUI;
  }
}
