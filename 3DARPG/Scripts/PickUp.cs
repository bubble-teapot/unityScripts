using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType
    {
        Heal,Coin
    }

    public PickUpType type;
    public int value = 20;

    public ParticleSystem CollectedVFX;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //����ײ���Ķ��󣬼����Լ�
            other.GetComponent<Character>().PickUpItem(this);
            //�����Ч��Ϊ�գ�����������Ч
            if (CollectedVFX != null)
            {
                Instantiate(CollectedVFX, transform.position, Quaternion.identity);
            }

            //�����Լ�
            Destroy(gameObject);
        }
    }
}
