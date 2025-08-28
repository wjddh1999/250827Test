using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum P_State
{
    Play,
    Die,
    Win,
    Count
}

public class Player_Ctrl : MonoBehaviour
{
    [HideInInspector] public P_State CurPState = P_State.Play;

    Game_Mgr Gm_Mgr = null;

    float h = 0.0f;

    float moveSpeed = 5.0f;
    Vector3 moveDir = Vector3.zero;
    public GameObject Body = null;
    GameObject Arrow = null;

    Vector3 CalcTs = Vector3.zero;

    bool isMove = false;
    bool isSkill = false;

    float AtkCool = 0.0f;
    float CalcAtkCool = 2.0f;

    [HideInInspector] public float HeroCurHp = 1500.0f;
    [HideInInspector] public float HeroMaxHp = 1500.0f;

    [HideInInspector] public float CalcSk1Cool = 0.0f;
    [HideInInspector] public float CalcSk2Cool = 0.0f;
    [HideInInspector] public float CalcSk3Cool = 0.0f;
    [HideInInspector] public float CalcSk4Cool = 0.0f;

    [HideInInspector] public float Sk1Cool = 3.0f;
    [HideInInspector] public float Sk2Cool = 4.0f;
    [HideInInspector] public float Sk3Cool = 5.0f;
    [HideInInspector] public float Sk4Cool = 6.0f;

    Animator m_Anim;

    // Start is called before the first frame update
    void Start()
    {
        HeroCurHp = 1500.0f;
        HeroMaxHp = 1500.0f;
        Gm_Mgr = GameObject.FindObjectOfType<Game_Mgr>();

        CurPState = P_State.Play;

        m_Anim = GetComponentInChildren<Animator>();

        AtkCool = CalcAtkCool;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurPState == P_State.Play)
        {
            Move();
            RimitMove();
            if (isMove == false && isSkill == false)
            {//이동, 스킬 사용 아닐 때 기본 공격 발동
                Atk();
            }
        }
        else if (CurPState == P_State.Die)
        {
            return;
        }
        else
        {//상대방을 죽이면 승리 애니메이션 재생
            m_Anim.Play("victory");
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

        HeroCurHp -= Dmg;

        if (HeroCurHp <= 0.0f)
        {
            HeroCurHp = 0.0f;
            CurPState = P_State.Die;
            m_Anim.Play("die");
            Gm_Mgr.HpBar.fillAmount = HeroCurHp / HeroMaxHp;
            Gm_Mgr.HpText.text = HeroCurHp.ToString("N0") + " / " + HeroMaxHp.ToString("N0");
            return;
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
            if (0.0f < CalcSk1Cool)
                return;

            CalcSk1Cool = Sk1Cool;
            m_Anim.SetFloat("atkSpeed", 3.0f);
            m_Anim.Play("Skill");
            StartCoroutine(Skill1());
        }
        else if (a_SkType == SkillType.Skill_1)
        {//대형화살
            if (0.0f < CalcSk2Cool)
                return;

            CalcSk2Cool = Sk2Cool;
            m_Anim.SetFloat("atkSpeed", 1.0f);
            m_Anim.Play("Skill");
            StartCoroutine(Skill2(0.6f));
        }
        else if (a_SkType == SkillType.Skill_2)
        {
            if (0.0f < CalcSk3Cool)
                return;

            CalcSk3Cool = Sk3Cool;
            Debug.Log("스킬3");
        }
        else if (a_SkType == SkillType.Skill_3)
        {
            if (0.0f < CalcSk4Cool)
                return;

            CalcSk4Cool = Sk4Cool;
            Debug.Log("스킬4");
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
}
