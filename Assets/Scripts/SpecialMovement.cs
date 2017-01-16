using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialMovement
{
  /** The target for the main piece being moved. */
  public readonly IntVector2 targetTile;
  /** The piece eliminated by the movement. */
  public readonly ChessPiece eliminatedPiece;
  /** Another piece that is moved by this movement. */
  public readonly ChessPiece additionalPieceMoved;
  /** Where to move the other piece affected by this movement. */
  public readonly IntVector2 additionalPieceMovedTarget;

  /** Construct a special movement that eliminates another piece. */
  public SpecialMovement(IntVector2 targetTile, ChessPiece eliminatedPiece)
  {
    this.targetTile = targetTile;
    this.eliminatedPiece = eliminatedPiece;
  }

  /** Construct a special movement that additionally moves another piece. */
  public SpecialMovement(IntVector2 targetTile, ChessPiece otherPieceMoved, IntVector2 otherPieceTarget)
  {
    this.targetTile = targetTile;
    additionalPieceMoved = otherPieceMoved;
    additionalPieceMovedTarget = otherPieceTarget;
  }
}
