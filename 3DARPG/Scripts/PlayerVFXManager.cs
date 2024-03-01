using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    //�Ų�VFX��Ч
    public VisualEffect footStep;
    //����������Ч
    public ParticleSystem Blade01;
    public ParticleSystem Blade02;
    public ParticleSystem Blade03;

    //������Ч
    public VisualEffect Slash;
    //��Ѫ��Ч
    public VisualEffect Heal;


    /// <summary>
    /// ����footStep��Ч�Ƿ�ķ���
    /// </summary>
    /// <param name="state"></param>
    public void Update_FootStep(bool state)
    {
        if (state)
        {
            footStep.Play();
        }
        else
        {
            footStep.Stop();
        }
    }

    /// <summary>
    /// ���ŵ�����¼�����
    /// </summary>
    public void PlayBlade01()
    {
        Blade01.Play();
    }
    public void PlayBlade02()
    {
        Blade02.Play();
    }
    public void PlayBlade03()
    {
        Blade03.Play();
    }
    /// <summary>
    /// ��ǰ��ֹ�ӿ���Ч
    /// </summary>
    public void StopBlade()
    {
        //��0s��ģ������ϵͳ�����ǲ���ʾ
        Blade01.Simulate(0);
        //ֹͣ����ϵͳ
        Blade01.Stop();

        Blade02.Simulate(0);
        Blade02.Stop();

        Blade03.Simulate(0);
        Blade03.Stop();
    }

    /// <summary>
    /// ����Slash�ķ��������ⲿ����
    /// </summary>
    /// <param name="pos">Slash��λ��</param>
    public void PlaySlash(Vector3 pos)
    {
        Slash.transform.position = pos;
        Slash.Play();
    }

    public void PlayHealVFX()
    {
        Heal.SendEvent("OnPlay");
        //����ʹ�� Heal.Play();
    }
}
