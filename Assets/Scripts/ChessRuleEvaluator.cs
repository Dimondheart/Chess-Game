using UnityEngine;
using System.Collections;

/** Contains functions for evaluating the legality of actions based on the current rule set. Also has utility
 * methods for actions such as reseting a board to start a new game.
 */
[System.Serializable]
public static class ChessRuleEvaluator
{
  public static ChessRuleSet currentRuleSet = new ChessRuleSet("Default Rule Set");

  /** Check if the specified promotion is allowed, including any rules dependent on the internal states of the chess
   * pieces, and excluding any rules dependent on the current state of the chess board.
   */
  public static bool CanPromotePiece(ChessPiece toPromote, ChessPiece.PieceType newType)
  {
    // TODO add a method that returns the reason for false
    if (newType == ChessPiece.PieceType.KING)
    {
      return false;
    }
    if (!currentRuleSet.promotionEnabled)
    {
      return false;
    }
    if (toPromote.pieceType != ChessPiece.PieceType.PAWN || newType == ChessPiece.PieceType.PAWN)
    {
      return false;
    }
    return true;
  }

  /** Check if the specified promotion is allowed, including any rules dependent on the current state of the
   * chess board and the current states of the chess pieces.
   */
  public static bool CanPromotePiece(ChessPiece toPromote, ChessPiece promoteTo, ChessBoard board)
  {
    // TODO add a method that returns the reason for false
    // UNITTEST contingent on implementation of GetTypeCount(...) and GetTypeCountLimit(...)
    if (!CanPromotePiece(toPromote, promoteTo.pieceType))
    {
      return false;
    }
    // Cannot promote an eliminated piece
    if (board.eliminatedWhitePieces.Contains(toPromote) || board.eliminatedBlackPieces.Contains(toPromote))
    {
      return false;
    }
    // Check if in the proper row
    if (toPromote.pieceColor == ChessPiece.PieceColor.WHITE &&
      (int) board.GetTileCoordinates(toPromote).y != board.boardTiles.GetLength(1) - 1
    )
    {
      return false;
    }
    if (toPromote.pieceColor == ChessPiece.PieceColor.BLACK && (int) board.GetTileCoordinates(toPromote).y != 0)
    {
      return false;
    }
    if (currentRuleSet.promotionOnlyToCaptured)
    {
      if (!(board.eliminatedWhitePieces.Contains(promoteTo) || board.eliminatedBlackPieces.Contains(promoteTo)))
      {
        return false;
      }
    }
    if (currentRuleSet.promotionLimitTypeCount)
    {
      if (GetTypeCount(promoteTo.pieceType, board) + 1 > GetTypeCountLimit(promoteTo.pieceType))
      {
        return false;
      }
    }
    // FEATURE check if promotion would cause stalemate
    return true;
  }

  /** Check if the specified piece can legally be moved the specified change in tiles. */
  public static bool CanMovePiece(ChessPiece chessPiece, Vector2 deltaTiles)
  {
    // TODO add a method that returns the reason for false
    // TODO implement
    if (Mathf.Approximately(deltaTiles.magnitude, 0.0f))
    {
      return false;
    }
    switch (chessPiece.pieceType)
    {
      case ChessPiece.PieceType.PAWN:
        if (Mathf.RoundToInt(Mathf.Abs(deltaTiles.y)) > 2)
        {
          return false;
        }
        if (!Mathf.Approximately(deltaTiles.x, 0.0f))
        {
          return false;
        }
        if (chessPiece.movementsMade > 0 && Mathf.Approximately(Mathf.Abs(deltaTiles.y), 2.0f))
        {
          return false;
        }
        if (chessPiece.pieceColor == ChessPiece.PieceColor.WHITE)
        {
          if (deltaTiles.y < 0.0f)
          {
            return false;
          }
        }
        else
        {
          if (deltaTiles.y > 0.0f)
          {
            return false;
          }
        }
        return true;
      case ChessPiece.PieceType.BISHOP:
        if (!Mathf.Approximately(Mathf.Abs(deltaTiles.x), Mathf.Abs(deltaTiles.y)))
        {
          return false;
        }
        break;
      case ChessPiece.PieceType.KNIGHT:
        int dx = (int) Mathf.Round(Mathf.Abs(deltaTiles.x));
        int dy = (int) Mathf.Round(Mathf.Abs(deltaTiles.y));
        if (!((Mathf.Approximately(dx, 1.0f) && Mathf.Approximately(dy, 2.0f)) || (Mathf.Approximately(dx, 2.0f) && Mathf.Approximately(dy, 1.0f))))
        {
          return false;
        }
        break;
      case ChessPiece.PieceType.ROOK:
        if (!Mathf.Approximately(deltaTiles.x, 0.0f) && !Mathf.Approximately(deltaTiles.y, 0.0f))
        {
          return false;
        }
        return true;
      case ChessPiece.PieceType.QUEEN:
        break;
      case ChessPiece.PieceType.KING:
        break;
      default:
        return false;
    }
    return true;
  }

