using UnityEngine;
using System.Collections;

public class CameraOrbitController : MonoBehaviour
{
  public GameObject toOrbit;
  public bool manualControlEnabled;
  public bool autoOrbitEnabled;
  public float autoOrbitSpeed;
  public float smoothingFactor;

  private Vector3 previousMousePosition;
  private Vector3 previousDelta = Vector3.zero;
  private Vector3 targetAngle = Vector3.one * 400.0f;

  void Reset()
  {
    manualControlEnabled = false;
  }

  void Start()
  {
    previousMousePosition = Input.mousePosition;
  }

  void Update()
  {
    Vector3 newMousePos = Input.mousePosition;
    if (manualControlEnabled)
    {
      if (Input.GetMouseButton(1))
      {
        targetAngle = Vector3.one * 400.0f;
        Vector3 deltaCamera = Vector3.ClampMagnitude(newMousePos - previousMousePosition, 5.0f);
        float swap = deltaCamera.x;
        deltaCamera.x = -deltaCamera.y;
        deltaCamera.y = swap;
        if (!Mathf.Approximately(deltaCamera.magnitude, 0.0f))
        {
          previousDelta = deltaCamera;
        }
        Vector3 newEuler = GetComponent<Rigidbody>().rotation.eulerAngles + deltaCamera;
        newEuler.x = Mathf.Clamp(newEuler.x, 0.0f, 89.5f);
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(newEuler);
      }
      if (!Mathf.Approximately(Input.mouseScrollDelta.y, 0.0f))
      {
        float newScaleMult = Mathf.Clamp(transform.localScale.x - Input.mouseScrollDelta.y * 0.05f, 0.5f, 2.0f);
        transform.localScale = Vector3.one * newScaleMult;
      }
    }
    previousMousePosition = newMousePos;
  }

  void FixedUpdate()
  {
    if (autoOrbitEnabled)
    {
      Vector3 deltaCamera = new Vector3(0.0f, -autoOrbitSpeed * Time.deltaTime, 0.0f);
      previousDelta = deltaCamera;
      Vector3 newEuler = GetComponent<Rigidbody>().rotation.eulerAngles + deltaCamera;
      newEuler.x = Mathf.Clamp(newEuler.x, 0.0f, 89.5f);
      GetComponent<Rigidbody>().rotation = Quaternion.Euler(newEuler);
    }
    else if (!Mathf.Approximately(targetAngle.x, 400.0f))
    {
      Vector3 deltaCamera = (targetAngle - GetComponent<Rigidbody>().rotation.eulerAngles) * 0.1f;
      if (deltaCamera.magnitude < 0.1f)
      {
        Vector3 newEuler = targetAngle;
        newEuler.x = Mathf.Clamp(newEuler.x, 0.0f, 89.5f);
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(newEuler);
        targetAngle = Vector3.one * 400.0f;
        previousDelta = Vector3.zero;
      }
      else
      {
        deltaCamera.x = deltaCamera.x % 36;
        deltaCamera.y = deltaCamera.y % 36;
        deltaCamera.z = deltaCamera.z % 36;
        if (deltaCamera.x > 18.0f)
        {
          deltaCamera.x = 36.0f - deltaCamera.x;
        }
        else if (deltaCamera.x < -18.0f)
        {
          deltaCamera.x = 36.0f + deltaCamera.x;
        }
        if (deltaCamera.y > 18.0f)
        {
          deltaCamera.y = 36.0f - deltaCamera.y;
        }
        else if (deltaCamera.y < -18.0f)
        {
          deltaCamera.y = 36.0f + deltaCamera.y;
        }
        if (deltaCamera.z > 18.0f)
        {
          deltaCamera.z = 36.0f - deltaCamera.z;
        }
        else if (deltaCamera.z < -18.0f)
        {
          deltaCamera.z = 36.0f + deltaCamera.z;
        }
        previousDelta = deltaCamera;
        Vector3 newEuler = GetComponent<Rigidbody>().rotation.eulerAngles + deltaCamera;
        newEuler.x = Mathf.Clamp(newEuler.x, 0.0f, 89.5f);
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(newEuler);
      }
    }
    else if (!Mathf.Approximately(previousDelta.magnitude, 0.0f))
    {
      previousDelta *= smoothingFactor;
      Vector3 newEuler = GetComponent<Rigidbody>().rotation.eulerAngles + previousDelta;
      newEuler.x = Mathf.Clamp(newEuler.x, 0.0f, 89.5f);
      GetComponent<Rigidbody>().rotation = Quaternion.Euler(newEuler);
    }
  }

  public void SmoothOrbit(Vector3 toEuler)
  {
    targetAngle = toEuler;
    targetAngle.x = Mathf.Clamp(targetAngle.x, 0.0f, 89.5f);
  }
}
