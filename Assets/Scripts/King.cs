using UnityEngine;
using System.Collections;

public class King : ChessPiece
{
  public King(ChessPiece.PieceColor color) : base(ChessPiece.PieceType.KING, color)
  {
  }

  public override Bitboard GetBasicMoves(ChessBoard chessBoard)
  {
    Bitboard moves = new Bitboard();
    IntVector2 currentTile = chessBoard.GetTileCoordinates(this);
    if (currentTile.x < 0)
    {
      Debug.LogError("King is not on the specified chess board");
      return null;
    }
    IntVector2[] deltas = {
      IntVector2.one,
      IntVector2.right - IntVector2.up,
      -IntVector2.one,
      -IntVector2.right + IntVector2.up,
      IntVector2.right,
      -IntVector2.up, -IntVector2.right,
      IntVector2.up
    };
    for (int i = 0; i < deltas.Length; i++)
    {
      IntVector2 check = currentTile + deltas[i];
      if (chessBoard.AreCoordinatesOverBoard(check))
      {
        ChessPiece atCheck = chessBoard.GetChessPieceAt(check);
        moves[check.x, check.y] = atCheck == null || atCheck.pieceColor != pieceColor;
      }
    }
    return moves;
  }

  public override SpecialMovement[] GetSpecialMoves(ChessBoard chessBoard)
  {
    if (movementsMade > 0)
    {
      return new SpecialMovement[0];
    }
    ChessPiece leftCorner = null;
    ChessPiece rightCorner = null;
    switch (pieceColor)
    {
      case PieceColor.BLACK:
        leftCorner = chessBoard.GetChessPieceAt(0, chessBoard.height - 1);
        rightCorner = chessBoard.GetChessPieceAt(chessBoard.width - 1, chessBoard.height - 1);
        break;
      case PieceColor.WHITE:
        leftCorner = chessBoard.GetChessPieceAt(0, 0);
        rightCorner = chessBoard.GetChessPieceAt(chessBoard.width - 1, 0);
        break;
      default:
        Debug.LogError("Unhandled chess piece color in switch case:" + (int) pieceColor);
        return new SpecialMovement[0];
    }
    if (leftCorner == null && rightCorner == null)
    {
      return new SpecialMovement[0];
    }
    SpecialMovement special1 = null;
    SpecialMovement special2 = null;
    IntVector2 currentTile = chessBoard.GetTileCoordinates(this);
    Bitboard piecePositions = chessBoard.GetPiecePositions();
    if (
      leftCorner != null
      && !chessBoard.HasPieceMoved(leftCorner)
      && leftCorner.pieceType == PieceType.ROOK
      && piecePositions.IsLineUniform(
        currentTile - IntVector2.right,
        new IntVector2(1, pieceColor == PieceColor.BLACK ? chessBoard.height - 1 : 0),
        false
      ))
    {
      special1 = new SpecialMovement(currentTile - 2 * IntVector2.right, leftCorner, currentTile - IntVector2.right);
    }
    if (
      rightCorner != null
      && !chessBoard.HasPieceMoved(rightCorner)
      && rightCorner.pieceType == PieceType.ROOK
      && piecePositions.IsLineUniform(
        currentTile + IntVector2.right,
        new IntVector2(chessBoard.width - 2, pieceColor == PieceColor.BLACK ? chessBoard.height - 1 : 0),
        false
      ))
    {
      special2 = new SpecialMovement(currentTile + 2 * IntVector2.right, rightCorner, currentTile + IntVector2.right);
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
