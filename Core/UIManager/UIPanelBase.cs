using UnityEngine;
namespace Core
{
    //UI�����࣬�����಻��ֱ��ʵ����
    public abstract class UIPanelBase : MonoBehaviour
    {
        protected UIPanelBase() { }
        public virtual void OnUIAwake()
        {

        }
        public virtual void OnUIStart()
        {

        }
        //����������
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