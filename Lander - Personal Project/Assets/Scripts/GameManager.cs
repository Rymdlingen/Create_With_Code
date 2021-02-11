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

    [SerializeField] TextMeshProUGUI successfulLandningText;
    [SerializeField] TextMeshProUGUI crashedText;

    private int fuelLeft = 4000;
    private int timer;
    private int seconds = 0;
    private int minutes = 0;
    private bool minuteAdded = true;
    public int oldTime;
    float addTime;

    public int newPoints;
    private string newPointsString;
    private int score = 0;
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
        // oldTime = Mathf.RoundToInt(Time.realtimeSinceStartup);
        //Debug.Log(oldTime);
        minutes = 0;
        CalculateScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (newPoints > 0)
        {
            CalculateScore();
        }

        if (fuelLeft > 0)
        {
            CalculateMinutesAndSeconds();
        }
        DisplayTime();
        CalculateFuel();

        altitudeText.SetText("Altitude " + Mathf.RoundToInt(playerControllerScript.hit.distance - 10));
        horizontalSpeedText.SetText("Horizontal speed: " + playerControllerScript.horizontalSpeed + "  " + playerControllerScript.horizontalArrow);
        verticalSpeedText.SetText("Vertical speed: " + playerControllerScript.verticalSpeed + "  " + playerControllerScript.verticalArrow);

        if (playerControllerScript.basePoints > 0)
        {
            SuccessfulLandingScreen(true);
            playerControllerScript.basePoints = 0;
        }

        if (fuelLeft < 1)
        {
            EndGame();
        }
    }

    private void CalculateMinutesAndSeconds()
    {
        // Start timer.
        addTime += Time.deltaTime;
        if (addTime >= 1)
        {
            addTime -= 1;
            timer += 1;
        }
        //Debug.Log(addTime);
        Debug.Log("timer " + timer + "minutes" + minutes + "seconds" + seconds);


        // Calculate how many seconds has passed minus all passed full minutes.
        seconds = timer - (60 * minutes);

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
        score += newPoints;
        newPointsString = newPoints.ToString();
        newPoints = 0;

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
        if (playerControllerScript.addFuel)
        {
            fuelLeft += 500;
            playerControllerScript.addFuel = false;
        }

        if (playerControllerScript.usingFuel && playerControllerScript.gameActive == true && fuelLeft > 0)
        {
            fuelLeft -= 1;
        }
        fuelText.SetText("Fuel " + fuelLeft);
    }

    public void ResetPlayer()
    {
        GameObject.Find("Focal Point").GetComponent<FollowPlayer>().EnableMainCamera();
        player = Instantiate(playerPrefab, new Vector3(-427, 147, -2), playerPrefab.transform.rotation);
        playerControllerScript = player.GetComponent<PlayerController>();
        player.GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0) * playerControllerScript.force * Time.deltaTime, ForceMode.Impulse);
        playerControllerScript.gameActive = true;
    }

    public void SuccessfulLandingScreen(bool active)
    {
        if (playerControllerScript.basePoints == 15)
        {
            successfulLandningText.SetText("You landed hard\nCommunication system destroyed\n" + newPointsString + " points");
        }
        else
        {
            successfulLandningText.SetText("Congratulations!\nThat was a great landing\n" + newPointsString + " points");
        }
        GameObject.Find("Canvas").transform.Find("SuccessfulLanding").gameObject.SetActive(active);
    }

    public void FailedLandingScreen(bool active)
    {
        crashedText.SetText("You just destroyed a 100 megabuck lander");
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
