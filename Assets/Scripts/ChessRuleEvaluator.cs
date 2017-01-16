using UnityEngine;
using System.Collections;

/** Contains functions for evaluating the legality of actions based on the current rule set. Also has utility
 * methods for actions such as reseting a board to start a new game.
 */
public static class ChessRuleEvaluator
{
  public static ChessRuleSet currentRuleSet = new ChessRuleSet("Default Rule Set");

  /** Check if the specified promotion is allowed, including any rules dependent on the internal states of the chess
   * pieces, and excluding any rules dependent on the current state of the chess board.
   */
  public static bool CanPromotePiece(ChessPiece toPromote, ChessPiece.PieceType newType)
  {
    // FEATURE add a method that returns the reason for false
    if (!currentRuleSet.promotionEnabled)
    {
      return false;
    }
    if (newType == ChessPiece.PieceType.KING)
    {
      return false;
    }
    if (newType == ChessPiece.PieceType.PAWN || toPromote.pieceType != ChessPiece.PieceType.PAWN)
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
    // FEATURE add a method that returns the reason for false
    // UNITTEST contingent on implementation of GetTypeCount(...) and GetTypeCountLimit(...)
    if (!CanPromotePiece(toPromote, promoteTo.pieceType))
    {
      return false;
    }
    // Cannot promote an eliminated piece
    if (board.IsEliminated(toPromote))
    {
      return false;
    }
    // Check if in the proper row
    if (toPromote.pieceColor == ChessPiece.PieceColor.WHITE && board.GetTileCoordinates(toPromote).y != board.width - 1)
    {
      return false;
    }
    if (toPromote.pieceColor == ChessPiece.PieceColor.BLACK && board.GetTileCoordinates(toPromote).y != 0)
    {
      return false;
    }
    if (currentRuleSet.promotionOnlyToCaptured)
    {
      if (!board.IsEliminated(promoteTo))
      {
        return false;
      }
    }
    if (currentRuleSet.promotionLimitTypeCount)
    {
      if (board.GetTypeCount(promoteTo.pieceType, toPromote.pieceColor) + 1 > ChessBoard.GetTypeCountLimit(promoteTo.pieceType))
      {
        return false;
      }
    }
//    if (!currentRuleSet.stalemateAllowed && DoesPromoteCauseStalemate(toPromote, promoteTo.pieceType, board))
//    {
//      return false;
//    }
    return true;
  }

  /** Check if the specified piece can legally be moved the specified change in tiles. */
  public static bool CanMovePiece(ChessPiece chessPiece, IntVector2 deltaTiles)
  {
    // FEATURE add a method that returns the reason for false
    // UNITTEST
    if (deltaTiles.IsZero())
    {
      return false;
    }
    switch (chessPiece.pieceType)
    {
      case ChessPiece.PieceType.PAWN:
        // Pawn normally only moves vertically
        if (!deltaTiles.IsVertical())
        {
          return false;
        }
        // Cannot move more than two tiles
        if (deltaTiles.stepMagnitude > 2)
        {
          return false;
        }
        // Make sure the pawn is allowed to move two if it is trying to
        if (deltaTiles.stepMagnitude == 2)
        {
          if (!currentRuleSet.pawnCanMoveTwoOnFirstMove || chessPiece.movementsMade > 0)
          {
            return false;
          }
        }
        // Cannot move pawns backwards
        if (chessPiece.pieceColor == ChessPiece.PieceColor.WHITE)
        {
          if (deltaTiles.y < 0)
          {
            return false;
          }
        }
        else
        {
          if (deltaTiles.y > 0)
          {
            return false;
          }
        }
        return true;

      case ChessPiece.PieceType.BISHOP:
        return deltaTiles.IsDiagonal();

      case ChessPiece.PieceType.KNIGHT:
        int dx = Mathf.Abs(deltaTiles.x);
        int dy = Mathf.Abs(deltaTiles.y);
        return (dx == 1 && dy == 2) || (dx == 2 && dy == 1);

      case ChessPiece.PieceType.ROOK:
        return deltaTiles.IsHorizontal() || deltaTiles.IsVertical();

      case ChessPiece.PieceType.QUEEN:
        return deltaTiles.IsDiagonal() || deltaTiles.IsHorizontal()|| deltaTiles.IsVertical();

      case ChessPiece.PieceType.KING:
        return deltaTiles.IsInUnitForm();

      default:
        Debug.LogWarning("Unhandled chess piece type. Value:" + (int) chessPiece.pieceType);
        return false;
    }
  }

