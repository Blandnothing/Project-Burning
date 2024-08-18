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
    SpriteRenderer healthBackground;    //血条
    SpriteRenderer healthValue;

    [SerializeField] float invincibleTime;
    [SerializeField] float deadTime = 1;
    [Header("掉落物列表")] [SerializeField] List<GameObject> drops = new();
    protected Rigidbody2D rb2d;
    protected SkeletonAnimation skeletonAnimation;
    protected bool isDead;
    protected bool isInvicible;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBackground = transform.Find("Health").GetComponent<SpriteRenderer>();
        healthValue = healthBackground.transform.Find("Value").GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    public void GetHit(Vector2 direction, float attackPower)
    {
        //if (direction.x >= 0)
        //{
        //    GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
        //}
        //else
        //{
        //    GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
        //}

        currentHealth -= attackPower;
        SetHealthValue();

        if (currentHealth <= 0)
        {
            StopCoroutine(Dead());
            StartCoroutine(Dead());
        }
        else
        {
            StopCoroutine(Invincible());
            StartCoroutine(Invincible());
        }
        
    }
    void SetHealthValue()
    {
        healthValue.transform.localScale = new Vector2(currentHealth / maxHealth, healthValue.transform.localScale.y);
    }
    IEnumerator Dead()
    {
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        rb2d.bodyType = RigidbodyType2D.Static;
        Destroy(healthBackground.gameObject);
        isDead = true;

        var skeleton = GetComponent<SkeletonAnimation>().skeleton;
        float deadTimer = 0;
        while (deadTimer < deadTime)
        {
            yield return null;
            deadTimer += Time.deltaTime;
            skeleton.SetColor(new Color(1, 1, 1, Mathf.Lerp(1, 0, deadTimer / deadTime)));
        }
        skeleton.SetColor(new Color(1, 1, 1, 0));
        Destroy(gameObject);
    }
    IEnumerator Invincible()
    {
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        GetComponent<MeshRenderer>().material.SetFloat("_FillPhase", 0.5f);
        isInvicible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvicible = false;
        GetComponent<MeshRenderer>().material.SetFloat("_FillPhase", 0);
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
    protected virtual void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
        {
            return;
        }

        foreach (var item in drops)
        {
            Instantiate(item,transform.position,Quaternion.identity);
        }
    }
}
