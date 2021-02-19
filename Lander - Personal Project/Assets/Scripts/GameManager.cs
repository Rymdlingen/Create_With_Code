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
    public int fuelLeft = 1;

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

    private EventSystem eventSystem;

    bool gamePaused = false;
    bool gameOver = false;

    public int crashes;
    public int successfulLandings;

    // Start is called before the first frame update
    void Start()
    {
        ResetPlayer();
        // oldTime = Mathf.RoundToInt(Time.realtimeSinceStartup);
        //Debug.Log(oldTime);
        minutes = 0;
        CalculateScore();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && gamePaused == false)
        {
            PauseScreen(true);
            gamePaused = true;
        }

        if (newPoints > 0)
        {
            CalculateScore();
        }

        if (fuelLeft > 0)
        {
            CalculateMinutesAndSeconds();
        }
        DisplayTime();

        if (!playerControllerScript.hasDrifteOutInSpace)
        {
            CalculateFuel();
        }
        else
        {
            DriftedOutInSpaceScreen(true);
            playerControllerScript.hasDrifteOutInSpace = false;
        }

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
                gameOver = true;
            }
            else if (gameOver)
            {
                EndScreen(true);
                gameOver = false;
            }
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(false);
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

        if (playerControllerScript.usingFuel && fuelLeft > 0 && !gamePaused && !playerControllerScript.hasDrifteOutInSpace)
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
        player.GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0).normalized * 30, ForceMode.VelocityChange);

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
            GameObject successScreen = GameObject.Find("Canvas").transform.Find("SuccessfulLanding").gameObject;
            successScreen.SetActive(active);

            GameObject[] buttons = new GameObject[] { successScreen.transform.Find("Continue Button").gameObject };

            if (active)
            {
                Time.timeScale = 0;
                successfulLandings++;
                StartCoroutine(WaitWithActivatingButton(buttons));
            }
            else
            {
                Time.timeScale = 1;
                foreach (GameObject button in buttons)
                {
                    button.GetComponent<Button>().interactable = false;
                }
            }
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
                foreach (GameObject button in buttons)
                {
                    button.GetComponent<Button>().interactable = false;
                }
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
            crashes++;
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            Time.timeScale = 1;
            foreach (GameObject button in buttons)
            {
                button.GetComponent<Button>().interactable = false;
            }
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
            foreach (GameObject button in buttons)
            {
                button.GetComponent<Button>().interactable = false;
            }
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

            foreach (GameObject button in buttons)
            {
                button.GetComponent<Button>().interactable = false;
            }

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
}
