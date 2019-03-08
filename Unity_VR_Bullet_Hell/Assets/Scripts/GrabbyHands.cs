using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GrabbyHands : MonoBehaviour
{
    SteamVR_Behaviour_Pose skeleton;

    [SerializeField]
    SteamVR_Action_Boolean grabAction;

    [SerializeField]
    GameObject touchingShip;

    [SerializeField]
    PlayerShip ship;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        skeleton = GetComponent<SteamVR_Behaviour_Pose>();
        grabAction.AddOnChangeListener(OnGrabActionChange, skeleton.inputSource);
    }

    void OnGrabActionChange(SteamVR_Action_In actionIn)
    {
        if (grabAction.GetState(skeleton.inputSource))
        {
            if (touchingShip != null)
            {
                ship.SetHand(skeleton.inputSource);
                ship.gameObject.SetActive(true);
                touchingShip.SetActive(false);
                touchingShip = null;
                enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "GrabMe")
        {
            touchingShip = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "GrabMe")
        {
            touchingShip = null;
        }
    }
}