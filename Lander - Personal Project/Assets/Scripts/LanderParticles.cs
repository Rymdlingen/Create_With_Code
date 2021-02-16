using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderParticles : MonoBehaviour
{
    public PlayerController playerControllerScript;
    public ParticleSystem[] fireParticles;

    private RaycastHit hit;
    private GameObject particleSpreadOnGround;
    private ParticleSystem dustParticles;

    public bool newLander;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (newLander)
        {
            particleSpreadOnGround = GameObject.Find("Fire on ground").gameObject;
            dustParticles = GameObject.Find("Dust particles").GetComponent<ParticleSystem>();
            newLander = false;
        }

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            PlaceParticleSpreadOnGround();


            if (playerControllerScript.usingFuel)
            {
                foreach (ParticleSystem particle in fireParticles)
                {
                    particle.Play();
                }
            }
            else
            {
                foreach (ParticleSystem particle in fireParticles)
                {
                    particle.Stop();
                }
            }
        }
    }

    private void PlaceParticleSpreadOnGround()
    {
        Vector3 localDown = transform.TransformDirection(Vector3.down);
        bool hitFound = Physics.Raycast(transform.position - localDown * 10, localDown, out hit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore);
        Debug.DrawLine(transform.position - localDown * 10, hit.point, Color.cyan, 5f);

        if (hitFound && hit.distance < 25)
        {
            particleSpreadOnGround.transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);
            particleSpreadOnGround.transform.rotation = hit.transform.rotation;

            if (playerControllerScript.usingFuel)
            {
                dustParticles.Play();
            }
            else
            {
                dustParticles.Stop();
            }
        }
        else
        {
            particleSpreadOnGround.transform.position = new Vector3(transform.position.x, transform.position.y - 30, transform.position.z);
            dustParticles.Stop();
        }
    }
}
