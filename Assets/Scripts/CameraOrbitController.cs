using UnityEngine;
using System.Collections;

public class CameraOrbitController : MonoBehaviour
{
  public GameObject toOrbit;
  public bool manualControlEnabled = true;

  private Vector3 previousMousePosition;

  void Reset()
  {
    manualControlEnabled = true;
  }

  void Start()
  {
    previousMousePosition = Input.mousePosition;
  }

  void Update()
  {
    Vector3 newMousePos = Input.mousePosition;
    if (Input.GetMouseButton(1) && manualControlEnabled)
    {
      Vector3 deltaCamera = Vector3.ClampMagnitude(newMousePos - previousMousePosition, 10.0f);
      float swap = deltaCamera.x;
      deltaCamera.x = -deltaCamera.y;
      deltaCamera.y = swap;
      Vector3 newEuler = GetComponent<Rigidbody>().rotation.eulerAngles + deltaCamera;
      newEuler.x = Mathf.Clamp(newEuler.x, 0.0f, 89.5f);
      GetComponent<Rigidbody>().rotation = Quaternion.Euler(newEuler);
    }
    if (!Mathf.Approximately(Input.mouseScrollDelta.y, 0.0f))
    {
      float newScaleMult = Mathf.Clamp(transform.localScale.x - Input.mouseScrollDelta.y * 0.05f, 0.5f, 2.0f);
      transform.localScale = Vector3.one * newScaleMult;
    }
    previousMousePosition = newMousePos;
  }
}
