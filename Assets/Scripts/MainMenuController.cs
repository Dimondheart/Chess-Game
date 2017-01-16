using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
  private enum TransitionState
  {
    TRANSITION_IN = 0,
    IN_MENU,
    TRANSITION_OUT,
    NOT_IN_MENU
  }

  public Mouse3D mouse3D;
  public GameObject mainMenuPanel;

  public bool inMainMenu { get; private set; }

  private TransitionState transitionState = TransitionState.NOT_IN_MENU;

  void Update()
  {
    switch (transitionState)
    {
      case TransitionState.NOT_IN_MENU:
        break;

      case TransitionState.IN_MENU:
        break;

      case TransitionState.TRANSITION_OUT:
        mainMenuPanel.SetActive(false);
        transitionState = TransitionState.NOT_IN_MENU;
        break;

      case TransitionState.TRANSITION_IN:
        mainMenuPanel.SetActive(true);
        transitionState = TransitionState.IN_MENU;
        break;

      default:
        Debug.LogWarning("Main menu in invalid transition state:" + (int) transitionState);
        break;
    }
  }

  public void Open()
  {
    switch (transitionState)
    {
      case TransitionState.TRANSITION_OUT:
        // Fall through
      case TransitionState.NOT_IN_MENU:
        CameraOrbitController mainCamOrbiter =
          GameObject.FindGameObjectWithTag("MainCameraContainer").GetComponent<CameraOrbitController>();
        mainCamOrbiter.manualControlEnabled = false;
        mainCamOrbiter.autoOrbitEnabled = true;
        mouse3D.enabled = false;
        transitionState = TransitionState.TRANSITION_IN;
        break;

      case TransitionState.IN_MENU:
        // Fall through
      case TransitionState.TRANSITION_IN:
        break;

      default:
        Debug.LogWarning("Main menu in invalid transition state:" + (int) transitionState);
        break;
    }
    inMainMenu = true;
  }

  public void Close()
  {
    switch (transitionState)
    {
      case TransitionState.TRANSITION_OUT:
        // Fall through
      case TransitionState.NOT_IN_MENU:
        break;

      case TransitionState.IN_MENU:
        // Fall through
      case TransitionState.TRANSITION_IN:
        transitionState = TransitionState.TRANSITION_OUT;
        break;

      default:
        Debug.LogWarning("Main menu in invalid transition state:" + (int) transitionState);
        break;
    }
    inMainMenu = false;
  }
}
