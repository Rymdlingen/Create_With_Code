using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] bigLanderRotations;
    [SerializeField] private Sprite[] smallLanderRotations;
    private FollowPlayer followPlayerScript;
    private SpriteRenderer spriteRenderer;
    private const float oneRotation = 11.25f;
    private const float halfRotation = oneRotation / 2;
    [SerializeField] private float landerRotation;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        followPlayerScript = transform.GetComponent<FollowPlayer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        // Get the rotation of the player.
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            landerRotation = GameObject.FindGameObjectWithTag("Player").transform.rotation.eulerAngles.z;
        }

        // The index of the rotated sprite int sprite array.
        int spriteIndex = 0;

        // Find the index for the rotation if the player is rotated to the left.
        if (landerRotation > halfRotation && landerRotation < 180)
        {
            spriteIndex = Mathf.RoundToInt(landerRotation / oneRotation);
        }
        // Find the index for the rotation if the player is rotated to the right.
        else if (landerRotation > 180 && landerRotation < 360 - halfRotation)
        {
            spriteIndex = -Mathf.RoundToInt(landerRotation / oneRotation) + 40;
        }

        // Chose the size of the sprite, big if the zoom camera is active.
        if (followPlayerScript.zoom)
        {
            spriteRenderer.sprite = bigLanderRotations[spriteIndex];
        }
        // Small sprite if the scene camera is active.
        else
        {
            spriteRenderer.sprite = smallLanderRotations[spriteIndex];
        }
    }
}
