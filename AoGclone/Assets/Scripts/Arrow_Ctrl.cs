using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum ArrowState
{//내가 쏜건지 ai가 쏜건지 구분
    user,
    ai,
    Count
}

public class Arrow_Ctrl : MonoBehaviour
{
    Transform Targetpos;
    Transform Startpos;

    public ArrowState AState = ArrowState.user;

    float angle = 0.0f;

    Rigidbody2D Rb = null;
    GameObject Enemy = null;
    GameObject Player = null;

    [HideInInspector] public bool isBig = false;

    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();

        if (AState == ArrowState.user)
        {//내가 쐈을 때
            Player = GameObject.Find("P_ShotPoint");
            Enemy = GameObject.Find("Enemy");

            Startpos = Player.transform;
            Targetpos = Enemy.transform;
        }
        else if(AState == ArrowState.ai)
        {//ai가 쐈을 때
            Player = GameObject.Find("Player");
            Enemy = GameObject.Find("E_ShotPoint");

            Startpos = Enemy.transform;
            Targetpos = Player.transform;
        }

        //시작 지점 쏜 사람 위치
        transform.position = Startpos.transform.position;

        StartCoroutine(LookForward());

        Destroy(gameObject, 5.0f);
    }

    //Update is called once per frame
    void Update()
    {
        Shot();
    }

    void Shot()
    {
        if (Startpos == null || Targetpos == null)
            return;

        // 화살의 시작 위치에서 목표까지의 거리
        Vector2 start = Startpos.position;
        Vector2 target = Targetpos.position;
        Vector2 distance = target - start;

        float flightTime = 1.5f;
        if (isBig == false)
        {
            flightTime = 1.0f;
        }
        else if (isBig == true)
        { 
            flightTime = 2.5f;
        }

        // 수평 속도 = 거리 / 시간
            float vx = distance.x / flightTime;

        // 수직 속도 = (거리 / 시간) + (중력 보정)
        float vy = (distance.y / flightTime) - (0.5f * Physics2D.gravity.y * flightTime);

        // 초기 속도 벡터 설정
        Vector2 velocity = new Vector2(vx, vy);

        // Rigidbody2D에 속도 적용
        Rb.velocity = velocity;

        enabled = false;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (this.gameObject.tag != "Enemy")
        {
            if (coll.gameObject.tag == "Enemy")
            {
                Destroy(gameObject);
            }
        }

        if (coll.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }

        if (coll.gameObject.tag == "Player")
        {
            if (isBig == true)
            {
                coll.GetComponent<Player_Ctrl>().HpUpdate(300.0f);
                Destroy(gameObject);
            }
            else if (isBig == false)
            {
                coll.GetComponent<Player_Ctrl>().HpUpdate(50.0f);
                Destroy(gameObject);
            }
        }

        if (coll.gameObject.tag == "Enemy")
        {
            if (isBig == true)
            {
                coll.GetComponent<Enemy_Ctrl>().E_HpUpdate(300.0f);
                Destroy(gameObject);
            }
            else if (isBig == false)
            {
                coll.GetComponent<Enemy_Ctrl>().E_HpUpdate(50.0f);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator LookForward()
    {
        while (true)
        {
            angle = Mathf.Atan2(Rb.velocity.y, Rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }
}
