using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnterName : MonoBehaviour
{
    // Fields for the current letter the player is changing.
    [SerializeField] private GameObject[] letters = new GameObject[3];

    private int thisLetter = 0;
    private char shownLetter = 'A';
    private TextMeshProUGUI currentLetterTextField;
    private SpriteRenderer upArrow;
    private SpriteRenderer downArrow;

    [SerializeField] private Color baseColor = new Color();
    [SerializeField] private Color clickedColor = new Color();
    [SerializeField] private TextMeshProUGUI scoreText;

    // Coldown.
    private bool canClick = false;
    [SerializeField] private float coolDownTime;
    private float coolDownCounter;

    // The three letters.
    public static char[] threeLetters = new char[] { 'A', 'A', 'A' };

    // Start is called before the first frame update
    void Start()
    {
        ChangeLetter(thisLetter, letters);
        coolDownCounter = coolDownTime;

        scoreText.text = MakeFourCharString(FindObjectOfType<GameManager>().score.ToString(), '0');
    }

    // Update is called once per frame
    void Update()
    {
        // TODO DONT FORGET ABOUT COLDOWN
        if (canClick) //coldown done
        {
            // Change letter.
            if (Input.GetAxis("Vertical") != 0)
            {

                float input = Input.GetAxis("Vertical");

                if (input > 0)
                {
                    // Change color of arrow to indicate click.
                    upArrow.color = clickedColor;

                    // Next letter.
                    if (shownLetter == 'Z')
                    {
                        shownLetter = 'A';
                    }
                    else
                    {
                        shownLetter++;
                    }
                }
                else if (input < 0)
                {
                    // Change color of arrow to indicate click.
                    downArrow.color = clickedColor;

                    // Previous letter.
                    if (shownLetter == 'A')
                    {
                        shownLetter = 'Z';
                    }
                    else
                    {
                        shownLetter--;
                    }
                }

                currentLetterTextField.text = shownLetter.ToString();
                threeLetters[thisLetter] = shownLetter;
                canClick = false;
            }
            // Move between letters.
            else if (Input.GetAxis("Horizontal") != 0)
            {
                float input = Input.GetAxis("Horizontal");
                threeLetters[thisLetter] = shownLetter;

                if (input > 0)
                {
                    // Next letter entry.
                    if (thisLetter == letters.Length - 1)
                    {
                        thisLetter = 0;
                    }
                    else
                    {
                        thisLetter++;
                    }
                }
                else
                {
                    // Previous letter entry.
                    if (thisLetter == 0)
                    {
                        thisLetter = letters.Length - 1;
                    }
                    else
                    {
                        thisLetter--;
                    }
                }

                // Apply change.
                currentLetterTextField.color = Color.white;
                ChangeLetter(thisLetter, letters);
                canClick = false;
            }
        }
        else
        {
            // Reduce cool down.
            coolDownCounter -= Time.deltaTime;

            // Reset click.
            if (coolDownCounter < 0)
            {
                canClick = true;
                coolDownCounter = coolDownTime;
                upArrow.color = baseColor;
                downArrow.color = baseColor;
            }
        }
    }

    private void ChangeLetter(int letter, GameObject[] letters)
    {
        // Text.
        currentLetterTextField = letters[letter].transform.Find("Letter").gameObject.GetComponent<TextMeshProUGUI>();
        currentLetterTextField.color = baseColor;
        shownLetter = currentLetterTextField.text[0];
        threeLetters[letter] = shownLetter;

        // Arrows.
        upArrow = letters[letter].transform.Find("Up").gameObject.GetComponent<SpriteRenderer>();
        downArrow = letters[letter].transform.Find("Down").gameObject.GetComponent<SpriteRenderer>();
    }

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
}
