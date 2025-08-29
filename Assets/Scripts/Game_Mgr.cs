using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SkillType
{//스킬 종류 구분
    Skill_0 = 0,
    Skill_1,        
    Skill_2,        
    Skill_3,        
    Skill_4,        
    Skill_5,        
    Count
}

public class Game_Mgr : MonoBehaviour
{
    public SkillType m_SkType = SkillType.Skill_0;
    
    [Header("MoveBtn")]
    public Button LeftBtn = null;
    public Button RIghtBtn = null;

    [Header("SkillBtn")]
    public Button[] Skill_Btn;
    public Text[] Skill_Cool;
    public Image[] Cool_Img;
    
    [Header("HpImg")]
    public Image HpBar;
    public Text HpText;

    [Header("E_HpImg")]
    public Image E_HpBar;
    public Text E_HpText;

    [Header("EndPanel")]
    public GameObject EndPanel;
    public Text EndText;
    public Button Restart_Btn;

    [HideInInspector] public bool isRightClick = false;
    [HideInInspector] public bool isLeftClick = false;

    Player_Ctrl m_RefPlayer = null;
    Enemy_Ctrl m_RefEnemy = null;

    float BuffTime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        isRightClick = false;
        isLeftClick = false;
        m_RefPlayer = GameObject.FindObjectOfType<Player_Ctrl>();
        m_RefEnemy = GameObject.FindObjectOfType<Enemy_Ctrl>();

        BuffTime = 10.0f;

        EndPanel.SetActive(false);

        m_RefPlayer.HpUpdate(0.0f);
        m_RefEnemy.E_HpUpdate(0.0f);

        if (Skill_Btn[0] != null)
        {//실행기 만들기
            Skill_Btn[0].onClick.AddListener(() =>
            {
                UseSill_Key(SkillType.Skill_0);
            });
        }
        if (Skill_Btn[1] != null)
        {
            Skill_Btn[1].onClick.AddListener(() =>
            {
                UseSill_Key(SkillType.Skill_1);
            });
        }
        if (Skill_Btn[2] != null)
        {
            Skill_Btn[2].onClick.AddListener(() =>
            {
                UseSill_Key(SkillType.Skill_2);
            });
        }
        if (Skill_Btn[3] != null)
        {
            Skill_Btn[3].onClick.AddListener(() =>
            {
                UseSill_Key(SkillType.Skill_3);
            });
        }
        if (Restart_Btn != null)
        {
            Restart_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        CoolCalc();
        GameEnd();
        BuffGen();
    }

    void BuffGen()
    {
        BuffTime -= Time.deltaTime;
        if (BuffTime <= 0.0f)
        {
            BuffTime = 0.0f;

            GameObject Obj = Resources.Load("BuffPrefab") as GameObject;
            GameObject buff = Instantiate(Obj);
            buff.transform.position = new Vector3(Random.Range(-8.0f, - 2.0f),1,0);
            BuffTime = 10.0f;
        }
    }

    void GameEnd()
    {//둘중 하나 체력 0이되면 화면 가리고 승리자 표시
        if (m_RefEnemy.CurEState == E_State.Die)
        {
            m_RefPlayer.CurPState = P_State.Win;
            EndPanel.SetActive(true);
            EndText.text = "플레이어 승리";
        }
        if (m_RefPlayer.CurPState == P_State.Die)
        {
            m_RefEnemy.CurEState = E_State.Win;
            EndPanel.SetActive(true);
            EndText.text = "AI 승리";
        }
    }

    void CoolCalc()
    {//스킬 아이콘 이미지, 쿨타임 생성
        if (m_RefPlayer.CalcSk1Cool <= 0.0f)
        {
            m_RefPlayer.CalcSk1Cool = 0.0f;
            Skill_Cool[0].gameObject.SetActive(false);
            Cool_Img[0].gameObject.SetActive(false);
        }
        else
        {
            m_RefPlayer.CalcSk1Cool -= Time.deltaTime;
            Skill_Cool[0].text = m_RefPlayer.CalcSk1Cool.ToString("N0");
            Skill_Cool[0].gameObject.SetActive(true);
            Cool_Img[0].gameObject.SetActive(true);
            Cool_Img[0].fillAmount = m_RefPlayer.CalcSk1Cool / m_RefPlayer.Sk1Cool;
        }

        if (m_RefPlayer.CalcSk2Cool <= 0.0f)
        {
            m_RefPlayer.CalcSk2Cool = 0.0f;
            Skill_Cool[1].gameObject.SetActive(false);
            Cool_Img[1].gameObject.SetActive(false);
        }
        else
        {
            m_RefPlayer.CalcSk2Cool -= Time.deltaTime;
            Skill_Cool[1].text = m_RefPlayer.CalcSk2Cool.ToString("N0");
            Skill_Cool[1].gameObject.SetActive(true);
            Cool_Img[1].gameObject.SetActive(true);
            Cool_Img[1].fillAmount = m_RefPlayer.CalcSk2Cool / m_RefPlayer.Sk2Cool;
        }

        if (m_RefPlayer.CalcSk3Cool <= 0.0f)
        {
            m_RefPlayer.CalcSk3Cool = 0.0f;
            Skill_Cool[2].gameObject.SetActive(false);
            Cool_Img[2].gameObject.SetActive(false);
        }
        else
        {
            m_RefPlayer.CalcSk3Cool -= Time.deltaTime;
            Skill_Cool[2].text = m_RefPlayer.CalcSk3Cool.ToString("N0");
            Skill_Cool[2].gameObject.SetActive(true);
            Cool_Img[2].gameObject.SetActive(true);
            Cool_Img[2].fillAmount = m_RefPlayer.CalcSk3Cool / m_RefPlayer.Sk3Cool;
        }

        if (m_RefPlayer.CalcSk4Cool <= 0.0f)
        {
            m_RefPlayer.CalcSk4Cool = 0.0f;
            Skill_Cool[3].gameObject.SetActive(false);
            Cool_Img[3].gameObject.SetActive(false);
        }
        else
        {
            m_RefPlayer.CalcSk4Cool -= Time.deltaTime;
            Skill_Cool[3].text = m_RefPlayer.CalcSk4Cool.ToString("N0");
            Skill_Cool[3].gameObject.SetActive(true);
            Cool_Img[3].gameObject.SetActive(true);
            Cool_Img[3].fillAmount = m_RefPlayer.CalcSk4Cool / m_RefPlayer.Sk4Cool;
        }
    }

    public void LeftDown()
    {
        isLeftClick = true;
    }
    public void LeftUp()
    {
        isLeftClick = false;
    }

    public void RIghtDown()
    {
        isRightClick = true;
    }
    public void RIghtUp()
    {
        isRightClick = false;
    }

    void UseSill_Key(SkillType a_SkType)
    {
        if (m_RefPlayer == null)
            return;

        m_RefPlayer.UseSkill(a_SkType);
    }
}
