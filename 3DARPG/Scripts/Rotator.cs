using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ӳ���������ת
/// </summary>
public class Rotator : MonoBehaviour
{
    public float speed = 80f;
    void Update()
    {
        //��y����ת����������ϵ
        transform.Rotate(Vector3.up, speed*Time.deltaTime, Space.World);
    }
}
