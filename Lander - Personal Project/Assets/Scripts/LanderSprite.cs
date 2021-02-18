using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderSprite : MonoBehaviour
{
    public Sprite[] bigLanderRotations;
    public Sprite[] smallLanderRotations;

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
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            landerRotation = GameObject.FindGameObjectWithTag("Player").transform.rotation.eulerAngles.z;
        }

        int spriteIndex = 0;

        if (landerRotation > halfRotation && landerRotation < 180)
        {
            spriteIndex = Mathf.RoundToInt(landerRotation / oneRotation);
        }
        else if (landerRotation > 180 && landerRotation < 360 - halfRotation)
        {
            spriteIndex = -Mathf.RoundToInt(landerRotation / oneRotation) + 40;
        }

        if (followPlayerScript.zoom)
        {
            spriteRenderer.sprite = bigLanderRotations[spriteIndex];
        }
        else
        {
            spriteRenderer.sprite = smallLanderRotations[spriteIndex];
        }
    }
}
