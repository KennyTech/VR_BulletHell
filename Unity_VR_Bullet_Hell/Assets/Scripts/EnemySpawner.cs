using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Refs")]
    [SerializeField]
    private GameObject asteroidClone;
    [SerializeField]
    private GameObject bossClone;
    [SerializeField]
    private List<GameObject> enemyClone;
    
    [Header("Player Ref")]
    [SerializeField]
    PlayerShip player;

    [Header("Enemy Pools")]
    [SerializeField]
    private int poolSize_asteroid;
    [SerializeField]
    private List<GameObject> asteroidPool;

    [SerializeField]
    private int poolSize_green;
    [SerializeField]
    private List<GameObject> enemyPool_Green;

    [SerializeField]
    private int poolSize_orange;
    [SerializeField]
    private List<GameObject> enemyPool_Orange;

    [SerializeField]
    private int poolSize_red;
    [SerializeField]
    private List<GameObject> enemyPool_Red;

    [SerializeField]
    private int poolSize_blue;
    [SerializeField]
    private List<GameObject> enemyPool_Blue;


    [Header("Spawning stuff")]
    [SerializeField]
    private bool randomStartPos = true;
    [SerializeField]
    private float spawnRate = 1f; // does nothing
    [SerializeField]
    private float bossSpawnInTime = 9001; // 1:30 to spawn in boss - disabled at the moment - boss spawns through another method

    private Bounds spawnArea;
    private float t_bossTimer = 0;
    private bool hasSpawnedBoos = false;



    [SerializeField]
    public int gamePhase = 3;                       // start phase 3 b/c asteroids don't work at the moment

    [SerializeField]
    private float spawnLength = 23.0f;              // duration that enemies spawn for in each phase, this is also the minimum duration of each phase

    [SerializeField]
    private bool enemiesDead = false;

    [SerializeField]
    private bool spawnLengthRequirement = false;    // we don't check for enemies all dead or next phase until enemies are done spawning

    [SerializeField]
    private int spawnAsteroidTime = 1;              // in seconds, how often asteroids spawn periodically (max 3)

    [SerializeField]
    private int spawnEnemyGreenTime = 3;            // in seconds, how often enemy spawn periodically (max 3)

    [SerializeField]
    private int spawnEnemyOrangeTime = 6;           // in seconds, how often enemy spawn periodically (max 3)

    [SerializeField]
    private int spawnEnemyRedTime = 8;              // in seconds, how often enemy spawn periodically (max 3)

    [SerializeField]
    private int spawnEnemyBlueTime = 10;            // in seconds, how often enemy spawn periodically (max 3)

    [SerializeField]
    private float timeBetweenPhase = 14.0f;         // in seconds, recommend 12.0 to 20.0 seconds (14 seconds is sweet spot due to dialogue and warp drive time)

    [SerializeField]
    private bool transitioning = false;             // bool that is set to true when player is transitioning between levels (dialogue, warp drive), disables enemy spawns temporarily

    [SerializeField]
    private int count = 0;                          // used for our timer periodic system


    void Start()
    {        
        BoxCollider b = GetComponent<BoxCollider>();
        spawnArea.center = b.center;
        spawnArea.extents = b.size;

        GameObject g = new GameObject("AsteroidBelt");
        g.transform.parent = transform;

        GameObject h = new GameObject("EnemyPool");


        // Make sure you connect all the enemy clones
        CreatePool(asteroidPool, g.transform, asteroidClone, poolSize_asteroid);
        CreatePool(enemyPool_Green, h.transform, enemyClone[0], poolSize_green);
        CreatePool(enemyPool_Orange, h.transform, enemyClone[1], poolSize_orange);
        CreatePool(enemyPool_Red, h.transform, enemyClone[2], poolSize_red);
        CreatePool(enemyPool_Blue, h.transform, enemyClone[3], poolSize_blue);

        t_bossTimer = 0;

        // INTRO DIALOGUE
        // THIS ALSO HANDLES INTRO DIALOGUE FOR PLAYER
        InvokeRepeating("UpdatePerSecond", 17.0f, 1.0f);
        Invoke("BeginPhase", spawnLength + 17.0f); // give 17 second buffer for first phase (intro sequence)
        Invoke("PlayDialogue", 10.0f);
        Invoke("PlayDialogue", 20.0f);

    }

    private void BeginPhase()
    {
        spawnLengthRequirement = true;  // minimum spawn length met, enemies done spawning, we can begin to check if all enemies are dead
    }

    private void PlayDialogue()
    {
        FindObjectOfType<DialogueTrigger>().TriggerDialogue();
    }

    public bool AreEnemiesDead()
    {
        CheckEnemyDead(asteroidPool);
        if (enemiesDead == false) // If alive, return false (not dead). If dead, keep going
            return false;

        CheckEnemyDead(enemyPool_Green);
        if (enemiesDead == false)
            return false;

        CheckEnemyDead(enemyPool_Orange);
        if (enemiesDead == false)
            return false;

        CheckEnemyDead(enemyPool_Red);
        if (enemiesDead == false)
            return false;

        CheckEnemyDead(enemyPool_Blue);
        if (enemiesDead == false)
            return false;

        enemiesDead = true; // All dead, set to true and return true
        return true;
    }

    public void CheckEnemyDead(List<GameObject> enemies)
    {
        enemiesDead = true; // Assume all enemies are dead, but the moment we iterate through all enemies and find an alive one, set to false (they're not all dead) 

        foreach (var enemy in enemies)
        {
            if (enemy.activeSelf)
            {
                enemiesDead = false;
            }
        }
    }

    private void UpdatePerSecond()  // Handles a lighter weight periodic spawning, and periodic checking if enemies are dead
    {
        count += 1; // counts seconds

        /*
        if (spawnLengthRequirement == false)
        {
            if (gamePhase >= 2 && count % spawnAsteroidTime == 0)   // spawn asteroid per x second if possible
            {
                SpawnAsteroid();
            }
        }
        */
        if (spawnLengthRequirement == false && transitioning == false && !hasSpawnedBoos)
        {
            if (gamePhase >= 3 && count % spawnEnemyGreenTime == 0)
            {
                SpawnEnemy_Green(); // spawn a green enemy every x seconds
            }
        }
        if (spawnLengthRequirement == false && transitioning == false && !hasSpawnedBoos)
        {
            if (gamePhase >= 4 && count % spawnEnemyOrangeTime == 0)
            {
                SpawnEnemy_Orange();
            }
        }
        if (spawnLengthRequirement == false && transitioning == false && !hasSpawnedBoos)
        {
            if (gamePhase >= 5 && count % spawnEnemyRedTime == 0)
            {
                SpawnEnemy_Red();
            }
        }
        if (spawnLengthRequirement == false && transitioning == false && !hasSpawnedBoos)
        {
            if (gamePhase >= 6 && count % spawnEnemyBlueTime == 0)
            {
                SpawnEnemy_Blue();
            }
        }
        if (spawnLengthRequirement == false && transitioning == false && !hasSpawnedBoos)
        {
            if (gamePhase >= 7)
            {
                Transform instBoss = Instantiate(bossClone).transform;
                instBoss.position = new Vector3(0, 15.04f, 4.221f);
                instBoss.gameObject.SetActive(true);
                print("Spawn Boss");
                hasSpawnedBoos = true;
            }
        }

        // EXTRA SPAWNS (DIFFICULTY BALANCE)
        // For Phase 4-6 (SECOND to FOURTH wave spawn balance, doesn't include FIRST wave because it is tutorial and FIFTH is boss)
        if (!hasSpawnedBoos && count % (2+(gamePhase-3)*3) == 0 && gamePhase >= 4 && gamePhase <= 6) // spawn an extra green every 5/8/11 seconds
        {
            SpawnEnemy_Green();
        }
        if (!hasSpawnedBoos && count % (3+(gamePhase-3)*4) == 0 && gamePhase >= 4 && gamePhase <= 6) // spawn an extra orange every 7/11/15 seconds
        {
            SpawnEnemy_Orange();
        }

        if (spawnLengthRequirement == true && !hasSpawnedBoos) // if enemies are done spawning, and not when boss is spawned, we can keep going to next phase (b/c boss is last)
        {
            print("This only runs when boss has not spawned");
            AreEnemiesDead();
            if (enemiesDead == true)    // If they're all dead, give player short break, play dialogue, then get on with next phase
            {
                transitioning = true;
                enemiesDead = false;
                spawnLengthRequirement = false;


                Invoke("PlayDialogue", 2.0f); // Play dialogue
                //Invoke("PlayDialogue", 11.0f); // Play dialogue

                Invoke("NextPhase", timeBetweenPhase - 1.0f);
                if (gamePhase == 4 || gamePhase == 5 || gamePhase == 6)
                {
                    Invoke("WarpDrive", 16.0f);
                }
            }
        }
    }

    private void WarpDrive()
    {
        FindObjectOfType<DialogueTrigger>().WarpDrive();
    }


    private void NextPhase()
    {
        if (gamePhase < 7)
        {
            transitioning = false;
            gamePhase += 1;
            spawnLengthRequirement = false; // Reset vars for next phase (already resetted, but just in case)
            Invoke("BeginPhase", spawnLength);
        }
    }


    private void Update()
    {

        // CHEAT FUNCTION click (left) to kill enemies instantly
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed primary button.");
            KillRandomEnemy();
        }
        

        /*

        if (!hasSpawnedBoos)
        {
            t_bossTimer += Time.deltaTime;
            if (t_bossTimer > bossSpawnInTime)
            {
                Transform instBoss = Instantiate(bossClone).transform;
                instBoss.position = new Vector3(0, 15.04f, 4.221f);
                instBoss.gameObject.SetActive(true);
                hasSpawnedBoos = true;
            }
        }
        */
    }

    // A cheat function for testing
    public void KillAnEnemy(List<GameObject> enemies)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.activeSelf) // its alive! let's kill it!
            {
                enemy.gameObject.SetActive(false); // bye
            }
        }
    }


    private void OnEnable()
    {
        /*
        // Start Spawn Stuff
        InvokeRepeating("SpawnAsteroid", 8.0f, 1f);
        InvokeRepeating("SpawnEnemy_Green", 8.0f, 2f);
        InvokeRepeating("SpawnEnemy_Orange", 23f, 4f);
        InvokeRepeating("SpawnEnemy_Red", 38f, 7f);
        InvokeRepeating("SpawnEnemy_Blue", 53f, 11f);
        */
    }

    // Cheat for test, kill with left click
    private void KillRandomEnemy()
    {
        KillAnEnemy(enemyPool_Green);
        KillAnEnemy(enemyPool_Orange);
        KillAnEnemy(enemyPool_Red);
        KillAnEnemy(enemyPool_Blue);
    }

    private void SpawnAsteroid()
    {
        ActivateEnemy(asteroidPool);
    }
    private void SpawnEnemy_Green()
    {
        ActivateEnemy(enemyPool_Green);
    }
    private void SpawnEnemy_Orange()
    {
        ActivateEnemy(enemyPool_Orange);
    }
    private void SpawnEnemy_Red()
    {
        ActivateEnemy(enemyPool_Red);
    }
    private void SpawnEnemy_Blue()
    {
        ActivateEnemy(enemyPool_Blue);
    }

    // searches through a list and sets the first inactive object active
    // placing it randomly
    public void ActivateEnemy(List<GameObject> enemies)
    {
        foreach (var enemy in enemies)
        {   
            if (!enemy.activeSelf)
            {
                enemy.SetActive(true);
                enemy.transform.position = transform.position;
                if (randomStartPos)
                {
                    enemy.transform.Translate(new Vector3(
                        Random.Range(spawnArea.min.x, spawnArea.max.x),
                        Random.Range(spawnArea.min.y, spawnArea.max.y),
                        Random.Range(spawnArea.min.z, spawnArea.max.z)
                        ));
                }
                enemy.transform.rotation = transform.rotation;
                break;
            }
        }
    }

    public void CreatePool(List<GameObject> pool, Transform poolParent, GameObject clone, int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            // Spawn enemies, set all inactive add to list
            GameObject enemyInst = Instantiate(clone, poolParent);
            enemyInst.SetActive(false);
            pool.Add(enemyInst);
        }
    }

    public PlayerShip GetPlayerReference()
    {
        return player;
    }
}