  /** Check if a piece can legally be moved to the specified tile on the board. */
  public static bool CanMovePiece(ChessPiece piece, Vector2 tileCoordinates, ChessBoard board)
  {
    // UNITTEST contingent on CanMovePiece(ChessPiece, Vector2) and IsPathClear(...)
    // TODO add a method that returns the reason for false
    Vector2 startCoords = board.GetTileCoordinates(piece);
    if (!board.AreCoordinatesOverBoard(tileCoordinates))
    {
      return false;
    }
    Vector2 deltaTiles = tileCoordinates - startCoords;
    // Check if the piece could legally move there based only on its normal movement pattern
    if (!CanMovePiece(piece, deltaTiles))
    {
      if (piece.pieceType == ChessPiece.PieceType.PAWN)
      {
        if (Mathf.Approximately(Mathf.Abs(deltaTiles.x), 1.0f) && Mathf.Approximately(Mathf.Abs(deltaTiles.y), 1.0f))
        {
          Debug.Log("Got there");
          int checkX = Mathf.RoundToInt(startCoords.x + deltaTiles.x);
          int checkY = Mathf.RoundToInt(startCoords.y + deltaTiles.y);
          Vector2 tileToCheck = new Vector2((float) checkX, (float) checkY);
          if (!board.AreCoordinatesOverBoard(tileToCheck))
          {
            Debug.Log("Got there2");
            return false;
          }
          ChessPiece atLoc = board.GetChessPieceAt(tileToCheck);
          Debug.Log("It:" + atLoc);
          if (atLoc == null || atLoc.pieceColor == piece.pieceColor)
          {
            return false;
          }
          else
          {
            return true;
          }
        }
        else
        {
          return false;
        }
      }
      else
      {
        return false;
      }
    }
    if (!IsPathClear(piece, tileCoordinates, board))
    {
      return false;
    }
    // TODO check if the move would cause check
    // FEATURE check for stalemate rule
    return true;
  }

  /** Check if the piece would encounter other pieces it cannot pass over while moving to the specified tile.
   * This method should only be called with verified legal target coordinates; illegal movement will return a
   * meaningless/undefined result. Legal coordinates are coordinates that return true for
   * CanMovePiece(ChessPiece chessPiece, Vector2 deltaTiles).
   */
  public static bool IsPathClear(ChessPiece chessPiece, Vector2 targetCoordinates, ChessBoard chessBoard)
  {
    // TODO implement
    // FEATURE add a method that returns the reason for false
    Vector2 startCoords = chessBoard.GetTileCoordinates(chessPiece);
    Vector2 deltaTiles = targetCoordinates - chessBoard.GetTileCoordinates(chessPiece);
    ChessPiece pieceAt = chessBoard.GetChessPieceAt(targetCoordinates);
    if (pieceAt != null && pieceAt.pieceColor == chessPiece.pieceColor)
    {
      return false;
    }
    int moveDelta = 0;
    switch (chessPiece.pieceType)
    {
      case ChessPiece.PieceType.PAWN:
        moveDelta = Mathf.RoundToInt(deltaTiles.y);
        if (Mathf.Abs(moveDelta) != 1)
        {
          if (moveDelta > 0)
          {
            if (chessBoard.GetChessPieceAt(Mathf.RoundToInt(startCoords.x), Mathf.RoundToInt(startCoords.y) + 1) != null)
            {
              return false;
            }
          }
          else
          {
            if (chessBoard.GetChessPieceAt(Mathf.RoundToInt(startCoords.x), Mathf.RoundToInt(startCoords.y) - 1) != null)
            {
              return false;
            }
          }
        }
        return pieceAt == null;
      case ChessPiece.PieceType.BISHOP:
        break;
      case ChessPiece.PieceType.KNIGHT:
        break;
      case ChessPiece.PieceType.ROOK:
        bool moveHoriz = !Mathf.Approximately(deltaTiles.x, 0.0f);
        moveDelta = (int) Mathf.Round(deltaTiles.x) + (int) Mathf.Round(deltaTiles.y);
        if (Mathf.Abs(moveDelta) != 1)
        {
          Vector2 newTarget = targetCoordinates;
          if (moveHoriz)
          {
            if (moveDelta > 0)
            {
              newTarget.x -= 1;
            }
            else
            {
              newTarget.x += 1;
            }
          }
          else
          {
            if (moveDelta > 0)
            {
              newTarget.y -= 1;
            }
            else
            {
              newTarget.y += 1;
            }
          }
          return IsPathClearRecursive(chessPiece, newTarget, chessBoard);
        }
        return true;
      case ChessPiece.PieceType.QUEEN:
        break;
      case ChessPiece.PieceType.KING:
        break;
      default:
        return false;
    }
    return true;
  }

