using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ChessBoard))]
public class ChessBoardGUI : MonoBehaviour
{
  public GameObject[] whitePieceModels;
  public GameObject[] blackPieceModels;
  public GameObject selectedPieceBasicMoveHighlight;
  public GameObject selectedPieceCaptureHighlight;
  public GameObject selectedPieceSpecialMoveHighlight;
  public GameObject additionalPieceSpecialMoveLine;
  public ChessPiece selectedChessPiece;

  private List<ChessPieceGUI> pieces = new List<ChessPieceGUI>(32);
  private ChessPiece lastSelectedPiece;
  private ChessPiece lastSelectedPieceMovementSet;
  private List<GameObject> selectedTileHighlights;
  private List<GameObject> specialsTileHighlights;

  public ChessBoard board
  {
    get
    {
      return GetComponent<ChessBoard>();
    }
  }

  /** Positioned at the lower-left corner of the lower-left tile, relative to the white side of the board.
   * Must be the first child of the game object containing this script.
   */
  public Transform boardTilesOrigin;
  /** Positioned at the upper-right corner of the upper-right tile, relative to the white side of the board.
   * Must be the first child of boardTiles.
   */
  public Transform boardTilesExtent;
  /** Positioned and rotated at the origin corner of the area for the eliminated white pieces.
   * Must be the second child of the game object containing this script.
   */
  public Transform eliminatedWhiteOrigin;
  /** Positioned and rotated at the origin corner of the area for the eliminated black pieces.
   * Must be the third child of the game object containing this script.
   */
  public Transform eliminatedBlackOrigin;
  /** Positioned at the extent of the first tile in the eliminatedWhite.
   * Must be the first child of eliminatedWhite.
   */
  public Transform eliminatedWhiteTileOneExtent
  {
    get
    {
      return eliminatedWhiteOrigin.parent.GetComponentsInChildren<Transform>()[2];
    }
  }
  /** Positioned at the origin of the second tile in the eliminatedWhite.
   * Must be the second child of eliminatedWhite.
   */
  public Transform eliminatedWhiteTileTwoOrigin
  {
    get
    {
      return eliminatedWhiteOrigin.parent.GetComponentsInChildren<Transform>()[3];
    }
  }
  /** Positioned at the extent of the first tile in the eliminatedBlack.
   * Must be the first child of eliminatedBlack.
   */
  public Transform eliminatedBlackTileOneExtent
  {
    get
    {
      return eliminatedBlackOrigin.parent.GetComponentsInChildren<Transform>()[2];
    }
  }
  /** Positioned at the origin of the second tile in the eliminatedBlack.
   * Must be the second child of eliminatedBlack.
   */
  public Transform eliminatedBlackTileTwoOrigin
  {
    get
    {
      return eliminatedBlackOrigin.parent.GetComponentsInChildren<Transform>()[3];
    }
  }

  void Start()
  {
    selectedTileHighlights = new List<GameObject>();
    specialsTileHighlights = new List<GameObject>(2);
  }

