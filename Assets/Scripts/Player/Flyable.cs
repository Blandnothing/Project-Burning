using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyable : MonoBehaviour
{
    public float speed = 20f;
    public Vector2 direction;
    Rigidbody2D rb2D;
    public float liveTime=10f;
    float liveTimer;
    bool isDead;
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        liveTimer+=Time.deltaTime;
        if (liveTimer > liveTime || isDead)
        {
            Succide();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            isDead = true;
        }
    }
    private void FixedUpdate()
    {
        rb2D.velocity = direction * speed;
    }
    public void Succide()
    {
        Destroy(gameObject);
    }
}
