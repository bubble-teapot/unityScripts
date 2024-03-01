using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core
{
    public class SceneCenter : Singleton<SceneCenter>,ManagerInit
    {
        //��ʼ����������
        public void Init()
        {
            Debug.Log("SceneCenter ��ʼ�����...");
        }
        /// <summary>
        /// �л����� ͬ��
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="fun">������ʼ��ִ�еĲ���</param>
        public void LoadScene(string name, UnityAction fun)
        {
            SceneManager.LoadScene(name);
            //������ɺ�ִ��fun
            fun?.Invoke();
        }

        /// <summary>
        /// �л����� �첽
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fun"></param>
        public void LoadSceneAsync(string name, UnityAction fun)
        {
            MonoProxy.Instance.StartCoroutine(ReallyLoadSceneAsync(name, fun));
            //ʵ�ʵ���Э��
            IEnumerator ReallyLoadSceneAsync(string name, UnityAction fun)
            {
                AsyncOperation ao = SceneManager.LoadSceneAsync(name);
                //�õ��������صĽ���
                while (!ao.isDone)
                {
                    //���¼���������ַ� �������
                    //EventCenter.TriggerEvent(Enum EventID.SceneProgress,ao.progress)
                    yield return null;
                }
                //������ɺ�ִ��fun
                fun?.Invoke();
            }
        }

    }
}
