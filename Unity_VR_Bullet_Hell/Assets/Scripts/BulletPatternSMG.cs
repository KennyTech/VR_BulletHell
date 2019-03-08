using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternSMG : BulletPattern
{
    public override void FireBullet()
    {
        GameObject bullet = bp.GetBullet(bulletColour);
        if (bullet != null)
        {
            bullet.transform.position = transform.position;

            bullet.transform.rotation = spawnPoint.rotation;
            bullet.transform.forward = spawnPoint.transform.forward;

            ApplyBulletProperties(bullet);

            bullet.SetActive(true);
        }
    }

    public override void FireBullet_Boss()
    {
        throw new System.NotImplementedException();
    }
}
