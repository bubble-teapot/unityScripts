using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    /// <summary>����ִ����Ϸȫ�ֵ�Update������ͳһ���ã�Ŀ��Ϊ�˽�ʡ����</summary>
    public class MonoProxy : SingletonMono<MonoProxy>,ManagerInit
    {
        //��Ҫִ�е�update�¼�
        private UnityEvent fixedUpdateProxy { get; set; } = null;
        private UnityEvent updateProxy { get; set; } = null;
        private UnityEvent lateUpdateProxy { get; set; } = null;

        //��ʼ������Ҫ�ⲿ�ȵ���
        public void Init()
        {
            //��ʼ���¼�
            fixedUpdateProxy = new UnityEvent();
            updateProxy = new UnityEvent();
            lateUpdateProxy = new UnityEvent();

            Debug.Log("Mono ��ʼ�����...");
        }

        #region ע�᷽��
        /// <summary> ע��FixedUpdate�ķ���</summary>
        /// <param name="callback">��Ҫ��ӵķ���</param>
        public void AddFixedUpdateListener(UnityAction callback)
        {
            fixedUpdateProxy.AddListener(callback);
        }
        /// <summary> ע��Update�ķ��� </summary>
        /// <param name="callback">��Ҫ��ӵķ���</param>
        public void AddUpdateListener(UnityAction callback)
        {
            updateProxy.AddListener(callback);
        }
        /// <summary> ע��LateUpdate�ķ��� </summary>
        /// <param name="callback">��Ҫ��ӵķ���</param>
        public void AddLateUpdateListener(UnityAction callback)
        {
            lateUpdateProxy.AddListener(callback);
        }
        #endregion

        #region ע������
        /// <summary> ע��FixedUpdate�ķ��� </summary>
        /// <param name="callback">��Ҫ�Ƴ��ķ���</param>
        public void RemoveFixedUpdateListener(UnityAction callback)
        {
            fixedUpdateProxy.RemoveListener(callback);
        }
        /// <summary> ע��Update�ķ��� </summary>
        /// <param name="callback">��Ҫ�Ƴ��ķ���</param>
        public void RemoveUpdateListener(UnityAction callback)
        {
            updateProxy.RemoveListener(callback);
        }
        /// <summary> ע��LateUpdate�ķ��� </summary>
        /// <param name="callback">��Ҫ�Ƴ��ķ���</param>
        public void RemoveLateUpdateListener(UnityAction callback)
        {
            lateUpdateProxy.RemoveListener(callback);
        }
        #endregion

        //ִ����������update���
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

        //��������ʱ�Ƴ����е��¼�����
        private void OnDestroy()
        {
            fixedUpdateProxy.RemoveAllListeners();
            updateProxy.RemoveAllListeners();
            lateUpdateProxy.RemoveAllListeners();
        }
    }
}