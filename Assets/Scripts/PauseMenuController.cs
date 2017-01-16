using UnityEngine;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
  public GameObject pauseMenuPanel;

  public bool isOpen
  {
    get
    {
      return pauseMenuPanel.activeInHierarchy;
    }
  }

  void Update()
  {
    if (!GetComponent<MainMenuController>().inMainMenu && Input.GetKeyUp(KeyCode.Escape))
    {
      if (pauseMenuPanel.activeSelf)
      {
        Close();
      }
      else
      {
        Open();
      }
    }
  }

  public void Open()
  {
    pauseMenuPanel.SetActive(true);
  }

  public void Close()
  {
    pauseMenuPanel.SetActive(false);
  }
}
