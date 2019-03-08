// Place this script in an empty object, create all the dialogue strings there
// Use FindObjectOfType<DialogueTrigger>().TriggerDialogue(); to call the next dialogue sequence (call 10 times to finish game)
// Use FindObjectOfType<DialogueTrigger>().WarpDrive(); to call forwards warp drive (also changes skybox) (call max 3 times b/c we have 3 skyboxes)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public static int dialogueCounter = 0;

    public Dialogue[] dialogue;

    public GameObject dialogueBox;
    public GameObject characterImage;
    public GameObject hologramRay;
    public AudioSource soundStart;
    public AudioSource soundEnd;
    public GameObject soundText;
    public Animator animator;

    public GameObject dialogueText;
    public GameObject warpDrive;

    private int flickerCount = 0;

    public Image myImageComponent;
    public Sprite myFirstImage;
    public Sprite mySecondImage;
    public Sprite myThirdImage;

    public int rando = 0;

    void Start()
    {
        myImageComponent = characterImage.GetComponent<Image>();
    }

    public void SetImage1()
    {
        myImageComponent.sprite = myFirstImage;
    }
    public void SetImage2()
    {
        myImageComponent.sprite = mySecondImage;
    }
    public void SetImage3()
    {
        myImageComponent.sprite = myThirdImage;
    }

    

    public void WarpDrive()
    {
        warpDrive.gameObject.SetActive(true);
        Invoke("ChangeSkybox", 1.5f);
        Invoke("StopWarpDrive", 4.25f);
    }

    private void ChangeSkybox()
    {
        FindObjectOfType<SkyboxManager>().NextSkybox();
    }

    private void StopWarpDrive()
    {
        warpDrive.gameObject.SetActive(false);
    }

    public void TriggerDialogue()
    {

        rando = Random.Range(1, 4);
        if (rando == 1)
            SetImage1();
        else if (rando == 2)
            SetImage2();
        else
            SetImage3();



        // Activate Dialogue Box UI
        dialogueBox.gameObject.SetActive(true);

        // Activate Hologram Start Sound
        soundStart.Play();

        // Animate Dialogue Box Open
        animator.SetBool("IsOpen", true);

        // Temporarily disable dialogue text until animation complete
        dialogueText.gameObject.SetActive(false);

        // Flicker Hologram after open animation
        InvokeRepeating("flickerHologram", 0.7f, 0.05f);

        // Stop flicker after delay
        Invoke("flickerStop", 1.1f);

        // Start Dialogue after delay
        Invoke("StartDialogue", 1.4f);

    }

    public void StartDialogue()
    {
        // Start dialogue character by character
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue[dialogueCounter]);
        // Activate dialogue text and text sound
        dialogueText.gameObject.SetActive(true);
        soundText.gameObject.SetActive(true);
        dialogueCounter += 1;
    }

    public void flickerHologram()
    {
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
            // De-Activate Dialogue Character Image
            characterImage.gameObject.SetActive(false);
            // De-Activate Hologram Ray
            hologramRay.gameObject.SetActive(false);
        }

    }

    public void flickerStop()
    {
        // De-Activate Dialogue Character Image
        characterImage.gameObject.SetActive(false);
        // De-Activate Hologram Ray
        hologramRay.gameObject.SetActive(false);
        // Cancel the repeating invoke
        CancelInvoke("flickerHologram");
        // Activate Dialogue Character Image
        characterImage.gameObject.SetActive(true);
        // Activate Hologram Ray
        hologramRay.gameObject.SetActive(true);

    }

}