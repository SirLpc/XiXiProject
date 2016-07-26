using UnityEngine;
using System.Collections.Generic;

public class HideDisvisible : MonoBehaviour
{

    private ParticleSystem particle;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnBecameInvisible()
    {
        particle.Pause(true);
        Debug.Log("invisible fired");
    }
    void OnBecameVisible()
    {
        particle.Play(true);
        Debug.Log("visible fired");
    }

}