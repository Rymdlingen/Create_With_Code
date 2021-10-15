using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveScore
{
    string Name;
    int Score;

    public SaveScore(string nameString, int scoreInt)
    {
        Name = nameString;
        Score = scoreInt;
    }
}

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
    [SerializeField] private TextMeshProUGUI arcadeGameOverText;

    [SerializeField] private TextMeshProUGUI highscore1;
    [SerializeField] private TextMeshProUGUI highscore210;
    [SerializeField] private TextMeshProUGUI highscore1119;
    [SerializeField] private TextMeshProUGUI highscore2028;


    // Amount of fuel the player starts with.
    public int fuelLeft = 3000;
    private float fuelCalculation;

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
    public int score = 0;

    // Power up variables.
    private int fuelInPowerUp = 500;

    public GameObject playerPrefab;
    public PlayerController playerControllerScript;
    public MainMenu mainMenuScript;
    private GameObject player;
    private EventSystem eventSystem;
    private GameObject lowFuelScreen;

    private float lowFuelScreenTimer = 0;
    private bool showLowFuelScreen = true;

    public bool gamePaused = false;
    private bool gameOver = false;

    public bool hasCrashed = false;

    // Counts landings and crashes.
    private int crashes;
    private int successfulLandings;

    private GameObject currentSelectedButton;

    private bool arcadeMode = false;

    // Highscore
    private List<string> highscores = new List<string> { };

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the first lander.
        ResetPlayer();

        // Reset the timer.
        minutes = 0;

        // Find the event system, used for buttons.
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        lowFuelScreen = GameObject.Find("Canvas").transform.Find("LowFuel").gameObject;

        fuelCalculation = fuelLeft;

        if (MainMenu.arcadeMode)
        {
            arcadeMode = true;
            if (!Reload.reloaded)
            {
                ArcadeStartScreen(true);
            }
        }

        if (Reload.reloaded)
        {
            // Show start screen

            Reload.reloaded = false;
            Time.timeScale = 1;
        }
    }

    private void FixedUpdate()
    {
        // Calculate and display how much fuel the player has left.
        CalculateFuel();
    }

    // Update is called once per frame
    void Update()
    {
        // Pauses the game if the any "Cancel" button is pressed and the game is not already paused. Doesn't work in arvade mode.
        if ((Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.P)) && !gamePaused && playerControllerScript.gameActive)
        {
            if (arcadeMode)
            {
                // Activates the arcade pause screen.
                ArcadePauseScreen(true);
                gamePaused = true;
            }
            else
            {
                // Activates the pause screen and sets the pause bool to true.
                PauseScreen(true);
                gamePaused = true;
            }
        }

        // If there is new points gained, call the calclation of score.
        if (newPoints > 0)
        {
            CalculateScore();
        }

        // As long as the player has fuel left (and time is not paused) the time is calculated.
        if (fuelLeft > 0 && playerControllerScript.gameActive)
        {
            CalculateMinutesAndSeconds();
        }
        DisplayTime();

        // Activate the out in space message if the player has exited the screen.
        if (playerControllerScript.hasDrifteOutInSpace)
        {
            DriftedOutInSpaceScreen(true);
            playerControllerScript.hasDrifteOutInSpace = false;
        }

        // Display the text for altitude and speed.
        altitudeText.SetText(Mathf.Max(Mathf.RoundToInt(playerControllerScript.hit.distance - 8), 0).ToString());
        horizontalSpeedText.SetText(playerControllerScript.horizontalSpeed.ToString());
        verticalSpeedText.SetText(playerControllerScript.verticalSpeed.ToString());

        // Activate the screen for a successful landing when there is new points gained.
        if (playerControllerScript.basePoints > 0)
        {
            SuccessfulLandingScreen(true);
            playerControllerScript.basePoints = 0;
        }


        // When the player is out of fuel, start the end.
        if (fuelLeft < 1)
        {
            // TODO add highscore
            lowFuelScreen.SetActive(false);

            // Make the lander fall and display the out of fuel message.
            if (!hasCrashed)
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
        } // When fuel is getting low a warning with text and sound is displayed.
        else if (fuelLeft < 200 && fuelLeft > 0 && playerControllerScript.gameActive && !gamePaused)
        {
            // Makes the text blink and sound activate every time the text becomes visible.
            if (lowFuelScreenTimer <= 0)
            {
                // Swithes between visible and not visible text.
                lowFuelScreen.SetActive(showLowFuelScreen);

                // If text bacomes visible sound playes.
                if (showLowFuelScreen)
                {
                    GameObject.Find("Canvas").GetComponent<AudioSource>().Play();
                }

                // Swithes between visible and not visible text.
                showLowFuelScreen = !showLowFuelScreen;
                // Resets the timer.
                lowFuelScreenTimer = 1;
            }

            // Counts down.
            lowFuelScreenTimer = Mathf.MoveTowards(lowFuelScreenTimer, 0, Time.deltaTime);
        }
        else if (!lowFuelScreen || lowFuelScreenTimer > 0 || !playerControllerScript.gameActive || gamePaused)
        {
            // Resets the low fuel screen.
            lowFuelScreen.SetActive(false);
            lowFuelScreenTimer = 0;
            showLowFuelScreen = true;
        }

        // If the mouse is clicked outside of the menu, the last selected button is reselected.
        if (!playerControllerScript.gameActive || gamePaused)
        {
            // Save the currently selected button.
            if (eventSystem.currentSelectedGameObject != null)
            {
                currentSelectedButton = eventSystem.currentSelectedGameObject;
            }

            // If no button is selected, make the last selected on selected.
            if (eventSystem.currentSelectedGameObject == null && currentSelectedButton != null)
            {
                CatchMouseClicks(currentSelectedButton);
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
            fuelCalculation += fuelInPowerUp;
            // Resets the bool.
            playerControllerScript.addFuel = false;
        }

        // Takes away fuel if space is pressed while game is active.
        if (playerControllerScript.usingFuel && fuelLeft > 0 && !gamePaused && !playerControllerScript.hasDrifteOutInSpace)
        {
            // Takes away fuel every fixed frame.
            fuelCalculation -= 0.5f;
        }

        // Round the fuel to whole numbers.
        fuelLeft = Mathf.RoundToInt(fuelCalculation);

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
            successfulLandings++;
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            // Start time and deactivate button.
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
                // Time.timeScale = 0;
                crashes++;
                StartCoroutine(WaitWithActivatingButton(buttons));
            }
            else
            {
                // Time.timeScale = 1;
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
            // Time.timeScale = 0;
            playerControllerScript.usingFuel = false;
            crashes++;
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            // Time.timeScale = 1;
            DeactivateButton(buttons);
        }
    }

    // Change text on screen.
    private void EndScreen(bool active)
    {
        if (arcadeMode)
        {
            ArcadeEndScreen(active);
        }
        else
        {
            // Set text.
            gameOverText.SetText("Game Over!\nYou had " + successfulLandings + " successful landings and " + crashes + " failed attempts.\nYou scored " + score + " points.\nYour mission lasted for " + minutes + " minutes and " + seconds + " seconds.");

            // Remove out of fuel warning.
            GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(false);

            // Display the text and button.
            GameObject endScreen = GameObject.Find("Canvas").transform.Find("GameOver").gameObject;
            endScreen.SetActive(active);

            GameObject[] buttons = new GameObject[] { endScreen.transform.Find("Continue Button").gameObject };

            // Activate button after waiting.
            if (active)
            {
                StartCoroutine(WaitWithActivatingButton(buttons));
            }
            else
            {
                DeactivateButton(buttons);
            }
        }
    }

    // Change text on screen.
    private void ArcadeEndScreen(bool active)
    {
        // Set text.
        arcadeGameOverText.SetText("Game Over!\nYou had " + successfulLandings + " successful landings and " + crashes + " failed attempts.\nYou scored " + score + " points.\nYour mission lasted for " + minutes + " minutes and " + seconds + " seconds.");

        // Remove out of fuel warning.
        GameObject.Find("Canvas").transform.Find("OutOfFuel").gameObject.SetActive(false);

        // Display the text and button.
        GameObject ArcadeEndScreen = GameObject.Find("Canvas").transform.Find("ArcadeGameOver").gameObject;
        ArcadeEndScreen.SetActive(active);

        GameObject[] buttons = new GameObject[] { ArcadeEndScreen.transform.Find("Continue Button").gameObject };

        // Activate button after waiting.
        if (active)
        {
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            DeactivateButton(buttons);
        }
    }
   
    // Start menu for arcade mode.
    private void ArcadeStartScreen(bool active)
    {
        if (arcadeMode)
        {
            gamePaused = true;

            // Display the text and button.
            GameObject startScreen = GameObject.Find("Canvas").transform.Find("ArcadeStartScreen").gameObject;
            startScreen.SetActive(active);

            GameObject[] buttons = new GameObject[] { startScreen.transform.Find("Start Button").gameObject, startScreen.transform.Find("Highscore Button").gameObject };

            // Activate button after waiting.
            if (active)
            {
                Time.timeScale = 0;
                CalculateFuel();
                StartCoroutine(WaitWithActivatingButton(buttons));
            }
            else
            {
                DeactivateButton(buttons);

                gamePaused = false;
            }
        }
    } // TODO add control info menu and fnish highscore menu

    // Move to and use the enter highscore screen. TODO save to file
    public void SaveHighscoreScreen(bool active)
    {
        ArcadeEndScreen(false);

        // Display the text and button.
        GameObject saveHighscoreScreen = GameObject.Find("Canvas").transform.Find("SaveHighscore").gameObject;
        saveHighscoreScreen.SetActive(active);

        // TODO add restart button? if player doesnt want to save.
        GameObject[] buttons = new GameObject[] { saveHighscoreScreen.transform.Find("Background").Find("Save Button").gameObject };

        // Activate button after waiting.
        if (active)
        {
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            DeactivateButton(buttons);
        }
    }

    // Save the score to a file.
    public void Save()
    {
        string destination = @"D:\save.txt";

        string name = "";
        foreach (char letter in EnterName.threeLetters)
        {
            name += letter;
        }

        // Read from file.
        if (File.Exists(destination))
        {
            highscores.AddRange(File.ReadAllLines(destination));
        }

        // Save to file.
        string newScore = MakeFourCharString(score.ToString(), '0') + " " + name;

        highscores.Add(newScore);
        highscores.Sort();
        highscores.Reverse();

        File.WriteAllLines(destination, highscores);

        // Switch screen (change to show highscore screen instead of menu) TODO
        SaveHighscoreScreen(false); 
        ArcadeStartScreen(true);
    }

    // Highscore menu screen
    public void ShowHighscores(bool active)
    {
        // Parse and display data
        highscores.Clear();
        highscores.AddRange(File.ReadAllLines(@"D:\save.txt"));

        List<string> score210 = new List<string>();
        List<string> score1119 = new List<string>();
        List<string> score2028 = new List<string>();

        for (int score = 1; score <= highscores.Count; score++)
        {
            if (score == 1)
            {
                // 1st
                highscore1.text = "1. " + highscores[0];
            }
            else if (score >= 2 && score <= 10)
            {
                // 2-10
                score210.Add(score + ". " + highscores[score - 1]);
            }
            else if (score >= 11 && score <= 19)
            {
                // 11-19
                score1119.Add(score + ". " + highscores[score - 1]);
            }
            else if (score >= 20 && score <= 28)
            {
                // 20-28
                score2028.Add(score + ". " + highscores[score - 1]);
            }
            else
            {
                score = highscores.Count;
            }
        }

        if (score210.Count > 0)
        {
            highscore210.text = string.Join("\n", score210);
        }

        if (score1119.Count > 0)
        {
            highscore1119.text = string.Join("\n", score1119);
        }

        if (score2028.Count > 0)
        {
            highscore2028.text = string.Join("\n", score2028);
        }

        // Hide arcade menu
        ArcadeStartScreen(false);

        // Display button.
        GameObject highscoreScreen = GameObject.Find("Canvas").transform.Find("Highscore").gameObject;
        highscoreScreen.SetActive(active);

        GameObject[] buttons = new GameObject[] { highscoreScreen.transform.Find("Background").Find("Menu Button").gameObject};

        // Activate button after waiting.
        if (active)
        {
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            DeactivateButton(buttons);
        }
    }

    // Control menu screen

    // Back to arcade menu button
    public void BackToArcadeMenu()
    {
        // Hide current screen and show arcade menu
        ShowHighscores(false);
        ArcadeStartScreen(true);
    }

    // Change text on screen.
    public void PauseScreen(bool active)
    {
        // Display the text and buttons.
        GameObject pauseScreen = GameObject.Find("Canvas").transform.Find("PauseScreen").gameObject;
        pauseScreen.SetActive(active);

        GameObject[] buttons = new GameObject[] { pauseScreen.transform.Find("Continue Button").gameObject, pauseScreen.transform.Find("Restart Button").gameObject };

        // Activate button after waiting and pause game.
        if (active)
        {
            player.GetComponent<AudioSource>().Pause();
            Time.timeScale = 0;
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            player.GetComponent<AudioSource>().UnPause();
            Time.timeScale = 1;
            DeactivateButton(buttons);

            gamePaused = false;
        }
    }

    // Change text on screen.
    public void ArcadePauseScreen(bool active)
    {
        // Display the text and buttons.
        GameObject pauseScreen = GameObject.Find("Canvas").transform.Find("ArcadePauseScreen").gameObject;
        pauseScreen.SetActive(active);

        GameObject[] buttons = new GameObject[] { pauseScreen.transform.Find("Continue Button").gameObject, pauseScreen.transform.Find("Restart Button").gameObject };

        // Activate button after waiting and pause game.
        if (active)
        {
            player.GetComponent<AudioSource>().Pause();
            Time.timeScale = 0;
            StartCoroutine(WaitWithActivatingButton(buttons));
        }
        else
        {
            player.GetComponent<AudioSource>().UnPause();
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
        ArcadeStartScreen(false);

        // Destroy the lander if it wasn't destroyed for some reason ( successful landing is a reason).
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            Destroy(playerControllerScript.gameObject);
        }

        // Spawn a new lander.
        ResetPlayer();
    }

    // Triggers the out of fuel function from player controller script.
    private void MakePlayerFall()
    {
        playerControllerScript.OutOfFuel();
    }

    // Loads the menu.
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Relode the whole game (used in arcade mode).
    public void ReloadGame()
    {
        SceneManager.LoadScene("Reload");
    }

    // Goes throgh and activates all the buttons on a screen.
    IEnumerator WaitWithActivatingButton(GameObject[] buttons)
    {
        yield return new WaitForSecondsRealtime(1);

        // Make all buttons interactable.
        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = true;
        }

        // Sets the first button as selected.
        eventSystem.SetSelectedGameObject(buttons[0]);
    }

    // Deactivates all buttons when a screen is not shown.
    private void DeactivateButton(GameObject[] buttons)
    {
        // Make all buttons not interactable.
        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    // Set the selected game object.
    private void CatchMouseClicks(GameObject setSelection)
    {
        EventSystem.current.SetSelectedGameObject(setSelection);
    }
}
