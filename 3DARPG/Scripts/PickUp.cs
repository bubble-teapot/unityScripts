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
            //让碰撞到的对象，捡起自己
            other.GetComponent<Character>().PickUpItem(this);
            //如果特效不为空，创建粒子特效
            if (CollectedVFX != null)
            {
                Instantiate(CollectedVFX, transform.position, Quaternion.identity);
            }

            //销毁自己
            Destroy(gameObject);
        }
    }
}
