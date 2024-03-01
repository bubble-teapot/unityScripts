using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManeger : MonoBehaviour
{
    public VisualEffect FootStep;
    public VisualEffect AttackVFX;
    public ParticleSystem BeingHitVFX;
    //被击中溅射特效
    public VisualEffect BeingHitSplashVFX;

    /// <summary>
    /// NPC 01的Walk动画的事件函数
    /// </summary>
    public void BurstFootStep()
    {
        //FootStep.Play();
        //或者使用 VFX制作时创造的事件Event
        FootStep.SendEvent("OnPlay");
    }

    /// <summary>
    /// 攻击事件函数
    /// </summary>
    public void PlayAttackVFX()
    {
        AttackVFX.SendEvent("OnPlay");
        //或者使用 AttackVFX.Play();
    }

    /// <summary>
    /// 播放受击特效
    /// </summary>
    /// <param name="attackerPos"></param>
    public void PlayBeingHitVFX(Vector3 attackerPos)
    {
        //播放受击特效
        //播放方向
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        //只考虑水平方向
        forceForward.y = 0;
        //修改VFX旋转方向
        BeingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        BeingHitVFX.Play();

        //播放溅射特效
        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        //创建特效实例
        VisualEffect newSplashVFX = Instantiate(BeingHitSplashVFX, splashPos, Quaternion.identity);
        newSplashVFX.SendEvent("OnPlay");
        //播放完，让存在10秒，然后销毁整个对象，因为创建的特效实例是依附在游戏对象上的，只删除特效组件不行
        Destroy(newSplashVFX.gameObject, 10f);
    }
}
