using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChessPiece
{
  public enum PieceType
  {
    PAWN = 0,
    BISHOP,
    KNIGHT,
    ROOK,
    QUEEN,
    KING
  }

  public enum PieceColor
  {
    WHITE = 0,
    BLACK
  }

  public PieceType pieceType { get; private set; }
  public readonly PieceColor pieceColor;
  public int movementsMade = 0;

  public ChessPiece(PieceType pieceType, PieceColor pieceColor)
  {
    // FIXME validate the enum range
    this.pieceType = pieceType;
    // FIXME validate the enum range
    this.pieceColor = pieceColor;
  }

  /** Promotes the piece to the specified type. Does not check if the specified promotion is legal. */
  public void Promote(ChessPiece promoteTo)
  {
    pieceType = promoteTo.pieceType;
  }

  /** Demotes the piece to a pawn, unless it is already a pawn. */
  public void DemoteToPawn()
  {
    if (pieceType != PieceType.PAWN)
    {
      pieceType = PieceType.PAWN;
    }
  }
}
