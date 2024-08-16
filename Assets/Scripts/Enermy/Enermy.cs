using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enermy : MonoBehaviour
{
    [SerializeField] int maxHealth;
    float currentHealth;
    Image healthBackground;    //ÑªÌõ
    Image healthValue;

    [SerializeField] float invincibleTime;
    protected new Rigidbody2D rigidbody;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBackground = transform.Find("Health").GetComponent<Image>();
        healthValue = healthBackground.transform.Find("Value").GetComponent<Image>();
        rigidbody = GetComponent<Rigidbody2D>();
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
        healthValue.rectTransform.sizeDelta = new Vector2(healthBackground.rectTransform.rect.width * currentHealth / maxHealth, healthValue.rectTransform.sizeDelta.y);
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
        gameObject.layer = LayerMask.NameToLayer("Enermy");
    }
    protected virtual void OnDestroy()
    {

    }
}
