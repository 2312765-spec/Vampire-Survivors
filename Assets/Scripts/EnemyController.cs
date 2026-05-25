using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Info")]
    public Rigidbody2D theRB;
    public float moveSpeed;
    private GameObject target;
    public float damage;
    public float hitWaitTime = 1f;
    private float hitCounter;
    public float health = 5f;
    public float knockBackTime = .5f;
    private float knockBackCounter;
    [Header("Drop")]
    public int expToGive = 1;
    public int coinValue = 1;
    public float coinDropRate = 0.5f;
    private Vector2 moveDirection;

    private void Awake()
    {
        theRB = GetComponent<Rigidbody2D>();
    }
    // void Start()
    // {
    //     target = PlayerHealthController.instance.transform;
    // }

    void FixedUpdate()
    {
        if (target.activeSelf)
        {
            if (knockBackCounter > 0)
            {
                knockBackCounter -= Time.fixedDeltaTime;

                // Knockback
                theRB.velocity = -moveDirection * moveSpeed * 2f;

                if (knockBackCounter <= 0)
                {
                    theRB.velocity = Vector2.zero;
                }
                return;
            }

            // Tính hướng di chuyển
            moveDirection = (target.transform.position - transform.position).normalized;

            // Di chuyển bằng Rigidbody2D
            theRB.velocity = moveDirection * moveSpeed;

            if (hitCounter > 0)
            {
                hitCounter -= Time.fixedDeltaTime;
            }
        }
        else
        {
            theRB.velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && hitCounter <= 0f)
        {
            PlayerHealthController.instance.TakeDamage(damage);

            hitCounter = hitWaitTime;
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        if (health <= 0)
        {
            Destroy(gameObject);

            ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);

            if (Random.value <= coinDropRate)
            {
                CoinController.instance.DropCoin(transform.position, coinValue);
            }

            SFXManager.instance.PlaySFXPitched(0);
        }
        else
        {
            SFXManager.instance.PlaySFXPitched(1);
        }

        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    public void TakeDamage(float damageToTake, bool shouldKnockBack)
    {
        TakeDamage(damageToTake);

        if (shouldKnockBack)
        {
            knockBackCounter = knockBackTime;
        }
    }


    public void SetTarget(GameObject newTarget)
    {
        this.target = newTarget;
    }
}