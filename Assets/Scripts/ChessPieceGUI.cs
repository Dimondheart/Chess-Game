using UnityEngine;
using System.Collections;

public class ChessPieceGUI : MonoBehaviour
{
  public static readonly float movementTime = 1.5f;

  public ChessPiece chessPiece;
  public ChessBoardGUI boardGUI;

  private float movementStartTime;
  private Vector3 startPosition;

  void Start()
  {
    startPosition = transform.localPosition;
    movementStartTime = -1.0f;
  }

  void Update()
  {
    if (boardGUI.board.IsPieceInPlay(chessPiece))
    {
      transform.SetParent(boardGUI.boardTilesOrigin, true);
    }
    else if (boardGUI.board.IsEliminated(chessPiece))
    {
      if (chessPiece.pieceColor == ChessPiece.PieceColor.BLACK)
      {
        transform.SetParent(boardGUI.eliminatedBlackOrigin.parent, true);
      }
      else
      {
        transform.SetParent(boardGUI.eliminatedWhiteOrigin.parent, true);
      }
    }
//    transform.localPosition = boardGUI.GetLocalCoordinates(chessPiece);
    Vector3 targetCoords = boardGUI.GetLocalCoordinates(chessPiece);
    if (movementStartTime > 0.0f)
    {
      Debug.Log("a");
      if (Time.time - movementStartTime >= movementTime)
      {
        Debug.Log("b");
        transform.localPosition = targetCoords;
        movementStartTime = -1.0f;
      }
      else
      {
        Debug.Log("c");
        transform.localPosition =
          Vector3.Lerp(startPosition, targetCoords, (Time.time - movementStartTime) / movementTime);
      }
    }
    else
    {
      if (!(Mathf.Approximately(targetCoords.x, transform.localPosition.x) && Mathf.Approximately(targetCoords.y, transform.localPosition.y) && Mathf.Approximately(targetCoords.z, transform.localPosition.z)))
      {
        movementStartTime = Time.time;
        startPosition = transform.localPosition;
      }
    }
  }

  public void Setup(ChessPiece chessPiece, ChessBoardGUI boardGUI, GameObject icon)
  {
    this.chessPiece = chessPiece;
    this.boardGUI = boardGUI;
  }
}
