using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制硬币物体的旋转
/// </summary>
public class Rotator : MonoBehaviour
{
    public float speed = 80f;
    void Update()
    {
        //绕y轴旋转，世界坐标系
        transform.Rotate(Vector3.up, speed*Time.deltaTime, Space.World);
    }
}
