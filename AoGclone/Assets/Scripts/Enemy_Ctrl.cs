using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ctrl : MonoBehaviour
{
    Vector3 CalcTs = Vector3.zero;
    Vector3 moveDir = Vector3.zero;
    float moveSpeed = 3.5f;
    int Run = 1;
    int temp = 0;

    float Timer = 0.0f;

    int RandNum = 0;

    Game_Mgr Gm_Mgr = null;

    [HideInInspector] public float EnemyCurHp = 1500.0f;
    [HideInInspector] public float EnemyMaxHp = 1500.0f;

    // Start is called before the first frame update
    void Start()
    {
        Gm_Mgr = GameObject.FindObjectOfType<Game_Mgr>();

        EnemyCurHp = 1500.0f;
        EnemyMaxHp = 1500.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        RimitMove();

        Timer -= Time.deltaTime;

        if (Timer <= 0.0f)
        {
            RandNum = Random.Range(0,2);

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
    {
        moveDir = new Vector3(Run, 0.0f, 0.0f);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    void RimitMove()
    {//이동제한 //-2 ~ -8
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
        EnemyCurHp -= Dmg;
        Gm_Mgr.E_HpBar.fillAmount = EnemyCurHp / EnemyMaxHp;
        Gm_Mgr.E_HpText.text = EnemyCurHp.ToString("N0") + " / " + EnemyMaxHp.ToString("N0");
    }

    IEnumerator Skill1()
    {
        temp = Run;
        Run = 0;

        GameObject Obj = Resources.Load("E_ArrowPrefab") as GameObject;
        Instantiate(Obj);
        yield return new WaitForSeconds(0.3f);
        Instantiate(Obj);
        yield return new WaitForSeconds(0.3f);
        Instantiate(Obj);

        Run = -temp;
    }

    IEnumerator Skill2()
    {
        temp = Run;
        Run = 0;

        GameObject Obj = Resources.Load("E_ArrowPrefab") as GameObject;
        GameObject Arrow = Instantiate(Obj);
        Arrow.GetComponent<Transform>().localScale *= 3.0f;
        Arrow.GetComponent<Arrow_Ctrl>().isBig = true;
        yield return new WaitForSeconds(0.3f);

        Run = -temp;
    }
}
