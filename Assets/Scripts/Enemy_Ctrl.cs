using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_State
{
    Play,
    Die,
    Win,
    Count
}

public class Enemy_Ctrl : MonoBehaviour
{
    [HideInInspector] public E_State CurEState = E_State.Play;

    Vector3 CalcTs = Vector3.zero;
    Vector3 moveDir = Vector3.zero;
    float moveSpeed = 3.5f;
    int Run = 1;
    int temp = 0;
    public GameObject Body = null;

    float Timer = 0.0f;

    int RandNum = 0;

    Game_Mgr Gm_Mgr = null;

    [HideInInspector] public float EnemyCurHp = 1500.0f;
    [HideInInspector] public float EnemyMaxHp = 1500.0f;

    Animator m_Anim;

    public Canvas DmgCanvas;                                    //데미지 출력용 캔버스

    // Start is called before the first frame update
    void Start()
    {
        Gm_Mgr = GameObject.FindObjectOfType<Game_Mgr>();

        EnemyCurHp = 1500.0f;
        EnemyMaxHp = 1500.0f;

        CurEState = E_State.Play;

        Timer = 5.0f;

        m_Anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurEState == E_State.Play)
        {
            Move();
            RimitMove();
            RandomAtk();
        }
        else if (CurEState == E_State.Die)
        {
            m_Anim.Play("die");
            return;
        }
        else
        {
            m_Anim.speed = 1.0f;
            m_Anim.Play("victory");
        }
    }

    void RandomAtk()
    {//스킬 랜덤 사용
        Timer -= Time.deltaTime;

        if (Timer <= 0.0f)
        {
            RandNum = Random.Range(0, 2);

            if (RandNum == 0)
            {
                StartCoroutine(Skill1());
            }
            else if (RandNum == 1)
            {
                StartCoroutine(Skill2());
            }

            Timer = 5.0f;
        }
    }

    void Move()
    {//자동이동
        moveDir = new Vector3(Run, 0.0f, 0.0f);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (Run == 0)
        {
            Body.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            m_Anim.speed = 1.0f;
            m_Anim.Play("walk");
            Body.gameObject.transform.localScale = new Vector3(-Run, 1, 1);
        }
    }

    void RimitMove()
    {//이동제한 //2 ~ 8
        CalcTs = transform.position;

        if (8.0f <= transform.position.x)
        {
            Run = -1;
            CalcTs = new Vector3(8.0f, CalcTs.y, CalcTs.z);
        }
        else if (transform.position.x <= 2.0f)
        {
            Run = 1;
            CalcTs = new Vector3(2.0f, CalcTs.y, CalcTs.z);
        }
        else
        {
            return;
        }

        transform.position = CalcTs;
    }

    public void E_HpUpdate(float Dmg)
    {//캐릭터 체력 업데이트
        if (EnemyCurHp <= 0.0f)
        {
            return;
        }

        EnemyCurHp -= Dmg;

        GameObject NumText = Resources.Load("Dmg_Text") as GameObject;

        if (Dmg < 0.0f)
        {//데미지 받을 때
            GameObject DmgText = Instantiate(NumText, DmgCanvas.transform);
            DmgText.GetComponent<Text>().text = (-Dmg).ToString("N0");
            DmgText.GetComponent<Text>().color = Color.green;
            Destroy(DmgText, 1.0f);
        }
        else if (0.0f < Dmg)
        {//힐 받을 때
            GameObject DmgText = Instantiate(NumText, DmgCanvas.transform);
            DmgText.GetComponent<Text>().text = (-Dmg).ToString("N0");
            DmgText.GetComponent<Text>().color = Color.red;
            Destroy(DmgText, 1.0f);
        }

        if (EnemyCurHp <= 0.0f)
        {
            EnemyCurHp = 0.0f;
            CurEState = E_State.Die;
        }

        Gm_Mgr.E_HpBar.fillAmount = EnemyCurHp / EnemyMaxHp;
        Gm_Mgr.E_HpText.text = EnemyCurHp.ToString("N0") + " / " + EnemyMaxHp.ToString("N0");
    }

    IEnumerator Skill1()
    {//스킬 사용할 때 마다 이동방향 변경
        temp = Run;
        Run = 0;

        GameObject Obj = Resources.Load("E_ArrowPrefab") as GameObject;

        for (int i = 0; i < 3; i++)
        {
            m_Anim.speed = 3.0f;
            m_Anim.Play("attack");
            Instantiate(Obj);
            yield return new WaitForSeconds(0.3f);
        }

        Run = -temp;
    }

    IEnumerator Skill2()
    {//스킬 사용할 때 마다 이동방향 변경
        temp = Run;
        Run = 0;

        m_Anim.speed = 2.0f;
        m_Anim.Play("attack");

        GameObject Obj = Resources.Load("E_ArrowPrefab") as GameObject;
        GameObject Arrow = Instantiate(Obj);
        Arrow.GetComponent<Transform>().localScale *= 3.0f;
        Arrow.GetComponent<Arrow_Ctrl>().isBig = true;
        yield return new WaitForSeconds(0.3f);

        Run = -temp;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        
    }
}
