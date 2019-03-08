using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VNScroll : MonoBehaviour
{
    SteamVR_Behaviour_Pose skeleton;

    [SerializeField]
    SteamVR_Action_Boolean forwardAction;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        skeleton = GetComponent<SteamVR_Behaviour_Pose>();
        forwardAction.AddOnChangeListener(OnGrabActionChange, skeleton.inputSource);
    }

    void OnGrabActionChange(SteamVR_Action_In actionIn)
    {
        if (forwardAction.GetState(skeleton.inputSource))
        {
            FindObjectOfType<DialogueTrigger>().TriggerDialogue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
