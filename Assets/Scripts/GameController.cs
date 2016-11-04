using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
  public static ChessRuleSet currentRuleSet = new ChessRuleSet();

  public ChessBoard currentBoard;
  public GameObject mouseMarker;

  private ChessPiece.PieceColor currentTurn;
  private ChessPiece selectedChessPiece;
  private ChessPiece highlightedPiece;

  public ChessBoardGUI currentBoardGUI
  {
    get
    {
      return currentBoard.GetComponent<ChessBoardGUI>();
    }
  }

  void Start()
  {
    ChessRuleEvaluator.currentRuleSet = currentRuleSet;
    ChessRuleEvaluator.ResetBoard(currentBoard);
    currentBoardGUI.ResetBoardGUI(currentBoard);
    currentTurn = ChessPiece.PieceColor.WHITE;
    mouseMarker.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);
  }

  void Update()
  {
    Vector3 mousePos = Input.mousePosition;
    mousePos.z = 1.0f;
    Vector3 mousePosInWorld =
      GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(mousePos);
    float prevY = mousePosInWorld.y;
    int counter = 0;
    while (mousePosInWorld.y > currentBoard.transform.position.y)
    {
      counter++;
      mousePos.z += 0.1f;
      mousePosInWorld =
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(mousePos);
      mouseMarker.transform.position = mousePosInWorld;
      if (mousePosInWorld.y > prevY || counter > 1000)
      {
        selectedChessPiece = null;
        break;
      }
      else if (Physics.CheckSphere(mouseMarker.transform.position, 0.025f))
      {
        bool stopSearching = false;
        Collider[] colliders = Physics.OverlapSphere(mouseMarker.transform.position, 0.025f);
        foreach (Collider c in colliders)
        {
          if (c.gameObject.GetComponent<ChessPieceGUI>() != null)
          {
            highlightedPiece = c.gameObject.GetComponent<ChessPieceGUI>().chessPiece;
            stopSearching = true;
            break;
          }
          else
          {
            highlightedPiece = null;
          }
        }
        if (stopSearching)
        {
          break;
        }
      }
    }
    if (Input.GetMouseButtonUp(0))
    {
      if (selectedChessPiece == null)
      {
        if (highlightedPiece != null && highlightedPiece.pieceColor == currentTurn && currentBoard.IsPieceInPlay(highlightedPiece))
        {
          selectedChessPiece = highlightedPiece;
          highlightedPiece = null;
        }
      }
      else
      {
        Vector2 targetCoords = -Vector2.one;
        if (highlightedPiece != null && highlightedPiece != selectedChessPiece)
        {
          targetCoords = currentBoard.GetTileCoordinates(highlightedPiece);
        }
        else
        {
          targetCoords = currentBoardGUI.GetBoardTileCoordinates(mousePosInWorld);
        }
        if (ChessRuleEvaluator.CanMovePiece(selectedChessPiece, targetCoords, currentBoard))
        {
          currentBoard.MovePiece(selectedChessPiece, targetCoords);
          StartNextTurn();
        }
        selectedChessPiece = null;
      }
    }
  }

  public void StartNextTurn()
  {
    if (currentTurn == ChessPiece.PieceColor.WHITE)
    {
      currentTurn = ChessPiece.PieceColor.BLACK;
      GameObject.FindGameObjectWithTag("MainCameraContainer").GetComponent<Rigidbody>()
        .MoveRotation(Quaternion.Euler(60.0f, 180.0f, 0.0f));
    }
    else
    {
      currentTurn = ChessPiece.PieceColor.WHITE;
      GameObject.FindGameObjectWithTag("MainCameraContainer").GetComponent<Rigidbody>()
        .MoveRotation(Quaternion.Euler(60.0f, 0.0f, 0.0f));
    }
    selectedChessPiece = null;
    highlightedPiece = null;
  }
}
