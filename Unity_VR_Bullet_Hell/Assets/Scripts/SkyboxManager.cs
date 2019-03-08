using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    [SerializeField]
    public Material[] skyboxMat;

    [SerializeField]
    public int num = 0;

    [SerializeField]
    public AudioSource warpDriveSound;

    // Cycle the next skybox
    public void NextSkybox()
    {
        RenderSettings.skybox = skyboxMat[num];
        DynamicGI.UpdateEnvironment();
        num += 1;
    }

    // Go to specific skybox
    public void SetSkybox(int val)
    {
        RenderSettings.skybox = skyboxMat[val];
        DynamicGI.UpdateEnvironment();
        num = val;
    }

    void Start()
    {
        //Invoke("Transition", 2.0f);
        //Invoke("Transition", 8.0f);
        //Invoke("Transition", 14.0f);
    }

    private void Transition()
    {
        Invoke("NextSkybox", 1.50f);
        warpDriveSound.pitch = 0.8f;
        warpDriveSound.volume = 1.0f;
        warpDriveSound.Play();
        FindObjectOfType<DialogueTrigger>().WarpDrive();
    }


  
}
