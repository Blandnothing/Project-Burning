using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerScript : MonoBehaviour
{
    public float atk=5;               //¹¥»÷Á¦
    public float atkItemBack=1;     //¹¥»÷³å»÷Á¦

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 v=other.transform.position-PlayerScript.Instance.transform.position;     //³å»÷Ð§¹û
            v.Normalize();
            
            other.GetComponent<Enemy>().GetHit(v, atk);
            other.GetComponent<Rigidbody2D>().velocity = v * atkItemBack;
        }
    }
}
