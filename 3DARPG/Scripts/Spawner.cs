using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> spawnPointList;
    //用来记录已经生成的角色 的脚本
    private List<Character> spawnCharacters;
    private bool hasSpawned;
    private bool allSpawnedAreDead;
    //unity事件，通过事件可以调用其他脚本的方法，在所有生成角色被消灭时执行
    public UnityEvent OnAllSpawnedCharacterEliminated;


    //用来在Gizmo中测试
    private Collider _collider;
    
    private void Awake()
    {
        //获取父节点的所有子节点的SpawnPoint脚本
        var spawnPointArray= transform.parent.GetComponentsInChildren<SpawnPoint>();
        //用上面的数组初始化列表
        spawnPointList = new List<SpawnPoint>(spawnPointArray);
        spawnCharacters = new List<Character>();
    }

    private void Update()
    {
        //如果还没生成或者生成的list数量为0，直接返回
        if (!hasSpawned || spawnCharacters.Count == 0) return;

        allSpawnedAreDead = true;
        //遍历,如果有活着的角色，allSpawnedAreDead记为false，中止遍历
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
            //清除列表，防止继续触发上面事件
            spawnCharacters.Clear();
        }
    }

    /// <summary>
    /// 生成角色的方法
    /// </summary>
    public void SpawnCharacters()
    {
        if (hasSpawned) return;
        hasSpawned = true;
        //遍历列表，如果记录的EnemyToSpawn不为空，生成这个敌人,在记录点生成,并把其Character脚本加入列表
        foreach(SpawnPoint point in spawnPointList)
        {
            GameObject spawnedGameObject = Instantiate(point.EnemyToSpawn, point.transform.position, point.transform.rotation);
            spawnCharacters.Add(spawnedGameObject.GetComponent<Character>());
        }
    }

    /// <summary>
    /// 触发器事件，如果玩家触碰到，就生成敌人
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
        //画出线条立方体，位置，大小
        Gizmos.DrawWireCube(transform.position, _collider.bounds.size);
    }
}