  void Update()
  {
    GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    // Update tile highlights
    if (selectedChessPiece != lastSelectedPiece)
    {
      // Clear current highlights
      selectedTileHighlights.ForEach((GameObject h) =>
        {
          GameObject.Destroy(h);
        }
      );
      selectedTileHighlights.Clear();
      selectedTileHighlights.ForEach((GameObject h) =>
        {
          GameObject.Destroy(h);
        }
      );
      // Add new highlights (if any)
      if (selectedChessPiece != null)
      {
        MovementSet moves = selectedChessPiece.GetMovementSet(board);
        // Standard & capture move highlights
        for (int x = 0; x < board.width; x++)
        {
          for (int y = 0; y < board.height; y++)
          {
            if (moves.movementBitboard[x, y])
            {
              GameObject newHighlight = null;
              if (board.GetChessPieceAt(x, y) == null)
              {
                newHighlight = Instantiate(selectedPieceBasicMoveHighlight, boardTilesOrigin.parent, false) as GameObject;
              }
              else
              {
                newHighlight = Instantiate(selectedPieceCaptureHighlight, boardTilesOrigin.parent, false) as GameObject;
              }
              selectedTileHighlights.Add(newHighlight);
              Vector3 newPos = newHighlight.transform.localPosition;
              newPos.x += (boardTilesExtent.transform.localPosition - boardTilesOrigin.transform.localPosition).x / 8.0f * x;
              newPos.z += (boardTilesExtent.transform.localPosition - boardTilesOrigin.transform.localPosition).z / 8.0f * y;
              newHighlight.transform.localPosition = newPos;
            }
          }
        }
        // Special move highlights
        foreach (SpecialMovement special in moves.specialMovements)
        {
          GameObject newHighlight = Instantiate(selectedPieceSpecialMoveHighlight, boardTilesOrigin.parent, false) as GameObject;
          selectedTileHighlights.Add(newHighlight);
          // move over the correct tile
          Vector3 newPos = newHighlight.transform.localPosition;
          newPos.x += (boardTilesExtent.transform.localPosition - boardTilesOrigin.transform.localPosition).x / 8.0f * special.targetTile.x;
          newPos.z += (boardTilesExtent.transform.localPosition - boardTilesOrigin.transform.localPosition).z / 8.0f * special.targetTile.y;
          newHighlight.transform.localPosition = newPos;
          if (special.additionalPieceMoved != null)
          {
            IntVector2 start = board.GetTileCoordinates(special.additionalPieceMoved);
            IntVector2 end = special.additionalPieceMovedTarget;
            newHighlight = Instantiate(additionalPieceSpecialMoveLine, boardTilesOrigin.parent, false) as GameObject;
            selectedTileHighlights.Add(newHighlight);
            additionalPieceSpecialMoveLine.GetComponent<LineRenderer>().SetPositions(new Vector3[] {
                new Vector3(start.x, 0.0f, start.y),
                new Vector3(end.x, 0.0f, end.y)
              });
          }
          // TODO highlight eliminated piece
          // TODO highlight additional moved piece
        }
      }
      lastSelectedPiece = selectedChessPiece;
    }
    // Selection, movement, turn advancement
    if (!(gameController.GetComponent<MainMenuController>().inMainMenu || gameController.GetComponent<PauseMenuController>().isOpen))
    {
      if (Input.GetMouseButtonUp(0))
      {
        GameObject highlighted = gameController.mouse3D.highlighted;
        if (selectedChessPiece == null)
        {
          if (highlighted != null && highlighted.GetComponent<ChessPieceGUI>() != null)
          {
            ChessPiece highlightedPiece = highlighted.GetComponent<ChessPieceGUI>().chessPiece;
            if (highlightedPiece.pieceColor == gameController.currentTurn && board.IsPieceInPlay(highlightedPiece))
            {
              selectedChessPiece = highlightedPiece;
            }
          }
        }
        else
        {
          IntVector2 targetCoords = -IntVector2.one;
          if (highlighted != null)
          {
            if (highlighted.GetComponent<ChessPieceGUI>() != null)
            {
              ChessPiece highlightedPiece = highlighted.GetComponent<ChessPieceGUI>().chessPiece;
              if (highlightedPiece != selectedChessPiece)
              {
                if (highlightedPiece.pieceColor == selectedChessPiece.pieceColor)
                {
                  selectedChessPiece = highlightedPiece;
                }
                else
                {
                  targetCoords = board.GetTileCoordinates(highlightedPiece);
                }
              }
            }
            else if (Object.ReferenceEquals(highlighted.GetComponent<ChessBoardGUI>(), this))
            {
              targetCoords = GetBoardTileCoordinates(gameController.mouse3D.transform.position);
            }
            if (ChessRuleEvaluator.CanMovePiece(selectedChessPiece, targetCoords, board))
            {
              board.MovePiece(selectedChessPiece, targetCoords);
              selectedChessPiece = null;
              gameController.StartNextTurn();
            }
          }
          else
          {
            selectedChessPiece = null;
          }
        }
      }
    }
  }

  void OnDrawGizmos()
  {
    if (selectedChessPiece != null && GetGUIPiece(selectedChessPiece) != null)
    {
      Gizmos.color = Color.green;
      Bounds b = GetGUIPiece(selectedChessPiece).GetComponent<Renderer>().bounds;
      Gizmos.DrawWireCube(b.center, b.size);
    }
  }

