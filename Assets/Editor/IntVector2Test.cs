using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class IntVector2Test
{
  [Test]
  public void Construction()
  {
    int vx = 10;
    int vy = -4;

    IntVector2 iv2 = new IntVector2(vx, vy);

    Assert.AreEqual(vx, iv2.x);
    Assert.AreEqual(vy, iv2.y);
  }

  [Test]
  public void AdditionOp()
  {
    int v1x = 10;
    int v2x = -5;
    int v1y = 20;
    int v2y = 5;
    IntVector2 v1 = new IntVector2(v1x, v1y);
    IntVector2 v2 = new IntVector2(v2x, v2y);

    IntVector2 result = v1 + v2;

    Assert.AreEqual(v1x + v2x, result.x);
    Assert.AreEqual(v1y + v2y, result.y);
  }

  [Test]
  public void SubtractionOp()
  {
    int v1x = 10;
    int v2x = -5;
    int v1y = 20;
    int v2y = 5;
    IntVector2 v1 = new IntVector2(v1x, v1y);
    IntVector2 v2 = new IntVector2(v2x, v2y);

    IntVector2 result = v1 - v2;

    Assert.AreEqual(v1x - v2x, result.x);
    Assert.AreEqual(v1y - v2y, result.y);
  }

  [Test]
  public void NegativeOp()
  {
    int vx = 10;
    int vy = -4;

    IntVector2 iv2 = new IntVector2(vx, vy);
    iv2 = -iv2;

    Assert.AreEqual(-vx, iv2.x);
    Assert.AreEqual(-vy, iv2.y);
  }

  [Test]
  public void EqualOp()
  {
    IntVector2 iv1 = new IntVector2(5, 27);
    IntVector2 iv2 = new IntVector2(5, 27);
    IntVector2 iv3 = new IntVector2(5, 21);

    Assert.True(iv1 == iv2);
    Assert.False(iv1 == iv3);
  }

  [Test]
  public void NotEqualOp()
  {
    IntVector2 iv1 = new IntVector2(5, 27);
    IntVector2 iv2 = new IntVector2(5, 27);
    IntVector2 iv3 = new IntVector2(5, 21);

    Assert.False(iv1 != iv2);
    Assert.True(iv1 != iv3);
  }

  [Test]
  public void IndexOp()
  {
    IntVector2 iv = new IntVector2(5, 27);

    Assert.AreEqual(iv.x, iv[0]);
    Assert.AreEqual(iv.y, iv[1]);
  }

  [Test]
  public void Magnitude()
  {
    IntVector2 iv = new IntVector2(3, 4);

    Assert.True(Mathf.Approximately(5, iv.magnitude));
  }

  [Test]
  public void StepMagnitude()
  {
    IntVector2 iv = new IntVector2(3, -4);

    Assert.AreEqual(7, iv.stepMagnitude);
  }

  [Test]
  public void IsZero()
  {
    IntVector2 iv = new IntVector2(0, 0);
    IntVector2 iv2 = new IntVector2(0, 5);

    Assert.IsTrue(iv.IsZero());
    Assert.IsFalse(iv2.IsZero());
  }

  [Test]
  public void IsDiagonal()
  {
    IntVector2 iv = new IntVector2(4, -4);
    IntVector2 iv2 = new IntVector2(4, -3);
    IntVector2 iv3 = IntVector2.zero;

    Assert.IsTrue(iv.IsDiagonal());
    Assert.IsFalse(iv2.IsDiagonal());
    Assert.IsFalse(iv3.IsDiagonal());
  }

  [Test]
  public void IsHorizontal()
  {
    IntVector2 yes = new IntVector2(5, 0);
    IntVector2 no = new IntVector2(5, 2);
    IntVector2 no2 = IntVector2.zero;

    Assert.IsTrue(yes.IsHorizontal());
    Assert.IsFalse(no.IsHorizontal());
    Assert.IsFalse(no2.IsHorizontal());
  }

  [Test]
  public void IsVertical()
  {
    IntVector2 yes = new IntVector2(0, 5);
    IntVector2 no = new IntVector2(5, 2);
    IntVector2 no2 = IntVector2.zero;

    Assert.IsTrue(yes.IsVertical());
    Assert.IsFalse(no.IsVertical());
    Assert.IsFalse(no2.IsVertical());
  }

  [Test]
  public void IsInUnitForm()
  {
    // TODO finish
    IntVector2 yes = new IntVector2(1, -1);
    IntVector2 yes2 = new IntVector2(0, 1);
    IntVector2 no = new IntVector2(5, 2);
    IntVector2 no2 = IntVector2.zero;

    Assert.IsTrue(yes.IsInUnitForm());
    Assert.IsTrue(yes2.IsInUnitForm());
    Assert.IsFalse(no.IsInUnitForm());
    Assert.IsFalse(no2.IsInUnitForm());
  }
}
