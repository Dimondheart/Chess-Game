using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class ChessPieceTest
{
  [Test]
  public void Promote()
  {
    ChessPiece piece1 = new Pawn(ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new Queen(ChessPiece.PieceColor.WHITE);

    piece1.Promote(piece2);

    Assert.AreEqual(piece1.pieceType, ChessPiece.PieceType.QUEEN);
  }
}
