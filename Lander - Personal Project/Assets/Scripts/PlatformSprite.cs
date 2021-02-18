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
        zoomCameraActive = zoomCamera.isActiveAndEnabled;

        if (zoomCameraActive)
        {
            if (spriteRenderer.sprite != bigSprite)
            {
                spriteRenderer.sprite = bigSprite;
            }
        }
        else
        {
            if (spriteRenderer.sprite != smallSprite)
            {
                spriteRenderer.sprite = smallSprite;
            }
        }
    }
}
