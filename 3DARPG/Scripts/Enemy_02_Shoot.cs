using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    //�ӵ�����ĵ�
    public Transform ShootingPoint;
    public GameObject DamageOrb;
    private Character cc;
    private void Awake()
    {
        cc = GetComponent<Character>();
    }
    private void Update()
    {
        //ÿ֡�������Ŀ��
        cc.RotateToTarget();
    }

    /// <summary>
    /// �����˺����ڶ���֡ʱ�����
    /// </summary>
    public void ShootTheDamageOrb()
    {
        Instantiate(DamageOrb, ShootingPoint.position, Quaternion.LookRotation(ShootingPoint.forward));
    }
}
