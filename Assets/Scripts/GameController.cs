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

  private ChessPiece.PieceColor currentTurn;
  private ChessPiece selectedChessPiece;
  private bool isInMainMenu;

  public ChessBoardGUI currentBoardGUI
  {
    get
    {
      return currentBoard.GetComponent<ChessBoardGUI>();
    }
  }

  private AudioSource bgmSource
  {
    get
    {
      return GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }
  }

  void Start()
  {
    ChessRuleEvaluator.currentRuleSet = currentRuleSet;
    currentBoard.SetupBoard();
    currentBoardGUI.ResetBoardGUI();
    currentTurn = ChessPiece.PieceColor.WHITE;
    isInMainMenu = true;
    mouse3D.enabled = false;
  }

  void Update()
  {
    CameraOrbitController mainCamOrbiter =
      GameObject.FindGameObjectWithTag("MainCameraContainer").GetComponent<CameraOrbitController>();
    mainCamOrbiter.manualControlEnabled = !isInMainMenu;
    mainCamOrbiter.autoOrbitEnabled = isInMainMenu;
    mouse3D.enabled = !isInMainMenu;
    if (isInMainMenu)
    {
      if (!Object.ReferenceEquals(bgmSource.clip, mainMenuMusic))
      {
        bgmSource.clip = mainMenuMusic;
        bgmSource.Play();
      }
    }
    else
    {
      if (Input.GetKeyUp(KeyCode.R))
      {
        RestartGame();
      }
      if (Input.GetMouseButtonUp(0))
      {
        if (selectedChessPiece == null)
        {
          GameObject highlighted = mouse3D.highlighted;
          if (highlighted != null && highlighted.GetComponent<ChessPieceGUI>() != null)
          {
            ChessPiece highlightedPiece = highlighted.GetComponent<ChessPieceGUI>().chessPiece;
            if (highlightedPiece.pieceColor == currentTurn && currentBoard.IsPieceInPlay(highlightedPiece))
            {
              selectedChessPiece = highlightedPiece;
            }
          }

        }
        else
        {
          IntVector2 targetCoords = -IntVector2.one;
          GameObject highlighted = mouse3D.highlighted;
          if (highlighted != null)
          {
            if (highlighted.GetComponent<ChessPieceGUI>() != null)
            {
              ChessPiece highlightedPiece = highlighted.GetComponent<ChessPieceGUI>().chessPiece;
              if (highlightedPiece != selectedChessPiece)
              {
                if (highlightedPiece.pieceColor == selectedChessPiece.pieceColor)
                {
                  selectedChessPiece = highlightedPiece;
                }
                else
                {
                  targetCoords = currentBoard.GetTileCoordinates(highlightedPiece);
                }
              }
            }
            else if (Object.ReferenceEquals(highlighted.GetComponent<ChessBoardGUI>(), currentBoardGUI))
            {
              targetCoords = currentBoardGUI.GetBoardTileCoordinates(mouse3D.transform.position);
            }
            if (ChessRuleEvaluator.CanMovePiece(selectedChessPiece, targetCoords, currentBoard))
            {
              currentBoard.MovePiece(selectedChessPiece, targetCoords);
              selectedChessPiece = null;
              StartNextTurn();
            }
          }
          else
          {
            selectedChessPiece = null;
          }
        }
      }
    }
  }

  void OnDrawGizmos()
  {
    if (selectedChessPiece != null && currentBoardGUI != null && currentBoardGUI.GetGUIPiece(selectedChessPiece) != null)
    {
      Gizmos.color = Color.green;
      Bounds b = currentBoardGUI.GetGUIPiece(selectedChessPiece).GetComponent<Renderer>().bounds;
      Gizmos.DrawWireCube(b.center, b.size);
    }
  }

  public void StartNextTurn()
  {
    if (currentTurn == ChessPiece.PieceColor.WHITE)
    {
      currentTurn = ChessPiece.PieceColor.BLACK;
      GameObject.FindGameObjectWithTag("MainCameraContainer").GetComponent<CameraOrbitController>().SmoothOrbit(
        new Vector3(60.0f, 180.0f, 0.0f)
      );
      whosTurnText.color = new Color(15 / 255.0f, 15 / 255.0f, 15 / 255.0f);
      whosTurnText.text = "Blacks Turn";
    }
    else
    {
      currentTurn = ChessPiece.PieceColor.WHITE;
      GameObject.FindGameObjectWithTag("MainCameraContainer").GetComponent<CameraOrbitController>().SmoothOrbit(
        new Vector3(60.0f, 0.0f, 0.0f)
      );
      whosTurnText.color = new Color(235 / 255.0f, 235 / 255.0f, 235 / 255.0f);
      whosTurnText.text = "Whites Turn";
    }
    selectedChessPiece = null;
  }

  public void RestartGame()
  {
    currentTurn = ChessPiece.PieceColor.WHITE;
    currentBoard.SetupBoard();
    currentBoardGUI.ResetBoardGUI();
    GameObject.FindGameObjectWithTag("MainCameraContainer").GetComponent<CameraOrbitController>().SmoothOrbit(
      new Vector3(60.0f, 0.0f, 0.0f)
    );
  }

  public void StartGame()
  {
    if (!isInMainMenu)
    {
      return;
    }
    isInMainMenu = false;
    mainMenuPanel.SetActive(isInMainMenu);
    RestartGame();
    bgmSource.clip = mainGameMusic;
    bgmSource.Play();
  }

  public void QuitApplication()
  {
    if (isInMainMenu)
    {
      Application.Quit();
    }
    else
    {
      Debug.Log("Check for any save content");
      Application.Quit();
    }
  }
}
