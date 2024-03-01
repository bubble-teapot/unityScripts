using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    /// <summary>
    /// ��������  �����е�һ������
    /// </summary>
    public class PoolData
    {
        //������ ������صĸ��ڵ�
        private GameObject fatherObj;
        //���������,�ö���Queue���б�ListЧ�ʸ�
        private Queue<GameObject> poolQueue;
        public Queue<GameObject> PoolQueue
        {
            get { return poolQueue; }
        }

        public PoolData(GameObject obj, GameObject poolObj)
        {
            //�����ǵĳ��� ����һ�������� ���Ұ�����Ϊ����pool(�¹�)�����������
            fatherObj = new GameObject(obj.name);
            fatherObj.transform.parent = poolObj.transform;
            poolQueue = new Queue<GameObject>() { };
            PushObj(obj);
        }

        /// <summary> ���������� ѹ���� </summary>
        /// <param name="obj"></param>
        public void PushObj(GameObject obj)
        {
            //ʧ�� ��������
            obj.SetActive(false);
            //������
            poolQueue.Enqueue(obj);
            //���ø�����
            obj.transform.parent = fatherObj.transform;
        }

        /// <summary> �ӳ������� ȡ���� </summary>
        /// <returns></returns>
        public GameObject GetObj()
        {
            GameObject obj = null;
            //ȡ����һ��
            obj = poolQueue.Dequeue();
            //���� ������ʾ
            obj.SetActive(true);
            //�Ͽ��˸��ӹ�ϵ
            obj.transform.parent = null;

            return obj;
        }
    }
}

