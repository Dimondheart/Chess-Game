using UnityEngine;
using System.Collections;

[System.Serializable]
public struct IntVector2
{
  public static readonly IntVector2 zero = new IntVector2(0, 0);
  public static readonly IntVector2 one = new IntVector2(1, 1);
  public static readonly IntVector2 right = new IntVector2(1, 0);
  public static readonly IntVector2 up = new IntVector2(0, 1);

  public static IntVector2 operator +(IntVector2 v1, IntVector2 v2)
  {
    return new IntVector2(v1.x + v2.x, v1.y + v2.y);
  }

  public static IntVector2 operator -(IntVector2 v1, IntVector2 v2)
  {
    return new IntVector2(v1.x - v2.x, v1.y - v2.y);
  }

  public static IntVector2 operator -(IntVector2 v)
  {
    return new IntVector2(-v.x, -v.y);
  }

  public static IntVector2 operator *(IntVector2 v, int m)
  {
    // UNITTEST
    return new IntVector2(v.x * m, v.y * m);
  }

  public static IntVector2 operator *(int m, IntVector2 v)
  {
    // UNITTEST
    return v * m;
  }

  public static IntVector2 operator *(IntVector2 v, float m)
  {
    // UNITTEST
    IntVector2 result = IntVector2.zero;
    Vector2 temp = new Vector2(v.x * m, v.y * m);
    if (Mathf.Approximately(Mathf.Abs(temp.x - Mathf.Round(temp.x)), 0.0f))
    {
      result.x = Mathf.RoundToInt(temp.x);
    }
    else
    {
      result.x = (int) temp.x;
    }
    if (Mathf.Approximately(Mathf.Abs(temp.y - Mathf.Round(temp.y)), 0.0f))
    {
      result.y = Mathf.RoundToInt(temp.y);
    }
    else
    {
      result.y = (int) temp.y;
    }
    return result;
  }

  public static IntVector2 operator *(float m, IntVector2 v)
  {
    // UNITTEST
    return v * m;
  }

  public static bool operator ==(IntVector2 v1, IntVector2 v2)
  {
    return v1.x == v2.x && v1.y == v2.y;
  }

  public static bool operator !=(IntVector2 v1, IntVector2 v2)
  {
    return v1.x != v2.x || v1.y != v2.y;
  }

  public int x;
  public int y;

  public IntVector2(int x, int y)
  {
    this.x = x;
    this.y = y;
  }

  public int this[int key]
  {
    get
    {
      if (key == 0)
      {
        return x;
      }
      else if (key == 1)
      {
        return y;
      }
      else
      {
        // TODO ???
        return 0;
      }
    }
    set
    {
      if (key == 0)
      {
        x = value;
      }
      else if (key == 1)
      {
        y = value;
      }
      else
      {
        // TODO ???
      }
    }
  }

  public float magnitude
  {
    get
    {
      return Mathf.Sqrt(Mathf.Pow(x, 2.0f) + Mathf.Pow(y, 2.0f));
    }
  }

  /** The number of horizontal and vetical 'steps' or units along both axes of this vector. */
  public int stepMagnitude
  {
    get
    {
      return Mathf.Abs(x) + Mathf.Abs(y);
    }
  }

  public override bool Equals(object obj)
  {
    return base.Equals(obj);
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }

  public override string ToString()
  {
    return string.Format("IntVector2:({0},{1})", x, y);
  }

  public bool IsZero()
  {
    return x == 0 && y == 0;
  }

  public bool IsDiagonal()
  {
    return x != 0 && Mathf.Abs(x) == Mathf.Abs(y);
  }

  public bool IsHorizontal()
  {
    return y == 0 && x != 0;
  }

  public bool IsVertical()
  {
    return x == 0 && y != 0;
  }

  /** Unit form for an integer vector is defined as having a distance of exactly tile away, either vertically, horizontally
   * or diagonally.
   */
  public bool IsInUnitForm()
  {
    return !IsZero() && Mathf.Abs(x) <= 1 && Mathf.Abs(y) <= 1;
  }

  /** Move this vector one unit along each axis towards zero. An axis value is not changed if it is already 0. */
  public void StepTowardsZero()
  {
    // UNITTEST
    x = x >= 0 ? Mathf.Max(x - 1, 0) : x + 1;
    y = y >= 0 ? Mathf.Max(y - 1, 0) : y + 1;
  }
}
