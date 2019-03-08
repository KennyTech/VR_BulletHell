using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;
using EnemySpace;
public class Enemy_Boss : Enemy
{
    public enum AttackState {
        Entrance,
        Laser,
        RingShot,
        ScatterShot,
        HelixShot,
        SquareShot,
        Count
    }

    [SerializeField]
    AttackState currentState;

    [SerializeField]
    private Animator bossEntrance;
    [SerializeField]
    private Transform player;
    [Header("Laser")]
    [SerializeField]
    private VolumetricLineBehavior bossLaser; // set position of laser
    [SerializeField]
    private Transform laserStartPos;
    private float t_laserTimer = 0;
    [SerializeField]
    private Transform lookAtTranformTest;       // Use this object to get the boss to look at the player, fixes multiple issues with rotating the boss
    [SerializeField]
    private List<Transform> emitters;           // Ships gun emitter
    [SerializeField]
    private float fireRate = 0.25f;             // Fire rate of all gun patterns on the ship
    [SerializeField]
    private BulletPattern patternRing;          // use this for the ring pattern generation

    [SerializeField]
    BulletPatternSquare square;

    [SerializeField]
    BulletPatternRing helixHelper;

    [SerializeField]
    BulletPatternRing scatterHelperOne, scatterHelperTwo;

    float maxHealth;

    [SerializeField]
    float attackDuration = 8f;
    private float t_fireRate = 0;
    private float t_newLocation = 0;
    private BulletPool bulletPool;
    private float t_abilityTimer = 0;
    private float t_rotateToPosition = 0;
    private bool hasNewLocation = false;
    private float PosX = 0;
    private float PosY = 2f;

    [SerializeField]
    Material healthMat;

    void OnEnable()
    {
        bulletPool = BulletPool.GetInstance();
        BossEntrance();

        bossLaser = GameObject.Find("BossLaser").GetComponent<VolumetricLineBehavior>();

        if (GameObject.Find("[CameraRig]").transform.Find("Controller (right)").childCount > 1)
            player = GameObject.Find("[CameraRig]").transform.Find("Controller (right)").transform.Find("Spaceship").transform;
        else
            player = GameObject.Find("[CameraRig]").transform.Find("Controller (left)").transform.Find("Spaceship").transform;


        InvokeRepeating("Attack", 0, attackDuration);

        maxHealth = health;
    }

    void FixedUpdate()
    {
        // Insurance check beacuse shit going south
        //if(bossLaser == null)
        //    bossLaser = GameObject.Find("BossLaser").GetComponent<VolumetricLineBehavior>();
        //if(player == null)
        //    player = GameObject.Find("[CameraRig]").transform;


        t_fireRate += Time.deltaTime;

        // If our entrance animation is finished
        if (bossEntrance.enabled == false)
        {
            t_abilityTimer += Time.deltaTime;

            // Produces a number from -1 to 1 starting at -1
            //Attack();
        }

        switch (currentState)
        {
            case AttackState.Laser:
                ShootingLaser();
                break;
            case AttackState.RingShot:
                //ShootRing();
                break;
            case AttackState.ScatterShot:
                ShootNormal();
                break;
            case AttackState.HelixShot:
                ShootHelix();
                break;
        }

        NewLocation();
    }

    public void BossEntrance()
    {
        currentState = AttackState.Entrance;
        transform.position = new Vector3(0, 15.04f, 4.221f);
        gameObject.SetActive(true);
        bossEntrance.SetTrigger("entrance");
    }
    public void EntranceFinish()
    {
        bossEntrance.enabled = false;
        currentState = AttackState.Laser;
    }

