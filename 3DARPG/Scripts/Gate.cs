using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject GateVisual;
    private Collider _gateCollider;
    public float OpenDuration = 2f;
    //门打开时y的坐标
    public float OpenTargetY = -1.5f;
    private void Awake()
    {
        _gateCollider = GetComponent<BoxCollider>();
    }
    /// <summary>
    /// 打开门的动作
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenGateAnimation()
    {
        float currentOpenDuration =0;
        Vector3 startPos = GateVisual.transform.position;
        Vector3 targetPos = startPos + Vector3.up * OpenTargetY;
        //在OpenDuration的时间内，移动门
        while (currentOpenDuration < OpenDuration)
        {
            currentOpenDuration += Time.deltaTime;
            GateVisual.transform.position = Vector3.Lerp(startPos, targetPos, currentOpenDuration / OpenDuration);
            yield return null;
        }
        _gateCollider.enabled = false;
    }

    /// <summary>
    /// 用来作为spawner的某事件函数
    /// </summary>
    public void Open()
    {
        StartCoroutine(OpenGateAnimation());
    }
}
