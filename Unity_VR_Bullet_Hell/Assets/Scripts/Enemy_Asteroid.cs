using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemySpace;
public class Enemy_Asteroid : Enemy
{
    private float t_LerpTimer = 0;
    //Rigidbody rb;

    private void Start()
    {
        t_LerpTimer = 0;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Velocity movement
        //Move(speed * transform.forward);
        rb.velocity = transform.forward * -5f;
        // Transform Rotate for space feel

        transform.localRotation = Quaternion.Euler(transform.localRotation.x + rotationDir.x, 
            transform.localRotation.y + rotationDir.y, 
            transform.localRotation.z + rotationDir.z);
        // Transform Scaling as it moves
        transform.localScale = ScaleInView(t_LerpTimer, 3);


        //Scaling objects timer from enemy base class
        t_LerpScale += Time.deltaTime;
        // Resets the asteroids after 4 secs
        if (t_LerpScale > 4)
        {
            InstantKillEnemy();
        }


        t_LerpTimer += Time.deltaTime;
    }

    public Vector3 ScaleInView(float timer, float timeToLast)
    {
        return Vector3.Lerp(new Vector3(0.00001f, 0.00001f, 0.00001f), new Vector3(2,2,3), timer/timeToLast);
    }

    // Asteroids dont have an attack
    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
