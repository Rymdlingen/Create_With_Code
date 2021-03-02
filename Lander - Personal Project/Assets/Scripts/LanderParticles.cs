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

    // Update is called once per frame
    void Update()
    {
        // If there is a new lander, get all the components.
        if (newLander)
        {
            particleSpreadOnGround = GameObject.Find("Fire on ground").gameObject;
            dustParticles = GameObject.Find("Dust particles").GetComponent<ParticleSystem>();
            newLander = false;
        }

        // Play the right particles.
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            // Position the spread of the ground particles at the right place and play the ground particles if they are needed.
            PlaceParticleSpreadOnGround();

            // When fuel is used play the fire particles.
            if (playerControllerScript.usingFuel)
            {
                // Play all fire particles.
                foreach (ParticleSystem particle in fireParticles)
                {
                    particle.Play();
                }
            }
            else
            {
                // Stop all fire particles.
                foreach (ParticleSystem particle in fireParticles)
                {
                    particle.Stop();
                }
            }
        }
    }

    // Positions the particel spread on the ground in the right angle.
    private void PlaceParticleSpreadOnGround()
    {
        // Check where the particles are hitting the ground.
        Vector3 localDown = transform.TransformDirection(Vector3.down);
        bool hitFound = Physics.Raycast(transform.position - localDown * 10, localDown, out hit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore);

        // Check if the lander is close enough to the ground.
        if (hitFound && hit.distance < 25)
        {
            // Position and rotate the particle spread.
            particleSpreadOnGround.transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);
            particleSpreadOnGround.transform.rotation = hit.transform.rotation;

            // If the fire is hiting the griund and not a platform, also play the ground particles.
            if (playerControllerScript.usingFuel && hit.transform.CompareTag("Ground"))
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
            // Move the spread to a safe distance when not needed.
            particleSpreadOnGround.transform.position = new Vector3(transform.position.x, transform.position.y - 30, transform.position.z);
            dustParticles.Stop();
        }
    }
}
