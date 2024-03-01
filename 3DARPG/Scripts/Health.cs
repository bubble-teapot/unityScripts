using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫѪ��
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

    //��ɫ�ű�
    private Character _cc;
    private void Awake()
    {
        CurrentHealth = MaxHealth;
        _cc = GetComponent<Character>();
    }

    public void ApplyDamage(int damage)
    {
        //��Ѫ
        CurrentHealth -= damage;
        //test
        Debug.Log(gameObject.name + "took damage:" + damage);
        Debug.Log(gameObject.name + "curentHealth:" + CurrentHealth);

        CheckHealth();
    }

    /// <summary>
    /// �������ֵ
    /// </summary>
    private void CheckHealth()
    {
        if (CurrentHealth <= 0)
        {
            _cc.SwitchStateTo(Character.CharacterState.Dead);
        }
    }
    /// <summary>
    /// ��������ֵ���������ⲿ����
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
