using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
  public static ChessRuleSet currentRuleSet = new ChessRuleSet();

  public ChessBoard currentBoard;
  public Mouse3D mouse3D;
  public AudioClip mainMenuMusic;
  public AudioClip mainGameMusic;
  public Text whosTurnText;
  public GameObject mainMenuPanel;

  public ChessPiece.PieceColor currentTurn { get; private set; }

  public ChessBoardGUI currentBoardGUI
  {
    get
    {
      return currentBoard.GetComponent<ChessBoardGUI>();
    }
  }

  private CameraOrbitController cameraOrbitController;

  void Start()
  {
    GetComponent<BGMController>().clip = mainMenuMusic;
    GetComponent<MainMenuController>().Open();
    cameraOrbitController =
      GameObject.FindGameObjectWithTag("MainCameraContainer").GetComponent<CameraOrbitController>();
    ChessRuleEvaluator.currentRuleSet = currentRuleSet;
    currentBoard.SetupBoard();
    currentBoardGUI.ResetBoardGUI();
    SetTurn(ChessPiece.PieceColor.WHITE);
  }

  void Update()
  {
    if (!GetComponent<MainMenuController>().inMainMenu)
    {
      cameraOrbitController.manualControlEnabled = true;
      cameraOrbitController.autoOrbitEnabled = false;
      mouse3D.enabled = true;
    }
  }

  public void StartNextTurn()
  {
    if (currentTurn == ChessPiece.PieceColor.WHITE)
    {
      SetTurn(ChessPiece.PieceColor.BLACK);
    }
    else
    {
      SetTurn(ChessPiece.PieceColor.WHITE);
    }
  }

  public void RestartGame()
  {
    SetTurn(ChessPiece.PieceColor.WHITE);
    currentBoard.SetupBoard();
    currentBoardGUI.ResetBoardGUI();
  }

  public void StartGame()
  {
    if (!GetComponent<MainMenuController>().inMainMenu)
    {
      return;
    }
    GetComponent<MainMenuController>().Close();
    mouse3D.enabled = true;
    cameraOrbitController.manualControlEnabled = true;
    cameraOrbitController.autoOrbitEnabled = false;
    RestartGame();
    GetComponent<BGMController>().clip = mainGameMusic;
  }

  public void QuitApplication()
  {
    if (GetComponent<MainMenuController>().inMainMenu)
    {
      Application.Quit();
    }
    else
    {
      Debug.Log("Check for any save content");
      Application.Quit();
    }
  }

  private void SetTurn(ChessPiece.PieceColor color)
  {
    currentTurn = color;
    if (color == ChessPiece.PieceColor.BLACK)
    {
      cameraOrbitController.SmoothOrbit(new Vector3(60.0f, 180.0f, 0.0f));
      whosTurnText.color = new Color(15 / 255.0f, 15 / 255.0f, 15 / 255.0f);
      whosTurnText.text = "Blacks Turn";
    }
    else
    {
      cameraOrbitController.SmoothOrbit(new Vector3(60.0f, 0.0f, 0.0f));
      whosTurnText.color = new Color(235 / 255.0f, 235 / 255.0f, 235 / 255.0f);
      whosTurnText.text = "Whites Turn";
    }
    currentBoardGUI.selectedChessPiece = null;
  }
}
