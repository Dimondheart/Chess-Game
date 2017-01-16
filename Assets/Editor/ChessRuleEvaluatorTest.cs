using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class ChessRuleEvaluatorTest
{
  [Test]
  public void CanPromotePieceTypeYes()
  {
    ChessRuleEvaluator.currentRuleSet = new ChessRuleSet("Default Rule Set");
    ChessPiece piece1 = new Pawn(ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new Queen(ChessPiece.PieceColor.WHITE);

    Assert.That(ChessRuleEvaluator.CanPromotePiece(piece1, piece2.pieceType));
  }

  [Test]
  public void CanPromotePieceTypeNoToPawn()
  {
    ChessRuleEvaluator.currentRuleSet = new ChessRuleSet("Default Rule Set");
    ChessPiece piece1 = new Pawn(ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new Pawn(ChessPiece.PieceColor.WHITE);

    Assert.That(!ChessRuleEvaluator.CanPromotePiece(piece1, piece2.pieceType));
  }

  [Test]
  public void CanPromotePieceTypeNoNotFromPawn()
  {
    ChessRuleEvaluator.currentRuleSet = new ChessRuleSet("Default Rule Set");
    ChessPiece piece1 = new Rook(ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new Queen(ChessPiece.PieceColor.WHITE);

    Assert.That(!ChessRuleEvaluator.CanPromotePiece(piece1, piece2.pieceType));
  }

  [Test]
  public void CanPromotePieceTypeNoPromotionDisabled()
  {
    ChessRuleEvaluator.currentRuleSet = new ChessRuleSet("Default Rule Set");
    ChessRuleEvaluator.currentRuleSet.promotionEnabled = false;
    ChessPiece piece1 = new Pawn(ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new Queen(ChessPiece.PieceColor.WHITE);

    Assert.That(!ChessRuleEvaluator.CanPromotePiece(piece1, piece2.pieceType));
  }
}
