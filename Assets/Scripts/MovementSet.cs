using UnityEngine;
using System.Collections;

public class MovementSet
{
  /** The bitboard for standard movements (moving to an empty square and captures.) */
  public Bitboard movementBitboard;
  /** Any unique moves that can be made which can affect another piece, such as castling. */
  public SpecialMovement[] specialMovements;
}
