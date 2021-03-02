using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParticelSize : MonoBehaviour
{
    private Camera sceneCamera;
    [SerializeField] private ParticleSystem[] particles;

    // Start is called before the first frame update
    void Start()
    {
        sceneCamera = GameObject.Find("Scene Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change the size of the particles based on what camera is active.
        if (sceneCamera.isActiveAndEnabled)
        {
            foreach (ParticleSystem particle in particles)
            {
                var main = particle.main;
                main.startSize = 1;
            }
        }
        else
        {
            foreach (ParticleSystem particle in particles)
            {
                var main = particle.main;
                main.startSize = 3;
            }
        }
    }
}
