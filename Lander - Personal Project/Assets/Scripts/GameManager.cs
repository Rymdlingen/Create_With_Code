using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Text fields.
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI altitudeText;
    [SerializeField] private TextMeshProUGUI horizontalSpeedText;
    [SerializeField] private TextMeshProUGUI verticalSpeedText;

    [SerializeField] private TextMeshProUGUI successfulLandningText;
    [SerializeField] private TextMeshProUGUI crashedText;
    [SerializeField] private TextMeshProUGUI outerSpaceText;

    [SerializeField] private TextMeshProUGUI gameOverText;

    // Amount of fuel the player starts with.
    public int fuelLeft = 3000;

    // Timer variables.
    private int timer;
    private string timeString;
    private int seconds = 0;
    private int minutes = 0;
    private bool minuteAdded = true;
    private float addTime;

    // Score variables.
    public int newPoints;
    private string newPointsString;
    private int score = 0;

    // Power p variables.
    private int fuelInPowerUp = 500;

    public GameObject playerPrefab;
    public PlayerController playerControllerScript;
    public MainMenu mainMenuScript;
    private GameObject player;
    private EventSystem eventSystem;

    bool gamePaused = false;
    bool gameOver = false;

    // Counts landings and crashes.
    private int crashes;
    private int successfulLandings;

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the first lander.
        ResetPlayer();

        // Reset the timer.
        minutes = 0;

        // Find the event system, used for buttons.
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // Pauses the game if the any "Cancel" button is pressed and the game is not already paused.
        if (Input.GetButtonDown("Cancel") && gamePaused == false)
        {
            // Activates the pase screen and sets the pause bool to true.
            PauseScreen(true);
            gamePaused = true;
        }

        // If there is new points gained, call the calclation of score.
        if (newPoints > 0)
        {
            CalculateScore();
        }

        // As long as the player has fuel left (and time is not paused) the time is calculated.
        if (fuelLeft > 0)
        {
            CalculateMinutesAndSeconds();
        }
        DisplayTime();

        // Calculate and display how much fuel the player has left.
        CalculateFuel();

        // Activate the out in space message if the player has exited the screen.
        if (playerControllerScript.hasDrifteOutInSpace)
        {
            DriftedOutInSpaceScreen(true);
            playerControllerScript.hasDrifteOutInSpace = false;
        }

        // Fisplay the text for altitde and speed.
        altitudeText.SetText(Mathf.RoundToInt(playerControllerScript.hit.distance - 8).ToString());
        horizontalSpeedText.SetText(playerControllerScript.horizontalSpeed.ToString());
        verticalSpeedText.SetText(playerControllerScript.verticalSpeed.ToString());

        // Activate the screen for a successfl landing when there is new points gained.
        if (playerControllerScript.basePoints > 0)
        {
            SuccessfulLandingScreen(true);
            playerControllerScript.basePoints = 0;
        }

        // When the player is out of fuel, start the end.
        if (fuelLeft < 1)
        {
            // As long as there is a lander, make the it fall and display the out of fuel message.
            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
            {
                GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(true);
                MakePlayerFall();
                gameOver = true;
            }
            else if (gameOver)
            {
                // Hide the out of fuel message and display the game over screen.
                GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(false);
                EndScreen(true);
                gameOver = false;
            }
        }
    }

    // Counts the time the player has spent trying to land, pauses when game is not active (because time is paused then).
    private void CalculateMinutesAndSeconds()
    {
        // Adds the time since the last frame.
        addTime += Time.deltaTime;
        // When a second is collected it is added to the timer.
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

    // Changes the displayed time.
    public void DisplayTime()
    {
        timeText.SetText(timeString);
    }

    // Takes the newly gained points and adds them to the total score.
    private void CalculateScore()
    {
        // Add the new points to the total.
        score += newPoints;
        // Make a string out of the new points to display them.
        newPointsString = newPoints.ToString();
        // Reset the new points.
        newPoints = 0;

        // Set the text to display the total score.
        scoreText.SetText(MakeFourCharString(score.ToString(), '0'));
    }

    // Calculates how much fuel is left and sets the displayed text to the right number.
    private void CalculateFuel()
    {
        // Adds fuel if a powerup is collected.
        if (playerControllerScript.addFuel)
        {
            fuelLeft += fuelInPowerUp;
            // Resets the bool.
            playerControllerScript.addFuel = false;
        }

        // Takes away fuel if space is pressed while game is active.
        if (playerControllerScript.usingFuel && fuelLeft > 0 && !gamePaused && !playerControllerScript.hasDrifteOutInSpace)
        {
            // Takes away 1 fuel every frame.
            fuelLeft -= 1;
        }

        // Sets the displayed text to the correct amount of fuel.
        fuelText.SetText(MakeFourCharString(fuelLeft.ToString(), '0'));
    }

    // Used to display strings with four chars by adding 0's in front.
    private string MakeFourCharString(string originalString, char fillerChar)
    {
        // Converting the char to a string.
        string fillerString = fillerChar.ToString();

        // Adding the filler until the length of the string to be displayed is 4.
        for (int stringLength = originalString.Length; stringLength < 4; stringLength++)
        {
            originalString = fillerString + originalString;
        }

        // Returns a four char string.
        return originalString;
    }

    public void ResetPlayer()
    {
        // Switch to scene camera if zoom camera is active.
        if (GameObject.Find("Zoom Camera").GetComponent<Camera>().isActiveAndEnabled)
        {
            GameObject.Find("Focal Point").GetComponent<FollowPlayer>().EnableSceneCamera();
        }

        // Caclculate starting position for new lander.
        Camera sceneCamera = GameObject.Find("Scene Camera").GetComponent<Camera>();
        int startX = sceneCamera.targetTexture.width + sceneCamera.targetTexture.width / 2;
        int startY = sceneCamera.targetTexture.height - sceneCamera.targetTexture.height / 3;

        // Instantiate a new lander and find the needed components.
        player = Instantiate(playerPrefab, new Vector3(-startX - 15, startY, -3), playerPrefab.transform.rotation);
        playerControllerScript = player.GetComponent<PlayerController>();
        LanderParticles particlesScript = player.transform.Find("Lander Fire Particles").GetComponent<LanderParticles>();
        particlesScript.newLander = true;

        // Add force to the player so it moves into the screen.
        player.GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0).normalized * 30, ForceMode.VelocityChange);

        // Set game to active.
        playerControllerScript.gameActive = true;
    }

    // METHODS THAT CHANGE WHAT TEXT IS DISPLAYED ON SCREEN.

    // Change text on screen.
    public void SuccessfulLandingScreen(bool active)
    {
        // Check if the player is given points for a good or a bad landing and display the right message.
        if (playerControllerScript.basePoints == 15)
        {
            // Text for bad landing.
            successfulLandningText.SetText("You landed hard\nCommunication system destroyed\n" + newPointsString + " points");
        }
        else
        {
            // Text for good landing.
            successfulLandningText.SetText("Congratulations!\nThat was a great landing\n" + newPointsString + " points");
        }

        // Display the text and button.
        GameObject successScreen = GameObject.Find("Canvas").transform.Find("SuccessfulLanding").gameObject;
        successScreen.SetActive(active);

        GameObject[] buttons = new GameObject[] { successScreen.transform.Find("Continue Button").gameObject };

        // Pause the time, add a successful landing and activate button.
        if (active)
        {
            Time.timeScale = 0;
            successfulLandings++;
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            // Start time and deactivate button.
            Time.timeScale = 1;
            DeactivateButton(buttons);
        }
    }

    // Change text on screen.
    public void FailedLandingScreen(bool active)
    {


        // Only display this screen if the player has fuel left, if the player has no fuel left the game over screen will be displayed instead.
        if (fuelLeft > 0)
        {
            crashedText.SetText("You just destroyed a 100 megabuck lander");

            // Display the text and button.
            GameObject failScreen = GameObject.Find("Canvas").transform.Find("FailedLanding").gameObject;
            failScreen.SetActive(active);

            GameObject[] buttons = new GameObject[] { failScreen.transform.Find("Continue Button").gameObject };

            if (active)
            {
                Time.timeScale = 0;
                crashes++;
                StartCoroutine(WaitWithActivatingButton(buttons));
            }
            else
            {
                Time.timeScale = 1;
                DeactivateButton(buttons);
            }
        }
    }

    // Change text on screen.
    public void DriftedOutInSpaceScreen(bool active)
    {
        outerSpaceText.SetText("You went to far away from the planet and your lander is no longer in orbit");

        // Display the text and button.
        GameObject spaceScreen = GameObject.Find("Canvas").transform.Find("OuterSpace").gameObject;
        spaceScreen.SetActive(active);

        GameObject[] buttons = new GameObject[] { spaceScreen.transform.Find("Continue Button").gameObject };

        if (active)
        {
            Time.timeScale = 0;
            playerControllerScript.usingFuel = false;
            crashes++;
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            Time.timeScale = 1;
            DeactivateButton(buttons);
        }

    }

    // Change text on screen.
    private void EndScreen(bool active)
    {
        gameOverText.SetText("Game Over!\nYou had " + successfulLandings + " successful landings and " + crashes + " failed attempts.\nYou scored " + score + " points.\nYour mission lasted for " + minutes + " minutes and " + seconds + " seconds.");
        GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(!active);

        // Display the text and button.
        GameObject endScreen = GameObject.Find("Canvas").transform.Find("GameOver").gameObject;
        endScreen.SetActive(active);

        GameObject[] buttons = new GameObject[] { endScreen.transform.Find("Continue Button").gameObject };

        if (active)
        {
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            DeactivateButton(buttons);
        }
    }

    // Change text on screen.
    public void PauseScreen(bool active)
    {
        // Display the text and buttons.
        GameObject pauseScreen = GameObject.Find("Canvas").transform.Find("PauseScreen").gameObject;
        pauseScreen.SetActive(active);

        GameObject[] buttons = new GameObject[] { pauseScreen.transform.Find("Continue Button").gameObject, pauseScreen.transform.Find("Restart Button").gameObject };

        if (active)
        {
            Time.timeScale = 0;
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            Time.timeScale = 1;

            DeactivateButton(buttons);

            gamePaused = false;
        }
    }


    // This happens if a button is pressed that makes the game continue.
    public void ContinueButton()
    {
        // Reset screens and losing condition.
        SuccessfulLandingScreen(false);
        FailedLandingScreen(false);
        DriftedOutInSpaceScreen(false);

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
        //SceneManager.UnloadSceneAsync("Lander");
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator WaitWithActivatingButton(GameObject[] buttons)
    {
        yield return new WaitForSecondsRealtime(1);

        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = true;
        }

        eventSystem.SetSelectedGameObject(buttons[0]);
    }

    private void DeactivateButton(GameObject[] buttons)
    {
        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }
}
