using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Ctrl : MonoBehaviour
{
    float Shot_Cool = 0.5f;
    float Cur_Cool = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Cur_Cool -= Time.deltaTime;

        if (Cur_Cool <= 0.0f)
        {
            GameObject Obj = Resources.Load("ArrowPrefab") as GameObject;
            GameObject Arrow = Instantiate(Obj);
            Arrow.GetComponent<Arrow_Ctrl>().AState = ArrowState.turret;

            Cur_Cool = Shot_Cool;
        }
    }
}
