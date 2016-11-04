using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class ChessRuleEvaluatorTest
{
  [Test]
  public void CanPromotePieceTypeYes()
  {
    ChessRuleEvaluator.currentRuleSet = new ChessRuleSet("Default Rule Set");
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.QUEEN, ChessPiece.PieceColor.WHITE);

    Assert.That(ChessRuleEvaluator.CanPromotePiece(piece1, piece2.pieceType));
  }

  [Test]
  public void CanPromotePieceTypeNoToPawn()
  {
    ChessRuleEvaluator.currentRuleSet = new ChessRuleSet("Default Rule Set");
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);

    Assert.That(!ChessRuleEvaluator.CanPromotePiece(piece1, piece2.pieceType));
  }

  [Test]
  public void CanPromotePieceTypeNoNotFromPawn()
  {
    ChessRuleEvaluator.currentRuleSet = new ChessRuleSet("Default Rule Set");
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.ROOK, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.QUEEN, ChessPiece.PieceColor.WHITE);

    Assert.That(!ChessRuleEvaluator.CanPromotePiece(piece1, piece2.pieceType));
  }

  [Test]
  public void CanPromotePieceTypeNoPromotionDisabled()
  {
    ChessRuleEvaluator.currentRuleSet = new ChessRuleSet("Default Rule Set");
    ChessRuleEvaluator.currentRuleSet.promotionEnabled = false;
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.QUEEN, ChessPiece.PieceColor.WHITE);

    Assert.That(!ChessRuleEvaluator.CanPromotePiece(piece1, piece2.pieceType));
  }
}
