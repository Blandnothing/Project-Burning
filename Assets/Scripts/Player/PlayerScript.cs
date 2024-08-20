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
using Spine;
using UnityEngine.Playables;

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
    [SerializeField,Header("����ʱ��")] float graceTime;    
    [SerializeField,Header("����ٶ�")] float m_speed = 4.0f;
    [SerializeField, Header("���ٵ�����ٶ�ʱ��")] float accelerateTime = 0.1f;
    [SerializeField, Header("����ʱ��")] float decelerateTIme = 0.05f;
    Vector2 prePos;       //��ɫ��һ֡��λ��
    //��Ծ
    [JsonProperty] public int maxJumpCount = 1;
    private int jumpCount;          //����Ծ����
    private bool jumpPressed;
    private bool isJump;
    bool isFalling;       //�Ƿ�����
    [SerializeField,Header("��Ծ�����߶�")] float jumpMax = 2.5f;
    [SerializeField, Header("��Ծ����С�߶�")] float jumpMin = 0.5f;
    [SerializeField,Header("��Ծ�ٶ�")] float jumpSpeed = 18;
    [SerializeField,Header("��Ծ�߶ȳ�����Ծ���߶�ʱ�Ľ����ٶ�")] float slowFallSpeed=100f;
    [SerializeField,Header("��Ծ�߶�С����Ծ���߶�ʱ�Ľ����ٶ�")] float FastFallSpeed=200f;
    [SerializeField,Header("���½׶εĽ�����ٶ�")] float fallSpeed=150f;
    [SerializeField,Header("���½׶ε�������ٶ�")] float fallMaxSpeed=24;
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
    [SerializeField] Skill attackInfo;  //�չ���Ϣ
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
        public float healValue;
        public GameObject vfx;

        public SkillInfo(SkillType type, string anim, float attackPower, float attackBack,GameObject vfx=null) : this()
        {
            this.type = type;
            this.anim = anim;
            this.atk = attackPower;
            this.atkBack = attackBack;
            this.vfx = vfx;
        }
        public SkillInfo(SkillType type, string anim, float healValue) : this()
        {
            this.type = type;
            this.anim = anim;
            this.healValue = healValue;
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
        InitState();
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
    /// <summary>
    /// ��ʼ�����״̬����ʼʱ�����¼���ƴͼЧ��ʱʹ��
    /// </summary>
    public void InitState()   
    {
        attackInfo.AddSkill(KeyCode.Mouse0);
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
                        AtkSkill(info.anim, info.atk, info.atkBack,info.vfx);
                        break;
                    case SkillInfo.SkillType.heal:
                        HealSkill(info.anim, info.healValue);
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
        
        if (isAttack)
        {
            m_body2d.velocity = new Vector2(transform.localScale.x * attackSpeed, m_body2d.velocity.y);
        }
        else
        {
            if (inputX > 0)
            {
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
                m_body2d.velocity=new Vector2(Mathf.Min(m_speed * Time.fixedDeltaTime / accelerateTime + m_body2d.velocity.x,m_speed), m_body2d.velocity.y);
            }
            else if (inputX < 0)
            {
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
                m_body2d.velocity=new Vector2(Mathf.Max(-m_speed * Time.fixedDeltaTime / accelerateTime + m_body2d.velocity.x,-m_speed), m_body2d.velocity.y);
            }
            else
            {
                m_body2d.velocity = new Vector2(Mathf.MoveTowards(m_body2d.velocity.x,0,m_speed*Time.fixedDeltaTime/decelerateTIme), m_body2d.velocity.y);
            }
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
            StopCoroutine(IntroJump());
            StartCoroutine(IntroJump());
            jumpCount--;
            jumpPressed = false;
            graceTimer = 0;
        }else if(jumpPressed && jumpCount > 0 && isJump)       //��������
        {
            SetAnimation("��Ծ1",false);
            StopCoroutine(IntroJump());
            StartCoroutine(IntroJump());
            jumpCount--;
            jumpPressed = false;
        } 
    }
    IEnumerator IntroJump()
    {
        float dis = 0;
        float startJumpPos = transform.position.y;
        // move up
        float curJumpMin = jumpMin;
        float curJumpMax = jumpMax;
        float curJumpSpeed = jumpSpeed;
        while (dis <= curJumpMin && m_body2d.velocity.y < curJumpSpeed)
        {
            //if (!CheckUpMove())   //����false˵��ײ��ǽ��������Ծ
            //{
            //    Velocity.y = 0;
            //    isIntroJump = false;
            //    isMove = true;
            //    yield break;
            //}
            //��ȡ��ǰ��ɫ����ڳ�ʼ��Ծʱ�ĸ߶�
            dis = transform.position.y - startJumpPos;
            m_body2d.velocity += 240 * Time.fixedDeltaTime*Vector2.up;
            yield return new WaitForFixedUpdate();
        }
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, curJumpSpeed);
        while (Input.GetButton("Jump") && dis < curJumpMax)
        {
            dis = transform.position.y - startJumpPos;
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, curJumpSpeed);
            yield return new WaitForFixedUpdate();
        }
        // slow down
        while (m_body2d.velocity.y > 0)
        {
            if (dis > jumpMax)
            {
                m_body2d.velocity -= slowFallSpeed * Time.fixedDeltaTime*Vector2.up;
            }
            else
            {
                m_body2d.velocity -= FastFallSpeed* Time.fixedDeltaTime * Vector2.up;
            }
            yield return new WaitForFixedUpdate();
        }
        // fall down
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, 0);
        while (!isGround)
        {
            m_body2d.velocity -= fallSpeed * Vector2.up*Time.fixedDeltaTime;
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, Mathf.Clamp(m_body2d.velocity.y, -fallMaxSpeed, m_body2d.velocity.y));
            yield return new WaitForFixedUpdate();
        }
    }
    /// <summary>
    /// ����
    /// </summary>
    /// <param name="atk">������</param>
    /// <param name="atkBack">���������</param>
    /// <returns></returns>
    IEnumerator  Attack(string anim,float atk,float atkBack,GameObject vfx)
    {      
        isAttack = true;

        atkTrigger.atk = atk;
        atkTrigger.atkItemBack = atkBack;
        atkCol.enabled = true;       
        var track = SetAnimation(anim,false);
        track.Complete += (TrackEntry) => { 
            isAttack = false; 
            atkCol.enabled = false;
        };
        if (vfx!=null)
        {
            track.Event += (e, s) =>
            {
                if (s.Data.Name == "skill")
                {
                    GenerateVfx(vfx,5);
                }
                else if (s.Data.Name=="attack")
                {
                    GenerateVfx(vfx, 3);
                }
            };
        }
        

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

    private void Track_Event(TrackEntry trackEntry, Spine.Event e)
    {
        throw new System.NotImplementedException();
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
    void GenerateVfx(GameObject vfx,float offset)
    {
        GameObject v = Instantiate(vfx, new Vector2(transform.position.x + m_skeleton.skeleton.ScaleX * offset, transform.position.y), Quaternion.identity);
        v.GetComponent<Flyable>().direction = new Vector2(m_skeleton.skeleton.ScaleX, 0);
        v.GetComponent<SpriteRenderer>().flipX = m_skeleton.skeleton.ScaleX<0;
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
        EventCenter.Instance.Invoke<Vector2>(EventName.playerMoveX, (Vector2)transform.position - prePos);
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

        impulseSource.GenerateImpulse(0.5f);
        ChangeHealth(-attackPower);

        if (currentHealth <= 0)
        {
            StopCoroutine(Dead());
            StartCoroutine(Dead());
        }
        StopCoroutine(Invincible());
        StartCoroutine(Invincible());
    }
    public void AtkSkill(string anim,float atk,float abk,GameObject vfx)
    {
        if (isAttack)
        {
            return;
        }

        if (curAtkC != null)
            StopCoroutine(curAtkC);
        curAtkC = StartCoroutine(Attack(anim, atk, abk,vfx));
    }
    public void HealSkill(string anim,float healValue)
    {
        if (anim != "")
        {
            SetAnimation(anim,false);
        }
        ChangeHealth(healValue);
    }
        
    IEnumerator Dead()
    {
        impulseSource.GenerateImpulse(1);
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        EventCenter.Instance.Invoke(EventName.dead);  
        float deadTime = 2;
        float deadTimer = 0;
        while (deadTimer<deadTime)
        {
            deadTimer+=Time.deltaTime;
            m_skeleton.skeleton.SetColor(new Color(1, 1, 1, Mathf.Lerp(1, 0, deadTimer / deadTime)));
            yield return null;
        }
    }
    
}