using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeController : MonoBehaviour
{
    public static ScreenShakeController useMethod;

    private float shakePower;
    private float shakeTimeRemaining;
    private float shakeFade;

    private bool resetCameraAfterShake = false;

    private Vector3 sceneCameraStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        // The starting position of the scene camera, used to reset after shake.
        sceneCameraStartPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        // Shake the camera.
        if (shakeTimeRemaining > 0)
        {
            // Random amount of shake.
            float shakeX = Random.Range(-1f, 1f) * shakePower;
            float shakeY = Random.Range(-1f, 1f) * shakePower;

            // Shake the active camera.
            if (transform.GetComponent<Camera>().isActiveAndEnabled)
            {
                transform.position = new Vector3(transform.position.x + shakeX, transform.position.y + shakeY, transform.position.z);
            }

            // Reduce the timer for all cameras with this script.
            shakeTimeRemaining = Mathf.MoveTowards(shakeTimeRemaining, 0f, shakeFade * Time.deltaTime);
        }
        else if (resetCameraAfterShake)
        {
            // Reset the scene cameras position if it is changed. (The zoom camera resets in the FollowPlayer script)
            if (gameObject.CompareTag("SceneCamera") && gameObject.GetComponent<Camera>().isActiveAndEnabled && transform.position != sceneCameraStartPosition)
            {
                transform.position = sceneCameraStartPosition;
            }

            // The camera is repositioned and reset is changed to false.
            resetCameraAfterShake = false;
        }

    }

    // Sets the duration and the power of the screen shake. Also calculates the fade of the shaking and sets the camera reset to true.
    public void ShakeScreen(float shakeLength, float shakePower)
    {
        shakeTimeRemaining = shakeLength;
        this.shakePower = shakePower;

        shakeFade = shakePower / shakeLength;

        resetCameraAfterShake = true;
    }
}
