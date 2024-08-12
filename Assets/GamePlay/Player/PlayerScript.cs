using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;
using Cinemachine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class PlayerScript : MonoBehaviour
{
    static PlayerScript instance;
    public static PlayerScript Instance { get { return instance; } }
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    [SerializeField] CinemachineImpulseSource impulseSource;
   
    //ÒÆ¶¯
    public Transform groundCheck;
    public LayerMask ground;
    private bool isGround;
    private float inputX;
    private float graceTimer;
    [SerializeField] float graceTime;
    [JsonProperty] [SerializeField] float m_speed = 4.0f;
    //ÌøÔ¾
    [JsonProperty] public int maxJumpCount = 1;
    private int jumpCount;
    private bool jumpPressed;
    private bool isJump;
    [SerializeField] float m_jumpForce = 7.5f;
    //ÉúÃü
    [JsonProperty] public float maxHealth=100;
    [JsonProperty] float currentHealth;
    [SerializeField] Slider sliderHealth;
    [SerializeField] TextMeshProUGUI textHealth;
    bool isDeath;
    //¹¥»÷
    [JsonProperty] public float attackPower = 5;
    [JsonProperty] public float criticalRate = 0.05f;
    [JsonProperty] public float criticalDamage = 0.5f;
    [JsonProperty] public float attackSpeed = 1;
    public float invincibleTime;
    bool isAttack;
    public float attackBehind=0.2f;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    //½»»¥
    [HideInInspector] public bool isInteracted;



    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
    }
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        if (sliderHealth==null)
        {
            Transform healthPanel = GameObject.Find("HealthPanel").transform;
            sliderHealth = healthPanel.Find("Slider").gameObject.GetComponent<Slider>();
            textHealth=healthPanel.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        }
        textHealth.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    void Update()
    {
        if (isDeath) return;

        m_timeSinceAttack += Time.deltaTime;

        inputX = Input.GetAxis("Horizontal");

        

        if (Input.GetButtonDown("Jump") && jumpCount>0)
        {
            jumpPressed=true;
        }

        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !isAttack)
        {
            StopCoroutine(Attack());
            StartCoroutine(Attack());
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            isInteracted=true;
        }
    }
    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.05f, ground);
        GroundMovement();
        Jump();

        SwitchAnim();
    }
    void GroundMovement()
    {
        if (isAttack)
        {
            m_body2d.velocity = new Vector2(transform.localScale.x * attackSpeed, m_body2d.velocity.y);
        }
        else
        {
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        }
        if (inputX > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }

        else if (inputX < 0)
        {
            transform.localScale = new Vector2(-1, 1);
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
    IEnumerator  Attack()
    {
        float startTime=Time.time;
        isAttack = true;

        m_currentAttack++;
        if (m_currentAttack > 3)
            m_currentAttack = 1;
        if (m_timeSinceAttack > 1.0f)
            m_currentAttack = 1;
        m_animator.SetTrigger("Attack" + m_currentAttack);
        m_timeSinceAttack = 0.0f;

       

        while(Time.time - startTime < attackBehind)
        {
            if(Time.time-startTime>attackBehind-0.05f)
                isAttack = false;
            m_body2d.velocity = Vector2.zero;
            yield return  null; 
        }


        isAttack = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enermy"))
        {
        }
    }
    void SwitchAnim()
    {
        m_animator.SetFloat("SpeedX",Mathf.Abs(inputX));
        if (isGround)
        {

            m_animator.SetBool("Falling", false);
            m_body2d.gravityScale = 1;
        }        
        else if (!isGround && m_body2d.velocity.y > 0) {
            m_animator.SetBool("Jump", true);
            m_body2d.gravityScale = 1;
            graceTimer = 0;
        }    
        else if (m_body2d.velocity.y<0)
        {
            m_animator.SetBool("Jump",false);
            m_animator.SetBool("Falling", true);
            m_body2d.gravityScale = 2;
        }
    }
    public void ChangeHealth(float amount)
    {
        if (amount<0)
        {
            m_animator.SetTrigger("Hurt");
            StopCoroutine(Invincible());
            StartCoroutine(Invincible());
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if(currentHealth <= 0)
        {
            isDeath = true;
            m_animator.SetTrigger("Death");
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