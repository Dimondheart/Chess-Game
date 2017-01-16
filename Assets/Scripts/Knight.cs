using UnityEngine;
using System.Collections;

public class Knight : ChessPiece
{
  public Knight(ChessPiece.PieceColor color) : base(ChessPiece.PieceType.KNIGHT, color)
  {
  }

  public override Bitboard GetBasicMoves(ChessBoard chessBoard)
  {
    Bitboard moves = new Bitboard();
    IntVector2 currentTile = chessBoard.GetTileCoordinates(this);
    IntVector2[] deltas = {
      IntVector2.right + 2 * IntVector2.up,
      2 * IntVector2.right + IntVector2.up,
      2 * IntVector2.right - IntVector2.up,
      IntVector2.right - 2 * IntVector2.up,
      -IntVector2.right + 2 * IntVector2.up,
      -2 * IntVector2.right + IntVector2.up,
      -2 * IntVector2.right - IntVector2.up,
      -IntVector2.right - 2 * IntVector2.up
    };
    for (int i = 0; i < deltas.Length; i++)
    {
      IntVector2 check = currentTile + deltas[i];
      if (!chessBoard.AreCoordinatesOverBoard(check))
      {
        continue;
      }
      ChessPiece pieceAt = chessBoard.GetChessPieceAt(check);
      if (pieceAt == null)
      {
        moves[check.x, check.y] = true;
      }
      else if (pieceAt.pieceColor != pieceColor)
      {
        moves[check.x, check.y] = true;
      }
    }
    return moves;
  }
}
