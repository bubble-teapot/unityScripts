using UnityEngine;

namespace Core
{
    //继承Mono的单例模式基类
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // 如果实例为空，则创建一个新的GameObject并添加该组件
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        instance = singletonObject.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
        //使用virtual虚函数，子类继承可能还需要用Awake()
        protected virtual void Awake()
        {
            // 确保在场景切换时不会销毁该实例
            DontDestroyOnLoad(gameObject);
            // 检查是否存在重复的实例
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Debug.LogWarning("存在重复的单例" + typeof(T).Name + "请删除");
                Destroy(gameObject);
            }
        }
    }
}

