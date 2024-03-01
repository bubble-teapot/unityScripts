namespace Core
{
    //单例模式基类,泛型约束为：存在无参构造函数，非抽象类
    //为什么约束，因为使用了instance = new T();
    public abstract class Singleton<T> where T : new()
    {
        private static T instance;
        //用于加锁
        private static object lockObj = new object();
        //保护构造函数，保证单例只能在内部创建
        protected Singleton() { }
        public static T Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
                return instance;
            }
        }
    }
}
