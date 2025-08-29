using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArrowState
{//발사 방식 구분
    user,
    ai,
    turret,
    Arrowrain1,
    Arrowrain2,
    Count
}

public class Arrow_Ctrl : MonoBehaviour
{ 
    Vector3 Targetpos;                              //발사 목표
    Vector3 Startpos;                               //시작 위치

    public ArrowState AState = ArrowState.user;     //기본적인 상태 구분

    float angle = 0.0f;                             //화살이 쳐다보는 각도

    Rigidbody2D Rb = null;                      
    GameObject Enemy = null;                        //발사 목표 오브젝트
    GameObject Player = null;                       //시작 위치 오브젝트

    AudioSource Arrow_Audio;

    [HideInInspector] public bool isBig = false;    //대형 화살 구분

    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();

        Arrow_Audio = GetComponent<AudioSource>();

        if (AState == ArrowState.user)
        {//내가 쐈을 때
            Player = GameObject.Find("P_ShotPoint");
            Enemy = GameObject.Find("Enemy");

            Startpos = Player.transform.position;
            Targetpos = Enemy.transform.position;
        }
        else if (AState == ArrowState.ai)
        {//ai가 쐈을 때
            Player = GameObject.Find("Player");
            Enemy = GameObject.Find("E_ShotPoint");

            Startpos = Enemy.transform.position;
            Targetpos = Player.transform.position;
        }
        else if (AState == ArrowState.turret)
        {//포탑 화살
            Player = GameObject.Find("Turret(Clone)");

            Startpos = Player.transform.position;
            Targetpos = new Vector3(Random.Range(2.0f, 9.0f), 0, 0);
        }
        else if (AState == ArrowState.Arrowrain1)
        {//화살비 트리거
            Player = GameObject.Find("Player");
            Enemy = GameObject.Find("RainPoint");

            Startpos = Player.transform.position;
            Targetpos = Enemy.transform.position;
        }
        else if (AState == ArrowState.Arrowrain2)
        {//화살비 본체
            Player = GameObject.Find("RainPoint");

            Startpos = Player.transform.position;
            Targetpos = new Vector3(Random.Range(2.0f, 9.0f), 0, 0);
        }

        //생성 지점 설정
        transform.position = Startpos;

        StartCoroutine(LookForward());

        //5초뒤 파괴
        Destroy(gameObject, 5.0f);

        //시작할 때 발사 힘 가하기
        Shot();
    }

    //Update is called once per frame
    void Update()
    {
        
    }

    void Shot()
    {
        if (Startpos == null || Targetpos == null)
            return;

        //거리 구하기
        Vector2 start = Startpos;
        Vector2 target = Targetpos;
        Vector2 distance = target - start;

        //비행시간 날아가는 높이 설정용
        float flightTime = 1.5f;
        
        if (isBig == false)
        {
            flightTime = 1.0f;
        }
        else if (isBig == true)
        {//대형 화살은 높게
            flightTime = 2.5f;
        }

        //수평 속도 = 거리 / 시간
            float vx = distance.x / flightTime;

        //수직 속도 = (거리 / 시간) + (중력 보정)
        float vy = (distance.y / flightTime) - (0.5f * Physics2D.gravity.y * flightTime);

        //속도 적용
        Vector2 velocity = new Vector2(vx, vy);
        Rb.velocity = velocity;
    }

    void OnShot()
    {
        GetComponentInChildren<BoxCollider2D>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Ground")
        {
            OnShot();
            Destroy(gameObject, 0.2f);
        }

        if (coll.gameObject.tag == "Player")
        {
            OnShot();
            if (coll.GetComponent<Player_Ctrl>().isShield == false)
            {
                Arrow_Audio.PlayOneShot(Resources.Load("Target Impact - Default (1)") as AudioClip);

                if (isBig == true)
                {//대형화살 데미지 증가
                    coll.GetComponent<Player_Ctrl>().HpUpdate(300.0f);
                    Destroy(gameObject, 0.2f);
                }
                else if (isBig == false)
                {
                    coll.GetComponent<Player_Ctrl>().HpUpdate(50.0f);
                    Destroy(gameObject, 0.2f);
                }
            }
            else
            {
                Arrow_Audio.PlayOneShot(Resources.Load("Target Impact - Armor Layer (1)") as AudioClip);

                Destroy(gameObject, 0.2f);
            }
        }

        if (coll.gameObject.tag == "Enemy")
        {
            OnShot();
            Arrow_Audio.PlayOneShot(Resources.Load("Target Impact - Default (1)") as AudioClip);

            if (isBig == true)
            {//대형화살 데미지 증가
                coll.GetComponent<Enemy_Ctrl>().E_HpUpdate(300.0f);
                Destroy(gameObject, 0.2f);
            }
            else if (isBig == false)
            {
                coll.GetComponent<Enemy_Ctrl>().E_HpUpdate(50.0f);
                Destroy(gameObject, 0.2f);
            }
        }
    }

    IEnumerator LookForward()
    {//진행방향 바라보기
        while (true)
        {//역탄젠트로 z축 회전
            angle = Mathf.Atan2(Rb.velocity.y, Rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }
}