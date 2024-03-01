using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    //伤害区域的碰撞器
    private Collider _damageCasterCollider;
    //攻击力
    public int Damage = 30;
    public string TargetTag;
    //存储已经伤害过的目标对象
    private List<Collider> _damageTargetList;
    private void Awake()
    {
        _damageCasterCollider = GetComponent<Collider>();
        //初始化禁用碰撞器
        _damageCasterCollider.enabled = false;
        _damageTargetList = new List<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TargetTag && !_damageTargetList.Contains(other))
        {
            //获取目标的 Character脚本
            Character targetCC = other.GetComponent<Character>();
            if (targetCC != null)
            {
                //让目标受伤
                targetCC.ApplyDamage(Damage,transform.parent.position);
                //获取父亲的VFX管理器
                PlayerVFXManager playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();
                //以下是播放特效，先获取特效位置，然后播放
                if (playerVFXManager != null)
                {
                    RaycastHit hit;
                    Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;
                    bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation,
                        _damageCasterCollider.bounds.extents.z, 1 << 6);
                    if (isHit)
                    {
                        //播放位置往上抬0.5f
                        playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }
                }


            }
            //记录已经攻击的目标
            _damageTargetList.Add(other);
        }
    }
    /// <summary>
    /// 启动伤害投掷器，作为动画帧的事件方法，在攻击开始的时候调用
    /// 但是动画帧事件函数被识别，只能写在 挂载了使用需要的Animator的对象上 的脚本
    /// 所以，在那个脚本（Charactor脚本）上再创建一个一样名字的方法，然后调用此方法就可以执行
    /// </summary>
    public void EnableDamageCaster()
    {
        //清空攻击目标列表
        _damageTargetList.Clear();
        //启动碰撞器
        _damageCasterCollider.enabled = true;
    }

    /// <summary>
    /// 关闭伤害投掷器，作为动画帧的事件方法，
    /// </summary>
    public void DisableDamageCaster()
    {
        _damageTargetList.Clear();
        //关闭碰撞器
        _damageCasterCollider.enabled = false;
    }

    /// <summary>
    /// 在场景测试的自带函数
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_damageCasterCollider == null)
        {
            _damageCasterCollider = GetComponent<Collider>();
        }
        //用来保存射线返回信息
        RaycastHit hit;
        //射线起始点  用碰撞器的中心点-向前的体积的一半向量  （bounds是体积类型 extent是一半的体积类型为Vector3）
        Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;

        //参数：盒子中心点，盒子各个轴的大小，盒子射的方向，返回的信息，盒形的旋转，
        //发射的最大距离，图层掩码（1<<6 即“二进制0100 0000”即考虑第6个位置的图层，图层从0开始）
        bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents /2, transform.forward, out hit, transform.rotation,
            _damageCasterCollider.bounds.extents.z, 1 << 6);

        //如果碰撞了
        if (isHit)
        {
            //设置Gizmos的颜色为红色
            Gizmos.color = Color.red;
            //画出一个线条球形，参数：原点，半径
            Gizmos.DrawWireSphere(hit.point, 0.3f);
        }
    }
}
