using UnityEngine;
using System.Collections.Generic;
using System;

namespace Core
{
    #region ί�����Ͷ���
    // �޲�ί��
    public delegate void CallBack();

    // ����ί��
    public delegate void CallBack<T>(T arg);
    public delegate void CallBack<T, X>(T arg1, X arg2);
    public delegate void CallBack<T, X, Y>(T arg1, X arg2, Y arg3);
    public delegate void CallBack<T, X, Y, Z>(T arg1, X arg2, Y arg3, Z arg4);
    public delegate void CallBack<T, X, Y, Z, W>(T arg1, X arg2, Y arg3, Z arg4, W arg5);
    #endregion

    // ����: �¼������࣬��ӡ�ɾ�����ַ��¼�
    public class EventCenter : Singleton<EventCenter>,ManagerInit
    {
        // �ֵ䣬���ڴ���¼����ί�ж�Ӧ
        private Dictionary<EventEnum, Delegate> eventTable =null;

        public void Init()
        {
            eventTable = new Dictionary<EventEnum, Delegate>();

            Debug.Log("EventCenter ��ʼ�����...");
        }

        #region ��Ӻ��Ƴ��¼�ǰ���ж�

        // ����¼�����ǰ���ж�
        private void OnAddListenerJudge(EventEnum eventEnum, Delegate callBack)
        {
            // ���ж��¼����Ƿ�������ֵ��У��������������
            if (!eventTable.ContainsKey(eventEnum))
            {
                // �ȸ��ֵ�����¼���,ί������Ϊ��
                eventTable.Add(eventEnum, null);
            }

            // �жϵ�ǰ�¼����ί�����ͺ�����ӵ�ί�������Ƿ�һ�£���һ�²�����ӣ��׳��쳣
            Delegate d = eventTable[eventEnum];
            if (d != null && d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("����Ϊ�¼���{0}��Ӳ�ͬ�¼���ί��,��ǰ�¼�����Ӧ��ί����{1},����ӵ�ί������{2}", eventEnum, d.GetType(), callBack.GetType()));
            }
        }

        // �Ƴ��¼���ǰ���ж�
        private void OnRemoveListenerBeforeJudge(EventEnum eventEnum)
        {
            // �ж��Ƿ����ָ���¼���
            if (!eventTable.ContainsKey(eventEnum))
            {
                throw new Exception(string.Format("�Ƴ���������;û���¼���", eventEnum));
            }
        }

        // �Ƴ��¼������ж�,�����Ƴ��ֵ��пյ��¼���
        private void OnRemoveListenerLaterJudge(EventEnum eventEnum)
        {
            if (eventTable[eventEnum] == null)
            {
                eventTable.Remove(eventEnum);
            }
        }

        #endregion

        #region ��Ӽ���
        // �޲�
        public void AddListener(EventEnum eventEnum, CallBack callBack)
        {
            OnAddListenerJudge(eventEnum, callBack);
            eventTable[eventEnum] = (CallBack)eventTable[eventEnum] + callBack;
        }
        // ������
        public void AddListener<T>(EventEnum eventEnum, CallBack<T> callBack)
        {
            OnAddListenerJudge(eventEnum, callBack);
            eventTable[eventEnum] = (CallBack<T>)eventTable[eventEnum] + callBack;
        }
        public void AddListener<T, X>(EventEnum eventEnum, CallBack<T, X> callBack)
        {
            OnAddListenerJudge(eventEnum, callBack);
            eventTable[eventEnum] = (CallBack<T, X>)eventTable[eventEnum] + callBack;
        }
        public void AddListener<T, X, Y>(EventEnum eventEnum, CallBack<T, X, Y> callBack)
        {
            OnAddListenerJudge(eventEnum, callBack);
            eventTable[eventEnum] = (CallBack<T, X, Y>)eventTable[eventEnum] + callBack;
        }
        public void AddListener<T, X, Y, Z>(EventEnum eventEnum, CallBack<T, X, Y, Z> callBack)
        {
            OnAddListenerJudge(eventEnum, callBack);
            eventTable[eventEnum] = (CallBack<T, X, Y, Z>)eventTable[eventEnum] + callBack;
        }
        public void AddListener<T, X, Y, Z, W>(EventEnum eventEnum, CallBack<T, X, Y, Z, W> callBack)
        {
            OnAddListenerJudge(eventEnum, callBack);
            eventTable[eventEnum] = (CallBack<T, X, Y, Z, W>)eventTable[eventEnum] + callBack;
        }

