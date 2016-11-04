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
    Vector2 expected = -Vector2.one;
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();

    Vector2 actual = board.GetTileCoordinates(null);

    Assert.That(Mathf.Approximately(expected.x, actual.x) && Mathf.Approximately(expected.y, actual.y));
  }

  [Test]
  public void GetTileCoordinatesWithPieceNotInBoard()
  {
    Vector2 expected = -Vector2.one;
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();

    Vector2 actual = board.GetTileCoordinates(new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE));

    Assert.That(Mathf.Approximately(expected.x, actual.x) && Mathf.Approximately(expected.y, actual.y));
  }

  [Test]
  public void GetTileCoordinates()
  {
    Vector2 expected = new Vector2(1.0f, 4.0f);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    Assert.NotNull(piece);
    board.boardTiles[(int) expected.x, (int) expected.y] = piece;

    Vector2 actual = board.GetTileCoordinates(piece);

    Assert.That(Mathf.Approximately(expected.x, actual.x) && Mathf.Approximately(expected.y, actual.y));
  }

  [Test]
  public void MoveToEmpty()
  {
    Vector2 initialLocation = Vector2.zero;
    Vector2 newLocation = new Vector2(1.0f, 4.0f);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    board.boardTiles[(int) initialLocation.x, (int) initialLocation.y] = piece;

    // Try to move the piece
    board.MovePiece(piece, newLocation);

    Assert.AreSame(board.boardTiles[(int) newLocation.x, (int) newLocation.y], piece);
    Assert.IsNull(board.boardTiles[(int) initialLocation.x, (int) initialLocation.y]);
  }

  [Test]
  public void MoveToOccupied()
  {
    Vector2 piece1Loc = Vector2.zero;
    Vector2 piece2Loc = new Vector2(1.0f, 4.0f);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.BLACK);
    board.boardTiles[(int) piece1Loc.x, (int) piece1Loc.y] = piece1;
    board.boardTiles[(int) piece2Loc.x, (int) piece2Loc.y] = piece2;

    // Try to move the piece
    board.MovePiece(piece1, piece2Loc);

    Assert.AreSame(board.boardTiles[(int) piece2Loc.x, (int) piece2Loc.y], piece1);
    Assert.IsNull(board.boardTiles[(int) piece1Loc.x, (int) piece1Loc.y]);
  }

  [Test]
  public void EliminationDifferentColor()
  {
    Vector2 piece1Loc = Vector2.zero;
    Vector2 piece2Loc = new Vector2(1.0f, 4.0f);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.BLACK);
    board.boardTiles[(int) piece1Loc.x, (int) piece1Loc.y] = piece1;
    board.boardTiles[(int) piece2Loc.x, (int) piece2Loc.y] = piece2;

    // Try to move the piece
    board.MovePiece(piece1, piece2Loc);

    Assert.That(board.eliminatedBlackPieces.Contains(piece2));
  }

  [Test]
  public void EliminationSameColor()
  {
    Vector2 piece1Loc = Vector2.zero;
    Vector2 piece2Loc = new Vector2(1.0f, 4.0f);
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    ChessPiece piece1 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    ChessPiece piece2 = new ChessPiece(ChessPiece.PieceType.PAWN, ChessPiece.PieceColor.WHITE);
    board.boardTiles[(int) piece1Loc.x, (int) piece1Loc.y] = piece1;
    board.boardTiles[(int) piece2Loc.x, (int) piece2Loc.y] = piece2;

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
    Vector2 valid = new Vector2(3.0f, 4.0f);

    Assert.That(board.AreCoordinatesOverBoard(valid));
  }

  [Test]
  public void AreCoordinatesOverBoardVectorNo()
  {
    GameObject obj = new GameObject();
    obj.AddComponent<ChessBoard>();
    ChessBoard board = obj.GetComponent<ChessBoard>();
    Vector2 v1 = new Vector2(-1.0f, 4.0f);
    Vector2 v2 = new Vector2(8.0f, 4.0f);

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
