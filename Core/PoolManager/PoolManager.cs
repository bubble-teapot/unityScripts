using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    /// <summary>
    /// 缓存池模块
    /// 1.Dictionary List
    /// 2.GameObject 和 Resources 两个公共类中的 API 
    /// </summary>
    public class PoolManager : Singleton<PoolManager>,ManagerInit
    {
        //缓存池容器 （衣柜）
        private Dictionary<string, PoolData> poolDic = null;

        private GameObject poolObj=null;

        public void Init()
        {
            poolDic = new Dictionary<string, PoolData>();
        }

        /// <summary>从池子取出东西 </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="name"></param>
        /// <param name="callBack">回调函数中的参数就是取得的对象</param>
        public void GetObj(string abName, string name, UnityAction<GameObject> callBack)
        {
            //有抽屉 并且抽屉里有东西
            if (poolDic.ContainsKey(name) && poolDic[name].PoolQueue.Count > 0)
            {
                callBack(poolDic[name].GetObj());
            }
            else
            {
                //通过异步加载资源 创建对象给外部用
                //资源加载可以换其他资源加载
                ABManager.Instance.LoadResAsync<GameObject>(abName, name, (obj) =>
                {
                    obj.name = name;
                    callBack(obj);
                });
            }
        }

        /// <summary> 换暂时不用的东西给我 </summary>
        public void PushObj(string name, GameObject obj)
        {
            if (poolObj == null)
                poolObj = new GameObject("Pool");

            //里面有抽屉
            if (poolDic.ContainsKey(name))
            {
                poolDic[name].PushObj(obj);
            }
            //里面没有抽屉
            else
            {
                poolDic.Add(name, new PoolData(obj, poolObj));
            }
        }


        /// <summary> 清空缓存池的方法 主要用在 场景切换时 </summary>
        public void Clear()
        {
            poolDic.Clear();
            poolObj = null;
        }

    }
}

