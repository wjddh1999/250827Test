using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Buff
{
    heal,
    haste,
    shield,
    count
}

public class Buff_Ctrl : MonoBehaviour
{
    [HideInInspector] public Buff bf = Buff.haste;
    // Start is called before the first frame update
    void Start()
    {
        bf = (Buff)Random.Range(0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (bf == Buff.heal)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (bf == Buff.haste)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (bf == Buff.shield)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
