using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public enum BulletColour
    {
        Red,
        Orange,
        Blue,
        Green
    }

    /// <summary>
    /// In order of BulletColour enums Red, Orange, Blue
    /// </summary>
    [SerializeField]
    Color[] bulletColours;

    [SerializeField]
    int numBullets;
    [SerializeField]
    int numBossBullets = 400;

    [SerializeField]
    GameObject[] bullets;
    [SerializeField]
    GameObject[] bullets_Boss;

    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject bulletBossPrefab;

    public static BulletPool Singleton;

    void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else
            Destroy(this);

        
        bullets = new GameObject[numBullets];
        bullets_Boss = new GameObject[numBossBullets];
        for (int i = 0; i < numBullets; i++)
        {
            GameObject b = Instantiate(bulletPrefab, transform);
            b.SetActive(false);
            bullets[i] = b;            
        }
        for (int i = 0; i < numBossBullets; i++)
        {
            GameObject bullet = Instantiate(bulletBossPrefab, transform);
            bullet.SetActive(false);
            bullets_Boss[i] = bullet;
        }
    }

    public static BulletPool GetInstance()
    {
        return Singleton;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        foreach (GameObject b in bullets)
        {
            b.SetActive(false);
        }
    }

    /// <summary>
    /// Grabs a deactivated bullet from the pool
    /// </summary>
    /// <returns>
    /// Not always there, check if it's null
    /// </returns>
    public GameObject GetBullet(BulletColour c)
    {
        foreach(GameObject b in bullets)
        {
            if (!b.activeSelf)
            {
                b.GetComponentInChildren<SpriteRenderer>().color = bulletColours[(int)c];
                return b;
            }
        }
        Debug.LogError("BulletPool Error: Ran out of Bullets!");
        return null;
    }

    public GameObject GetBullet_Boss()
    {
        foreach(GameObject b in bullets_Boss)
        {
            if (!b.activeSelf)
            {
                return b;
            }
        }
        Debug.LogError("BulletPool Error: Ran out of Bullets!");
        return null;
    }
}
