using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSprite : MonoBehaviour
{
    [SerializeField] private Sprite smallSprite;
    [SerializeField] private Sprite bigSprite;

    private Camera zoomCamera;
    private bool zoomCameraActive;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        zoomCamera = GameObject.Find("Zoom Camera").GetComponent<Camera>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the zoom camera is active or not.
        zoomCameraActive = zoomCamera.isActiveAndEnabled;

        // If the zoom camera is active render the big sprites.
        if (zoomCameraActive)
        {
            if (spriteRenderer.sprite != bigSprite)
            {
                spriteRenderer.sprite = bigSprite;
            }
        }
        else
        {
            // If the zoom camera is not active then the scene camera is active, render the small sprite.
            if (spriteRenderer.sprite != smallSprite)
            {
                spriteRenderer.sprite = smallSprite;
            }
        }
    }
}
