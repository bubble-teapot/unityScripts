using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    /// <summary>代理执行游戏全局的Update方法，统一调用，目的为了节省性能</summary>
    public class MonoProxy : SingletonMono<MonoProxy>,ManagerInit
    {
        //需要执行的update事件
        private UnityEvent fixedUpdateProxy { get; set; } = null;
        private UnityEvent updateProxy { get; set; } = null;
        private UnityEvent lateUpdateProxy { get; set; } = null;

        //初始化，需要外部先调用
        public void Init()
        {
            //初始化事件
            fixedUpdateProxy = new UnityEvent();
            updateProxy = new UnityEvent();
            lateUpdateProxy = new UnityEvent();

            Debug.Log("Mono 初始化完成...");
        }

        #region 注册方法
        /// <summary> 注册FixedUpdate的方法</summary>
        /// <param name="callback">需要添加的方法</param>
        public void AddFixedUpdateListener(UnityAction callback)
        {
            fixedUpdateProxy.AddListener(callback);
        }
        /// <summary> 注册Update的方法 </summary>
        /// <param name="callback">需要添加的方法</param>
        public void AddUpdateListener(UnityAction callback)
        {
            updateProxy.AddListener(callback);
        }
        /// <summary> 注册LateUpdate的方法 </summary>
        /// <param name="callback">需要添加的方法</param>
        public void AddLateUpdateListener(UnityAction callback)
        {
            lateUpdateProxy.AddListener(callback);
        }
        #endregion

        #region 注销方法
        /// <summary> 注销FixedUpdate的方法 </summary>
        /// <param name="callback">需要移除的方法</param>
        public void RemoveFixedUpdateListener(UnityAction callback)
        {
            fixedUpdateProxy.RemoveListener(callback);
        }
        /// <summary> 注销Update的方法 </summary>
        /// <param name="callback">需要移除的方法</param>
        public void RemoveUpdateListener(UnityAction callback)
        {
            updateProxy.RemoveListener(callback);
        }
        /// <summary> 注销LateUpdate的方法 </summary>
        /// <param name="callback">需要移除的方法</param>
        public void RemoveLateUpdateListener(UnityAction callback)
        {
            lateUpdateProxy.RemoveListener(callback);
        }
        #endregion

        //执行生命周期update相关
        private void FixedUpdate()
        {
            fixedUpdateProxy?.Invoke();
        }
        private void Update()
        {
            updateProxy?.Invoke();
        }
        private void LateUpdate()
        {
            lateUpdateProxy?.Invoke();
        }

        //物体销毁时移除所有的事件监听
        private void OnDestroy()
        {
            fixedUpdateProxy.RemoveAllListeners();
            updateProxy.RemoveAllListeners();
            lateUpdateProxy.RemoveAllListeners();
        }
    }
}