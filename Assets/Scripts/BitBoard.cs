using UnityEngine;
using System.Collections;

/** A minimal resource cost method for storing simple true/false data, such as legal movements for a piece. */
public class BitBoard
{
  private static readonly int width = 8;
  private static readonly int height = 8;

  public static BitBoard Between(IntVector2 p1, IntVector2 p2)
  {
    BitBoard between = new BitBoard();
    // TODO implement
    return between;
  }

  private BitArray[] columns;

  public BitArray this[int x]
  {
    get
    {
      return columns[x];
    }
  }

  public BitBoard()
  {
    columns = new BitArray[width];
    for (int i = 0; i < width; i++)
    {
      columns[i] = new BitArray(height);
    }
  }

  public void And(BitBoard board)
  {
    // UNITTEST
    for (int i = 0; i < width; i++)
    {
      columns[i].And(board.columns[i]);
    }
  }

  /** Sets all bits to false. */
  public void Clear()
  {
    // UNITTEST
    for (int i = 0; i < width; i++)
    {
      columns[i].SetAll(false);
    }
  }
}
