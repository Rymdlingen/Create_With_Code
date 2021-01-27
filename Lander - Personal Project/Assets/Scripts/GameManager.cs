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

    private int fuelLeft = 2000;
    private int timer;
    private int seconds;
    private int minutes = 0;
    private bool minuteAdded = false;

    public int score = 0;
    private string scoreString = "";

    PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            score += 50;
        }

        scoreString = score.ToString();
        if (scoreString.Length == 1)
        {
            scoreString = "000" + scoreString;
        }
        else if (scoreString.Length == 2)
        {
            scoreString = "00" + scoreString;
        }
        else if (scoreString.Length == 3)
        {
            scoreString = "0" + scoreString;
        }

        scoreText.SetText("Score " + scoreString);

        CalculateMinutesAndSeconds();

        if (playerControllerScript.usingFuel && playerControllerScript.gameActive == true)
        {
            fuelLeft -= 1;
        }
        fuelText.SetText("Fuel " + fuelLeft);


        altitudeText.SetText("Altitude ");
        horizontalSpeedText.SetText("Horizontal speed: " + playerControllerScript.horizontalSpeed + "  " + playerControllerScript.horizontalArrow);
        verticalSpeedText.SetText("Vertical speed: " + playerControllerScript.verticalSpeed + "  " + playerControllerScript.verticalArrow);
    }

    private void CalculateMinutesAndSeconds()
    {
        // Start timer.
        timer = Mathf.RoundToInt(Time.realtimeSinceStartup);

        // Calculate how many seconds has passed minus all passed full minutes.
        seconds = timer - 60 * minutes;

        // When 60 seconds has passed, add on minute to the minute counter. Change minute added to true, resets to false after one second.
        if (seconds % 60 == 0 && !minuteAdded)
        {
            minutes += 1;
            minuteAdded = true;
        }
        else if (seconds == 1)
        {
            minuteAdded = false;
        }

        // Converts the minutes to a string and adds a 0 to the minute text if less than 10 minutes has passed.
        string minutesText = minutes.ToString();
        if (minutesText.Length < 2)
        {
            minutesText = "0" + minutes;
        }

        // Converts the seconds to a string and adds a 0 to the second text if less than 10 seconds has passed.
        string secondsText = seconds.ToString();
        if (secondsText.Length < 2)
        {
            secondsText = "0" + seconds;
        }

        // Concatenate the minute and seconds.
        string time = minutesText + ":" + secondsText;

        // Change the displayed time.
        timeText.SetText("Time " + time);
    }
}
