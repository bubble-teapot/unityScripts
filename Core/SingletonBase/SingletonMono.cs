using UnityEngine;

namespace Core
{
    //�̳�Mono�ĵ���ģʽ����
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // ���ʵ��Ϊ�գ��򴴽�һ���µ�GameObject����Ӹ����
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        instance = singletonObject.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
        //ʹ��virtual�麯��������̳п��ܻ���Ҫ��Awake()
        protected virtual void Awake()
        {
            // ȷ���ڳ����л�ʱ�������ٸ�ʵ��
            DontDestroyOnLoad(gameObject);
            // ����Ƿ�����ظ���ʵ��
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Debug.LogWarning("�����ظ��ĵ���" + typeof(T).Name + "��ɾ��");
                Destroy(gameObject);
            }
        }
    }
}

