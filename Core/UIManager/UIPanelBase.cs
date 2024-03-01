using UnityEngine;
namespace Core
{
    //UI面板基类，抽象类不能直接实例化
    public abstract class UIPanelBase : MonoBehaviour
    {
        protected UIPanelBase() { }
        public virtual void OnUIAwake()
        {

        }
        public virtual void OnUIStart()
        {

        }
        //传入面板参数
        public virtual void SetData(object data)
        {

        }
        public virtual void OnUIEnable()
        {
            gameObject.SetActive(true);
        }
        public virtual void OnUIDisable()
        {
            gameObject.SetActive(false);
        }
        public virtual void OnUIDestroy()
        {
            Destroy(gameObject);
        }
    }
}