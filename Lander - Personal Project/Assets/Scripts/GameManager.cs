using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI fuelText;
    [SerializeField] TextMeshProUGUI altitudeText;
    [SerializeField] TextMeshProUGUI horizontalSpeedText;
    [SerializeField] TextMeshProUGUI verticalSpeedText;

    private int fuelLeft = 4000;
    private int timer;
    private int seconds;
    private int minutes = 0;
    private bool minuteAdded = false;
    public int oldTime;

    public int score = 0;
    private string scoreString = "";

    public GameObject playerPrefab;

    public PlayerController playerControllerScript;

    private GameObject player;

    string timeString;

    public MainMenu mainMenuScript;

    // Start is called before the first frame update
    void Start()
    {
        ResetPlayer();
        //oldTime = Mathf.RoundToInt(Time.realtimeSinceStartup);
        //minutes = 0;
    }

    // Update is called once per frame
    void Update()
    {

        CalculateScore();
        if (fuelLeft > 1)
        {
            CalculateMinutesAndSeconds();
        }
        DisplayTime();
        CalculateFuel();

        altitudeText.SetText("Altitude " + Mathf.RoundToInt(playerControllerScript.hit.distance - 10));
        horizontalSpeedText.SetText("Horizontal speed: " + playerControllerScript.horizontalSpeed + "  " + playerControllerScript.horizontalArrow);
        verticalSpeedText.SetText("Vertical speed: " + playerControllerScript.verticalSpeed + "  " + playerControllerScript.verticalArrow);

        if (fuelLeft < 1)
        {
            EndGame();
        }
    }

    private void CalculateMinutesAndSeconds()
    {
        // Start timer.
        timer = Mathf.RoundToInt(Time.realtimeSinceStartup) - oldTime;

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
        timeString = minutesText + ":" + secondsText;


    }

    public void DisplayTime()
    {
        // Change the displayed time.
        timeText.SetText("Time " + timeString);
    }

    private void CalculateScore()
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
    }

    private void CalculateFuel()
    {
        if (playerControllerScript.usingFuel && playerControllerScript.gameActive == true && fuelLeft > 0)
        {
            fuelLeft -= 1;
        }
        fuelText.SetText("Fuel " + fuelLeft);
    }

    public void ResetPlayer()
    {
        GameObject.Find("Focal Point").GetComponent<FollowPlayer>().zoom = false;
        GameObject.Find("Focal Point").GetComponent<FollowPlayer>().EnableMainCamera();
        player = Instantiate(playerPrefab, new Vector3(-427, 147, -2), playerPrefab.transform.rotation);
        playerControllerScript = player.GetComponent<PlayerController>();
        player.GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0) * playerControllerScript.force * Time.deltaTime, ForceMode.Impulse);
        playerControllerScript.gameActive = true;
    }

    public void SuccessfulLandingScreen(bool active)
    {
        GameObject.Find("Canvas").transform.Find("SuccessfulLanding").gameObject.SetActive(active);
    }

    public void FailedLandingScreen(bool active)
    {
        GameObject.Find("Canvas").transform.Find("FailedLanding").gameObject.SetActive(active);
    }

    private void EndScreen(bool active)
    {
        GameObject.Find("Canvas").transform.Find("GameOver").gameObject.SetActive(active);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ContinueButton()
    {
        SuccessfulLandingScreen(false);
        FailedLandingScreen(false);
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            Destroy(playerControllerScript.gameObject);
        }
        ResetPlayer();
    }

    private void EndGame()
    {
        playerControllerScript.StopPlayer();
        EndScreen(true);
    }
}
