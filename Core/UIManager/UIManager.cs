using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    //UI������
    //����
    //UI�����صĽű�Ҫ��UI���������ͬ
    public class UIManager : SingletonMono<UIManager>,ManagerInit
    {
        /// <summary>UI��弯�ϣ���¼��ǰ���������е����</summary>
        public Dictionary<Type, UIPanelBase> PanelDic = null;
        /// <summary>UI����</summary>
        public Canvas Canvas { get; private set; } = null;
        /// <summary>UI�¼�ϵͳ</summary>
        public EventSystem EventSystem { get; private set; } = null;
        /// <summary>UI�������</summary>
        public Camera Camera { get; private set; } = null;
        /// <summary>UIRoot�㼶RectTransform����</summary>
        private Dictionary<UIPanelLayer, RectTransform> layers = null;
        /// <summary>��ȡָ����UI�㼶</summary>
        public RectTransform GetLayer(UIPanelLayer layer)
        {
            return layers[layer];
        }

        public void Init()
        {
            //Mono�������
            //����UIManage��������UIRoot����ΪUIManager������UIManager
            GameObject obj = ABManager.Instance.LoadRes<GameObject>("prefab/ui", "UIRoot");
            //�������ڱ��ֺ�UIManager��ͬ
            DontDestroyOnLoad(obj);

            //��ȡ�������
            Canvas = obj.GetComponentInChildren<Canvas>();
            EventSystem = obj.GetComponentInChildren<EventSystem>();
            Camera = obj.GetComponentInChildren<Camera>();
            PanelDic = new Dictionary<Type, UIPanelBase>();

            //��ȡUIRoot���в㼶����
            layers = new Dictionary<UIPanelLayer, RectTransform>();
            foreach (UIPanelLayer layer in Enum.GetValues(typeof(UIPanelLayer)))
            {
                layers.Add(layer, Canvas.transform.Find(layer.ToString()) as RectTransform);
            }

            Debug.Log("UIManager ��ʼ�����...");
        }

        /// <summary> ��һ����� </summary>
        /// <typeparam name="T">�������</typeparam>
        /// <param name="data">��Ҫ��ʼ��������</param>
        public void Open<T>(object data = null) where T : UIPanelBase
        {
            //�����С��������С�շ�����
            void openPanel(UIPanelBase panel)
            {
                //���������������棬��ʾ�����ϲ�
                panel.transform.SetAsLastSibling();
                panel.SetData(data);
                panel.OnUIEnable();
            }
            //�������С����
            UIPanelBase clonePanel()
            {
                string abName = "prefab/ui";

                //�������
                GameObject gameObject = ABManager.Instance.LoadRes<GameObject>(abName,typeof(T).Name);
                if (gameObject == null)
                {
                    Debug.Log($"��AB����{abName}δ�ҵ�����壺{typeof(T).Name}");
                }
                //����������ڲ㼶��ͨ������
                RectTransform layer = Instance.GetLayer(UIPanelLayer.Normal);//��Ĭ����ʾ��������
                object[] objects = typeof(T).GetCustomAttributes(typeof(UILayerAttribute), true);
                if (objects?.Length > 0)
                {
                    var layerAttr = objects[0] as UILayerAttribute;
                    //�õ���ز㼶��RectTransform
                    layer = Instance.GetLayer(layerAttr.layer);
                }
                //�������,�����ø�����,�޸����ƣ�������
                gameObject.transform.SetParent(layer,false);//ע�����ø�����ʱ����false������������ԭʼλ��
                gameObject.name = typeof(T).Name;
                var newPanel = gameObject.AddComponent<T>();
                return newPanel;
            }

            //����Ѿ��������
            UIPanelBase panel;
            if (PanelDic.TryGetValue(typeof(T), out panel))
            {
                //�����
                openPanel(panel);
            }
            else
            {
                //�����µ����
                panel = clonePanel();
                //��¼���
                PanelDic.Add(typeof(T), panel);
                panel.OnUIAwake();
                //�ӳ�һִ֡��OnUIStart,Ϊ�˺�unity�������ڱ���һ��
                Instance.StartCoroutine(MyInvoke(panel.OnUIStart));
                //�����
                openPanel(panel);
            }
        }

        #region �ر���巽��
        /// <summary>
        /// �ر�һ����壬�������ű�����
        /// </summary>
        /// <typeparam name="T">�������</typeparam>
        public void Close<T>() where T : UIPanelBase
        {
            //���δ��ȡ�����
            if (!PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                Debug.LogError($"������δ���ҵ�{typeof(T).Name}���,�ر�ʧ��");
            }
            else
            {
                Close(panel);
            }
        }
        /// <summary>
        /// �ر�һ����壬����������Ľű�������
        /// </summary>
        /// <param name="panel">Ҫ�رյ����</param>
        public void Close(UIPanelBase panel)
        {
            panel?.OnUIDisable();
        }
        /// <summary>
        /// �ر��������
        /// </summary>
        public void CloseAll()
        {
            foreach (var panel in PanelDic.Values)
            {
                Instance.Close(panel);
            }
        }
        #endregion

        #region ������巽��
        /// <summary>
        /// ����һ����壬�������ű�����
        /// </summary>
        /// <typeparam name="T">�������</typeparam>
        public void Destroy<T>() where T : UIPanelBase
        {
            //���δ��ȡ�����
            if (!PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                Debug.LogError($"������δ���ҵ�{typeof(T).Name}��壬����ʧ��");
            }
            else
            {
                panel.OnUIDestroy();
                //ʹ��UIManager��ʵ����Destroy��������ֹ����
                Instance.Destroy(panel);
            }
        }
        /// <summary>
        /// ����һ����壬����������Ľű�������
        /// </summary>
        /// <param name="panel">��Ҫ���ٵ����</param>
        public void Destroy(UIPanelBase panel)
        {
            panel?.OnUIDisable();
            panel?.OnUIDestroy();
            //�Ƴ�������
            if (PanelDic.ContainsKey(panel.GetType()))
            {
                PanelDic.Remove(panel.GetType());
            }
        }
        /// <summary>
        /// �����������
        /// </summary>
        public void DestroyAll()
        {
            //ʹ���ֵ������ֵ���г�ʼ��
            List<UIPanelBase> panels = new List<UIPanelBase>(PanelDic.Values);
            foreach (var panel in panels)
            {
                Instance.Destroy(panel);
            }
            PanelDic.Clear();
            panels.Clear();
        }
        #endregion

        /// <summary> ��ȡһ�����ű� </summary>
        /// <typeparam name="T">�������</typeparam>
        /// <returns></returns>
        public T Get<T>() where T : UIPanelBase
        {
            //�����ȡ�������
            if (PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                return panel as T;
            }
            else
            {
                Debug.LogError($"������δ���ҵ�{typeof(T).Name}��壬��ȡĬ��ֵ");
                return default(T);
            }

        }

        /// <summary> �ӳ�һִ֡�лص����� </summary>
        /// <param name="callback">��Ҫִ�еĻص�����</param>
        /// <returns></returns>
        private IEnumerator MyInvoke(Action callback)
        {
            //�ӳ�1֡
            yield return new WaitForEndOfFrame();
            //ִ�лص��������Ϊ��
            callback?.Invoke();
        }
    }
}

