using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderSprite : MonoBehaviour
{
    public Sprite[] bigLanderRotations;
    public Sprite[] smallLanderRotations;

    private FollowPlayer followPlayerScript;
    private SpriteRenderer spriteRenderer;
    private float oneRotation = 11.25f;
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
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            landerRotation = GameObject.FindGameObjectWithTag("Player").transform.rotation.eulerAngles.z;
        }

        // Lander straight up.
        if (landerRotation < oneRotation || landerRotation > 361 - oneRotation)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[0];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[0];
            }
        }

        // One step to the left.
        if (landerRotation < oneRotation * 2 && landerRotation > oneRotation)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[1];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[1];
            }
        }

        // Two steps to the left.
        if (landerRotation < oneRotation * 3 && landerRotation > oneRotation * 2)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[2];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[2];
            }
        }

        // Three steps to the left.
        if (landerRotation < oneRotation * 4 && landerRotation > oneRotation * 3)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[3];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[3];
            }
        }

        // Four steps to the left.
        if (landerRotation < oneRotation * 5 && landerRotation > oneRotation * 4)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[4];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[4];
            }
        }

        // Five steps to the left.
        if (landerRotation < oneRotation * 6 && landerRotation > oneRotation * 5)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[5];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[5];
            }
        }

        // Six steps to the left.
        if (landerRotation < oneRotation * 7 && landerRotation > oneRotation * 6)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[6];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[6];
            }
        }

        // Seven steps to the left.
        if (landerRotation < oneRotation * 8 && landerRotation > oneRotation * 7)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[7];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[7];
            }
        }

        // Eight steps to the left.
        if (landerRotation < oneRotation * 9 && landerRotation > oneRotation * 8)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[8];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[8];
            }
        }

        // One step to the left.
        if (landerRotation > 361 - oneRotation * 2 && landerRotation < 361 - oneRotation)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[9];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[9];
            }
        }

        // Two steps to the right.
        if (landerRotation > 361 - oneRotation * 3 && landerRotation < 361 - oneRotation * 2)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[10];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[10];
            }
        }

        // Three steps to the right.
        if (landerRotation > 361 - oneRotation * 4 && landerRotation < 361 - oneRotation * 3)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[11];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[11];
            }
        }

        // Four steps to the right.
        if (landerRotation > 361 - oneRotation * 5 && landerRotation < 361 - oneRotation * 4)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[12];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[12];
            }
        }

        // Five steps to the right.
        if (landerRotation > 361 - oneRotation * 6 && landerRotation < 361 - oneRotation * 5)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[13];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[13];
            }
        }

        // Six steps to the right.
        if (landerRotation > 361 - oneRotation * 7 && landerRotation < 361 - oneRotation * 6)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[14];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[14];
            }
        }

        // Seven steps to the right.
        if (landerRotation > 361 - oneRotation * 8 && landerRotation < 361 - oneRotation * 7)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[15];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[15];
            }
        }

        // Eight steps to the right.
        if (landerRotation > 361 - oneRotation * 9 && landerRotation < 361 - oneRotation * 8)
        {
            if (followPlayerScript.zoom)
            {
                spriteRenderer.sprite = bigLanderRotations[16];
            }
            else
            {
                spriteRenderer.sprite = smallLanderRotations[16];
            }
        }
    }
}
