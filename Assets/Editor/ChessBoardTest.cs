using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class ChessBoardTest
{
  [Test]
  public void InitialTileStates()
  {
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();

   // Verify that all tiles are empty
    foreach (ChessPiece piece in board.GetAllPiecesInPlay())
    {
      Assert.Null(piece);
    }
    // TODO check eliminated
  }

  [Test]
  public void GetTileCoordinatesWithNull()
  {
    IntVector2 expected = -IntVector2.one;
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();

    IntVector2 actual = board.GetTileCoordinates(null);

    Assert.That(actual == expected);
  }

  [Test]
  public void GetTileCoordinatesWithPieceNotInBoard()
  {
    IntVector2 expected = -IntVector2.one;
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();

    IntVector2 actual = board.GetTileCoordinates(new Pawn(ChessPiece.PieceColor.WHITE));

    Assert.That(actual == expected);
  }

  [Test]
  public void GetTileCoordinates()
  {
    IntVector2 expected = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece = new Pawn(ChessPiece.PieceColor.WHITE);
    Assert.NotNull(piece);
    board.PlacePiece(piece, expected);

    IntVector2 actual = board.GetTileCoordinates(piece);

    Assert.That(actual == expected);
  }

  [Test]
  public void MoveToEmpty()
  {
    IntVector2 initialLocation = IntVector2.zero;
    IntVector2 newLocation = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece = new Pawn(ChessPiece.PieceColor.WHITE);
    board.PlacePiece(piece, initialLocation);

    // Try to move the piece
    board.MovePiece(piece, newLocation);

    Assert.AreSame(board.GetChessPieceAt(newLocation), piece);
    Assert.IsNull(board.GetChessPieceAt(initialLocation));
  }

  [Test]
  public void MoveToOccupied()
  {
    IntVector2 piece1Loc = IntVector2.zero;
    IntVector2 piece2Loc = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new Pawn(ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new Pawn(ChessPiece.PieceColor.BLACK);
    board.PlacePiece(piece1, piece1Loc);
    board.PlacePiece(piece2, piece2Loc);

    // Try to move the piece
    board.MovePiece(piece1, piece2Loc);

    Assert.AreSame(board.GetChessPieceAt(piece2Loc), piece1);
    Assert.IsNull(board.GetChessPieceAt(piece1Loc));
  }

  [Test]
  public void EliminationDifferentColor()
  {
    IntVector2 piece1Loc = IntVector2.zero;
    IntVector2 piece2Loc = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new Pawn(ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new Pawn(ChessPiece.PieceColor.BLACK);
    board.PlacePiece(piece1, piece1Loc);
    board.PlacePiece(piece2, piece2Loc);

    // Try to move the piece
    board.MovePiece(piece1, piece2Loc);

    Assert.That(board.IsEliminated(piece2));
  }

  [Test]
  public void EliminationSameColor()
  {
    IntVector2 piece1Loc = IntVector2.zero;
    IntVector2 piece2Loc = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new Pawn(ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new Pawn(ChessPiece.PieceColor.WHITE);
    board.PlacePiece(piece1, piece1Loc);
    board.PlacePiece(piece2, piece2Loc);

    // Try to move the piece
    board.MovePiece(piece1, piece2Loc);

    Assert.That(board.IsEliminated(piece2));
  }

  [Test]
  public void AreCoordinatesOverBoardVectorYes()
  {
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    IntVector2 valid = new IntVector2(3, 4);

    Assert.That(board.AreCoordinatesOverBoard(valid));
  }

  [Test]
  public void AreCoordinatesOverBoardVectorNo()
  {
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    IntVector2 v1 = new IntVector2(-1, 4);
    IntVector2 v2 = new IntVector2(8, 4);

    Assert.That(!board.AreCoordinatesOverBoard(v1) && !board.AreCoordinatesOverBoard(v2));
  }

  [Test]
  public void AreCoordinatesOverBoard2IntsYes()
  {
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();

    Assert.That(board.AreCoordinatesOverBoard(3, 4));
  }

  [Test]
  public void AreCoordinatesOverBoard2IntsNo()
  {
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();

    Assert.That(!board.AreCoordinatesOverBoard(-1, 4) && !board.AreCoordinatesOverBoard(8, 4));
  }
}
