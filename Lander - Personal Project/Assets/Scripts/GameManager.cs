using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Text fields.
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI fuelText;
    [SerializeField] TextMeshProUGUI altitudeText;
    [SerializeField] TextMeshProUGUI horizontalSpeedText;
    [SerializeField] TextMeshProUGUI verticalSpeedText;

    [SerializeField] TextMeshProUGUI successfulLandningText;
    [SerializeField] TextMeshProUGUI crashedText;
    [SerializeField] TextMeshProUGUI outerSpaceText;

    [SerializeField] TextMeshProUGUI gameOverText;

    // Amount of fuel the player starts with.
    public int fuelLeft = 3000;

    // Timer variables.
    private int timer;
    private string timeString;
    private int seconds = 0;
    private int minutes = 0;
    private bool minuteAdded = true;
    private int oldTime;
    private float addTime;

    // Score variables.
    public int newPoints;
    private string newPointsString;
    private int score = 0;
    private string scoreString = "";


    public GameObject playerPrefab;

    public PlayerController playerControllerScript;

    private GameObject player;

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

        altitudeText.SetText(Mathf.RoundToInt(playerControllerScript.hit.distance - 8).ToString());
        horizontalSpeedText.SetText(playerControllerScript.horizontalSpeed.ToString());
        verticalSpeedText.SetText(playerControllerScript.verticalSpeed.ToString());

        if (playerControllerScript.basePoints > 0)
        {
            SuccessfulLandingScreen(true);
            playerControllerScript.basePoints = 0;
        }

        if (fuelLeft < 1)
        {
            GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(true);

            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
            {
                MakePlayerFall();
            }
            else
            {
                EndScreen(true);
            }
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(false);
        }

        DriftedOutInSpaceScreen(playerControllerScript.hasDrifteOutInSpace);
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
        timeText.SetText(timeString);
    }

    private void CalculateScore()
    {
        score += newPoints;
        newPointsString = newPoints.ToString();
        newPoints = 0;

        scoreString = MakeFourCharString(score.ToString(), '0');

        scoreText.SetText(scoreString);
    }

    private void CalculateFuel()
    {
        if (playerControllerScript.addFuel)
        {
            fuelLeft += 500;
            playerControllerScript.addFuel = false;
        }

        if (playerControllerScript.usingFuel && fuelLeft > 0)
        {
            fuelLeft -= 1;
        }

        fuelText.SetText(MakeFourCharString(fuelLeft.ToString(), '0'));
    }

    private string MakeFourCharString(string originalString, char fillerChar)
    {
        string fillerString = fillerChar.ToString();

        for (int stringLength = originalString.Length; stringLength < 4; stringLength++)
        {
            originalString = fillerString + originalString;
        }

        return originalString;
    }

    public void ResetPlayer()
    {
        // Switch to scene camera if zoom camera is active.
        if (GameObject.Find("Zoom Camera").GetComponent<Camera>().isActiveAndEnabled)
        {
            GameObject.Find("Focal Point").GetComponent<FollowPlayer>().EnableSceneCamera();
        }

        // TODO fix the hardcoded position, needs to be based on screen size.
        // Instantiate a new lander.
        player = Instantiate(playerPrefab, new Vector3(-427, 147, -2), playerPrefab.transform.rotation);
        playerControllerScript = player.GetComponent<PlayerController>();

        LanderParticles particlesScript = player.transform.Find("Lander Fire Particles").GetComponent<LanderParticles>();
        particlesScript.newLander = true;

        // Add force to the player so it moves into the screen.
        player.GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0) * playerControllerScript.force * Time.deltaTime, ForceMode.Impulse);

        // Set game to active.
        playerControllerScript.gameActive = true;
    }

    // METHODS THAT CHANGE WHAT TEXT IS DISPLAYED ON SCRREN.

    // Change text on screen.
    public void SuccessfulLandingScreen(bool active)
    {
        // Successful landings can only happen if the player has fuel left.
        if (fuelLeft > 0)
        {
            if (playerControllerScript.basePoints == 15)
            {
                // Text for hard landing.
                successfulLandningText.SetText("You landed hard\nCommunication system destroyed\n" + newPointsString + " points");
            }
            else
            {
                // Text for soft landing.
                successfulLandningText.SetText("Congratulations!\nThat was a great landing\n" + newPointsString + " points");
            }

            // Display the text and button.
            GameObject.Find("Canvas").transform.Find("SuccessfulLanding").gameObject.SetActive(active);
        }
    }

    // Change text on screen.
    public void FailedLandingScreen(bool active)
    {
        // Only display this screen if the player has fuel left, if the player has no fuel left the game over screen will be displayed instead.
        if (fuelLeft > 0)
        {
            crashedText.SetText("You just destroyed a 100 megabuck lander");
            GameObject.Find("Canvas").transform.Find("FailedLanding").gameObject.SetActive(active);
        }
    }

    // Change text on screen.
    public void DriftedOutInSpaceScreen(bool active)
    {
        outerSpaceText.SetText("The lander drifted out in outer space.");
        GameObject.Find("Canvas").transform.Find("OuterSpace").gameObject.SetActive(active);
    }

    // Change text on screen.
    private void EndScreen(bool active)
    {
        gameOverText.SetText("Game Over!\nYou had " + playerControllerScript.successfulLandings + " successful landings and you crashed " + playerControllerScript.crashes + " times.\n You scored " + score + " points.\nYour mission lasted for " + minutes + " minutes and " + seconds + " seconds.");
        GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(!active);
        GameObject.Find("Canvas").transform.Find("GameOver").gameObject.SetActive(active);
    }



    // This happens if a button is pressed that makes the game continue.
    public void ContinueButton()
    {
        // Reset screens and losing condition.
        SuccessfulLandingScreen(false);
        FailedLandingScreen(false);
        playerControllerScript.hasDrifteOutInSpace = false;

        // Destroy the lander if it wasn't destroyed for some reason ( successful landing is a reason).
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            Destroy(playerControllerScript.gameObject);
        }

        // Spawn a new lander.
        ResetPlayer();
    }

    private void MakePlayerFall()
    {
        playerControllerScript.OutOfFuel();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
