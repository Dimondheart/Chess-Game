using UnityEngine;
using System.Collections;

public class MovementSet
{
  public BitBoard bitBoard = new BitBoard();

  public void Constrain(ChessBoard chessBoard, ChessPiece chessPiece)
  {
    // UNITTEST
    IntVector2 tile = chessBoard.GetTileCoordinates(chessPiece);
    if (tile.x < 0)
    {
      return;
    }
    switch (chessPiece.pieceType)
    {
      case ChessPiece.PieceType.PAWN:
        IntVector2 forward1 = tile;
        IntVector2 forward2 = tile;
        IntVector2 rightFwd = tile + IntVector2.right;
        IntVector2 leftFwd = tile - IntVector2.right;
        if (chessPiece.pieceColor == ChessPiece.PieceColor.WHITE)
        {
          rightFwd += IntVector2.up;
          leftFwd += IntVector2.up;
          forward1 += IntVector2.up;
          forward2 = forward1 + IntVector2.up;
        }
        else
        {
          rightFwd -= IntVector2.up;
          leftFwd -= IntVector2.up;
          forward1 -= IntVector2.up;
          forward2 = forward1 - IntVector2.up;
        }
        // Forward
        if (chessBoard.AreCoordinatesOverBoard(forward1))
        {
          bitBoard[forward1.x][forward1.y] = chessBoard.GetChessPieceAt(forward1) == null;
          if (chessPiece.movementsMade == 0 && chessBoard.AreCoordinatesOverBoard(forward2))
          {
            bitBoard[forward2.x][forward2.y] = chessBoard.GetChessPieceAt(forward2) == null;
          }
        }
        // Diagonal attacks
        if (chessBoard.GetChessPieceAt(rightFwd) == null)
        {
          bitBoard[rightFwd.x][rightFwd.y] = false;
        }
        else
        {
          ConstrainPoint(chessBoard, chessPiece, rightFwd - tile);
        }
        if (chessBoard.GetChessPieceAt(leftFwd) == null)
        {
          bitBoard[leftFwd.x][leftFwd.y] = false;
        }
        else
        {
          ConstrainPoint(chessBoard, chessPiece, leftFwd - tile);
        }
        break;

      case ChessPiece.PieceType.BISHOP:
        ConstrainRay(chessBoard, chessPiece, IntVector2.one);
        ConstrainRay(chessBoard, chessPiece, IntVector2.right - IntVector2.up);
        ConstrainRay(chessBoard, chessPiece, -IntVector2.one);
        ConstrainRay(chessBoard, chessPiece, -IntVector2.right + IntVector2.up);
        break;

      case ChessPiece.PieceType.KNIGHT:
        ConstrainPoint(chessBoard, chessPiece, IntVector2.right + 2 * IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, 2 * IntVector2.right + IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, 2 * IntVector2.right - IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, IntVector2.right - 2 * IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, -IntVector2.right + 2 * IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, -2 * IntVector2.right + IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, -2 * IntVector2.right - IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, -IntVector2.right - 2 * IntVector2.up);
        break;

      case ChessPiece.PieceType.ROOK:
        ConstrainRay(chessBoard, chessPiece, IntVector2.right);
        ConstrainRay(chessBoard, chessPiece, -IntVector2.up);
        ConstrainRay(chessBoard, chessPiece, -IntVector2.right);
        ConstrainRay(chessBoard, chessPiece, IntVector2.up);
        break;

      case ChessPiece.PieceType.QUEEN:
        ConstrainRay(chessBoard, chessPiece, IntVector2.one);
        ConstrainRay(chessBoard, chessPiece, IntVector2.right - IntVector2.up);
        ConstrainRay(chessBoard, chessPiece, -IntVector2.one);
        ConstrainRay(chessBoard, chessPiece, -IntVector2.right + IntVector2.up);
        ConstrainRay(chessBoard, chessPiece, IntVector2.right);
        ConstrainRay(chessBoard, chessPiece, -IntVector2.up);
        ConstrainRay(chessBoard, chessPiece, -IntVector2.right);
        ConstrainRay(chessBoard, chessPiece, IntVector2.up);
        break;

      case ChessPiece.PieceType.KING:
        ConstrainPoint(chessBoard, chessPiece, IntVector2.one);
        ConstrainPoint(chessBoard, chessPiece, IntVector2.right - IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, -IntVector2.one);
        ConstrainPoint(chessBoard, chessPiece, -IntVector2.right + IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, IntVector2.right);
        ConstrainPoint(chessBoard, chessPiece, -IntVector2.up);
        ConstrainPoint(chessBoard, chessPiece, -IntVector2.right);
        ConstrainPoint(chessBoard, chessPiece, IntVector2.up);
        break;

      default:
        Debug.LogWarning("Unhandled chess piece type. Value:" + (int) chessPiece.pieceType);
        break;
    }
  }

  private void ConstrainPoint(ChessBoard chessBoard, ChessPiece chessPiece, IntVector2 delta)
  {
    // UNITTEST
    IntVector2 tile = chessBoard.GetTileCoordinates(chessPiece);
    if (tile.x < 0 || !chessBoard.AreCoordinatesOverBoard(tile + delta))
    {
      return;
    }
    ChessPiece pieceAt = chessBoard.GetChessPieceAt(tile + delta);
    bitBoard[delta.x][delta.y] = (pieceAt == null || pieceAt.pieceColor != chessPiece.pieceColor);
  }

  private void ConstrainRay(ChessBoard chessBoard, ChessPiece chessPiece, IntVector2 direction)
  {
    // UNITTEST
    IntVector2 tile = chessBoard.GetTileCoordinates(chessPiece);
    if (tile.x < 0)
    {
      return;
    }
    tile += direction;
    while (chessBoard.AreCoordinatesOverBoard(tile))
    {
      if (chessBoard.AreCoordinatesOverBoard(tile))
      {
        break;
      }
      ChessPiece onTile = chessBoard.GetChessPieceAt(tile);
      if (onTile == null)
      {
        bitBoard[tile.x][tile.y] = true;
        tile += direction;
        continue;
      }
      else if (onTile.pieceColor != chessPiece.pieceColor)
      {
        bitBoard[tile.x][tile.y] = true;
        break;
      }
      else
      {
        break;
      }
    }
  }
}
