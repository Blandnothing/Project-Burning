using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class WalkEnemy : Enermy
{
    public float speed;
    [Header("左巡逻点")] Transform leftPoint;
    [Header("右巡逻点")] Transform rightPoint;
    [Header("初始方向，右为true,左为false")]public bool direction;
    public float atkPower;
    private void Start()
    {
        leftPoint = transform.Find("Left");
        rightPoint = transform.Find("Right");
    }
    void Move()
    {
        rigidbody.velocity = (direction ? 1 : -1) *speed*Vector2.right;
    }
    private void Update()
    {
        ChangeDirection();
        Move();
    }
    void ChangeDirection()
    {
        if (transform.position.x <= leftPoint.position.x)
        {
            direction= true;
        }else if (transform.position.x >= rightPoint.position.x)
        {
            direction = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerScript>().GetHit(transform.position - collision.transform.position, atkPower);
        }
    }
}
