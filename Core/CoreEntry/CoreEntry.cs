
namespace Core
{
    /// <summary> ���ڳ�ʼ�����е�Manager </summary>
    public class CoreEntry
    {
        //��ʼ������Managers
        public static void Init()
        {
            MonoProxy.Instance.Init();
            EventCenter.Instance.Init();
            ABManager.Instance.Init();
            SceneCenter.Instance.Init();
            MusicManager.Instance.Init();
            PoolManager.Instance.Init();
            UIManager.Instance.Init();
        }
    }
}

