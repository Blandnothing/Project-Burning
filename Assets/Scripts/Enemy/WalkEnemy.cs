using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkEnemy : Enemy
{
    public float speed;
    float leftPoint;
    float rightPoint;
    [Header("初始方向，右为true,左为false")]public bool direction;
    public float atkPower;
    public float atkBack;
    private void Start()
    {
        leftPoint = transform.Find("Left").position.x;
        rightPoint = transform.Find("Right").position.x;
    }
    void Move()
    {
        rb2d.velocity = (direction ? 1 : -1) *speed*Vector2.right;
    }
    private void Update()
    {
        if(isDead || isInvicible) return;
        ChangeDirection();
        Move();
    }
    void ChangeDirection()
    {
        if (transform.position.x <= leftPoint)
        {
            direction= true;
            skeletonAnimation.skeleton.ScaleX = -1;
        }else if (transform.position.x >= rightPoint)
        {
            direction = false;
            skeletonAnimation.skeleton.ScaleX = 1;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Rigidbody2D>().velocity = (direction ? 1 : -1) *Vector2.right*atkBack;
            collision.GetComponent<PlayerScript>().GetHit(transform.position - collision.transform.position, atkPower);
        }
    }
}
