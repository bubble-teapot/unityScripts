namespace Core
{
    //����ģʽ����,����Լ��Ϊ�������޲ι��캯�����ǳ�����
    //ΪʲôԼ������Ϊʹ����instance = new T();
    public abstract class Singleton<T> where T : new()
    {
        private static T instance;
        //���ڼ���
        private static object lockObj = new object();
        //�������캯������֤����ֻ�����ڲ�����
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
