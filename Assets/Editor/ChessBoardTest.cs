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
    foreach (ChessPiece piece in board.boardTiles)
    {
      Assert.Null(piece);
    }
  }

  [Test]
  public void InitialEliminatedStates()
  {
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();

    // Verify that all eliminated pieces lists are empty
    foreach (ChessPiece piece in board.eliminatedWhitePieces)
    {
      Assert.Null(piece);
    }
    foreach (ChessPiece piece in board.eliminatedBlackPieces)
    {
      Assert.Null(piece);
    }
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

    IntVector2 actual = board.GetTileCoordinates(new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE));

    Assert.That(actual == expected);
  }

  [Test]
  public void GetTileCoordinates()
  {
    IntVector2 expected = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    Assert.NotNull(piece);
    board.boardTiles[expected.x, expected.y] = piece;

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
    ChessPiece piece = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    board.boardTiles[initialLocation.x, initialLocation.y] = piece;

    // Try to move the piece
    board.MovePiece(piece, newLocation);

    Assert.AreSame(board.boardTiles[newLocation.x, newLocation.y], piece);
    Assert.IsNull(board.boardTiles[initialLocation.x, initialLocation.y]);
  }

  [Test]
  public void MoveToOccupied()
  {
    IntVector2 piece1Loc = IntVector2.zero;
    IntVector2 piece2Loc = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.BLACK);
    board.boardTiles[piece1Loc.x, piece1Loc.y] = piece1;
    board.boardTiles[piece2Loc.x, piece2Loc.y] = piece2;

    // Try to move the piece
    board.MovePiece(piece1, piece2Loc);

    Assert.AreSame(board.boardTiles[piece2Loc.x, piece2Loc.y], piece1);
    Assert.IsNull(board.boardTiles[piece1Loc.x, piece1Loc.y]);
  }

  [Test]
  public void EliminationDifferentColor()
  {
    IntVector2 piece1Loc = IntVector2.zero;
    IntVector2 piece2Loc = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.BLACK);
    board.boardTiles[piece1Loc.x, piece1Loc.y] = piece1;
    board.boardTiles[piece2Loc.x, piece2Loc.y] = piece2;

    // Try to move the piece
    board.MovePiece(piece1, piece2Loc);

    Assert.That(board.eliminatedBlackPieces.Contains(piece2));
  }

  [Test]
  public void EliminationSameColor()
  {
    IntVector2 piece1Loc = IntVector2.zero;
    IntVector2 piece2Loc = new IntVector2(1, 4);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    board.boardTiles[piece1Loc.x, piece1Loc.y] = piece1;
    board.boardTiles[piece2Loc.x, piece2Loc.y] = piece2;

    // Try to move the piece
    board.MovePiece(piece1, piece2Loc);

    Assert.That(board.eliminatedWhitePieces.Contains(piece2));
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
