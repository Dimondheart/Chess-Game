using UnityEngine;
using System.Collections;

public class BGMController : MonoBehaviour
{
  public AudioClip clip;

  private AudioSource bgmSource;

  void Start()
  {
    bgmSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
  }

  void Update()
  {
    if (!Object.ReferenceEquals(bgmSource.clip, clip))
    {
      bgmSource.clip = clip;
      bgmSource.Play();
    }
  }
}
