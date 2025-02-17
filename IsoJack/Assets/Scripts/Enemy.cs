﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;

    public bool aggressive = false;

    public float moveSpeed;
    public float stopDistance;
    private Transform target;
    public CharTalk talk;


    //combat stuff
    private float attackCd = 0;
    public float attackTimer;
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public float damage;

    private Animator anim;


    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (aggressive == true)
        {

            if (Vector2.Distance(transform.position, target.position) < 5 && Vector2.Distance(transform.position, target.position) > 1)
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

            if (attackCd <= 0)
            {

                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemiesToDamage[i].GetComponent<PlayerAttack>().TakeDamage(damage);
                    anim.SetTrigger("attack");
                }

                attackCd = attackTimer;

            }

            else
            {
                attackCd -= Time.deltaTime;
            }
        }

        if (health <= 0)
        {
            Destroy(gameObject);

        }
    }

    


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);

    }

   

    public void EnemyTakeDamage(float damage)
    {

        health -= damage;
        Debug.Log("Enemy takes damage!!!");

        aggressive = true;
    }
}
