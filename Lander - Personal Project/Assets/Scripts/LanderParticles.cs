using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderParticles : MonoBehaviour
{
    public PlayerController playerControllerScript;
    public ParticleSystem[] fireParticles;

    private RaycastHit hit;
    private GameObject particleSpreadOnGround;

    // Start is called before the first frame update
    void Start()
    {
        particleSpreadOnGround = GameObject.Find("Fire on ground").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        PlaceParticleSpreadOnGround();
        Debug.DrawLine(transform.position, hit.point, Color.cyan, 5f);

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

    private void PlaceParticleSpreadOnGround()
    {
        Vector3 localDown = transform.TransformDirection(Vector3.down);
        Physics.Raycast(transform.position, localDown, out hit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore);

        if (hit.distance < 15)
        {
            float yOffset = hit.transform.lossyScale.y / 2;
            particleSpreadOnGround.transform.position = new Vector3(transform.position.x, hit.transform.position.y + yOffset, transform.position.z);
            particleSpreadOnGround.transform.rotation = hit.transform.rotation;
        }
        else
        {
            particleSpreadOnGround.transform.position = new Vector3(transform.position.x, transform.position.y - 30, transform.position.z);
        }
    }
}
