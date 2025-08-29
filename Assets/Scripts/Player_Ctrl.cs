using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum P_State
{
    Play,
    Die,
    Win,
    Count
}

public class Player_Ctrl : MonoBehaviour
{
    [HideInInspector] public P_State CurPState = P_State.Play;  //플레이어 상태 초기 설정

    Game_Mgr Gm_Mgr = null;                                     //게임 메니저 참조

    float h = 0.0f;                                             //이동 변수

    float moveSpeed = 5.0f;                                     //이동 속도
    Vector3 moveDir = Vector3.zero;                             //이동 방향
    Vector3 CalcTs = Vector3.zero;                              //이동 범위 제한용 벡터

    public GameObject Body = null;                              //몸통
    GameObject Arrow = null;                                    //생성할 화살

    bool isMove = false;                                        //이동 중 상태 판정
    bool isSkill = false;                                       //스킬 사용 중 상태 판정

    float AtkCool = 0.0f;                                       //평타 딜레이 계산용 변수
    float CalcAtkCool = 2.0f;                                   //평타 딜레이 값

    [HideInInspector]public bool isShield = false;                                      //쉴드 체크
    public GameObject Shield;                                   //쉴드 오브젝트
    float Shield_Time = 3.0f;                                   //쉴드 지속시간
    float CurSh_Time = 0.0f;                                    //쉴드 시간 계산용
    public GameObject Shield_Icon;                              //쉴드 아이콘
    public Image Shield_Dur;                                    //지속시간 표시용

    bool isHaste = false;                                       //헤이스트 체크
    float Haste_Time = 3.0f;                                    //헤이스트 지속시간
    float CurHs_Time = 0.0f;                                    //헤이스트 시간 계산용
    public GameObject Haste_Icon;                               //쉴드 아이콘
    public Image Haste_Dur;                                     //지속시간 표시용

    bool isTurret = false;                                      //터렛 체크
    float Turret_Timer = 5.0f;                                  //지속시간
    float CurTurret_Timer = 0.0f;                               //지속시간 계산용
    public GameObject Turret_Icon;                              //포탑 아이콘
    public Image Turret_Dur;                                    //지속시간 표시용

    [HideInInspector] public float HeroCurHp = 1500.0f;         //체력
    [HideInInspector] public float HeroMaxHp = 1500.0f;         //최대체력

    [HideInInspector] public float CalcSk1Cool = 0.0f;          //스킬1 계산용 변수
    [HideInInspector] public float CalcSk2Cool = 0.0f;          //스킬2 계산용 변수
    [HideInInspector] public float CalcSk3Cool = 0.0f;          //스킬3 계산용 변수
    [HideInInspector] public float CalcSk4Cool = 0.0f;          //스킬4 계산용 변수

    [HideInInspector] public float Sk1Cool = 7.0f;              //스킬1 쿨
    [HideInInspector] public float Sk2Cool = 8.0f;              //스킬2 쿨
    [HideInInspector] public float Sk3Cool = 20.0f;             //스킬3 쿨
    [HideInInspector] public float Sk4Cool = 30.0f;             //스킬4 쿨

    Animator m_Anim;                                            //캐릭터 애니메이터
    public Canvas DmgCanvas;                                    //데미지 출력용 캔버스

