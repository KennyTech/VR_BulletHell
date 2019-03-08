using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemySpace;

public class Enemy_Ship : Enemy
{
    public enum State
    {
        Tracking,
        Shooting,
        Done
    }

    [Header("Enemy Movement")]

    [SerializeField]
    State currentState;

    /// <summary>
    /// Amount of times enemy fires before giving up
    /// </summary>
    [Tooltip("Amount of times enemy fires before giving up")]
    [SerializeField]
    int fireAmount;

    [SerializeField]
    float fireRate;

    Vector3 targetPosition = Vector3.zero;

    EnemySpawner spawner;

    BulletPattern bp;

    [SerializeField]
    float rotationOffset = 20;

    [SerializeField]
    float minimumDist = 1;

    /// <summary>
    /// Disappears after flying in space for this duration
    /// </summary>
    [SerializeField]
    float lifeTime = 5;

    [SerializeField]
    bool trackPlayer;

    AudioManager am;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        bp = GetComponent<BulletPattern>();
        am = AudioManager.GetInstance();
    }

    private void OnEnable()
    {
        transform.Rotate(new Vector3(Random.Range(-rotationOffset, rotationOffset), Random.Range(-rotationOffset, rotationOffset), Random.Range(-rotationOffset, rotationOffset)));
        targetPosition = transform.position + transform.forward * 5;
        if (rb == null) rb = GetComponent<Rigidbody>();
        currentState = State.Tracking;
    }

    private void OnDisable()
    {
        CancelInvoke("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Tracking:
                transform.LookAt(spawner.GetPlayerReference().transform);
                Move(transform.forward);
                if (Vector3.Distance(transform.position, spawner.GetPlayerReference().transform.position) < minimumDist)
                {
                    currentState = State.Shooting;
                    Move(Vector3.zero);
                }
                break;
            case State.Shooting:
                InvokeRepeating("Attack", 0, fireRate);
                currentState = State.Done;
                break;
            case State.Done:
                if (trackPlayer)
                {
                    transform.LookAt(spawner.GetPlayerReference().transform);
                }
                if (fireAmount == 0)
                {
                    Move(transform.forward);
                }
                break;
        }
    }

    public override void Attack()
    {
        bp.FireBullet();
        fireAmount--;
        am.PlaySoundOnce(AudioManager.Sound.EnemyFire1, AudioManager.Priority.Low, transform);
        if (fireAmount == 0)
        {
            CancelInvoke("Attack");
        }
    }

    IEnumerator KillYouAreSelf()
    {
        yield return new WaitForSeconds(lifeTime);
        am.PlaySoundOnce(AudioManager.Sound.EnemyDeath, AudioManager.Priority.Default, transform);
        gameObject.SetActive(false);
    }

}