  /** Check if a piece can legally be moved to the specified tile on the board. */
  public static bool CanMovePiece(ChessPiece piece, IntVector2 tileCoordinates, ChessBoard board)
  {
    // UNITTEST contingent on CanMovePiece(ChessPiece, IntVector2) and IsPathClear(...)
    // FEATURE add a method that returns the reason for false
    if (!CanMovePieceNoEndConditions(piece, tileCoordinates, board))
    {
      return false;
    }
//    if (DoesMoveCauseCheck(piece, tileCoordinates, piece.pieceColor, board))
//    {
//      return false;
//    }
//    if (!currentRuleSet.stalemateAllowed && DoesMoveCauseStalemate(piece, tileCoordinates, piece.pieceColor, board))
//    {
//      return false;
//    }
    return true;
  }

  /** Check if a piece can legally be moved to the specified tile on the board, excluding rules involving
   * check, checkmate, and stalemate.
   */
  public static bool CanMovePieceNoEndConditions(ChessPiece piece, IntVector2 tileCoordinates, ChessBoard board)
  {
    // UNITTEST contingent on CanMovePiece(ChessPiece, IntVector2) and IsPathClear(...)
    // FEATURE add a method that returns the reason for false
    IntVector2 startCoords = board.GetTileCoordinates(piece);
    if (!board.AreCoordinatesOverBoard(tileCoordinates))
    {
      return false;
    }
    IntVector2 deltaTiles = tileCoordinates - startCoords;
    // Check if the piece could legally move there based only on its normal movement pattern
    if (!CanMovePiece(piece, deltaTiles))
    {
      // Check for pawn diagonal movement
      if (piece.pieceType == ChessPiece.PieceType.PAWN)
      {
        if (deltaTiles.IsDiagonal() && deltaTiles.IsInUnitForm() && IsMovingForward(piece, deltaTiles))
        {
          ChessPiece atLoc = board.GetChessPieceAt(tileCoordinates);
          return atLoc != null && atLoc.pieceColor != piece.pieceColor;
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
    return true;
  }

  /** Check if the piece would encounter other pieces it cannot pass over while moving to the specified tile.
   * This method should only be called with verified legal target coordinates; illegal movement will return a
   * meaningless/undefined result. Legal coordinates are coordinates within the board that return true for
   * CanMovePiece(ChessPiece chessPiece, Vector2 deltaTiles).
   */
  public static bool IsPathClear(ChessPiece chessPiece, IntVector2 targetCoordinates, ChessBoard chessBoard)
  {
    // TODO implement
    // FEATURE add a method that returns the reason for false
    IntVector2 startCoords = chessBoard.GetTileCoordinates(chessPiece);
    IntVector2 deltaTiles = targetCoordinates - chessBoard.GetTileCoordinates(chessPiece);
    ChessPiece pieceAt = chessBoard.GetChessPieceAt(targetCoordinates);
    // Cannot move to a space if it is occupied by a piece of the same color
    if (pieceAt != null && pieceAt.pieceColor == chessPiece.pieceColor)
    {
      return false;
    }
    int moveDelta = 0;
    IntVector2 newTarget = IntVector2.zero;
    switch (chessPiece.pieceType)
    {
      case ChessPiece.PieceType.PAWN:
        moveDelta = deltaTiles.y;
        if (Mathf.Abs(moveDelta) == 2)
        {
          if (moveDelta > 0)
          {
            if (chessBoard.GetChessPieceAt(startCoords.x, startCoords.y + 1) != null)
            {
              return false;
            }
          }
          else
          {
            if (chessBoard.GetChessPieceAt(startCoords.x, startCoords.y - 1) != null)
            {
              return false;
            }
          }
        }
        return pieceAt == null;

      case ChessPiece.PieceType.BISHOP:
        if (deltaTiles.IsInUnitForm())
        {
          return true;
        }
        newTarget = targetCoordinates;
        if (deltaTiles.y > 0)
        {
          newTarget.y -= 1;
        }
        else
        {
          newTarget.y += 1;
        }
        if (deltaTiles.x > 0)
        {
          newTarget.x -= 1;
        }
        else
        {
          newTarget.x += 1;
        }
        return IsPathClearRecursive(chessPiece, newTarget, chessBoard);

      case ChessPiece.PieceType.KNIGHT:
        // The knight does not have to pass through intermediate tiles, so if it got this far it can move there
        return true;

      case ChessPiece.PieceType.ROOK:
        moveDelta = deltaTiles.x + deltaTiles.y;
        if (Mathf.Abs(moveDelta) != 1)
        {
          newTarget = targetCoordinates;
          if (deltaTiles.IsHorizontal())
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
        if (deltaTiles.IsInUnitForm())
        {
          return true;
        }
        newTarget = targetCoordinates;
        if (deltaTiles.IsDiagonal())
        {
          if (deltaTiles.y > 0)
          {
            newTarget.y -= 1;
          }
          else
          {
            newTarget.y += 1;
          }
          if (deltaTiles.x > 0)
          {
            newTarget.x -= 1;
          }
          else
          {
            newTarget.x += 1;
          }
        }
        else if (deltaTiles.IsHorizontal() || deltaTiles.IsVertical())
        {
          moveDelta = deltaTiles.x + deltaTiles.y;
          if (deltaTiles.IsHorizontal())
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
        }
        return IsPathClearRecursive(chessPiece, newTarget, chessBoard);

      case ChessPiece.PieceType.KING:
        return deltaTiles.IsInUnitForm();

      default:
        Debug.LogWarning("Unhandled chess piece type. Value:" + (int) chessPiece.pieceType);
        return false;
    }
  }

  private static bool IsPathClearRecursive(ChessPiece chessPiece, IntVector2 targetCoordinates, ChessBoard chessBoard)
  {
    // FEATURE add a method that returns the reason for false
    IntVector2 startCoords = chessBoard.GetTileCoordinates(chessPiece);
    IntVector2 deltaTiles = targetCoordinates - startCoords;
    ChessPiece pieceAt = chessBoard.GetChessPieceAt(targetCoordinates);
    // If the target is occupied, we cannot move through it
    if (pieceAt != null)
    {
      Debug.Log(pieceAt);
      return false;
    }
    IntVector2 newTarget = IntVector2.zero;
    // Check if the path up to the target is clear
    switch (chessPiece.pieceType)
    {
      case ChessPiece.PieceType.PAWN:
        // Handled completely in the non-recursive portion
        return true;

      case ChessPiece.PieceType.BISHOP:
        if (deltaTiles.IsInUnitForm())
        {
          return true;
        }
        newTarget = targetCoordinates;
        if (deltaTiles.y > 0)
        {
          newTarget.y -= 1;
        }
        else
        {
          newTarget.y += 1;
        }
        if (deltaTiles.x > 0)
        {
          newTarget.x -= 1;
        }
        else
        {
          newTarget.x += 1;
        }
        return IsPathClearRecursive(chessPiece, newTarget, chessBoard);

      case ChessPiece.PieceType.KNIGHT:
        // Handled completely in the non-recursive portion
        return true;

      case ChessPiece.PieceType.ROOK:
        int moveDelta = deltaTiles.x + deltaTiles.y;
        if (Mathf.Abs(moveDelta) != 1)
        {
          newTarget = targetCoordinates;
          if (deltaTiles.IsHorizontal())
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
        if (deltaTiles.IsInUnitForm())
        {
          return true;
        }
        newTarget = targetCoordinates;
        if (deltaTiles.IsDiagonal())
        {
          if (deltaTiles.y > 0)
          {
            newTarget.y -= 1;
          }
          else
          {
            newTarget.y += 1;
          }
          if (deltaTiles.x > 0)
          {
            newTarget.x -= 1;
          }
          else
          {
            newTarget.x += 1;
          }
        }
        else if (deltaTiles.IsHorizontal() || deltaTiles.IsVertical())
        {
          moveDelta = deltaTiles.x + deltaTiles.y;
          if (deltaTiles.IsHorizontal())
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
        }
        return IsPathClearRecursive(chessPiece, newTarget, chessBoard);

      case ChessPiece.PieceType.KING:
        // Handled completely in the non-recursive portion
        return true;

      default:
        Debug.LogWarning("Unhandled chess piece type. Value:" + (int) chessPiece.pieceType);
        return false;
    }
  }

  public static bool CanPerformCastling(ChessPiece.PieceColor ofColor, ChessBoard chessBoard)
  {
    // TODO implement
    return false;
  }

  public static bool IsEnPassantPossible(ChessPiece attackingPiece, ChessPiece defendingPiece, ChessBoard chessBoard)
  {
    // TODO implement
    return false;
  }

  /** Checks if the specified chess piece is move forward, meaning moving towards the enemy side of the board,
   * or towards there side of the board under certain circumstances
   * Movement with no vertical component will return false.
   */
  public static bool IsMovingForward(ChessPiece chessPiece, IntVector2 deltaTiles)
  {
    // UNITTEST
    if (chessPiece.pieceColor == ChessPiece.PieceColor.WHITE)
    {
      return deltaTiles.y > 0;
    }
    else
    {
      return deltaTiles.y < 0;
    }
  }

//  public static bool DoesPromoteCauseStalemate(ChessPiece toPromote, ChessPiece.PieceType promoteTo, ChessBoard chessBoard)
//  {
//    ChessPiece.PieceColor opposingColor =
//      toPromote.pieceColor == ChessPiece.PieceColor.BLACK ? ChessPiece.PieceColor.WHITE : ChessPiece.PieceColor.BLACK;
//    Vector2 coords = chessBoard.GetTileCoordinates(chessBoard.GetKing(opposingColor));
//    return AreSurroundingTilesDangerousAfterPromote(coords, toPromote, promoteTo, chessBoard)
//      && !IsTileDangerousAfterPromote(coords, toPromote, promoteTo, chessBoard);
//  }
//
//  public static bool DoesMoveCauseStalemate(ChessPiece toMove, Vector2 moveTo, ChessPiece.PieceColor against, ChessBoard chessBoard)
//  {
//    // TODO handle differently if the king is moving
//    Vector2 coords = chessBoard.GetTileCoordinates(chessBoard.GetKing(against));
//    return AreSurroundingTilesDangerousAfterMove(coords, toMove, moveTo, against, chessBoard)
//      && !IsTileDangerousAfterMove(coords, toMove, moveTo, against, chessBoard);
//  }
//
//  public static bool DoesPromoteCauseCheck(ChessPiece toPromote, ChessPiece.PieceType promoteTo, ChessBoard chessBoard)
//  {
//    ChessPiece.PieceColor opposingColor =
//      toPromote.pieceColor == ChessPiece.PieceColor.BLACK ? ChessPiece.PieceColor.WHITE : ChessPiece.PieceColor.BLACK;
//    Vector2 coords = chessBoard.GetTileCoordinates(chessBoard.GetKing(opposingColor));
//    return IsTileDangerousAfterPromote(coords, toPromote, promoteTo, chessBoard);
//  }
//
//  public static bool DoesMoveCauseCheck(ChessPiece toMove, Vector2 moveTo, ChessPiece.PieceColor against, ChessBoard chessBoard)
//  {
//    // TODO handle differently if the king is moving
//    Vector2 coords = chessBoard.GetTileCoordinates(chessBoard.GetKing(against));
//    return IsTileDangerousAfterMove(coords, toMove, moveTo, against, chessBoard);
//  }
//
//  public static bool DoesPromoteCauseCheckmate(ChessPiece toPromote, ChessPiece.PieceType promoteTo, ChessBoard chessBoard)
//  {
//    ChessPiece.PieceColor opposingColor =
//      toPromote.pieceColor == ChessPiece.PieceColor.BLACK ? ChessPiece.PieceColor.WHITE : ChessPiece.PieceColor.BLACK;
//    Vector2 coords = chessBoard.GetTileCoordinates(chessBoard.GetKing(opposingColor));
//    return IsTileDangerousAfterPromote(coords, toPromote, promoteTo, chessBoard)
//      && AreSurroundingTilesDangerousAfterPromote(coords, toPromote, promoteTo, chessBoard);
//  }
//
//  public static bool DoesMoveCauseCheckmate(ChessPiece toMove, Vector2 moveTo, ChessPiece.PieceColor against, ChessBoard chessBoard)
//  {
//    // TODO handle differently if the king is moving
//    Vector2 coords = chessBoard.GetTileCoordinates(chessBoard.GetKing(against));
//    return IsTileDangerousAfterMove(coords, toMove, moveTo, against, chessBoard)
//      && AreSurroundingTilesDangerousAfterMove(coords, toMove, moveTo, against, chessBoard);
//  }
//
//  private static bool AreSurroundingTilesDangerousAfterMove(Vector2 tile, ChessPiece toMove, Vector2 moveTo, ChessPiece.PieceColor against, ChessBoard chessBoard)
//  {
//    for (int dx = -1; dx < 2; dx++)
//    {
//      for (int dy = -1; dy < 2; dy++)
//      {
//        if (dx == 0 && dy == 0)
//        {
//          continue;
//        }
//        Vector2 newTile = tile;
//        newTile.x += dx;
//        newTile.y += dy;
//        if (!IsTileDangerousAfterMove(newTile, toMove, moveTo, against, chessBoard))
//        {
//          return false;
//        }
//      }
//    }
//    return true;
//  }
//
//  private static bool AreSurroundingTilesDangerousAfterPromote(Vector2 tile, ChessPiece toPromote, ChessPiece.PieceType promoteTo, ChessBoard chessBoard)
//  {
//    for (int dx = -1; dx < 2; dx++)
//    {
//      for (int dy = -1; dy < 2; dy++)
//      {
//        if (dx == 0 && dy == 0)
//        {
//          continue;
//        }
//        Vector2 newTile = tile;
//        newTile.x += dx;
//        newTile.y += dy;
//        if (!IsTileDangerousAfterPromote(newTile, toPromote, promoteTo, chessBoard))
//        {
//          return false;
//        }
//      }
//    }
//    return true;
//  }
//
//  private static bool IsTileDangerousAfterMove(Vector2 tile, ChessPiece toMove, Vector2 moveTo, ChessPiece.PieceColor against, ChessBoard chessBoard)
//  {
//    Vector2 tileEmptyOverride = chessBoard.GetTileCoordinates(toMove);
//    ChessPiece toCheck = null;
//    bool overrideTriggered = false;
//    for (int x = 0; x < chessBoard.boardTiles.GetLength(0); x++)
//    {
//      for (int y = 0; y < chessBoard.boardTiles.GetLength(1); y++)
//      {
//        if (overrideTriggered || (Mathf.Approximately(tileEmptyOverride.x, x) && Mathf.Approximately(tileEmptyOverride.y, y)))
//        {
//          toCheck = null;
//          overrideTriggered = true;
//        }
//        else
//        {
//          toCheck = chessBoard.boardTiles[x, y];
//        }
//        // If empty or the checked piece has the same color as the king
//        if (toCheck == null || toCheck.pieceColor == against)
//        {
//          continue;
//        }
//        // FIXME allows for check/checkmate against moving player
//        if (CanMovePieceNoEndConditions(toCheck, tile, chessBoard))
//        {
//          return true;
//        }
//      }
//    }
//    return false;
//  }
//
//  private static bool IsTileDangerousAfterPromote(Vector2 tile, ChessPiece toPromote, ChessPiece.PieceType promoteTo, ChessBoard chessBoard)
//  {
//    ChessPiece toCheck = null;
//    return false;
//  }
}
