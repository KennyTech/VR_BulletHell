using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class PlayerShip : MonoBehaviour
{
    [SerializeField]
    SteamVR_Behaviour_Pose skeletonL;
    [SerializeField]
    SteamVR_Behaviour_Pose skeletonR;

    [SerializeField]
    SteamVR_Action_Vibration haptics;

    SteamVR_Input_Sources inputSource;

    [SerializeField]
    SmartBullet[] bullets;

    [SerializeField]
    SteamVR_Action_Boolean shootAction;

    [SerializeField]
    int health, maxHealth;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    Transform gunA;
    [SerializeField]
    Transform gunB;
    [SerializeField]
    Transform gunC;
    [SerializeField]
    Transform gunD;

    [SerializeField]
    GameObject deathParticles;

    bool altFire;

    Animator anim;

    Vector3 offsetPos;
    Quaternion offSetRot;

    bool firing;

    AudioManager am;

    // Start is called before the first frame update
    void Start()
    {
        GameObject g = new GameObject("PlayerBulletPool");
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i] = Instantiate(bulletPrefab, g.transform).GetComponent<SmartBullet>();
        }

        anim = GetComponent<Animator>();

        am = AudioManager.GetInstance();

        offsetPos = transform.localPosition;
        offSetRot = transform.localRotation;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        skeletonL.GetComponent<GrabbyHands>().enabled = false;
        skeletonR.GetComponent<GrabbyHands>().enabled = false;
        shootAction.AddOnChangeListener(OnShootActionChange, SteamVR_Input_Sources.RightHand);
        shootAction.AddOnChangeListener(OnShootActionChange, SteamVR_Input_Sources.LeftHand);
        InvokeRepeating("ShootBullet", 8, 0.1f);
        Invoke("StartSound", 8);
    }

    private void OnDisable()
    {
        skeletonL.GetComponent<GrabbyHands>().enabled = true;
        skeletonR.GetComponent<GrabbyHands>().enabled = true;
        am.StopSoundLoop(AudioManager.Sound.PlayerLaser);
        CancelInvoke("ShootBullet");
        CancelInvoke("StartSound");
    }

    void StartSound()
    {
        am.PlaySoundLoop(AudioManager.Sound.PlayerLaser, AudioManager.Priority.Spam, transform);
    }

    public void SetHand(SteamVR_Input_Sources source)
    {
        inputSource = source;
        switch (inputSource)
        {
            case SteamVR_Input_Sources.LeftHand:
                transform.parent = skeletonL.transform;
                break;
            case SteamVR_Input_Sources.RightHand:
                transform.parent = skeletonR.transform;
                break;
        }
        transform.localRotation = offSetRot;
        transform.localPosition = offsetPos;
        gameObject.SetActive(true);
    }

    void OnShootActionChange(SteamVR_Action_In actionIn)
    {
        if (inputSource == SteamVR_Input_Sources.RightHand)
        {
            if (shootAction.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                altFire = true;
            }
            else if (shootAction.GetStateUp(SteamVR_Input_Sources.RightHand))
            {
                altFire = false;
            }
        }
        else if (inputSource == SteamVR_Input_Sources.LeftHand)
        {
            if (shootAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                altFire = true;
            }
            else if (shootAction.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                altFire = false;
            }
        }
    }

    void ShootBullet()
    {
        //Shoot two bullets
        if (!altFire)
        {
            SmartBullet b = GetBullet();
            b.transform.rotation = transform.rotation;
            b.transform.position = gunA.position;
            b.gameObject.SetActive(true);

            b = GetBullet();
            b.transform.rotation = transform.rotation;
            b.transform.position = gunB.position;
            b.gameObject.SetActive(true);
        }
        else{
            SmartBullet b = GetBullet();
            b.transform.rotation = transform.rotation;
            b.transform.position = gunC.position;
            b.gameObject.SetActive(true);

            b = GetBullet();
            b.transform.rotation = transform.rotation;
            b.transform.position = gunD.position;
            b.gameObject.SetActive(true);
        }

    }

    SmartBullet GetBullet()
    {
        foreach (SmartBullet b in bullets)
        {
            if (!b.gameObject.activeSelf)
            {
                return b;
            }
        }
        Debug.LogError("Player Ship Error: Ran out of bullets!");
        return null;
    }

    public void DealDamage()
    {
        am.PlaySoundOnce(AudioManager.Sound.PlayerHit, AudioManager.Priority.High);
        anim.SetTrigger("ShipHit");
        health--;
        haptics.Execute(0, 0.1f, 100.0f, 0.75f, inputSource);
        if (health < 0)
        {
            if (deathParticles != null)
            {
                GameObject particleInst = Instantiate(deathParticles);
                particleInst.transform.position = transform.position;
                Destroy(particleInst, 5);
            }
            Invoke("RestartGame", 1f);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("projectile")) {
            other.transform.parent.parent.gameObject.SetActive(false);
            DealDamage();
        }
    }
}