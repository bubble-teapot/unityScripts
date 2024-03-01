using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> spawnPointList;
    //������¼�Ѿ����ɵĽ�ɫ �Ľű�
    private List<Character> spawnCharacters;
    private bool hasSpawned;
    private bool allSpawnedAreDead;
    //unity�¼���ͨ���¼����Ե��������ű��ķ��������������ɽ�ɫ������ʱִ��
    public UnityEvent OnAllSpawnedCharacterEliminated;


    //������Gizmo�в���
    private Collider _collider;
    
    private void Awake()
    {
        //��ȡ���ڵ�������ӽڵ��SpawnPoint�ű�
        var spawnPointArray= transform.parent.GetComponentsInChildren<SpawnPoint>();
        //������������ʼ���б�
        spawnPointList = new List<SpawnPoint>(spawnPointArray);
        spawnCharacters = new List<Character>();
    }

    private void Update()
    {
        //�����û���ɻ������ɵ�list����Ϊ0��ֱ�ӷ���
        if (!hasSpawned || spawnCharacters.Count == 0) return;

        allSpawnedAreDead = true;
        //����,����л��ŵĽ�ɫ��allSpawnedAreDead��Ϊfalse����ֹ����
        foreach (Character c in spawnCharacters)
        {
            if (c.CurrentState != Character.CharacterState.Dead)
            {
                allSpawnedAreDead = false;
                break;
            }
        }

        if (allSpawnedAreDead)
        {
            if (OnAllSpawnedCharacterEliminated != null)
            {
                OnAllSpawnedCharacterEliminated.Invoke();
            }
            //����б���ֹ�������������¼�
            spawnCharacters.Clear();
        }
    }

    /// <summary>
    /// ���ɽ�ɫ�ķ���
    /// </summary>
    public void SpawnCharacters()
    {
        if (hasSpawned) return;
        hasSpawned = true;
        //�����б������¼��EnemyToSpawn��Ϊ�գ������������,�ڼ�¼������,������Character�ű������б�
        foreach(SpawnPoint point in spawnPointList)
        {
            GameObject spawnedGameObject = Instantiate(point.EnemyToSpawn, point.transform.position, point.transform.rotation);
            spawnCharacters.Add(spawnedGameObject.GetComponent<Character>());
        }
    }

    /// <summary>
    /// �������¼��������Ҵ������������ɵ���
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpawnCharacters();
        }
    }

    //test
    private void OnDrawGizmos()
    {
        _collider = GetComponent<BoxCollider>();
        Gizmos.color = Color.red;
        //�������������壬λ�ã���С
        Gizmos.DrawWireCube(transform.position, _collider.bounds.size);
    }
}
