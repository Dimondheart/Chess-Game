using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class ChessPieceTest
{
  [Test]
  public void Promote()
  {
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.QUEEN, ChessPiece.PieceColor.WHITE);

    piece1.Promote(piece2);

    Assert.AreEqual(piece1.pieceType, ChessPiece.PieceType.QUEEN);
  }

  [Test]
  public void DemoteToPawn()
  {
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.QUEEN, ChessPiece.PieceColor.WHITE);

    piece1.DemoteToPawn();

    Assert.AreEqual(piece1.pieceType, ChessPiece.PieceType.PAWN);
  }
}
