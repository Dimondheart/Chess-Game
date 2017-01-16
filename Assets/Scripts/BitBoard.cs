using UnityEngine;
using System.Collections;

/** A minimal resource cost method for storing simple true/false data, such as legal movements for a piece. */
public class Bitboard
{
  private static readonly int width = 8;
  private static readonly int height = 8;

  public static Bitboard Between(IntVector2 p1, IntVector2 p2)
  {
    Bitboard between = new Bitboard();
    // TODO implement
    return between;
  }

  private BitArray[] columns;

  // TODO eliminate this accessor
  public BitArray this[int x]
  {
    get
    {
      return columns[x];
    }
  }

  public bool this[int x, int y]
  {
    get
    {
      return columns[x][y];
    }
    set
    {
      columns[x][y] = value;
    }
  }

  public bool this[IntVector2 point]
  {
    get
    {
      return this[point.x, point.y];
    }
    set
    {
      this[point.x, point.y] = value;
    }
  }

  public Bitboard()
  {
    columns = new BitArray[width];
    for (int i = 0; i < width; i++)
    {
      columns[i] = new BitArray(height);
    }
  }

  public override string ToString()
  {
    string output = "";
    for (int y = height - 1; y >= 0; y--)
    {
      for (int x = 0; x < width; x++)
      {
        output += this[x, y] ? "*" : "-";
      }
      output += "\n";
    }
    return output;
  }

  public void And(Bitboard board)
  {
    // UNITTEST
    for (int i = 0; i < width; i++)
    {
      columns[i].And(board.columns[i]);
    }
  }

  public void Or(Bitboard board)
  {
    // UNITTEST
    for (int i = 0; i < width; i++)
    {
      columns[i].Or(board.columns[i]);
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

  public void DrawRay(int x1, int y1, int dx, int dy)
  {
    // TODO implement
  }

  public void DrawRay(IntVector2 start, IntVector2 direction)
  {
    // UNITTEST
    DrawRay(start.x, start.y, direction.x, direction.y);
  }

  public void DrawLine(int x1, int y1, int dx, int dy)
  {
    // UNITTEST
    DrawRay(x1, y1, dx, dy);
    DrawRay(x1, y1, -dx, -dy);
  }

  public void DrawLine(IntVector2 point, IntVector2 direction)
  {
    // UNITTEST
    DrawRay(point, direction);
    DrawRay(point, -direction);
  }

  public bool IsLineUniform(IntVector2 point1, IntVector2 point2)
  {
    // UNITTEST
    IntVector2 delta = point2 - point1;
    delta.StepTowardsZero();
    for (bool valueSoFar = this[point2];; delta.StepTowardsZero())
    {
      if (this[point1 + delta] != valueSoFar)
      {
        return false;
      }
      // Must check point1 once, so this had to be separated since magnitude is always non-negative
      if (delta.stepMagnitude == 0)
      {
        break;
      }
    }
    return true;
  }

  public bool IsLineUniform(IntVector2 point1, IntVector2 point2, bool ofValue)
  {
    // UNITTEST
    return ofValue == this[point1] && IsLineUniform(point1, point2);
  }
}