    // Start is called before the first frame update
    void Start()
    {
        HeroCurHp = 1500.0f;
        HeroMaxHp = 1500.0f;
        Gm_Mgr = GameObject.FindObjectOfType<Game_Mgr>();

        CurPState = P_State.Play;

        m_Anim = GetComponentInChildren<Animator>();

        AtkCool = CalcAtkCool;

        Sk1Cool = 7.0f;
        Sk2Cool = 8.0f;
        Sk3Cool = 20.0f;
        Sk4Cool = 30.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurPState == P_State.Play)
        {
            Move();
            RimitMove();
            Shield_Check();
            Haste_Check();
            Turret_Check();
            if (isMove == false && isSkill == false)
            {//이동, 스킬 사용 아닐 때 기본 공격 발동
                Atk();
            }
        }
        else if (CurPState == P_State.Die)
        {//사망 애니메이션
            m_Anim.Play("die");
            return;
        }
        else
        {//상대방을 죽이면 승리 애니메이션 재생
            m_Anim.Play("victory");
        }
    }

    void Shield_Check()
    {//방어막 on/off
        if (isShield == false)
        {
            Shield.gameObject.SetActive(false);
            Shield_Icon.gameObject.SetActive(false);
            CurSh_Time = Shield_Time;
        }
        else if (isShield == true)
        {
            Shield.gameObject.SetActive(true);
            Shield_Icon.gameObject.SetActive(true);
            CurSh_Time -= Time.deltaTime;

            Shield_Dur.fillAmount = CurSh_Time / Shield_Time;

            if (CurSh_Time <= 0.0f)
            {
                isShield = false;
            }
        }
    }

    void Haste_Check()
    {//헤이스트 on/off
        if (isHaste == false)
        {
            Haste_Icon.gameObject.SetActive(false);
            moveSpeed = 5.0f;
            CurHs_Time = Haste_Time;
        }
        else if (isHaste == true)
        {
            Haste_Icon.gameObject.SetActive(true);

            moveSpeed = 10.0f;

            CurHs_Time -= Time.deltaTime;

            Haste_Dur.fillAmount = CurHs_Time / Haste_Time;

            if (CurHs_Time <= 0.0f)
            {
                isHaste = false;
            }
        }
    }

    void Turret_Check()
    {//포탑 확인용
        if (isTurret == false)
        {
            Turret_Icon.SetActive(false);
            CurTurret_Timer = Turret_Timer;
        }
        else if (isTurret == true)
        {
            Turret_Icon.SetActive(true);

            CurTurret_Timer -= Time.deltaTime;

            Turret_Dur.fillAmount = CurTurret_Timer / Turret_Timer;

            if (CurTurret_Timer <= 0.0f)
            {
                isTurret = false;
            }
        }
    }

    void Move()
    {//이동
        h = 0.0f;

        if (Gm_Mgr.isRightClick == false && Gm_Mgr.isLeftClick == false)
        {//버튼 안누를 때 제자리 판정 후 리턴
            isMove = false;
            Body.gameObject.transform.localScale = new Vector3(1, 1, 1);
            return;
        }
        else if (Gm_Mgr.isRightClick == true)
        {
            isMove = true;
            h = 1.0f;
        }
        else if (Gm_Mgr.isLeftClick == true)
        {
            isMove = true;
            h = -1.0f;
        }

        if (h != 0.0f)
        {//버튼 입력 시 이동
            if (isSkill == false)
            {//스킬 사용중엔 이동 x
                m_Anim.Play("walk");
                Body.gameObject.transform.localScale = new Vector3(h, 1, 1);
                moveDir = new Vector3(h, 0.0f, 0.0f);

                if (1.0f < moveDir.magnitude)
                    moveDir.Normalize();

                transform.position += moveDir * moveSpeed * Time.deltaTime;
            }
        }
    }

    void RimitMove()
    {//이동제한 //-2 ~ -8
        CalcTs = transform.position;

        if (transform.position.x <= -8.0f)
        {
            CalcTs = new Vector3(-8.0f, CalcTs.y, CalcTs.z);
        }
        else if (-2.0f <= transform.position.x)
        {
            CalcTs = new Vector3(-2.0f, CalcTs.y, CalcTs.z);
        }
        else
        {
            return;
        }

        transform.position = CalcTs;
    }

    public void HpUpdate(float Dmg)
    {//캐릭터 체력 업데이트
        if (HeroCurHp <= 0.0f)
        {
            HeroCurHp = 0.0f;
            return;
        }

        if(Dmg != 0)
            TextGen((-Dmg).ToString("N0"));

        HeroCurHp -= Dmg;

        if (HeroCurHp <= 0.0f)
        {
            HeroCurHp = 0.0f;
            CurPState = P_State.Die;
        }

        Gm_Mgr.HpBar.fillAmount = HeroCurHp / HeroMaxHp;
        Gm_Mgr.HpText.text = HeroCurHp.ToString("N0") + " / " + HeroMaxHp.ToString("N0");
    }

    void Atk()
    {//기본공격
        AtkCool -= Time.deltaTime;

        if (AtkCool <= 0.0f)
        {
            if (isMove == false && isSkill == false)
            {
                m_Anim.SetFloat("atkSpeed", 1.0f);
                m_Anim.Play("attack");
                GameObject Obj = Resources.Load("ArrowPrefab") as GameObject;
                if (m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f &&
                m_Anim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
                {
                    Instantiate(Obj);
                    AtkCool = CalcAtkCool;
                }
            }
        }
        else
        {//이동, 스킬 사용, 공격중 모두 아닐때는 아이들 상태
            if (isMove == false && isSkill == false)
            {
                m_Anim.Play("idle");
            }
        }
    }

    public void UseSkill(SkillType a_SkType)
    {
        if (HeroCurHp <= 0.0f)
        {
            return;
        }

        if (a_SkType == SkillType.Skill_0)
        {//3연발
            if (0.0f < CalcSk1Cool || isSkill == true)
            {
                return;
            }

            CalcSk1Cool = Sk1Cool;
            m_Anim.SetFloat("atkSpeed", 3.0f);
            m_Anim.Play("Skill");
            StartCoroutine(Skill1());
        }
        else if (a_SkType == SkillType.Skill_1)
        {//대형화살
            if (0.0f < CalcSk2Cool || isSkill == true)
            {
                return;
            }

            CalcSk2Cool = Sk2Cool;
            m_Anim.SetFloat("atkSpeed", 1.0f);
            m_Anim.Play("Skill");
            StartCoroutine(Skill2(0.6f));
        }
        else if (a_SkType == SkillType.Skill_2)
        {
            if (0.0f < CalcSk3Cool || isSkill == true)
            {
                return;
            }

            CalcSk3Cool = Sk3Cool;

            m_Anim.SetFloat("atkSpeed", 1.0f);
            m_Anim.Play("Skill");

            StartCoroutine(Skill3(0.6f));
        }
        else if (a_SkType == SkillType.Skill_3)
        {
            if (0.0f < CalcSk4Cool || isSkill == true)
            {
                return;
            }

            CalcSk4Cool = Sk4Cool;

            m_Anim.Play("casting");
            StartCoroutine(Skill4(0.9f));
        }
    }

    IEnumerator Skill1()
    {//0.3초 텀으로 3발 발사
        isSkill = true;

        GameObject Obj = Resources.Load("ArrowPrefab") as GameObject;

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.3f);
            Instantiate(Obj);
        }
        m_Anim.Play("idle");

        isSkill = false;
    }

    IEnumerator Skill2(float delay)
    {//대형화살 발사
        isSkill = true;

        yield return new WaitForSeconds(delay);

        GameObject Obj = Resources.Load("ArrowPrefab") as GameObject;

        Arrow = Instantiate(Obj);
        Arrow.GetComponent<Transform>().localScale *= 3.0f;
        Arrow.GetComponent<Arrow_Ctrl>().isBig = true;

        isSkill = false;
    }

    IEnumerator Skill3(float delay)
    {//화살비
        isSkill = true;

        yield return new WaitForSeconds(delay);

        GameObject Obj = Resources.Load("ArrowPrefab") as GameObject;
        Arrow = Instantiate(Obj);
        Arrow.GetComponent<Arrow_Ctrl>().AState = ArrowState.Arrowrain1;

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 15; i++)
        {
            Arrow = Instantiate(Obj);
            Arrow.GetComponent<Arrow_Ctrl>().AState = ArrowState.Arrowrain2;
            yield return new WaitForSeconds(0.01f);
        }

        isSkill = false;
    }

    IEnumerator Skill4(float delay)
    {//랜덤 난사 터렛
        isSkill = true;

        yield return new WaitForSeconds(delay);

        isTurret = true;
        GameObject Obj = Resources.Load("Turret") as GameObject;
        Instantiate(Obj);

        isSkill = false;
    }

    void TextGen(string Msg = "")
    {//데미지, 버프 출력
        int TestNum;
        
        GameObject NumText = Resources.Load("Dmg_Text") as GameObject;
        GameObject DmgText = Instantiate(NumText, DmgCanvas.transform);
        DmgText.GetComponent<Text>().text = Msg;

        TestNum = int.TryParse(Msg, out TestNum) ? TestNum : 0;

        if (0 < TestNum)
        {
            DmgText.GetComponent<Text>().color = Color.green;
        }
        else if (TestNum < 0)
        {
            DmgText.GetComponent<Text>().color = Color.red;
        }
        else
        {
            DmgText.GetComponent<Text>().color = Color.white;
        } 

        Destroy(DmgText, 1.0f);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Buff")
        {//버프
            if (coll.gameObject.GetComponent<Buff_Ctrl>().bf == Buff.heal)
            {
                if (1500.0f <= HeroCurHp)
                {
                    Destroy(coll.gameObject);
                }
                else
                {
                    HpUpdate(-25.0f);
                    Destroy(coll.gameObject);
                } 
            }
            else if (coll.gameObject.GetComponent<Buff_Ctrl>().bf == Buff.haste)
            {
                isHaste = true;
                TextGen("가속");
                Destroy(coll.gameObject);
            }
            else if (coll.gameObject.GetComponent<Buff_Ctrl>().bf == Buff.shield)
            {
                isShield = true;
                TextGen("보호막");
                Destroy(coll.gameObject);
            }
        }
    }
}