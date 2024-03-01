using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public float Speed = 2f;
    public int Damage = 10;
    public ParticleSystem HitVFX;
    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        //移动本物体
        _rb.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character cc = other.gameObject.GetComponent<Character>();
        if (cc != null && cc.IsPlayer)
        {
            cc.ApplyDamage(Damage, transform.position);
        }
        //创建击中特效实例
        Instantiate(HitVFX, transform.position, Quaternion.identity);
        //销毁本物体
        Destroy(gameObject);
    }
}
