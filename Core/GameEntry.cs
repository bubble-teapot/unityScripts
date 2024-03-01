using System.Collections;
using UnityEngine;
using Core;

public class GameEntry : MonoBehaviour
{
    private void Awake()
    {
        CoreEntry.Init();
    }
    private void Start()
    {
        MonoProxy.Instance.AddUpdateListener(OnUpdate);
    }
    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            PoolManager.Instance.GetObj("prefab/son", "Cube", (obj) =>
            {
                float x = UnityEngine.Random.Range(-10, 10);
                float y = UnityEngine.Random.Range(-10, 10);
                float z = UnityEngine.Random.Range(-10, 10);
                obj.transform.position =new Vector3(x,y,z);
                StartCoroutine(PushPoolOneSecond(obj));
            });
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PoolManager.Instance.GetObj("prefab/son", "Capsule", (obj) =>
            {
                float x = UnityEngine.Random.Range(-10, 10);
                float y = UnityEngine.Random.Range(-10, 10);
                float z = UnityEngine.Random.Range(-10, 10);
                obj.transform.position = new Vector3(x, y, z);
                StartCoroutine(PushPoolOneSecond(obj));
            });
        }
    }
    IEnumerator PushPoolOneSecond(GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        PoolManager.Instance.PushObj(obj.name,obj);
    }

    //private void OnEnable()
    //{
    //    //test
    //    EventCenter.Instance.AddListener(EventEnum.Log, () =>
    //    {
    //        Debug.Log("A�¼�ִ��");
    //    });
    //    EventCenter.Instance.AddListener<string, string>(EventEnum.TestLog, (s,s2) =>
    //    {
    //        Debug.Log(s +s2+ "�¼�ִ��");
    //    });
    //}
    //private void Start()
    //{
    //    //test
    //    EventCenter.Instance.Broadcast(EventEnum.Log);
    //    object s = "22";
    //    EventCenter.Instance.Broadcast<string,string>(EventEnum.TestLog, "sda",s as string);

    //    MonoProxy.Instance.AddUpdateListener(() =>
    //    {
    //        Debug.Log("��Mono��Update��ӡ");
    //    });

    //    //SceneCenter.Instance.LoadScene("Test",()=>
    //    //{
    //    //    Debug.Log("�л�����");
    //    //});

    //    GameObject cube = ABManager.Instance.LoadRes<GameObject>("prefab/son", "Cube");
    //    cube.transform.position = Vector3.one;
    //    GameObject capsule = ABManager.Instance.LoadRes("prefab/son", "Capsule",typeof(GameObject)) as GameObject;
    //    capsule.transform.position = -Vector3.one;

    //    //test music
    //    //MusicManager.Instance.PlayBkMusic("theshy����");

    //    //test UI
    //    UIManager.Instance.Open<PanelA>();
    //}
}
