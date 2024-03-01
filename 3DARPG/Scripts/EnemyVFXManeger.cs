using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManeger : MonoBehaviour
{
    public VisualEffect FootStep;
    public VisualEffect AttackVFX;
    public ParticleSystem BeingHitVFX;
    //�����н�����Ч
    public VisualEffect BeingHitSplashVFX;

    /// <summary>
    /// NPC 01��Walk�������¼�����
    /// </summary>
    public void BurstFootStep()
    {
        //FootStep.Play();
        //����ʹ�� VFX����ʱ������¼�Event
        FootStep.SendEvent("OnPlay");
    }

    /// <summary>
    /// �����¼�����
    /// </summary>
    public void PlayAttackVFX()
    {
        AttackVFX.SendEvent("OnPlay");
        //����ʹ�� AttackVFX.Play();
    }

    /// <summary>
    /// �����ܻ���Ч
    /// </summary>
    /// <param name="attackerPos"></param>
    public void PlayBeingHitVFX(Vector3 attackerPos)
    {
        //�����ܻ���Ч
        //���ŷ���
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        //ֻ����ˮƽ����
        forceForward.y = 0;
        //�޸�VFX��ת����
        BeingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        BeingHitVFX.Play();

        //���Ž�����Ч
        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        //������Чʵ��
        VisualEffect newSplashVFX = Instantiate(BeingHitSplashVFX, splashPos, Quaternion.identity);
        newSplashVFX.SendEvent("OnPlay");
        //�����꣬�ô���10�룬Ȼ����������������Ϊ��������Чʵ������������Ϸ�����ϵģ�ֻɾ����Ч�������
        Destroy(newSplashVFX.gameObject, 10f);
    }
}
