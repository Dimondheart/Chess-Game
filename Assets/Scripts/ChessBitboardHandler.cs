using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChessBitboardHandler
{
  /** The chess board this bitboard handler is managing bitboards for. */
  public readonly ChessBoard chessBoard;

  /** Buffer for bitboards only constrained to the movement pattern, board boundaries, and the positions of other
   * pieces on the board.
   */
  private readonly Dictionary<ChessPiece, Bitboard> minimalConstraintBuffer = new Dictionary<ChessPiece, Bitboard>();
  /** Buffer for bitboards constrained in all possible ways, meaining the minimally constrained bitboard constrained to
   * all additional rules such as not allowing the king to be exposed to check.
   */
  private readonly Dictionary<ChessPiece, Bitboard> fullConstraintBuffer = new Dictionary<ChessPiece, Bitboard>();

  /** Basic constructor. */
  public ChessBitboardHandler(ChessBoard board)
  {
    chessBoard = board;
    UpdateBuffers();
  }

  /** Update the bitboards to match the current board state. */
  public void UpdateBuffers()
  {
//    // TODO update special movements
//
//    Dictionary<ChessPiece, Bitboard> partiallyConstrained = new Dictionary<ChessPiece, Bitboard>();
//    // Update unconstrained buffers
//    partiallyConstrained.Clear();
//    for (int x = 0; x < chessBoard.width; x++)
//    {
//      for (int y = 0; y < chessBoard.height; y++)
//      {
//        ChessPiece chessPiece = chessBoard.GetChessPieceAt(x, y);
//        if (chessPiece == null)
//        {
//          continue;
//        }
//        Bitboard bb = new Bitboard();
//        IntVector2 tile = new IntVector2(x, y);
//        switch (chessPiece.pieceType)
//        {
//          case ChessPiece.PieceType.PAWN:
//            IntVector2 forward1 = tile;
//            IntVector2 forward2 = tile;
//            IntVector2 rightFwd = tile + IntVector2.right;
//            IntVector2 leftFwd = tile - IntVector2.right;
//            if (chessPiece.pieceColor == ChessPiece.PieceColor.WHITE)
//            {
//              rightFwd += IntVector2.up;
//              leftFwd += IntVector2.up;
//              forward1 += IntVector2.up;
//              forward2 = forward1 + IntVector2.up;
//            }
//            else
//            {
//              rightFwd -= IntVector2.up;
//              leftFwd -= IntVector2.up;
//              forward1 -= IntVector2.up;
//              forward2 = forward1 - IntVector2.up;
//            }
//            // Forward
//            bb.TrySetPoint(forward1.x, forward1.y, true);
//            if (chessPiece.movementsMade == 0)
//            {
//              bb.TrySetPoint(forward2.x, forward2.y, true);
//            }
//            // Diagonal attacks
//            bb.TrySetPoint(rightFwd.x, rightFwd.y, true);
//            bb.TrySetPoint(leftFwd.x, leftFwd.y, true);
//            break;
//
//          case ChessPiece.PieceType.BISHOP:
//            bb.DrawLine(x, y, 1, 1);
//            bb.DrawLine(x, y, 1, -1);
//            break;
//
//          case ChessPiece.PieceType.KNIGHT:
//            bb.TrySetPoint(tile + IntVector2.right + 2 * IntVector2.up, true);
//            bb.TrySetPoint(tile + 2 * IntVector2.right + IntVector2.up, true);
//            bb.TrySetPoint(tile + 2 * IntVector2.right - IntVector2.up, true);
//            bb.TrySetPoint(tile + IntVector2.right - 2 * IntVector2.up, true);
//            bb.TrySetPoint(tile + -IntVector2.right + 2 * IntVector2.up, true);
//            bb.TrySetPoint(tile + -2 * IntVector2.right + IntVector2.up, true);
//            bb.TrySetPoint(tile + -2 * IntVector2.right - IntVector2.up, true);
//            bb.TrySetPoint(tile + -IntVector2.right - 2 * IntVector2.up, true);
//            break;
//
//          case ChessPiece.PieceType.ROOK:
//            bb.DrawLine(x, y, 0, 1);
//            bb.DrawLine(x, y, 1, 0);
//            break;
//
//          case ChessPiece.PieceType.QUEEN:
//            bb.DrawLine(x, y, 0, 1);
//            bb.DrawLine(x, y, 1, 0);
//            bb.DrawLine(x, y, 1, 1);
//            bb.DrawLine(x, y, 1, -1);
//            break;
//
//          case ChessPiece.PieceType.KING:
//            bb.TrySetPoint(x, y+1, true);
//            bb.TrySetPoint(x+1, y+1, true);
//            bb.TrySetPoint(x+1, y, true);
//            bb.TrySetPoint(x+1, y-1, true);
//            bb.TrySetPoint(x, y-1, true);
//            bb.TrySetPoint(x-1, y-1, true);
//            bb.TrySetPoint(x-1, y, true);
//            bb.TrySetPoint(x-1, y+1, true);
//            break;
//
//          default:
//            Debug.LogWarning("Unhandled chess piece type. Value:" + (int) chessPiece.pieceType);
//            break;
//        }
//        bb[x, y] = false;
//        partiallyConstrained[chessPiece] = bb;
//      }
//    }
//
//    // Update minimal constraints buffers
//    minimalConstraintBuffer.Clear();
//    for (int x = 0; x < chessBoard.width; x++)
//    {
//      for (int y = 0; y < chessBoard.height; y++)
//      {
//        ChessPiece chessPiece = chessBoard.GetChessPieceAt(x, y);
//        if (chessPiece == null)
//        {
//          continue;
//        }
//        Bitboard bb = new Bitboard();
//        bb.Or(partiallyConstrained[chessPiece]);
//        IntVector2 tile = new IntVector2(x, y);
//        switch (chessPiece.pieceType)
//        {
//          case ChessPiece.PieceType.PAWN:
//            IntVector2 forward1 = tile;
//            IntVector2 forward2 = tile;
//            IntVector2 rightFwd = tile + IntVector2.right;
//            IntVector2 leftFwd = tile - IntVector2.right;
//            if (chessPiece.pieceColor == ChessPiece.PieceColor.WHITE)
//            {
//              rightFwd += IntVector2.up;
//              leftFwd += IntVector2.up;
//              forward1 += IntVector2.up;
//              forward2 = forward1 + IntVector2.up;
//            }
//            else
//            {
//              rightFwd -= IntVector2.up;
//              leftFwd -= IntVector2.up;
//              forward1 -= IntVector2.up;
//              forward2 = forward1 - IntVector2.up;
//            }
//            // Forward
//            bb.TrySetPoint(forward1.x, forward1.y, true);
//            if (chessPiece.movementsMade == 0)
//            {
//              bb.TrySetPoint(forward2.x, forward2.y, true);
//            }
//            // Diagonal attacks
//            bb.TrySetPoint(rightFwd.x, rightFwd.y, true);
//            bb.TrySetPoint(leftFwd.x, leftFwd.y, true);
//            break;
//
//          case ChessPiece.PieceType.BISHOP:
//            bb.DrawLine(x, y, 1, 1);
//            bb.DrawLine(x, y, 1, -1);
//            break;
//
//          case ChessPiece.PieceType.KNIGHT:
//            bb.TrySetPoint(tile + IntVector2.right + 2 * IntVector2.up, true);
//            bb.TrySetPoint(tile + 2 * IntVector2.right + IntVector2.up, true);
//            bb.TrySetPoint(tile + 2 * IntVector2.right - IntVector2.up, true);
//            bb.TrySetPoint(tile + IntVector2.right - 2 * IntVector2.up, true);
//            bb.TrySetPoint(tile + -IntVector2.right + 2 * IntVector2.up, true);
//            bb.TrySetPoint(tile + -2 * IntVector2.right + IntVector2.up, true);
//            bb.TrySetPoint(tile + -2 * IntVector2.right - IntVector2.up, true);
//            bb.TrySetPoint(tile + -IntVector2.right - 2 * IntVector2.up, true);
//            break;
//
//          case ChessPiece.PieceType.ROOK:
//            bb.DrawLine(x, y, 0, 1);
//            bb.DrawLine(x, y, 1, 0);
//            break;
//
//          case ChessPiece.PieceType.QUEEN:
//            bb.DrawLine(x, y, 0, 1);
//            bb.DrawLine(x, y, 1, 0);
//            bb.DrawLine(x, y, 1, 1);
//            bb.DrawLine(x, y, 1, -1);
//            break;
//
//          case ChessPiece.PieceType.KING:
//            bb.TrySetPoint(x, y+1, true);
//            bb.TrySetPoint(x+1, y+1, true);
//            bb.TrySetPoint(x+1, y, true);
//            bb.TrySetPoint(x+1, y-1, true);
//            bb.TrySetPoint(x, y-1, true);
//            bb.TrySetPoint(x-1, y-1, true);
//            bb.TrySetPoint(x-1, y, true);
//            bb.TrySetPoint(x-1, y+1, true);
//            break;
//
//          default:
//            Debug.LogWarning("Unhandled chess piece type. Value:" + (int) chessPiece.pieceType);
//            break;
//        }
//        bb[x, y] = false;
//        partiallyConstrained[chessPiece] = bb;
//      }
//    }
//
//    fullConstraintBuffer.Clear();
//    // TODO update fully constrained buffers
//
//    // TODO reverify special movements
//
//    // Clear the buffer only currently used internally
//    partiallyConstrained.Clear();
  }

  /** Get the bitboard for the specified piece, constrained only to the pieces movement pattern, board boundaries,
   * and the positions of other pieces on the board.
   */
  public Bitboard GetMovementPatternBitboard(ChessPiece piece)
  {
    // TODO implement
    return minimalConstraintBuffer[piece];
  }

  public Bitboard GetFullyConstrainedBitboard(ChessPiece piece)
  {
    // TODO implement
    return null;
  }

  public SpecialMovement[] GetSpecialMovements(ChessPiece piece)
  {
    // TODO implement
    return null;
  }
}
