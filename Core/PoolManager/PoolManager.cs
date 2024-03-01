using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    /// <summary>
    /// �����ģ��
    /// 1.Dictionary List
    /// 2.GameObject �� Resources �����������е� API 
    /// </summary>
    public class PoolManager : Singleton<PoolManager>,ManagerInit
    {
        //��������� ���¹�
        private Dictionary<string, PoolData> poolDic = null;

        private GameObject poolObj=null;

        public void Init()
        {
            poolDic = new Dictionary<string, PoolData>();
        }

        /// <summary>�ӳ���ȡ������ </summary>
        /// <param name="abName">AB����</param>
        /// <param name="name"></param>
        /// <param name="callBack">�ص������еĲ�������ȡ�õĶ���</param>
        public void GetObj(string abName, string name, UnityAction<GameObject> callBack)
        {
            //�г��� ���ҳ������ж���
            if (poolDic.ContainsKey(name) && poolDic[name].PoolQueue.Count > 0)
            {
                callBack(poolDic[name].GetObj());
            }
            else
            {
                //ͨ���첽������Դ ����������ⲿ��
                //��Դ���ؿ��Ի�������Դ����
                ABManager.Instance.LoadResAsync<GameObject>(abName, name, (obj) =>
                {
                    obj.name = name;
                    callBack(obj);
                });
            }
        }

        /// <summary> ����ʱ���õĶ������� </summary>
        public void PushObj(string name, GameObject obj)
        {
            if (poolObj == null)
                poolObj = new GameObject("Pool");

            //�����г���
            if (poolDic.ContainsKey(name))
            {
                poolDic[name].PushObj(obj);
            }
            //����û�г���
            else
            {
                poolDic.Add(name, new PoolData(obj, poolObj));
            }
        }


        /// <summary> ��ջ���صķ��� ��Ҫ���� �����л�ʱ </summary>
        public void Clear()
        {
            poolDic.Clear();
            poolObj = null;
        }

    }
}

