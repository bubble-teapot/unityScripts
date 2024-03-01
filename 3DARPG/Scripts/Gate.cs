using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject GateVisual;
    private Collider _gateCollider;
    public float OpenDuration = 2f;
    //�Ŵ�ʱy������
    public float OpenTargetY = -1.5f;
    private void Awake()
    {
        _gateCollider = GetComponent<BoxCollider>();
    }
    /// <summary>
    /// ���ŵĶ���
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenGateAnimation()
    {
        float currentOpenDuration =0;
        Vector3 startPos = GateVisual.transform.position;
        Vector3 targetPos = startPos + Vector3.up * OpenTargetY;
        //��OpenDuration��ʱ���ڣ��ƶ���
        while (currentOpenDuration < OpenDuration)
        {
            currentOpenDuration += Time.deltaTime;
            GateVisual.transform.position = Vector3.Lerp(startPos, targetPos, currentOpenDuration / OpenDuration);
            yield return null;
        }
        _gateCollider.enabled = false;
    }

    /// <summary>
    /// ������Ϊspawner��ĳ�¼�����
    /// </summary>
    public void Open()
    {
        StartCoroutine(OpenGateAnimation());
    }
}
