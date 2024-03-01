using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    //子弹发射的点
    public Transform ShootingPoint;
    public GameObject DamageOrb;
    private Character cc;
    private void Awake()
    {
        cc = GetComponent<Character>();
    }
    private void Update()
    {
        //每帧看向玩家目标
        cc.RotateToTarget();
    }

    /// <summary>
    /// 创建伤害球，在动画帧时间调用
    /// </summary>
    public void ShootTheDamageOrb()
    {
        Instantiate(DamageOrb, ShootingPoint.position, Quaternion.LookRotation(ShootingPoint.forward));
    }
}
