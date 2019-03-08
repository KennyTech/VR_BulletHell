using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public Text nameText;
    public Text dialogueText;

    public Animator animator;

    private Queue<string> sentences;

    public GameObject dialogueBox;
    public GameObject characterImage;
    public GameObject hologramRay;
    public AudioSource soundStart;
    public AudioSource soundEnd;
    public GameObject soundText;
    public float readTime = 1.5f; // time player can read message after text scrolling is done
    public EnemySpawner enemySpawner;

    public char largerFont = '*';   // put this char in the dialogue text box to make font size bigger by 10 (does not reset, use smallerFont after to reset) 
    public char smallerFont = '&';  // put this char in the dialogue text box to make font size smaller by 10
    public int fontSize = 0;
    private int absFontSize = 0;

    private int flickerCount;

    void Start()
    {
        sentences = new Queue<string>();
    }
    public void ActivateTextAudio()
    {
        soundText.gameObject.SetActive(true);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();


    }

    public void DisplayNextSentence()
    {
         
        // Change font size accordingly
        if (fontSize != 0)
        {
            for (int i = 0; i < Mathf.Abs(fontSize); i++)
            {
                if (fontSize > 0)
                {
                    dialogueText.fontSize += 20;
                }
                else if (fontSize < 0)
                {
                    dialogueText.fontSize -= 20;
                }
            }
            fontSize = 0;
        }

        // Play text sound
        soundText.gameObject.SetActive(true);
        if (sentences.Count == 0)
        {
            flicker();
            return;
        }

        string sentence = sentences.Dequeue();
        print(sentence);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        //char firstChar = ' ';
        int charCounter = 0;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            charCounter += 1;
            if (letter == largerFont)
            {
                if (charCounter == 1) // if its the first char in the stream, change size right away (if it's not, ie. mid or last char, we'll change the font size with a delay to affect the next dialogue box))
                    dialogueText.fontSize += 20;
                else
                    fontSize += 1;
            }

            if (letter == smallerFont)
            {
                if (charCounter == 1) // if its the first char in the stream, change size right away (if it's not, ie. mid or last char, we'll change the font size with a delay to affect the next dialogue box))
                    dialogueText.fontSize -= 20;
                else
                    fontSize -= 1;
            }
            if (letter != largerFont && letter != smallerFont)
                dialogueText.text += letter;    // print all chars in the dialogue if they're not the largerFont or smallerFont char (* and & by default)
            yield return null;
        }

        // De-Activate Text Sound
        soundText.gameObject.SetActive(false);

        // Display the next set of text after characters are done printing, after a delay
        if (enemySpawner.gamePhase >= 7) // lower readTime (faster text scroll) on the last set of dialogues for game feel
        {
            readTime = 0.75f;
            if (dialogueText.fontSize < 100)
                dialogueText.fontSize += 10;
        }

        Invoke("DisplayNextSentence", readTime);

    }

    public void biggerFontSize(bool inc)
    {
        
    }

    public void flickerLoop()
    {
        // Quick bug patch band-aid (it's turning on for some reason)
        soundText.gameObject.SetActive(false);

        flickerCount += 1;

        if (flickerCount % 2 == 0) // even
        {
            // Activate Dialogue Character Image
            characterImage.gameObject.SetActive(true);
            // Activate Hologram Ray
            hologramRay.gameObject.SetActive(true);
        }
        else // odd
        {
            // Activate Dialogue Character Image
            characterImage.gameObject.SetActive(false);
            // Activate Hologram Ray
            hologramRay.gameObject.SetActive(false);
        }

    }

    void flicker()
    {
        // Flicker Hologram after open animation
        InvokeRepeating("flickerLoop", 0.05f, 0.05f);

        // Start End Dialogue after delay
        Invoke("EndDialogue", 0.5f);
    }

    void EndDialogue()
    {
        // De-Activate Dialogue Character Image
        characterImage.gameObject.SetActive(false);

        // De-Activate Hologram Ray
        hologramRay.gameObject.SetActive(false);

        // Cancel repeating invoke
        CancelInvoke("flickerLoop");

        // Play close animation
        animator.SetBool("IsOpen", false);

        // Activate the end sound
        soundEnd.Play();
        //soundEnd.gameObject.SetActive(true);

        // Deactivate dialogue UI after close animation (0.5 seconds)
        Invoke("DeactivateDialogue", 0.5f);
    }

    void DeactivateDialogue()
    {
        // De-Activate Dialogue Box UI
        dialogueBox.gameObject.SetActive(false);

        // De-Activate Dialogue Character Image
        characterImage.gameObject.SetActive(false);

        // De-Activate Hologram Ray Image
        hologramRay.gameObject.SetActive(false);

        // De-Activate Text Sound
        soundText.gameObject.SetActive(false);

        // Stop the repeating invoke
        CancelInvoke("DisplayNextSentence");
    }

}