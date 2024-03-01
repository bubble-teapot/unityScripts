using UnityEngine;
using System.Collections.Generic;
using System;

namespace Core
{
    #region 委托类型定义
    // 无参委托
    public delegate void CallBack();

    // 带参委托
    public delegate void CallBack<T>(T arg);
    public delegate void CallBack<T, X>(T arg1, X arg2);
    public delegate void CallBack<T, X, Y>(T arg1, X arg2, Y arg3);
    public delegate void CallBack<T, X, Y, Z>(T arg1, X arg2, Y arg3, Z arg4);
    public delegate void CallBack<T, X, Y, Z, W>(T arg1, X arg2, Y arg3, Z arg4, W arg5);
    #endregion

    // 描述: 事件中心类，添加、删除、分发事件
    public class EventCenter : Singleton<EventCenter>,ManagerInit
    {
        // 字典，用于存放事件码和委托对应
        private Dictionary<EventEnum, Delegate> eventTable =null;

        public void Init()
        {
            eventTable = new Dictionary<EventEnum, Delegate>();

            Debug.Log("EventCenter 初始化完成...");
        }

        #region 添加和移除事件前的判断

        // 添加事件监听前的判断
        private void OnAddListenerJudge(EventEnum eventEnum, Delegate callBack)
        {
            // 先判断事件码是否存在于字典中，不存在则先添加
            if (!eventTable.ContainsKey(eventEnum))
            {
                // 先给字典添加事件码,委托设置为空
                eventTable.Add(eventEnum, null);
            }

            // 判断当前事件码的委托类型和你添加的委托类型是否一致，不一致不能添加，抛出异常
            Delegate d = eventTable[eventEnum];
            if (d != null && d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("尝试为事件码{0}添加不同事件的委托,当前事件所对应的委托是{1},你添加的委托类型{2}", eventEnum, d.GetType(), callBack.GetType()));
            }
        }

        // 移除事件码前的判断
        private void OnRemoveListenerBeforeJudge(EventEnum eventEnum)
        {
            // 判断是否包含指定事件码
            if (!eventTable.ContainsKey(eventEnum))
            {
                throw new Exception(string.Format("移除监听错误;没有事件码", eventEnum));
            }
        }

        // 移除事件码后的判断,用于移除字典中空的事件码
        private void OnRemoveListenerLaterJudge(EventEnum eventEnum)
        {
            if (eventTable[eventEnum] == null)
            {
                eventTable.Remove(eventEnum);
            }
        }

        #endregion

        #region 添加监听
        // 无参
        public void AddListener(EventEnum eventEnum, CallBack callBack)
        {
            OnAddListenerJudge(eventEnum, callBack);
            eventTable[eventEnum] = (CallBack)eventTable[eventEnum] + callBack;
        }
        // 带参数
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

        #region 移除监听

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

        #region 广播事件
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
