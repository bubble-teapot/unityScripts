using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    /// <summary>
    /// 抽屉数据  池子中的一列容器
    /// </summary>
    public class PoolData
    {
        //抽屉中 对象挂载的父节点
        private GameObject fatherObj;
        //对象的容器,用队列Queue比列表List效率高
        private Queue<GameObject> poolQueue;
        public Queue<GameObject> PoolQueue
        {
            get { return poolQueue; }
        }

        public PoolData(GameObject obj, GameObject poolObj)
        {
            //给我们的抽屉 创建一个父对象 并且把他作为我们pool(衣柜)对象的子物体
            fatherObj = new GameObject(obj.name);
            fatherObj.transform.parent = poolObj.transform;
            poolQueue = new Queue<GameObject>() { };
            PushObj(obj);
        }

        /// <summary> 往抽屉里面 压东西 </summary>
        /// <param name="obj"></param>
        public void PushObj(GameObject obj)
        {
            //失活 让其隐藏
            obj.SetActive(false);
            //存起来
            poolQueue.Enqueue(obj);
            //设置父对象
            obj.transform.parent = fatherObj.transform;
        }

        /// <summary> 从抽屉里面 取东西 </summary>
        /// <returns></returns>
        public GameObject GetObj()
        {
            GameObject obj = null;
            //取出第一个
            obj = poolQueue.Dequeue();
            //激活 让其显示
            obj.SetActive(true);
            //断开了父子关系
            obj.transform.parent = null;

            return obj;
        }
    }
}

