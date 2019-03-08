using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternRing : BulletPattern
{
    //public enum RingAxis
    //{
    //    AxisX,
    //    AxisY,
    //    AxisZ
    //}

    //[SerializeField]
    //RingAxis ringAxis;

    [SerializeField]
    protected int bulletRingAmount = 3;

    [SerializeField]
    bool emitAxisX;
    [SerializeField]
    bool emitAxisY;

    private void OnEnable()
    {
        //InvokeRepeating("FireBullet", 0, fireRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (enableSpin)
            SpinEmitter();
    }

    public override void FireBullet()
    {
        if (emitAxisX)
        {
            for (int i = 0; i < bulletRingAmount; i++)
            {
                GameObject bullet = bp.GetBullet(bulletColour);
                if (bullet != null)
                {
                    bullet.transform.position = spawnPoint.position;
                    bullet.transform.rotation = spawnPoint.rotation;
                    bullet.transform.forward = spawnPoint.transform.forward;

                    bullet.transform.Rotate(i * (360 / bulletRingAmount), 0, 0);

                    ApplyBulletProperties(bullet);

                    bullet.SetActive(true);
                }
            }
        }

        if (emitAxisY)
        {
            for (int i = 0; i < bulletRingAmount; i++)
            {
                GameObject bullet = bp.GetBullet(bulletColour);
                if (bullet != null)
                {
                    bullet.transform.position = spawnPoint.position;
                    bullet.transform.rotation = spawnPoint.rotation;
                    bullet.transform.forward = spawnPoint.transform.forward;

                    bullet.transform.Rotate(0, i * (360 / bulletRingAmount), 0);

                    ApplyBulletProperties(bullet);

                    bullet.SetActive(true);
                }
            }
        }
    }

    public override void FireBullet_Boss()
    {
        if (emitAxisX)
        {
            for (int i = 0; i < bulletRingAmount; i++)
            {
                GameObject bullet = bp.GetBullet_Boss();
                if (bullet != null)
                {
                    bullet.transform.position = spawnPoint.position;
                    bullet.transform.rotation = spawnPoint.rotation;
                    bullet.transform.forward = spawnPoint.transform.forward;

                    bullet.transform.Rotate(i * (360 / bulletRingAmount), 0, 0);

                    ApplyBulletProperties(bullet);

                    bullet.SetActive(true);
                }
            }
        }

        if (emitAxisY)
        {
            for (int i = 0; i < bulletRingAmount; i++)
            {
                GameObject bullet = bp.GetBullet_Boss();
                if (bullet != null)
                {
                    bullet.transform.position = spawnPoint.position;
                    bullet.transform.rotation = spawnPoint.rotation;
                    bullet.transform.forward = spawnPoint.transform.forward;

                    bullet.transform.Rotate(0, i * (360 / bulletRingAmount), 0);

                    ApplyBulletProperties(bullet);

                    bullet.SetActive(true);
                }
            }
        }
    }
}
