using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Ctrl : MonoBehaviour
{
    Game_Mgr Gm_Mgr = null;

    float h = 0.0f;

    float moveSpeed = 5.0f;
    Vector3 moveDir = Vector3.zero;

    Vector3 CalcTs = Vector3.zero;

    GameObject Enemy = null;
    GameObject Player = null;

    bool isMove = false;
    float AtkCool = 0.0f;

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

    // Start is called before the first frame update
    void Start()
    {
        HeroCurHp = 1500.0f;
        HeroMaxHp = 1500.0f;
        Gm_Mgr = GameObject.FindObjectOfType<Game_Mgr>();

        Enemy = GameObject.Find("Enemy");
        Player = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        RimitMove();
        Atk();

        Debug.Log(isMove);
    }

    void Move()
    {//이동
        h = Input.GetAxis("Horizontal");

        if (Gm_Mgr.isRightClick == false && Gm_Mgr.isLeftClick == false)
        {
            isMove = false;
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
        {
            moveDir = new Vector3(h, 0.0f, 0.0f);
            if (1.0f < moveDir.magnitude)
                moveDir.Normalize();

            transform.position += moveDir * moveSpeed * Time.deltaTime;
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
        HeroCurHp -= Dmg;
        Gm_Mgr.HpBar.fillAmount = HeroCurHp / HeroMaxHp;
        Gm_Mgr.HpText.text = HeroCurHp.ToString("N0") + " / " + HeroMaxHp.ToString("N0");
    }

    void Atk()
    {
        AtkCool -= Time.deltaTime;

        if (AtkCool <= 0.0f)
        {
            if (isMove == false)
            {
                GameObject Obj = Resources.Load("ArrowPrefab") as GameObject;
                Instantiate(Obj);
            }
            else
            {
                AtkCool = 1.0f;
            }
            AtkCool = 1.0f;
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
            StartCoroutine(Skill1());
        }
        else if (a_SkType == SkillType.Skill_1)
        {//대형화살
            if (0.0f < CalcSk2Cool)
                return;

            CalcSk2Cool = Sk2Cool;
            GameObject Obj = Resources.Load("ArrowPrefab") as GameObject;
            GameObject Arrow = Instantiate(Obj);
            Arrow.GetComponent<Transform>().localScale *= 3.0f;
            Arrow.GetComponent<Arrow_Ctrl>().isBig = true;
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
        GameObject Obj = Resources.Load("ArrowPrefab") as GameObject;
        Instantiate(Obj);
        yield return new WaitForSeconds(0.3f);
        Instantiate(Obj);
        yield return new WaitForSeconds(0.3f);
        Instantiate(Obj);
    }
}
