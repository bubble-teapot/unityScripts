using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    //脚步VFX特效
    public VisualEffect footStep;
    //刀光粒子特效
    public ParticleSystem Blade01;
    public ParticleSystem Blade02;
    public ParticleSystem Blade03;

    //击中特效
    public VisualEffect Slash;
    //回血特效
    public VisualEffect Heal;


    /// <summary>
    /// 控制footStep特效是否的方法
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
    /// 播放刀光的事件函数
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
    /// 提前终止挥砍特效
    /// </summary>
    public void StopBlade()
    {
        //在0s内模拟粒子系统，但是不显示
        Blade01.Simulate(0);
        //停止粒子系统
        Blade01.Stop();

        Blade02.Simulate(0);
        Blade02.Stop();

        Blade03.Simulate(0);
        Blade03.Stop();
    }

    /// <summary>
    /// 播放Slash的方法，在外部调用
    /// </summary>
    /// <param name="pos">Slash的位置</param>
    public void PlaySlash(Vector3 pos)
    {
        Slash.transform.position = pos;
        Slash.Play();
    }

    public void PlayHealVFX()
    {
        Heal.SendEvent("OnPlay");
        //或者使用 Heal.Play();
    }
}