  private static bool IsPathClearRecursive(ChessPiece chessPiece, Vector2 targetCoordinates, ChessBoard chessBoard)
  {
    Vector2 startCoords = chessBoard.GetTileCoordinates(chessPiece);
    Vector2 deltaTiles = targetCoordinates - chessBoard.GetTileCoordinates(chessPiece);
    if (!CanMovePiece(chessPiece, targetCoordinates - startCoords))
    {
      return false;
    }
    // TODO implement
    // FEATURE add a method that returns the reason for false
    switch (chessPiece.pieceType)
    {
      case ChessPiece.PieceType.PAWN:
        // Handled completely in the non-recursive portion
        return true;
      case ChessPiece.PieceType.BISHOP:
        break;
      case ChessPiece.PieceType.KNIGHT:
        break;
      case ChessPiece.PieceType.ROOK:
        bool moveHoriz = !Mathf.Approximately(deltaTiles.x, 0.0f);
        int moveDelta = (int) Mathf.Round(deltaTiles.x) + (int) Mathf.Round(deltaTiles.y);
        if (Mathf.Abs(moveDelta) != 1)
        {
          Vector2 newTarget = targetCoordinates;
          if (moveHoriz)
          {
            if (moveDelta > 0)
            {
              newTarget.x -= 1;
            }
            else
            {
              newTarget.x += 1;
            }
          }
          else
          {
            if (moveDelta > 0)
            {
              newTarget.y -= 1;
            }
            else
            {
              newTarget.y += 1;
            }
          }
          return chessBoard.GetChessPieceAt(targetCoordinates) == null && IsPathClear(chessPiece, newTarget, chessBoard);
        }
        return chessBoard.GetChessPieceAt(targetCoordinates) == null;
      case ChessPiece.PieceType.QUEEN:
        break;
      case ChessPiece.PieceType.KING:
        break;
      default:
        return false;
    }
    return true;
  }

  public static bool CanPerformCastling(ChessPiece.PieceColor ofColor, ChessBoard chessBoard)
  {
    // TODO implement
    return true;
  }

  public static bool IsEnPassantPossible(ChessPiece attackingPiece, ChessPiece defendingPiece, ChessBoard chessBoard)
  {
    // TODO implement
    return true;
  }

  public static int GetTypeCountLimit(ChessPiece.PieceType pieceType)
  {
    // TODO implement
    return 1;
  }

  public static int GetTypeCount(ChessPiece.PieceType pieceType, ChessBoard board)
  {
    // TODO implement
    return 1;
  }

  public static void ResetBoard(ChessBoard board)
  {
    // UNITTEST
    board.eliminatedWhitePieces.Clear();
    board.eliminatedBlackPieces.Clear();
    for (int x = 0; x < board.boardTiles.GetLength(0); x++)
    {
      // White pawns
      board.boardTiles[x, 1] = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
      // Black pawns
      board.boardTiles[x, board.boardTiles.GetLength(1) - 2] =
        new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.BLACK);
      // Empty spaces
      for (int y = 2; y < 6; y++)
      {
        board.boardTiles[x, y] = null;
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
        board.boardTiles[x, 0] = newPiece1;
        board.boardTiles[board.boardTiles.GetLength(0) - 1 - x, 0] = newPiece2;
        board.boardTiles[x, board.boardTiles.GetLength(1) - 1] = newPiece3;
        board.boardTiles[board.boardTiles.GetLength(0) - 1 - x, board.boardTiles.GetLength(1) - 1] = newPiece4;
      }
    }
  }
}
