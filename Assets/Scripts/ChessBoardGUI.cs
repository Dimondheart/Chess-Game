using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ChessBoard))]
public class ChessBoardGUI : MonoBehaviour
{
  public GameObject[] whitePieceModels;
  public GameObject[] blackPieceModels;

  private List<ChessPieceGUI> pieces = new List<ChessPieceGUI>(32);

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
  /** Positioned at the extent of the first tile in the eliminatedWhite.
   * Must be the first child of eliminatedWhite.
   */
  public Transform eliminatedWhiteTileOneExtent;
  /** Positioned at the origin of the second tile in the eliminatedWhite.
   * Must be the second child of eliminatedWhite.
   */
  public Transform eliminatedWhiteTileTwoOrigin;
  /** Positioned and rotated at the origin corner of the area for the eliminated black pieces.
   * Must be the third child of the game object containing this script.
   */
  public Transform eliminatedBlackOrigin;
  /** Positioned at the extent of the first tile in the eliminatedBlack.
   * Must be the first child of eliminatedBlack.
   */
  public Transform eliminatedBlackTileOneExtent;
  /** Positioned at the origin of the second tile in the eliminatedBlack.
   * Must be the second child of eliminatedBlack.
   */
  public Transform eliminatedBlackTileTwoOrigin;

  public Vector3 GetLocalCoordinates(ChessPiece chessPiece)
  {
    // UNITTEST
    Vector3 localPos = Vector3.zero;
    if (board.eliminatedWhitePieces.Contains(chessPiece))
    {
      int tileNum = board.eliminatedWhitePieces.IndexOf(chessPiece);
      float tileWidth = eliminatedWhiteTileOneExtent.localPosition.x;
      float tileHeight = eliminatedWhiteTileOneExtent.localPosition.z;
      float xPerTile = eliminatedWhiteTileTwoOrigin.localPosition.x;
      float zPerTile = eliminatedWhiteTileTwoOrigin.localPosition.z;
      localPos = new Vector3(tileWidth / 2.0f + tileNum * xPerTile, 0.0f, tileHeight / 2.0f + tileNum * zPerTile);
    }
    if (board.IsPieceInPlay(chessPiece))
    {
      float tileSize = boardTilesExtent.localPosition.x / 8.0f;
      Vector2 tileCoords = board.GetTileCoordinates(chessPiece);
      localPos = new Vector3(tileSize * tileCoords.x + (tileSize / 2.0f), 0.0f, tileSize * tileCoords.y + (tileSize / 2.0f));
    }
    if (board.eliminatedBlackPieces.Contains(chessPiece))
    {
      int tileNum = board.eliminatedBlackPieces.IndexOf(chessPiece);
      float tileWidth = eliminatedBlackTileOneExtent.localPosition.x;
      float tileHeight = eliminatedBlackTileOneExtent.localPosition.z;
      float xPerTile = eliminatedBlackTileTwoOrigin.localPosition.x;
      float zPerTile = eliminatedBlackTileTwoOrigin.localPosition.z;
      localPos = new Vector3(tileWidth / 2.0f + tileNum * xPerTile, 0.0f, tileHeight / 2.0f + tileNum * zPerTile);
    }
    return localPos;
  }

  public void ResetBoardGUI(ChessBoard board)
  {
    // UNITTEST ?
    pieces.ForEach((ChessPieceGUI cpg) =>
      {
        Destroy(cpg.gameObject);
      }
    );
    pieces.Clear();
    for (int x = 0; x < board.boardTiles.GetLength(0); x++)
    {
      for (int y = 0; y < board.boardTiles.GetLength(1); y++)
      {
        ChessPiece currPiece = board.boardTiles[x, y];
        if (currPiece != null)
        {
          ChessPieceGUI newGUIElement = GetChessPieceGUIObject(currPiece).GetComponent<ChessPieceGUI>();
          pieces.Add(newGUIElement);
        }
      }
    }
  }

  public Vector2 GetBoardTileCoordinates(Vector3 worldCoordinates)
  {
    // UNITTEST
    Vector2 tileCoords = Vector2.zero;
    Vector3 temp = worldCoordinates - boardTilesOrigin.position;
    float tileWidth = boardTilesExtent.localPosition.x / 8.0f;
    float tileHeight = boardTilesExtent.localPosition.z/ 8.0f;
    tileCoords.x = Mathf.Ceil(temp.x / tileWidth) - 1.0f;
    tileCoords.y = Mathf.Ceil(temp.z / tileHeight) - 1.0f;
    return tileCoords;
  }

  private GameObject GetChessPieceGUIObject(ChessPiece chessPiece)
  {
    // UNITTEST ?
    GameObject piece = null;
    if (chessPiece.pieceColor == ChessPiece.PieceColor.WHITE)
    {
      piece = Instantiate(whitePieceModels[(int) chessPiece.pieceType]) as GameObject;
    }
    if (chessPiece.pieceColor == ChessPiece.PieceColor.BLACK)
    {
      piece = Instantiate(blackPieceModels[(int) chessPiece.pieceType]) as GameObject;
    }
    piece.GetComponent<ChessPieceGUI>().Setup(chessPiece, this, null);
    return piece;
  }
}
