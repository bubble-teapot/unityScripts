using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core
{
    public class SceneCenter : Singleton<SceneCenter>,ManagerInit
    {
        //初始化场景中心
        public void Init()
        {
            Debug.Log("SceneCenter 初始化完成...");
        }
        /// <summary>
        /// 切换场景 同步
        /// </summary>
        /// <param name="name">场景名</param>
        /// <param name="fun">场景初始化执行的操作</param>
        public void LoadScene(string name, UnityAction fun)
        {
            SceneManager.LoadScene(name);
            //加载完成后执行fun
            fun?.Invoke();
        }

        /// <summary>
        /// 切换场景 异步
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fun"></param>
        public void LoadSceneAsync(string name, UnityAction fun)
        {
            MonoProxy.Instance.StartCoroutine(ReallyLoadSceneAsync(name, fun));
            //实际调用协程
            IEnumerator ReallyLoadSceneAsync(string name, UnityAction fun)
            {
                AsyncOperation ao = SceneManager.LoadSceneAsync(name);
                //得到场景加载的进度
                while (!ao.isDone)
                {
                    //在事件中心向外分发 进度情况
                    //EventCenter.TriggerEvent(Enum EventID.SceneProgress,ao.progress)
                    yield return null;
                }
                //加载完成后执行fun
                fun?.Invoke();
            }
        }

    }
}
