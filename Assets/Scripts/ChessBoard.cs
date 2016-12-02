using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** The game logic pertaining to a chess board, including what tiles are occupied, what pieces have been eliminated,
 * and other game logic. 
 */
public class ChessBoard : MonoBehaviour
{
  public static int GetTypeCountLimit(ChessPiece.PieceType pieceType)
  {
    switch (pieceType)
    {
      case ChessPiece.PieceType.PAWN:
        return 8;
      case ChessPiece.PieceType.BISHOP:
        // Fall through
      case ChessPiece.PieceType.KNIGHT:
        // Fall through
      case ChessPiece.PieceType.ROOK:
        return 2;
      case ChessPiece.PieceType.QUEEN:
        // Fall through
      case ChessPiece.PieceType.KING:
        return 1;
      default:
        Debug.LogWarning("Unhandled chess piece type. Value:" + (int) pieceType);
        return 0;
    }
  }

  public readonly ChessPiece[,] boardTiles = new ChessPiece[8, 8];
  public readonly List<ChessPiece> eliminatedWhitePieces = new List<ChessPiece>(8);
  public readonly List<ChessPiece> eliminatedBlackPieces = new List<ChessPiece>(8);

  public int width
  {
    get
    {
      return boardTiles.GetLength(0);
    }
  }

  public int height
  {
    get
    {
      return boardTiles.GetLength(1);
    }
  }

