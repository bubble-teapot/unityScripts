using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    //Ҫ���ɵĵ���
    public GameObject EnemyToSpawn;

    /// <summary>
    /// �����ڲ��Խ׶Σ��鿴���ɵ�ķ����λ��
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = transform.position + new Vector3(0, 0.5f, 0);
        Gizmos.DrawWireCube(center,Vector3.one);
        //����������ǰ��һ����
        Gizmos.DrawLine(center, center + transform.forward * 2);
    }
}