  public Vector3 GetLocalCoordinates(ChessPiece chessPiece)
  {
    // UNITTEST
    Vector3 localPos = Vector3.zero;
    if (board.IsEliminated(chessPiece))
    {
      if (chessPiece.pieceColor == ChessPiece.PieceColor.BLACK)
      {
        int tileNum = board.GetOrderEliminated(chessPiece);
        float tileWidth = eliminatedBlackTileOneExtent.localPosition.x - eliminatedBlackOrigin.localPosition.x;
        float tileHeight = eliminatedBlackTileOneExtent.localPosition.z - eliminatedBlackOrigin.localPosition.z;
        float xPerTile = eliminatedBlackTileTwoOrigin.localPosition.x - eliminatedBlackOrigin.localPosition.x;
        float zPerTile = eliminatedBlackTileTwoOrigin.localPosition.z - eliminatedBlackOrigin.localPosition.z;
        localPos = new Vector3(tileWidth / 2.0f + tileNum * xPerTile, 0.0f, tileHeight / 2.0f + tileNum * zPerTile);
      }
      else
      {
        int tileNum = board.GetOrderEliminated(chessPiece);
        float tileWidth = eliminatedWhiteTileOneExtent.localPosition.x - eliminatedWhiteOrigin.localPosition.x;
        float tileHeight = eliminatedWhiteTileOneExtent.localPosition.z - eliminatedWhiteOrigin.localPosition.z;
        float xPerTile = eliminatedWhiteTileTwoOrigin.localPosition.x - eliminatedWhiteOrigin.localPosition.x;
        float zPerTile = eliminatedWhiteTileTwoOrigin.localPosition.z - eliminatedWhiteOrigin.localPosition.z;
        localPos = new Vector3(tileWidth / 2.0f + tileNum * xPerTile, 0.0f, tileHeight / 2.0f + tileNum * zPerTile);
      }
    }
    else if (board.IsPieceInPlay(chessPiece))
    {
      float tileSize = boardTilesExtent.localPosition.x / 8.0f;
      IntVector2 tileCoords = board.GetTileCoordinates(chessPiece);
      localPos = new Vector3(tileSize * tileCoords.x + (tileSize / 2.0f), 0.0f, tileSize * tileCoords.y + (tileSize / 2.0f));
    }
    return localPos;
  }

  public void ResetBoardGUI()
  {
    // UNITTEST ?
    pieces.ForEach((ChessPieceGUI cpg) =>
      {
        cpg.gameObject.GetComponent<Renderer>().enabled = false;
        Destroy(cpg.gameObject);
      }
    );
    pieces.Clear();
    for (int x = 0; x < board.width; x++)
    {
      for (int y = 0; y < board.height; y++)
      {
        ChessPiece currPiece = board.GetChessPieceAt(x, y);
        if (currPiece != null)
        {
          ChessPieceGUI newGUIElement = GetChessPieceGUIObject(currPiece).GetComponent<ChessPieceGUI>();
          pieces.Add(newGUIElement);
        }
      }
    }
  }

  public IntVector2 GetBoardTileCoordinates(Vector3 worldCoordinates)
  {
    // UNITTEST
    IntVector2 tileCoords = IntVector2.zero;
    Vector3 temp = worldCoordinates - boardTilesOrigin.position;
    float tileWidth = boardTilesExtent.localPosition.x / 8.0f;
    float tileHeight = boardTilesExtent.localPosition.z / 8.0f;
    tileCoords.x = Mathf.CeilToInt(temp.x / tileWidth) - 1;
    tileCoords.y = Mathf.CeilToInt(temp.z / tileHeight) - 1;
    return tileCoords;
  }

  public ChessPieceGUI GetGUIPiece(ChessPiece forPiece)
  {
    ChessPieceGUI gui = pieces.Find((ChessPieceGUI cpg) =>
      {
        return cpg.chessPiece != null && Object.ReferenceEquals(cpg.chessPiece, forPiece);
      }
    );
    return gui;
  }

  private GameObject GetChessPieceGUIObject(ChessPiece chessPiece)
  {
    // UNITTEST ?
    GameObject piece = null;
    if (chessPiece.pieceColor == ChessPiece.PieceColor.WHITE)
    {
      piece = Instantiate(whitePieceModels[(int) chessPiece.pieceType]) as GameObject;
    }
    else
    {
      piece = Instantiate(blackPieceModels[(int) chessPiece.pieceType]) as GameObject;
    }
    piece.GetComponent<ChessPieceGUI>().Setup(chessPiece, this, null);
    return piece;
  }
}
