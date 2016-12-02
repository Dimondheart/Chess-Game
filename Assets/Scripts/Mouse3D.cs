using UnityEngine;
using System.Collections;

public class Mouse3D : MonoBehaviour
{
  private static readonly float preliminaryDetectionOuterMultiplier = 40.0f;
  private static readonly float preliminaryDetectionInnerMultiplier = 10.0f;
  private static readonly float preliminaryOffset = 0.3f;

  public float minimumHeight;
  public float maximumHeight;
  public float maxDistanceFromCamera;
  public GameObject highlighted { get; private set; }

  void Update()
  {
    // TODO add a larger outer sphere for preliminary detection to decrease the # of iterations
    Vector3 mousePos = Input.mousePosition;
    // Start searching at the near clip plane of the camera
    mousePos.z =
      GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().nearClipPlane
      + transform.localScale.x * 0.5f;
    Vector3 mousePosInWorld =
      GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(mousePos);
    transform.position = mousePosInWorld;
    highlighted = null;
    float prevY = mousePosInWorld.y;
    // Move the camera to be within the max height, or return if this is not possible
    while (mousePosInWorld.y > maximumHeight)
    {
      mousePos.z += 0.4f * transform.localScale.x;
      mousePosInWorld =
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(mousePos);
      transform.position = mousePosInWorld;
      if (mousePosInWorld.y > prevY || mousePos.z > maxDistanceFromCamera)
      {
        return;
      }
    }
    // Move the camera to be within the min height, or return if this is not possible
    prevY = mousePosInWorld.y;
    while (mousePosInWorld.y < minimumHeight)
    {
      mousePos.z += 0.4f * transform.localScale.x;
      mousePosInWorld =
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(mousePos);
      transform.position = mousePosInWorld;
      if (mousePosInWorld.y < prevY || mousePos.z > maxDistanceFromCamera)
      {
        return;
      }
    }
    bool shell1 = Physics.CheckSphere(transform.position, transform.localScale.x * 0.5f * preliminaryDetectionOuterMultiplier);
    bool shell2 = Physics.CheckSphere(transform.position, transform.localScale.x * 0.5f * preliminaryDetectionInnerMultiplier);
    while (mousePosInWorld.y > minimumHeight && mousePosInWorld.y < maximumHeight)
    {
      if (mousePos.z > maxDistanceFromCamera)
      {
        return;
      }
      if (shell1 || Physics.CheckSphere(transform.position, transform.localScale.x * 0.5f * preliminaryDetectionOuterMultiplier))
      {
        shell1 = true;
        if (shell2 || Physics.CheckSphere(transform.position, transform.localScale.x * 0.5f * preliminaryDetectionInnerMultiplier))
        {
          shell2 = true;
          if (Physics.CheckSphere(transform.position, transform.localScale.x * 0.5f))
          {
            bool stopSearching = false;
            Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.x * 0.5f);
            foreach (Collider c in colliders)
            {
              GameObject go = c.gameObject;
              if (go.CompareTag("Selectable"))
              {
                highlighted = go;
                stopSearching = true;
                break;
              }
            }
            if (stopSearching)
            {
              break;
            }
          }
          mousePos.z += 0.4f * transform.localScale.x;
        }
        else
        {
          mousePos.z += 0.4f * transform.localScale.x * preliminaryDetectionInnerMultiplier;
        }
      }
      else
      {
        mousePos.z += 0.4f * transform.localScale.x * preliminaryDetectionOuterMultiplier;
      }
      mousePosInWorld =
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(mousePos);
      transform.position = mousePosInWorld;
    }
  }

  void OnEnable()
  {
    transform.position = Vector3.zero;
    highlighted = null;
  }

  void OnDisable()
  {
    transform.position = Vector3.zero;
    highlighted = null;
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
    Gizmos.color = new Color(0.5f, 1, 0.5f, 1);
    Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f * preliminaryDetectionInnerMultiplier);
    Gizmos.color = new Color(0.75f, 1, 0.75f, 1);
    Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f * preliminaryDetectionOuterMultiplier);
    if (highlighted != null)
    {
      Gizmos.color = Color.blue;
      Bounds b = highlighted.GetComponent<Renderer>().bounds;
      Gizmos.DrawWireCube(b.center, b.size);
    }
  }
}
