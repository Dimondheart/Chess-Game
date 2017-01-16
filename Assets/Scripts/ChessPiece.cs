using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class ChessPiece
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

  protected ChessPiece(PieceType pieceType, PieceColor pieceColor)
  {
    if ((int) pieceType < 0 || (int) pieceType > (int) PieceType.KING)
    {
      Debug.LogError("Attempted to create a chess piece with an invalid type.");
    }
    this.pieceType = pieceType;
    if ((int) pieceColor < 0 || (int) pieceColor > (int) PieceColor.BLACK)
    {
      Debug.LogError("Attempted to create a chess piece with an invalid color.");
    }
    this.pieceColor = pieceColor;
  }

  public abstract Bitboard GetBasicMoves(ChessBoard chessBoard);

  public virtual SpecialMovement[] GetSpecialMoves(ChessBoard chessBoard)
  {
    return new SpecialMovement[0];
  }

  public MovementSet GetMovementSet(ChessBoard chessBoard)
  {
    MovementSet moveSet = new MovementSet();
    moveSet.movementBitboard = GetBasicMoves(chessBoard);
    moveSet.specialMovements = GetSpecialMoves(chessBoard);
    return moveSet;
  }

  /** Promotes the piece to the specified type. Does not check if the specified promotion is legal. */
  public void Promote(ChessPiece promoteTo)
  {
    pieceType = promoteTo.pieceType;
  }
}
