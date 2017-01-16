using UnityEngine;
using System.Collections;

public class Pawn : ChessPiece
{
  public Pawn(ChessPiece.PieceColor color) : base(ChessPiece.PieceType.PAWN, color)
  {
  }

  public override Bitboard GetBasicMoves(ChessBoard chessBoard)
  {
    Bitboard moves = new Bitboard();
    IntVector2 currentTile = chessBoard.GetTileCoordinates(this);
    if (currentTile.x < 0)
    {
      Debug.LogError("Pawn is not on the specified chess board");
      return null;
    }
    IntVector2 forwardDelta = IntVector2.up;
    if (pieceColor == PieceColor.BLACK)
    {
      forwardDelta = -IntVector2.up;
    }
    IntVector2 check = currentTile + forwardDelta;
    if (chessBoard.AreCoordinatesOverBoard(check))
    {
      moves[check.x, check.y] = chessBoard.GetChessPieceAt(check) == null;
      check += forwardDelta;
      if (movementsMade == 0 && chessBoard.AreCoordinatesOverBoard(check) && chessBoard.GetChessPieceAt(check) == null)
      {
        moves[check.x, check.y] = true;
      }
      check = currentTile + forwardDelta + IntVector2.right;
      if (chessBoard.AreCoordinatesOverBoard(check) && chessBoard.GetChessPieceAt(check) != null && chessBoard.GetChessPieceAt(check).pieceColor != pieceColor)
      {
        moves[check.x, check.y] = true;
      }
      check = currentTile + forwardDelta - IntVector2.right;
      if (chessBoard.AreCoordinatesOverBoard(check) && chessBoard.GetChessPieceAt(check) != null && chessBoard.GetChessPieceAt(check).pieceColor != pieceColor)
      {
        moves[check.x, check.y] = true;
      }
    }
    return moves;
  }

  public override SpecialMovement[] GetSpecialMoves(ChessBoard chessBoard)
  {
    IntVector2 currentTile = chessBoard.GetTileCoordinates(this);
    if ((pieceColor == PieceColor.BLACK && currentTile.y != 3) || (pieceColor == PieceColor.WHITE && currentTile.y != 4))
    {
      return new SpecialMovement[0];
    }
    SpecialMovement special1 = null;
    SpecialMovement special2 = null;
    ChessPiece left = chessBoard.GetChessPieceAt(currentTile.x - 1, currentTile.y);
    ChessPiece right = chessBoard.GetChessPieceAt(currentTile.x + 1, currentTile.y);
    IntVector2 forward = currentTile + (pieceColor == PieceColor.BLACK ? -IntVector2.up : IntVector2.up);
    if (left != null && left.movementsMade == 1 && chessBoard.GetNumberOfLastMove(left) == chessBoard.pieceMovementCount && left.pieceColor != pieceColor && left.pieceType == PieceType.PAWN)
    {
      special1 = new SpecialMovement(forward - IntVector2.right, left);
    }
    if (right != null && right.movementsMade == 1 && chessBoard.GetNumberOfLastMove(right) == chessBoard.pieceMovementCount && right.pieceColor != pieceColor && right.pieceType == PieceType.PAWN)
    {
      special2 = new SpecialMovement(forward + IntVector2.right, right);
    }
    if (special1 != null)
    {
      // Both specials
      if (special2 != null)
      {
        return new SpecialMovement[]{ special1, special2 };
      }
      // Just special1
      else
      {
        return new SpecialMovement[]{ special1 };
      }
    }
    // Just special2
    else if (special2 != null)
    {
      return new SpecialMovement[]{ special2 };
    }
    return new SpecialMovement[0];
  }
}
