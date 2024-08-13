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

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBackground = transform.Find("Health").GetComponent<Image>();
        healthValue = healthBackground.transform.Find("Value").GetComponent<Image>();
    }
    public void GetHit(Vector2 direction, float attackPower)
    {
        if (direction.x >= 0)
        {
            GetComponent<SpriteRenderer>().flipX=false;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
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
        gameObject.layer = LayerMask.NameToLayer("invincible");
        yield return new WaitForSeconds(invincibleTime);
        gameObject.layer = LayerMask.NameToLayer("Enermy");
    }
    protected virtual void OnDestroy()
    {

    }
}
