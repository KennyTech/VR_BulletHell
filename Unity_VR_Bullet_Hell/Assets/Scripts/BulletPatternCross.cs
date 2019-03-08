using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternCross : BulletPattern
{
    [SerializeField]
    bool alternateFire = true;

    [SerializeField]
    Vector3 emitterRotation = new Vector3(75, -90, 0);

    [SerializeField]
    Vector3 emitterRotationAlt = new Vector3(75, 90, 0);

    [SerializeField]
    bool rotate = false;

    private void Update()
    {
        if (enableSpin)
            SpinEmitter();
    }

    public override void FireBullet()
    {
        if (alternateFire)
        {
            rotate = !rotate;
            if (rotate)
            {
                spawnPoint.transform.localEulerAngles = emitterRotation;
            }
            else
            {
                spawnPoint.transform.localEulerAngles = emitterRotationAlt;
            }
        }
        
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = bp.GetBullet(bulletColour);
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                
                bullet.transform.rotation = spawnPoint.rotation;

                switch (i)
                {
                    case 0:
                        bullet.transform.forward = spawnPoint.transform.forward;
                        break;
                    case 1:
                        bullet.transform.forward = spawnPoint.transform.right;
                        break;
                    case 2:
                        bullet.transform.forward = -spawnPoint.transform.right;
                        break;
                    case 3:
                        bullet.transform.forward = spawnPoint.transform.up;
                        break;
                    case 4:
                        bullet.transform.forward = -spawnPoint.transform.up;
                        break;
                }
                //bullet.transform.Rotate(i * (360 / 4), 0, 0);

                ApplyBulletProperties(bullet);

                bullet.SetActive(true);
            }
        }
    }

    public override void FireBullet_Boss()
    {
        throw new System.NotImplementedException();
    }
}
