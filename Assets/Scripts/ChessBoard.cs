using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** The game logic pertaining to a chess board, including what tiles are occupied, what pieces have been eliminated,
 * and other game logic. 
 */
public class ChessBoard : MonoBehaviour
{
  public readonly ChessPiece[,] boardTiles = new ChessPiece[8, 8];
  public readonly List<ChessPiece> eliminatedWhitePieces = new List<ChessPiece>(8);
  public readonly List<ChessPiece> eliminatedBlackPieces = new List<ChessPiece>(8);


  public void MovePiece(Vector2 pieceAt, Vector2 tileCoordinates)
  {
    MovePiece(boardTiles[(int) pieceAt.x, (int) pieceAt.y], tileCoordinates);
  }

  public void MovePiece(ChessPiece piece, Vector2 tileCoordinates)
  {
    // FIXME don't allow moving to same tile
    if (piece == null)
    {
      Debug.LogWarning("Specified null for the chess piece.");
      return;
    }
    // Find the piece on the board
    Vector2 posOfPiece = GetTileCoordinates(piece);
    if (posOfPiece.x < 0.0f)
    {
      Debug.LogWarning("Specified (non-null) piece not found in this board.");
      return;
    }
    // Eliminate the piece (if any) at the target location
    ChessPiece atDest = boardTiles[(int) tileCoordinates.x, (int) tileCoordinates.y];
    if (atDest != null)
    {
      if (atDest.pieceColor == ChessPiece.PieceColor.WHITE)
      {
        eliminatedWhitePieces.Add(atDest);
      }
      else
      {
        eliminatedBlackPieces.Add(atDest);
      }
      atDest.movementsMade++;
    }
    // Actually move the piece
    piece.movementsMade++;
    boardTiles[(int) posOfPiece.x, (int) posOfPiece.y] = null;
    boardTiles[(int) tileCoordinates.x, (int) tileCoordinates.y] = piece;
    LogBoardState();
  }

  public ChessPiece GetChessPieceAt(Vector2 tileCoordinates)
  {
    return GetChessPieceAt(Mathf.RoundToInt(tileCoordinates.x), Mathf.RoundToInt(tileCoordinates.y));
  }

  public ChessPiece GetChessPieceAt(int x, int y)
  {
    if (!AreCoordinatesOverBoard(x, y))
    {
      Debug.LogWarning("Specified invalid tile coordinates.");
      return null;
    }
    return boardTiles[x, y];
  }

  public Vector2 GetTileCoordinates(ChessPiece piece)
  {
    if (piece == null)
    {
      Debug.LogWarning("Specified null for the chess piece.");
      return -Vector2.one;
    }
    Vector2 tileCoords = -Vector2.one;
    for (int x = 0; x < boardTiles.GetLength(0); x++)
    {
      for (int y = 0; y < boardTiles.GetLength(1); y++)
      {
        if (Object.ReferenceEquals(piece, boardTiles[x, y]))
        {
          tileCoords.x = x;
          tileCoords.y = y;
          break;
        }
      }
      if (!Mathf.Approximately(tileCoords.x, -1.0f))
      {
        break;
      }
    }
    if (tileCoords.x < 0.0f)
    {
      Debug.LogWarning("Specified (non-null) piece not found in this board.");
    }
    return tileCoords;
  }

  public bool IsPieceInPlay(ChessPiece piece)
  {
    // UNITTEST
    return !Mathf.Approximately(GetTileCoordinates(piece).x, -1.0f);
  }

  public bool AreCoordinatesOverBoard(Vector2 coordinates)
  {
    return AreCoordinatesOverBoard((int) coordinates.x, (int) coordinates.y);
  }

  public bool AreCoordinatesOverBoard(int x, int y)
  {
    return 0 <= (int) x && (int) x < 8 && 0 <= (int) y && (int) y < 8;
  }

  public void LogBoardState()
  {
    string toPrint = "";
    for (int y = boardTiles.GetLength(1) - 1; y >= 0; y--)
    {
      for (int x = 0; x < boardTiles.GetLength(0); x++)
      {
        if (boardTiles[x, y] != null)
        {
          switch (boardTiles[x, y].pieceType)
          {
            case ChessPiece.PieceType.PAWN:
              toPrint += "P";
              break;
            case ChessPiece.PieceType.BISHOP:
              toPrint += "B";
              break;
            case ChessPiece.PieceType.KNIGHT:
              toPrint += "N";
              break;
            case ChessPiece.PieceType.ROOK:
              toPrint += "R";
              break;
            case ChessPiece.PieceType.QUEEN:
              toPrint += "Q";
              break;
            case ChessPiece.PieceType.KING:
              toPrint += "K";
              break;
            default:
              toPrint += "?";
              break;
          }
        }
        else
        {
          toPrint += "-";
        }
      }
      toPrint += "\n";
    }
    Debug.Log(toPrint);
  }
}
