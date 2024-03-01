using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色血量
/// </summary>
public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;
    public float CurrentHealthPercentage
    {
        get
        {
            return (float)CurrentHealth / MaxHealth;
        }
    }

    //角色脚本
    private Character _cc;
    private void Awake()
    {
        CurrentHealth = MaxHealth;
        _cc = GetComponent<Character>();
    }

    public void ApplyDamage(int damage)
    {
        //扣血
        CurrentHealth -= damage;
        //test
        Debug.Log(gameObject.name + "took damage:" + damage);
        Debug.Log(gameObject.name + "curentHealth:" + CurrentHealth);

        CheckHealth();
    }

    /// <summary>
    /// 检查生命值
    /// </summary>
    private void CheckHealth()
    {
        if (CurrentHealth <= 0)
        {
            _cc.SwitchStateTo(Character.CharacterState.Dead);
        }
    }
    /// <summary>
    /// 增加生命值，用来给外部调用
    /// </summary>
    /// <param name="health"></param>
    public void AddHealth(int health)
    {
        if (CurrentHealth + health >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth += health;
        }
    }
}