        #endregion

        #region �Ƴ�����

        public void RemoveListener(EventEnum eventEnum, CallBack callBack)
        {
            OnRemoveListenerBeforeJudge(eventEnum);
            eventTable[eventEnum] = (CallBack)eventTable[eventEnum] - callBack;
            OnRemoveListenerLaterJudge(eventEnum);
        }

        public void RemoveListener<T>(EventEnum eventEnum, CallBack<T> callBack)
        {
            OnRemoveListenerBeforeJudge(eventEnum);
            eventTable[eventEnum] = (CallBack<T>)eventTable[eventEnum] - callBack;
            OnRemoveListenerLaterJudge(eventEnum);
        }

        public void RemoveListener<T, X>(EventEnum eventEnum, CallBack<T, X> callBack)
        {
            OnRemoveListenerBeforeJudge(eventEnum);
            eventTable[eventEnum] = (CallBack<T, X>)eventTable[eventEnum] - callBack;
            OnRemoveListenerLaterJudge(eventEnum);
        }

        public void RemoveListener<T, X, Y>(EventEnum eventEnum, CallBack<T, X, Y> callBack)
        {
            OnRemoveListenerBeforeJudge(eventEnum);
            eventTable[eventEnum] = (CallBack<T, X, Y>)eventTable[eventEnum] - callBack;
            OnRemoveListenerLaterJudge(eventEnum);
        }

        public void RemoveListener<T, X, Y, Z>(EventEnum eventEnum, CallBack<T, X, Y, Z> callBack)
        {
            OnRemoveListenerBeforeJudge(eventEnum);
            eventTable[eventEnum] = (CallBack<T, X, Y, Z>)eventTable[eventEnum] - callBack;
            OnRemoveListenerLaterJudge(eventEnum);
        }

        public void RemoveListener<T, X, Y, Z, W>(EventEnum eventEnum, CallBack<T, X, Y, Z, W> callBack)
        {
            OnRemoveListenerBeforeJudge(eventEnum);
            eventTable[eventEnum] = (CallBack<T, X, Y, Z, W>)eventTable[eventEnum] - callBack;
            OnRemoveListenerLaterJudge(eventEnum);
        }

        #endregion

        #region �㲥�¼�
        public void Broadcast(EventEnum eventEnum)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventEnum, out d))
            {
                CallBack callBack = d as CallBack;
                if (callBack != null)
                {
                    callBack();
                }
            }
        }

        public void Broadcast<T>(EventEnum eventEnum, T arg1)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventEnum, out d))
            {
                CallBack<T> callBack = d as CallBack<T>;
                if (callBack != null)
                {
                    callBack(arg1);
                }
            }
        }

        public void Broadcast<T, X>(EventEnum eventEnum, T arg1, X arg2)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventEnum, out d))
            {
                CallBack<T, X> callBack = d as CallBack<T, X>;
                if (callBack != null)
                {
                    callBack(arg1, arg2);
                }
            }
        }

        public void Broadcast<T, X, Y>(EventEnum eventEnum, T arg1, X arg2, Y arg3)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventEnum, out d))
            {
                CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
                if (callBack != null)
                {
                    callBack(arg1, arg2, arg3);
                }
            }
        }

        public void Broadcast<T, X, Y, Z>(EventEnum eventEnum, T arg1, X arg2, Y arg3, Z arg4)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventEnum, out d))
            {
                CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
                if (callBack != null)
                {
                    callBack(arg1, arg2, arg3, arg4);
                }
            }
        }

        public void Broadcast<T, X, Y, Z, W>(EventEnum eventEnum, T arg1, X arg2, Y arg3, Z arg4, W arg5)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventEnum, out d))
            {
                CallBack<T, X, Y, Z, W> callBack = d as CallBack<T, X, Y, Z, W>;
                if (callBack != null)
                {
                    callBack(arg1, arg2, arg3, arg4, arg5);
                }
            }
        }

        #endregion
    }
}
