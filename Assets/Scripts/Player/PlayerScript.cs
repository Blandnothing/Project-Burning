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
using System.Collections.Generic;

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
    Spine.TrackEntry currentTrack;
    CinemachineImpulseSource impulseSource;
   
    //�ƶ�
    private bool isGround;         //�Ƿ��ڵ����ϣ�������Ծ
    [SerializeField] LayerMask groundLayer;
    private float inputX;
    private float graceTimer;
    [SerializeField] float graceTime;
    [JsonProperty] [SerializeField] float m_speed = 4.0f;
    Vector2 prePos;       //��ɫ��һ֡��λ��
    //��Ծ
    [JsonProperty] public int maxJumpCount = 1;
    private int jumpCount;          //����Ծ����
    private bool jumpPressed;
    private bool isJump;
    bool isFalling;       //�Ƿ�����
    [SerializeField] float m_jumpForce = 7.5f;
    public string preAnimation;
    //����
    [JsonProperty] public float maxHealth=100;
    [JsonProperty] float currentHealth;
    [SerializeField] Slider sliderHealth;
    [SerializeField] Text textHealth;
    bool isDeath;
    //����
    [JsonProperty] public float attackPower = 5;
    [JsonProperty] public float attackBack = 5;    //�����
    [JsonProperty] public float attackSpeed = 1;   //����ʱ���ƶ��ٶ�
    public float invincibleTime;                   //�޵�֡ʱ��
    bool isAttack;
    public float attackBehind=0.2f;                //���������ȴʱ��
    BoxCollider2D atkCol;        //������ײ��
    [SerializeField] AttackTriggerScript atkTrigger;
    Coroutine curAtkC;  //��ǰ����Э��

    public Dictionary<KeyCode, SkillInfo> dicSkill = new();
   
    //����
    [HideInInspector] public bool isInteracted;

    public struct SkillInfo
    {
        public enum SkillType
        {
            atk,
            heal
        }
        public SkillType type;
        public string anim;
        public float atk;
        public float atkBack;

        public SkillInfo(SkillType type, string anim, float attackPower, float attackBack) : this()
        {
            this.type = type;
            this.anim = anim;
            this.atk = attackPower;
            this.atkBack = attackBack;
        }
    }

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        m_body2d = GetComponent<Rigidbody2D>();
        m_skeleton = GetComponent<SkeletonAnimation>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentAnimation = "ս������";
        atkCol = atkTrigger.GetComponent<BoxCollider2D>();
        prePos = transform.position;
        
    }
    void Start()
    {
        dicSkill[KeyCode.Mouse0] = new SkillInfo(SkillInfo.SkillType.atk,"����", attackPower, attackBack);
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
        MoveTrigger();

        inputX = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && jumpCount>0)
        {

            jumpPressed = true;
        }

        foreach (var item in dicSkill)
        {
            if (Input.GetKeyDown(item.Key))
            {
                var info = item.Value;
                switch (info.type)
                {
                    case SkillInfo.SkillType.atk:
                        AtkSkill(info.anim, info.atk, info.atkBack);
                        break;
                    case SkillInfo.SkillType.heal:
                        break;
                    default:
                        break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            isInteracted=true;
        }
    }
    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapBoxAll(transform.position, new Vector2(1.7f, 0.1f), 0, groundLayer).Length!=0;
        isFalling = m_body2d.velocity.y < -0.01;
        GroundMovement();
        Jump();

        SwitchAnim();
    }
    void GroundMovement()
    {
        if (inputX > 0)
        {
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
        }

        else if (inputX < 0)
        {
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
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
        
        if (jumpPressed && (isGround  || graceTimer>0))     //��������
        {
            SetAnimation("��Ծ1",false);        
            isJump = true;
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            jumpCount--;
            jumpPressed = false;
            graceTimer = 0;
        }else if(jumpPressed && jumpCount > 0 && isJump)       //��������
        {
            if (currentAnimation != "��Ծ1")
                preAnimation = currentAnimation;
            SetAnimation("��Ծ1",false);          
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            jumpCount--;
            jumpPressed = false;
        } 
    }
    /// <summary>
    /// ����
    /// </summary>
    /// <param name="atk">������</param>
    /// <param name="atkBack">���������</param>
    /// <returns></returns>
    IEnumerator  Attack(string anim,float atk,float atkBack)
    {      
        isAttack = true;

        atkCol.enabled = true;
        atkTrigger.atk = atk;
        atkTrigger.atkItemBack = atkBack;
        var track = SetAnimation(anim,false);
        track.Complete += (TrackEntry) => { 
            isAttack = false; 
            atkCol.enabled = false;
        };
        

        while (isAttack)
        {
            yield return null;  
        }
        float startTime = Time.time;

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
        if(animation == currentAnimation)
        {
            if(currentTrack != null && !currentTrack.IsComplete) return currentTrack;
            else return null;
        }
        Spine.TrackEntry track = m_skeleton.state.SetAnimation(0, animation, loop);

        track.Complete += Track_Complete;
        currentTrack=track;
        currentAnimation = animation;

        return track;
    }

    private void Track_Complete(Spine.TrackEntry trackEntry)
    {
        
    }

    void SwitchAnim()
    {
        if (isAttack) return;
        //Debug.Log(isFalling);
        if (isFalling)
        {
            SetAnimation("��Ծ2", false);
            //Debug.Log(2);
        }
        else if (isJump)
        {
            SetAnimation("��Ծ1", false);
            //Debug.Log(3);
        }
        else if (inputX!=0)
        {
            SetAnimation("�ܲ�", true);
            //Debug.Log(4);
        }
        else
        {
            SetAnimation("ս������", true);
            //Debug.Log(5);
        }
    }
    void SwitchAnim(string anim)
    {
        if (anim=="����")
        {
            SetAnimation("����", false);
        }
        else if (anim == "��Ծ1")
        {
            SetAnimation("��Ծ1", false);
        }
        else if (anim == "�ܲ�")
        {
            SetAnimation("�ܲ�", true);
        }
        else
        {
            SetAnimation("ս������", true);
        }
    }
    public void ChangeHealth(float amount)
    {
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
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        MeshRenderer mesh= GetComponent<MeshRenderer>();
        mesh.material.SetFloat("_FillPhase", 0.5f);
        float invicibleTimer = 0;
        while (invicibleTimer<invincibleTime)
        {
            yield return null;
            invicibleTimer+=Time.deltaTime;
            mesh.material.SetFloat("_FillPhase", Mathf.Lerp(0.5f,0,invicibleTimer/invincibleTime));
        }
        mesh.material.SetFloat("_FillPhase", 0);
        gameObject.layer = LayerMask.NameToLayer("Player");
    } 
    void MoveTrigger() //��ɫ�ƶ��¼�
    {
        EventCenter.Instance.Invoke<float>(EventName.playerMoveX, transform.position.x - prePos.x);
        prePos=transform.position;
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

        ChangeHealth(-attackPower);

        if (currentHealth <= 0)
        {
            Dead();
        }
        StopCoroutine(Invincible());
        StartCoroutine(Invincible());
    }
    public void AtkSkill(string anim,float atk,float abk)
    {
        if (isAttack)
        {
            return;
        }

        if (curAtkC != null)
            StopCoroutine(curAtkC);
        curAtkC = StartCoroutine(Attack(anim,attackPower, attackBack));
    }
    void Dead()
    {
        SceneManager.LoadScene(0);
    }
    
}