  /** Clears the board and adds new chess pieces with the correct initial positions. */
  public void SetupBoard()
  {
    // UNITTEST
    eliminatedWhitePieces.Clear();
    eliminatedBlackPieces.Clear();
    for (int x = 0; x < boardTiles.GetLength(0); x++)
    {
      // White pawns
      boardTiles[x, 1] = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
      // Black pawns
      boardTiles[x, boardTiles.GetLength(1) - 2] =
        new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.BLACK);
      // Empty spaces
      for (int y = 2; y < 6; y++)
      {
        boardTiles[x, y] = null;
      }
      // All other pieces
      if (x < 4)
      {
        ChessPiece newPiece1 = null;
        ChessPiece newPiece2 = null;
        ChessPiece newPiece3 = null;
        ChessPiece newPiece4 = null;
        switch (x)
        {
          case 0:
            newPiece1 = new ChessPiece(ChessPiece.PieceType.ROOK, ChessPiece.PieceColor.WHITE);
            newPiece2 = new ChessPiece(ChessPiece.PieceType.ROOK, ChessPiece.PieceColor.WHITE);
            newPiece3 = new ChessPiece(ChessPiece.PieceType.ROOK, ChessPiece.PieceColor.BLACK);
            newPiece4 = new ChessPiece(ChessPiece.PieceType.ROOK, ChessPiece.PieceColor.BLACK);
            break;
          case 1:
            newPiece1 = new ChessPiece(ChessPiece.PieceType.KNIGHT, ChessPiece.PieceColor.WHITE);
            newPiece2 = new ChessPiece(ChessPiece.PieceType.KNIGHT, ChessPiece.PieceColor.WHITE);
            newPiece3 = new ChessPiece(ChessPiece.PieceType.KNIGHT, ChessPiece.PieceColor.BLACK);
            newPiece4 = new ChessPiece(ChessPiece.PieceType.KNIGHT, ChessPiece.PieceColor.BLACK);
            break;
          case 2:
            newPiece1 = new ChessPiece(ChessPiece.PieceType.BISHOP, ChessPiece.PieceColor.WHITE);
            newPiece2 = new ChessPiece(ChessPiece.PieceType.BISHOP, ChessPiece.PieceColor.WHITE);
            newPiece3 = new ChessPiece(ChessPiece.PieceType.BISHOP, ChessPiece.PieceColor.BLACK);
            newPiece4 = new ChessPiece(ChessPiece.PieceType.BISHOP, ChessPiece.PieceColor.BLACK);
            break;
          case 3:
            newPiece1 = new ChessPiece(ChessPiece.PieceType.QUEEN, ChessPiece.PieceColor.WHITE);
            newPiece2 = new ChessPiece(ChessPiece.PieceType.KING, ChessPiece.PieceColor.WHITE);
            newPiece3 = new ChessPiece(ChessPiece.PieceType.QUEEN, ChessPiece.PieceColor.BLACK);
            newPiece4 = new ChessPiece(ChessPiece.PieceType.KING, ChessPiece.PieceColor.BLACK);
            break;
          default:
            break;
        }
        boardTiles[x, 0] = newPiece1;
        boardTiles[width - 1 - x, 0] = newPiece2;
        boardTiles[x, height - 1] = newPiece3;
        boardTiles[width - 1 - x, height - 1] = newPiece4;
      }
    }
  }

  public void SetupBoardTest()
  {
    eliminatedWhitePieces.Clear();
    eliminatedBlackPieces.Clear();
    for (int x = 0; x < boardTiles.GetLength(0); x++)
    {
      for (int y = 0; y < boardTiles.GetLength(1); y++)
      {
        boardTiles[x, y] = null;
      }
    }
    boardTiles[1, 1] = new ChessPiece(ChessPiece.PieceType.ROOK, ChessPiece.PieceColor.WHITE);
    boardTiles[1, 3] = new ChessPiece(ChessPiece.PieceType.KING, ChessPiece.PieceColor.BLACK);
  }

  public void MovePiece(IntVector2 pieceAt, IntVector2 tileCoordinates)
  {
    if (!AreCoordinatesOverBoard(tileCoordinates))
    {
      Debug.LogWarning("Starting piece coordinates must be within the board.");
      return;
    }
    MovePiece(GetChessPieceAt(pieceAt), tileCoordinates);
  }

  public void MovePiece(ChessPiece piece, IntVector2 tileCoordinates)
  {
    if (piece == null)
    {
      Debug.LogWarning("Specified null for the chess piece.");
      return;
    }
    if (!AreCoordinatesOverBoard(tileCoordinates))
    {
      Debug.LogWarning("Cannot move a chess piece outside the board area.");
      return;
    }
    // Find the piece on the board
    IntVector2 posOfPiece = GetTileCoordinates(piece);
    if (posOfPiece.x < 0)
    {
      return;
    }
    if ((tileCoordinates - posOfPiece).stepMagnitude == 0)
    {
      Debug.LogWarning("Target position is the same as the current position.");
      return;
    }
    // Eliminate the piece (if any) at the target location
    ChessPiece atDest = GetChessPieceAt(tileCoordinates);
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
      // Count elimination as a movement for safety reasons
      atDest.movementsMade++;
    }
    // Move the piece
    piece.movementsMade++;
    boardTiles[posOfPiece.x, posOfPiece.y] = null;
    boardTiles[tileCoordinates.x, tileCoordinates.y] = piece;
  }

  public int GetTypeCount(ChessPiece.PieceType pieceType, ChessPiece.PieceColor pieceColor)
  {
    // UNITTEST
    int count = 0;
    for (int x = 0; x < boardTiles.GetLength(0); x++)
    {
      for (int y = 0; y < boardTiles.GetLength(1); y++)
      {
        ChessPiece piece = boardTiles[x, y];
        if (piece != null && piece.pieceType == pieceType && piece.pieceColor == pieceColor)
        {
          count++;
        }
      }
    }
    return count;
  }

  public ChessPiece GetChessPieceAt(IntVector2 tileCoordinates)
  {
    return GetChessPieceAt(tileCoordinates.x, tileCoordinates.y);
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

  public ChessPiece GetChessPiece(ChessPiece.PieceType pieceType, ChessPiece.PieceColor pieceColor)
  {
    for (int x = 0; x < boardTiles.GetLength(0); x++)
    {
      for (int y = 0; y < boardTiles.GetLength(1); y++)
      {
        ChessPiece piece = boardTiles[x, y];
        if (piece != null && piece.pieceType == pieceType && piece.pieceColor == pieceColor)
        {
          return piece;
        }
      }
    }
    return null;
  }

  public ChessPiece GetOpposingKing(ChessPiece.PieceColor opposedTo)
  {
    if (opposedTo == ChessPiece.PieceColor.WHITE)
    {
      return GetKing(ChessPiece.PieceColor.BLACK);
    }
    else
    {
      return GetKing(ChessPiece.PieceColor.WHITE);
    }
  }

  public ChessPiece GetKing(ChessPiece.PieceColor ofColor)
  {
    return GetChessPiece(ChessPiece.PieceType.KING, ofColor);
  }

  public IntVector2 GetTileCoordinates(ChessPiece piece)
  {
    if (piece == null)
    {
      Debug.LogWarning("Specified null for the chess piece.");
      return -IntVector2.one;
    }
    IntVector2 tileCoords = -IntVector2.one;
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        if (Object.ReferenceEquals(piece, boardTiles[x, y]))
        {
          tileCoords.x = x;
          tileCoords.y = y;
          break;
        }
      }
      if (tileCoords.x >= 0)
      {
        break;
      }
    }
    return tileCoords;
  }

  public bool IsPieceInPlay(ChessPiece piece)
  {
    // UNITTEST
    return GetTileCoordinates(piece).x >= 0;
  }

  public bool AreCoordinatesOverBoard(IntVector2 coordinates)
  {
    return AreCoordinatesOverBoard(coordinates.x, coordinates.y);
  }

  public bool AreCoordinatesOverBoard(int x, int y)
  {
    return 0 <= x && x < width && 0 <= y && y < height;
  }

  public List<ChessPiece> GetAllPiecesInPlay()
  {
    // UNITTEST
    List<ChessPiece> pieces = new List<ChessPiece>();
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        ChessPiece cp = boardTiles[x, y];
        if (cp != null)
        {
          pieces.Add(cp);
        }
      }
    }
    return pieces;
  }

  public List<ChessPiece> GetAllPiecesInPlay(ChessPiece.PieceColor ofColor)
  {
    // UNITTEST
    List<ChessPiece> pieces = new List<ChessPiece>();
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        ChessPiece cp = boardTiles[x, y];
        if (cp != null && cp.pieceColor == ofColor)
        {
          pieces.Add(cp);
        }
      }
    }
    return pieces;
  }

  public void LogBoardState()
  {
    string toPrint = "";
    for (int y = height - 1; y >= 0; y--)
    {
      for (int x = 0; x < width; x++)
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
