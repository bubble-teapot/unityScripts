
namespace Core
{
    /// <summary> 用于初始化所有的Manager </summary>
    public class CoreEntry
    {
        //初始化所有Managers
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

