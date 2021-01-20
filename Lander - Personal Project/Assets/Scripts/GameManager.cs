using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI fuelText;
    [SerializeField] TextMeshProUGUI altitudeText;
    [SerializeField] TextMeshProUGUI horizontalSpeedText;
    [SerializeField] TextMeshProUGUI verticalSpeedText;

    PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.SetText("Score ");
        timeText.SetText("Time ");
        fuelText.SetText("Fuel ");

        altitudeText.SetText("Altitude ");
        horizontalSpeedText.SetText("Horizontal speed: ");
        verticalSpeedText.SetText("Vertical speed: " + playerControllerScript.verticalSpeed);
    }
}
