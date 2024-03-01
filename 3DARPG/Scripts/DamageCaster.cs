using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    //�˺��������ײ��
    private Collider _damageCasterCollider;
    //������
    public int Damage = 30;
    public string TargetTag;
    //�洢�Ѿ��˺�����Ŀ�����
    private List<Collider> _damageTargetList;
    private void Awake()
    {
        _damageCasterCollider = GetComponent<Collider>();
        //��ʼ��������ײ��
        _damageCasterCollider.enabled = false;
        _damageTargetList = new List<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TargetTag && !_damageTargetList.Contains(other))
        {
            //��ȡĿ��� Character�ű�
            Character targetCC = other.GetComponent<Character>();
            if (targetCC != null)
            {
                //��Ŀ������
                targetCC.ApplyDamage(Damage,transform.parent.position);
                //��ȡ���׵�VFX������
                PlayerVFXManager playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();
                //�����ǲ�����Ч���Ȼ�ȡ��Чλ�ã�Ȼ�󲥷�
                if (playerVFXManager != null)
                {
                    RaycastHit hit;
                    Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;
                    bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation,
                        _damageCasterCollider.bounds.extents.z, 1 << 6);
                    if (isHit)
                    {
                        //����λ������̧0.5f
                        playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }
                }


            }
            //��¼�Ѿ�������Ŀ��
            _damageTargetList.Add(other);
        }
    }
    /// <summary>
    /// �����˺�Ͷ��������Ϊ����֡���¼��������ڹ�����ʼ��ʱ�����
    /// ���Ƕ���֡�¼�������ʶ��ֻ��д�� ������ʹ����Ҫ��Animator�Ķ����� �Ľű�
    /// ���ԣ����Ǹ��ű���Charactor�ű������ٴ���һ��һ�����ֵķ�����Ȼ����ô˷����Ϳ���ִ��
    /// </summary>
    public void EnableDamageCaster()
    {
        //��չ���Ŀ���б�
        _damageTargetList.Clear();
        //������ײ��
        _damageCasterCollider.enabled = true;
    }

    /// <summary>
    /// �ر��˺�Ͷ��������Ϊ����֡���¼�������
    /// </summary>
    public void DisableDamageCaster()
    {
        _damageTargetList.Clear();
        //�ر���ײ��
        _damageCasterCollider.enabled = false;
    }

    /// <summary>
    /// �ڳ������Ե��Դ�����
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_damageCasterCollider == null)
        {
            _damageCasterCollider = GetComponent<Collider>();
        }
        //�����������߷�����Ϣ
        RaycastHit hit;
        //������ʼ��  ����ײ�������ĵ�-��ǰ�������һ������  ��bounds��������� extent��һ����������ΪVector3��
        Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;

        //�������������ĵ㣬���Ӹ�����Ĵ�С��������ķ��򣬷��ص���Ϣ�����ε���ת��
        //����������룬ͼ�����루1<<6 ����������0100 0000�������ǵ�6��λ�õ�ͼ�㣬ͼ���0��ʼ��
        bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents /2, transform.forward, out hit, transform.rotation,
            _damageCasterCollider.bounds.extents.z, 1 << 6);

        //�����ײ��
        if (isHit)
        {
            //����Gizmos����ɫΪ��ɫ
            Gizmos.color = Color.red;
            //����һ���������Σ�������ԭ�㣬�뾶
            Gizmos.DrawWireSphere(hit.point, 0.3f);
        }
    }
}
