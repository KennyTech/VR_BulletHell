using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletPattern : MonoBehaviour
{
    [Header("Bullet Properties")]

    [SerializeField]
    protected BulletPool.BulletColour bulletColour;

    [SerializeField]
    protected float forwardSpeed = 30;
    [SerializeField]
    protected float rightSpeed = 0;
    [SerializeField]
    protected float upSpeed = 0;

    [SerializeField]
    protected bool anchoredToBoss = false;

    [SerializeField]
    protected float accelDelay = 0;

    [SerializeField]
    protected float accel = 0;

    [SerializeField]
    protected float scaleAmount = 1;

    [SerializeField]
    protected float scaleDuration = 0;

    [SerializeField]
    protected float rotationTime = 0;

    [SerializeField]
    protected float rotationAmountX = 0;
    [SerializeField]
    protected float rotationAmountY = 0;
    [SerializeField]
    protected float rotationAmountZ = 0;

    [SerializeField]
    protected float lifeTime = 3;

    [Header("Emitter Properties")]
    [SerializeField]
    protected Vector3 emitterSpin;

    [SerializeField]
    protected bool enableSpin;

    protected BulletPool bp;

    [SerializeField]
    protected Transform spawnPoint;

    private void Start()
    {
        bp = BulletPool.GetInstance();
    }

    /// <summary>
    /// Spins the emitter, call this every update
    /// </summary>
    public void SpinEmitter()
    {
        spawnPoint.transform.Rotate(emitterSpin * Time.deltaTime);
    }

    public Transform GetEmitter()
    {
        return spawnPoint;
    }

    public abstract void FireBullet();
    public abstract void FireBullet_Boss();

    protected void ApplyBulletProperties(GameObject bullet)
    {
        SmartBullet theBullet = bullet.GetComponent<SmartBullet>();
        theBullet.SetAnchored(anchoredToBoss);
        theBullet.SetForwardSpeed(forwardSpeed);
        theBullet.SetRightSpeed(rightSpeed);
        theBullet.SetUpSpeed(upSpeed);
        theBullet.SetAccelDelay(accelDelay);
        theBullet.SetBulletAccel(accel);
        theBullet.SetScaleAmount(new Vector3(scaleAmount, scaleAmount, scaleAmount));
        theBullet.SetScaleDuration(scaleDuration);
        theBullet.SetRotationDuration(rotationTime);
        theBullet.SetRotationAmountX(rotationAmountX);
        theBullet.SetRotationAmountY(rotationAmountY);
        theBullet.SetRotationAmountZ(rotationAmountZ);
        theBullet.SetLifeTime(lifeTime);
    }
}
