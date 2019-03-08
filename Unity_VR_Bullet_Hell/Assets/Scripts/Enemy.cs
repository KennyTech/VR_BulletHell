using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemySpace
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        public float health = 100F;       // Health of Enemy
        public float damage = 1F;       // Damage to player
        [SerializeField]
        protected float speed;        // Movement Speed of enemy
        [Header("Set-up")]
        public GameObject deathParticles;

        public Vector3 rotationDir;
        public float RotSpeed = 50f;
        public float t_LerpScale = 0;

        protected Rigidbody rb;

        // Override function for enemies
        public abstract void Attack();

        public void Move(Vector3 dir)
        {
            rb.velocity = dir * speed;
        }

        private void OnEnable()
        {
            rotationDir = new Vector3(Random.Range(-RotSpeed, RotSpeed), Random.Range(-RotSpeed, RotSpeed), Random.Range(-RotSpeed, RotSpeed));
            health = 100f;
        }

        public void DamageEnemy(float amount)
        {
            health -= amount;
            if (health <= 0)
            {
                InstantKillEnemy();
            }
        }

        public void InstantKillEnemy()
        {
            gameObject.SetActive(false);
            if (deathParticles)
            {
                GameObject particleInst = Instantiate(deathParticles);
                particleInst.transform.position = transform.position;
                Destroy(particleInst, 5);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                InstantKillEnemy();
                other.GetComponent<PlayerShip>().DealDamage();
            }
            else if (other.CompareTag("PlayerBullet"))
            {
                DamageEnemy(1);
                other.gameObject.SetActive(false);
            }
        }
    }
}
