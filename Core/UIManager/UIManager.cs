using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    //UI管理器
    //规则：
    //UI面板挂载的脚本要和UI面板名字相同
    public class UIManager : SingletonMono<UIManager>,ManagerInit
    {
        /// <summary>UI面板集合，记录当前场景中所有的面板</summary>
        public Dictionary<Type, UIPanelBase> PanelDic = null;
        /// <summary>UI画布</summary>
        public Canvas Canvas { get; private set; } = null;
        /// <summary>UI事件系统</summary>
        public EventSystem EventSystem { get; private set; } = null;
        /// <summary>UI正交相机</summary>
        public Camera Camera { get; private set; } = null;
        /// <summary>UIRoot层级RectTransform集合</summary>
        private Dictionary<UIPanelLayer, RectTransform> layers = null;
        /// <summary>获取指定的UI层级</summary>
        public RectTransform GetLayer(UIPanelLayer layer)
        {
            return layers[layer];
        }

        public void Init()
        {
            //Mono单例相关
            //创建UIManage，，创建UIRoot改名为UIManager，挂载UIManager
            GameObject obj = ABManager.Instance.LoadRes<GameObject>("prefab/ui", "UIRoot");
            //生命周期保持和UIManager相同
            DontDestroyOnLoad(obj);

            //获取其他组件
            Canvas = obj.GetComponentInChildren<Canvas>();
            EventSystem = obj.GetComponentInChildren<EventSystem>();
            Camera = obj.GetComponentInChildren<Camera>();
            PanelDic = new Dictionary<Type, UIPanelBase>();

            //获取UIRoot所有层级物体
            layers = new Dictionary<UIPanelLayer, RectTransform>();
            foreach (UIPanelLayer layer in Enum.GetValues(typeof(UIPanelLayer)))
            {
                layers.Add(layer, Canvas.transform.Find(layer.ToString()) as RectTransform);
            }

            Debug.Log("UIManager 初始化完成...");
        }

        /// <summary> 打开一个面板 </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="data">需要初始化的数据</param>
        public void Open<T>(object data = null) where T : UIPanelBase
        {
            //打开面板小方法，用小驼峰命名
            void openPanel(UIPanelBase panel)
            {
                //将此面板放在最下面，显示在最上层
                panel.transform.SetAsLastSibling();
                panel.SetData(data);
                panel.OnUIEnable();
            }
            //创建面板小方法
            UIPanelBase clonePanel()
            {
                string abName = "prefab/ui";

                //加载面板
                GameObject gameObject = ABManager.Instance.LoadRes<GameObject>(abName,typeof(T).Name);
                if (gameObject == null)
                {
                    Debug.Log($"该AB包下{abName}未找到此面板：{typeof(T).Name}");
                }
                //查找面板所在层级，通过反射
                RectTransform layer = Instance.GetLayer(UIPanelLayer.Normal);//先默认显示在正常层
                object[] objects = typeof(T).GetCustomAttributes(typeof(UILayerAttribute), true);
                if (objects?.Length > 0)
                {
                    var layerAttr = objects[0] as UILayerAttribute;
                    //得到相关层级的RectTransform
                    layer = Instance.GetLayer(layerAttr.layer);
                }
                //创建面板,并设置父物体,修改名称，添加组件
                gameObject.transform.SetParent(layer,false);//注意设置父物体时加入false参数，不保留原始位置
                gameObject.name = typeof(T).Name;
                var newPanel = gameObject.AddComponent<T>();
                return newPanel;
            }

            //如果已经存在面板
            UIPanelBase panel;
            if (PanelDic.TryGetValue(typeof(T), out panel))
            {
                //打开面板
                openPanel(panel);
            }
            else
            {
                //创建新的面板
                panel = clonePanel();
                //记录面板
                PanelDic.Add(typeof(T), panel);
                panel.OnUIAwake();
                //延迟一帧执行OnUIStart,为了和unity生命周期保持一致
                Instance.StartCoroutine(MyInvoke(panel.OnUIStart));
                //打开面板
                openPanel(panel);
            }
        }

        #region 关闭面板方法
        /// <summary>
        /// 关闭一个面板，根据面板脚本泛型
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        public void Close<T>() where T : UIPanelBase
        {
            //如果未获取到面板
            if (!PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                Debug.LogError($"场景中未查找到{typeof(T).Name}面板,关闭失败");
            }
            else
            {
                Close(panel);
            }
        }
        /// <summary>
        /// 关闭一个面板，根据面板对象的脚本的引用
        /// </summary>
        /// <param name="panel">要关闭的面板</param>
        public void Close(UIPanelBase panel)
        {
            panel?.OnUIDisable();
        }
        /// <summary>
        /// 关闭所有面板
        /// </summary>
        public void CloseAll()
        {
            foreach (var panel in PanelDic.Values)
            {
                Instance.Close(panel);
            }
        }
        #endregion

        #region 销毁面板方法
        /// <summary>
        /// 销毁一个面板，根据面板脚本泛型
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        public void Destroy<T>() where T : UIPanelBase
        {
            //如果未获取到面板
            if (!PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                Debug.LogError($"场景中未查找到{typeof(T).Name}面板，消耗失败");
            }
            else
            {
                panel.OnUIDestroy();
                //使用UIManager的实例的Destroy方法，防止异议
                Instance.Destroy(panel);
            }
        }
        /// <summary>
        /// 销毁一个面板，根据面板对象的脚本的引用
        /// </summary>
        /// <param name="panel">需要销毁的面板</param>
        public void Destroy(UIPanelBase panel)
        {
            panel?.OnUIDisable();
            panel?.OnUIDestroy();
            //移除脏数据
            if (PanelDic.ContainsKey(panel.GetType()))
            {
                PanelDic.Remove(panel.GetType());
            }
        }
        /// <summary>
        /// 销毁所有面板
        /// </summary>
        public void DestroyAll()
        {
            //使用字典的所有值进行初始化
            List<UIPanelBase> panels = new List<UIPanelBase>(PanelDic.Values);
            foreach (var panel in panels)
            {
                Instance.Destroy(panel);
            }
            PanelDic.Clear();
            panels.Clear();
        }
        #endregion

        /// <summary> 获取一个面板脚本 </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <returns></returns>
        public T Get<T>() where T : UIPanelBase
        {
            //如果获取到了面板
            if (PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                return panel as T;
            }
            else
            {
                Debug.LogError($"场景中未查找到{typeof(T).Name}面板，获取默认值");
                return default(T);
            }

        }

        /// <summary> 延迟一帧执行回调函数 </summary>
        /// <param name="callback">需要执行的回调函数</param>
        /// <returns></returns>
        private IEnumerator MyInvoke(Action callback)
        {
            //延迟1帧
            yield return new WaitForEndOfFrame();
            //执行回调，如果不为空
            callback?.Invoke();
        }
    }
}

