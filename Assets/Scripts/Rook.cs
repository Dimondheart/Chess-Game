using UnityEngine;
using System.Collections;

public class Rook : ChessPiece
{
  public Rook(ChessPiece.PieceColor color) : base(ChessPiece.PieceType.ROOK, color)
  {
  }

  public override Bitboard GetBasicMoves(ChessBoard chessBoard)
  {
    Bitboard moves = new Bitboard();
    IntVector2 currentTile = chessBoard.GetTileCoordinates(this);
    IntVector2[] deltas = {
      IntVector2.right,
      -IntVector2.right,
      IntVector2.up,
      -IntVector2.up
    };
    for (int i = 0; i < deltas.Length; i++)
    {
      for (IntVector2 check = currentTile + deltas[i]; chessBoard.AreCoordinatesOverBoard(check); check += deltas[i])
      {
        ChessPiece pieceAt = chessBoard.GetChessPieceAt(check);
        if (pieceAt == null)
        {
          moves[check.x, check.y] = true;
        }
        else if (pieceAt.pieceColor != pieceColor)
        {
          moves[check.x, check.y] = true;
          break;
        }
        else
        {
          break;
        }
      }
    }
    return moves;
  }
}
