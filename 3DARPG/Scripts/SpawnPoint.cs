using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    //要生成的敌人
    public GameObject EnemyToSpawn;

    /// <summary>
    /// 用来在测试阶段，查看生成点的方向和位置
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = transform.position + new Vector3(0, 0.5f, 0);
        Gizmos.DrawWireCube(center,Vector3.one);
        //在正方体向前画一条线
        Gizmos.DrawLine(center, center + transform.forward * 2);
    }
}
