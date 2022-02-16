using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHpbar : MonoBehaviour
{
    [SerializeField] Slider hpbar;
    public GameObject boss;
    public float maxHp = 3000;

    void Awake()
    {
        hpbar.value = 1;
    }
    public void SetHp()
    {
        hpbar.value = 1;
    }

    public void HandleHp(float curHp)
    {
        hpbar.value = (float)curHp / (float)maxHp;
    }
}