    // New location stuff
    public void NewLocation()
    {
        // Increment time
        t_newLocation += Time.deltaTime;

        // if we want to find a new location
        if (t_newLocation <= 1)
        {
            if (!hasNewLocation)
            {
                PosX = Random.Range(-1f, 1);
                PosY = Random.Range(0, 2f);
                hasNewLocation = true;
            }

            Vector3 nextPosition = new Vector3(PosX, PosY, 3f);

            transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * 2);
        }
        if (t_newLocation > attackDuration)
        {
            hasNewLocation = false;
            t_newLocation = 0;
        }
    }

    public void ShootHelix()
    {
        // Increment timer
        t_rotateToPosition += Time.deltaTime;

        // rotate to position
        if (t_rotateToPosition <= 2)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime);
        }
        else // Fire helix shot
        {
            // hot dog roll the ship
            transform.Rotate(Time.deltaTime * 300, 0, 0);
            if (t_fireRate > fireRate)
                foreach (var e in emitters)
                {
                    GameObject bulletInst = BulletPool.GetInstance().GetBullet_Boss();

                    AudioManager.GetInstance().PlaySoundOnce(AudioManager.Sound.EnemyFire2, AudioManager.Priority.Low, transform);

                    bulletInst.SetActive(true);
                    bulletInst.transform.position = e.transform.position;
                    bulletInst.transform.LookAt(player.transform.position);
                    t_fireRate = 0;
                }
        }
    }

    public void ShootNormal()
    {
        lookAtTranformTest.LookAt(player.transform.position);
        lookAtTranformTest.Rotate(new Vector3(0, -90, -110));

        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtTranformTest.rotation, Time.deltaTime * 2f);
        if (t_fireRate > fireRate)
            foreach (var e in emitters)
            {
                //GameObject bulletInst = bulletPool.GetBullet_Boss();
                GameObject bulletInst = BulletPool.GetInstance().GetBullet_Boss();

                AudioManager.GetInstance().PlaySoundOnce(AudioManager.Sound.EnemyFire3, AudioManager.Priority.Low, transform);

                bulletInst.SetActive(true);
                bulletInst.transform.position = e.transform.position;
                bulletInst.transform.LookAt(player.transform.position);
                t_fireRate = 0;
            }
    }

    [SerializeField]
    bool isShootingLaser = false;
    public void ShootingLaser()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime);

        Vector3 pos = Vector3.LerpUnclamped(bossLaser.EndPos, player.transform.position, Time.deltaTime * 2);

        bossLaser.StartPos = laserStartPos.position;    // Attach the emitter to the start pos
        bossLaser.EndPos = pos;

        // Damage Player
        if (Vector3.Distance(pos, player.transform.position) < 0.25f)
        {
            //player.GetComponent<PlayerShip>().DealDamage();
        }
    }

    public void SuperSizeLaser()
    {
        bossLaser.LineWidth = Mathf.Lerp(bossLaser.LineWidth, 1.5f, Time.deltaTime);
    }

    public void ShootRing()
    {
        patternRing.FireBullet();
        AudioManager.GetInstance().PlaySoundOnce(AudioManager.Sound.EnemyFire1, AudioManager.Priority.Low, transform);
    }

    public void ShootSquare()
    {
        square.FireBullet();
    }

    public void ShootHelixHelper()
    {
        helixHelper.FireBullet();
    }

    public void ShootScatterHelper()
    {
        scatterHelperOne.FireBullet();
        scatterHelperTwo.FireBullet();
    }

    public override void Attack()
    {
        //Reset some settings
        bossLaser.gameObject.SetActive(false);
        CancelInvoke("ShootRing");
        CancelInvoke("SuperSizeLaser");
        CancelInvoke("ShootSquare");
        CancelInvoke("ShootHelixHelper");
        CancelInvoke("ShootScatterHelper");
        bossLaser.LineWidth = 0.1f;
        bossLaser.StartPos = transform.position;
        bossLaser.EndPos = transform.position;

        if (bossEntrance.enabled == false)
        {
            switch(Random.Range((int)AttackState.Laser, (int)AttackState.Count) - 1){
                //switch (0) {
                case (int)AttackState.Laser:
                    currentState = AttackState.Laser;
                    bossLaser.gameObject.SetActive(true);
                    InvokeRepeating("SuperSizeLaser", 1.5f, 0.01f);
                    break;
                case (int)AttackState.RingShot:
                    // Ring Pattern shooting
                    currentState = AttackState.RingShot;
                    InvokeRepeating("ShootRing", 0, fireRate);
                    break;
                case (int)AttackState.ScatterShot:
                    currentState = AttackState.ScatterShot;
                    InvokeRepeating("ShootScatterHelper", 0, fireRate * 3);
                    break;
                case (int)AttackState.HelixShot:
                    currentState = AttackState.HelixShot;
                    InvokeRepeating("ShootHelixHelper", 0, fireRate * 2);
                    break;
                case (int)AttackState.SquareShot:
                    currentState = AttackState.SquareShot;
                    InvokeRepeating("ShootSquare", 0, fireRate);
                    square.transform.LookAt(player);
                    break;
            }
        }
    }

    new public void DamageEnemy(float amount)
    {
        health -= amount;

        Color a = healthMat.color;
        a.a = Mathf.Lerp(255, 0, health / maxHealth);
        healthMat.color = a;

        if (health <= 0)
        {
            InstantKillEnemy();
        }
    }
}
