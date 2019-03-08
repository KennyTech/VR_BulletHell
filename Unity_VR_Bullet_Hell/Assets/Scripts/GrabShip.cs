using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabShip : MonoBehaviour
{
    [SerializeField]
    Animator earth;
    [SerializeField]
    GameObject ship;

    [SerializeField]
    GameObject spawner;

    [SerializeField]
    AudioSource audio;

    [SerializeField]
    GameObject particles;

    // Start is called before the first frame update
    void Start()
    {
        //Invoke("OnDisable", 1.0f); //for testing (without VR)
    }

    public void StartTestButton()
    {
        //Invoke("OnDisable", 0.1f); //for testing (without VR)
        GameObject.Find("NO_VR_TEST_BUTTON").SetActive(false);
    }

    private void OnDisable()
    {
        earth.SetTrigger("FlyAway");
        spawner.SetActive(true);
        //Invoke("WarpDrive", 5.0f);

        AudioManager.GetInstance().PlayMusic(AudioManager.Music.MainTheme, true);

        // The first woosh sound
        if (audio != null)
        {
            audio.pitch = 0.3f;
            audio.volume = 0.6f;
            audio.Play();
        }
        Invoke("WooshAgain", 5.0f);
    }

    public void WarpDrive()
    {
        FindObjectOfType<DialogueTrigger>().WarpDrive();
    }

    private void WooshAgain()
    {
        // particles
        particles.gameObject.SetActive(true);
        // big woosh audio
        audio.pitch = 0.8f;
        audio.volume = 1.0f;
        audio.Play();
        Invoke("WooshEnd", 4.5f);
    }

    private void WooshEnd()
    {
        particles.gameObject.SetActive(false);

        /*
        // DIALOGUE: Call the next dialogue
        FindObjectOfType<DialogueTrigger>().TriggerDialogue();

        // TEST FUNCTION ONLY: Periodically call for rest of the dialogue and warps
        Invoke("RunDialogue", 10.0f);
        Invoke("RunDialogue", 17.0f);
        Invoke("RunWarp", 21.0f);

        Invoke("RunDialogue", 30.0f);
        Invoke("RunDialogue", 37.0f);
        Invoke("RunWarp", 41.0f);

        Invoke("RunDialogue", 50.0f);
        Invoke("RunDialogue", 57.0f);
        Invoke("RunWarp", 61.0f);

        Invoke("RunDialogue", 70.0f);
        Invoke("RunDialogue", 77.0f);
        */

    }

    private void RunDialogue()
    {
        FindObjectOfType<DialogueTrigger>().TriggerDialogue();
    }

    private void RunWarp()
    {
        FindObjectOfType<DialogueTrigger>().WarpDrive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
