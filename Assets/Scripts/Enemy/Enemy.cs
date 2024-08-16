using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] int maxHealth;
    float currentHealth;
    SpriteRenderer healthBackground;    //ÑªÌõ
    SpriteRenderer healthValue;

    [SerializeField] float invincibleTime;
    protected new Rigidbody2D rigidbody;
    protected SkeletonAnimation skeletonAnimation;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBackground = transform.Find("Health").GetComponent<SpriteRenderer>();
        healthValue = healthBackground.transform.Find("Value").GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    public void GetHit(Vector2 direction, float attackPower)
    {
        if (direction.x >= 0)
        {
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
        }
        else
        {
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
        }

        currentHealth -= attackPower;
        SetHealthValue();

        if (currentHealth <= 0)
        {
            Dead();
        }
        StopCoroutine(Invincible());
        StartCoroutine(Invincible());
    }
    void SetHealthValue()
    {
        healthValue.transform.localScale = new Vector2(currentHealth / maxHealth, healthValue.transform.localScale.y);
    }
    public void Dead()
    {
        Destroy(gameObject);
    }
    IEnumerator Invincible()
    {
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        GetComponent<MeshRenderer>().material.SetFloat("_FillPhase", 0.5f);
        yield return new WaitForSeconds(invincibleTime);
        GetComponent<MeshRenderer>().material.SetFloat("_FillPhase", 0);
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
    protected virtual void OnDestroy()
    {

    }
}
