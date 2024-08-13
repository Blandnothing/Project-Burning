using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;
using Cinemachine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Spine.Unity;

[JsonObject(MemberSerialization.OptIn)]
public class PlayerScript : MonoBehaviour
{
    static PlayerScript instance;
    public static PlayerScript Instance { get {
            if (instance==null)
            {
                Debug.LogError("Player Lost");
            }
            return instance; } }
    private Rigidbody2D m_body2d;
    private SkeletonAnimation m_skeleton;
    string currentAnimation;
    CinemachineImpulseSource impulseSource;
   
    //移动
    private bool isGround;         //是否在地面上，用于跳跃
    [SerializeField] LayerMask groundLayer;
    private float inputX;
    private float graceTimer;
    [SerializeField] float graceTime;
    [JsonProperty] [SerializeField] float m_speed = 4.0f;
    //跳跃
    [JsonProperty] public int maxJumpCount = 1;
    private int jumpCount;          //可跳跃次数
    private bool jumpPressed;
    private bool isJump;
    [SerializeField] float m_jumpForce = 7.5f;
    //生命
    [JsonProperty] public float maxHealth=100;
    [JsonProperty] float currentHealth;
    [SerializeField] Slider sliderHealth;
    [SerializeField] Text textHealth;
    bool isDeath;
    //攻击
    [JsonProperty] public float attackPower = 5;
    [JsonProperty] public float attackBack = 5;    //冲击力
    [JsonProperty] public float attackSpeed = 1;   //攻击时的移动速度
    public float invincibleTime;                   //无敌帧时间
    bool isAttack;
    public float attackBehind=0.2f;                //攻击后的冷却时间
    private float m_timeSinceAttack = 0.0f;
    BoxCollider2D atkCol;        //攻击碰撞体
    [SerializeField] AttackTriggerScript atkTrigger;
    Coroutine curAtkC;  //当前攻击协程
   
    //交互
    [HideInInspector] public bool isInteracted;



    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        m_body2d = GetComponent<Rigidbody2D>();
        m_skeleton = GetComponent<SkeletonAnimation>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentAnimation = "战斗待机";
        atkCol = atkTrigger.GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        
        currentHealth = maxHealth;
        if (sliderHealth==null)
        {
            sliderHealth = GameObject.Find("UI").transform.Find("Health").GetComponent<Slider>();
            if (sliderHealth == null)
            {
                Debug.LogError("SliderHealth Lost");
            }
            textHealth =sliderHealth.transform.Find("Text").gameObject.GetComponent<Text>();
        }
        sliderHealth.value = currentHealth / maxHealth;
        textHealth.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    void Update()
    {

        if (isDeath) return;
        m_timeSinceAttack += Time.deltaTime;

        inputX = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && jumpCount>0)
        {

            jumpPressed = true;
        }

        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !isAttack)
        {
            if(curAtkC!=null)
            StopCoroutine(curAtkC);
            curAtkC = StartCoroutine(Attack(attackPower,attackBack));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            isInteracted=true;
        }
    }
    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapBoxAll(transform.position, new Vector2(1.7f, 0.1f), 0, groundLayer).Length!=0;
        GroundMovement();
        Jump();

        SwitchAnim();
    }
    void GroundMovement()
    {
        if (inputX > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        else if (inputX < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        if (isAttack)
        {
            m_body2d.velocity = new Vector2(transform.localScale.x * attackSpeed, m_body2d.velocity.y);
        }
        else
        {
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        }
        
    }
    void Jump()
    {
        if (isGround)
        {
            jumpCount = maxJumpCount;
            isJump = false;
            graceTimer = graceTime;
        }
        else
        {
            graceTimer-=Time.fixedDeltaTime;
        }
        if (isAttack)
        {
            jumpPressed = false;
            return;
        }
        
        if (jumpPressed && (isGround  || graceTimer>0))
        {
            isJump = true;
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            jumpCount--;
            jumpPressed = false;
            graceTimer = 0;
        }else if(jumpPressed && jumpCount > 0 && isJump)
        {
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            jumpCount--;
            jumpPressed = false;
        } 
    }
    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="atk">攻击力</param>
    /// <param name="atkBack">攻击冲击力</param>
    /// <returns></returns>
    IEnumerator  Attack(float atk,float atkBack)
    {      
        isAttack = true;

        atkCol.enabled = true;
        atkTrigger.atk = atk;
        atkTrigger.atkItemBack = atkBack;
        var track = SetAnimation("攻击",false);
        track.Complete += (TrackEntry) => { 
            isAttack = false; 
            atkCol.enabled = false;
        };
        

        while (isAttack)
        {
            yield return null;  
        }
        float startTime = Time.time;
        m_timeSinceAttack = 0f;

        while (Time.time - startTime < attackBehind)
        {
            if(Time.time-startTime>attackBehind-0.05f)
                isAttack = false;
            m_body2d.velocity = Vector2.zero;
            yield return  null; 
        }


        isAttack = false;
    }
    Spine.TrackEntry SetAnimation(string animation,bool loop)
    {
        if(animation == currentAnimation) return null;
        Spine.TrackEntry track = m_skeleton.state.SetAnimation(0, animation, loop);

        currentAnimation = animation;

        return track;
    }
    void SwitchAnim()
    {
        if (isAttack) return;
        if (inputX!=0)
        {
            SetAnimation("跑步", true);
        }
        else
        {
            SetAnimation("战斗待机", true);
        }
        //if (isGround)
        //{

        //    m_animator.SetBool("Falling", false);
        //    m_body2d.gravityScale = 1;
        //}        
        //else if (!isGround && m_body2d.velocity.y > 0) {
        //    m_animator.SetBool("Jump", true);
        //    m_body2d.gravityScale = 1;
        //    graceTimer = 0;
        //}    
        //else if (m_body2d.velocity.y<0)
        //{
        //    m_animator.SetBool("Jump",false);
        //    m_animator.SetBool("Falling", true);
        //    m_body2d.gravityScale = 2;
        //}
    }
    public void ChangeHealth(float amount)
    {
        if (amount<0)
        {
            //m_animator.SetTrigger("Hurt");
            StopCoroutine(Invincible());
            StartCoroutine(Invincible());
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if(currentHealth <= 0)
        {
            isDeath = true;
            //m_animator.SetTrigger("Death");
        }sliderHealth.value = currentHealth/maxHealth;
        textHealth.text=currentHealth.ToString()+"/"+maxHealth.ToString();
    }

    IEnumerator Invincible()
    {
        gameObject.layer = LayerMask.NameToLayer("invincible");
        yield return new WaitForSeconds(invincibleTime);
        gameObject.layer = LayerMask.NameToLayer("player");
    } 
    void Death()
    {
        SceneManager.LoadScene(0);
    }
}