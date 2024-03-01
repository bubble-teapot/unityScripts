using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    /// <summary> AB�������� ���ⲿ������Դ���� </summary>
    public class ABManager : SingletonMono<ABManager>,ManagerInit
    {
        //AB�������ظ����أ�����ᱨ����Ҫ���ֵ�����������ж�
        //�ֵ䣺�洢���ع���AB��
        private Dictionary<string, AssetBundle> abDic = null;
        //����
        private AssetBundle mainAB = null;
        //�����������ļ������д洢����������Ϣ
        private AssetBundleManifest mainfest = null;
        //AB���Ĵ��·��
        private string PathUrl
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }
        //������
        private string MainABName
        {
            get
            {
                //����ƽ̨���ز�ͬ��������
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
                return "PC";
#endif
            }
        }

        /// <summary> ��ʼ������Ҫ�����ⲿ��ʼ������ʹ�� </summary>
        public void Init()
        {
            abDic = new Dictionary<string, AssetBundle>();

            Debug.Log("ABManager ��ʼ�����...");
        }

        #region ͬ��������Դ
        /// <summary> ͬ������AB�� </summary>
        /// <param name="abName">AB����</param>
        public void LoadAB(string abName)
        {
            //1.����������
            //ͨ��������������ȡ����������Ϣ��������������������
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(System.IO.Path.Combine(PathUrl, MainABName));
                mainfest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            //��ȡ��������������
            AssetBundle tempAB = null;
            string[] strs = mainfest.GetAllDependencies(abName);
            //��������������ӽ��ֵ�
            for (int i = 0; i < strs.Length; i++)
            {
                //���δ���ع�
                if (!abDic.ContainsKey(strs[i]))
                {
                    tempAB = AssetBundle.LoadFromFile(System.IO.Path.Combine(PathUrl, strs[i]));
                    abDic.Add(strs[i], tempAB);
                }
            }

            //2.����Ŀ���
            //���δ���ع�
            if (!abDic.ContainsKey(abName))
            {
                tempAB = AssetBundle.LoadFromFile(System.IO.Path.Combine(PathUrl, abName));
                abDic.Add(abName, tempAB);
            }
        }
        /// <summary> ͬ��������Դ ��ָ������ </summary>
        /// <param name="abName">Ŀ��AB����</param>
        /// <param name="resName">��Դ��</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName)
        {
            //����AB��
            LoadAB(abName);
            //������Դ
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                Object obj = ab.LoadAsset(resName);
                //�����GameObject��ʵ�������ٷ���
                if (obj is GameObject)
                    return Instantiate(obj);
                else
                    return obj;
            }
            else
            {
                Debug.Log($"û��{abName}���AB��");
            }
            return null;
        }
        /// <summary> ͬ��������Դ ���ݷ���ָ������ </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">Ŀ��AB����</param>
        /// <param name="resName">��Դ��</param>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            //����AB��
            LoadAB(abName);
            //������Դ
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                T obj = ab.LoadAsset<T>(resName);
                //�����GameObject��ʵ�������ٷ���
                if (obj is GameObject)
                    return Instantiate(obj);
                else
                    return obj;
            }
            else
            {
                Debug.Log($"û��{abName}���AB��");
            }
            return null;
        }
        /// <summary> ͬ��������Դ ����Typeָ������ �ȸ���Ҫʹ�ô˷��� </summary>
        /// <param name="abName">Ŀ��AB����</param>
        /// <param name="resName">��Դ��</param>
        /// <param name="type">��Դ����</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, System.Type type)
        {
            //����AB��
            LoadAB(abName);
            //������Դ
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                Object obj = ab.LoadAsset(resName, type);
                //�����GameObject��ʵ�������ٷ���
                if (obj is GameObject)
                    return Instantiate(obj);
                else
                    return obj;
            }
            else
            {
                Debug.Log($"û��{abName}���AB��");
            }
            return null;
        }
        #endregion

        #region �첽������Դ
        /// <summary> �첽����AB����Э�� </summary>
        /// <param name="abName">AB����</param>
        /// <returns></returns>
        public IEnumerator LoadABAsync(string abName)
        {
            //1.����������
            //ͨ��������������ȡ����������Ϣ��������������������
            if (mainAB == null)
            {
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(PathUrl, MainABName));
                yield return abcr;
                mainAB = abcr.assetBundle;
                AssetBundleRequest abr = mainAB.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                yield return abr;
                mainfest = abr.asset as AssetBundleManifest;
            }
            //��ȡ��������������
            AssetBundle tempAB = null;
            string[] strs = mainfest.GetAllDependencies(abName);
            //��������������ӽ��ֵ�
            for (int i = 0; i < strs.Length; i++)
            {
                //���δ���ع�
                if (!abDic.ContainsKey(strs[i]))
                {
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(PathUrl, strs[i]));
                    yield return abcr;
                    tempAB = abcr.assetBundle;
                    abDic.Add(strs[i], tempAB);
                }
            }

            //2.����Ŀ���
            //���δ���ع�
            if (!abDic.ContainsKey(abName))
            {
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(PathUrl, abName));
                yield return abcr;
                tempAB = abcr.assetBundle;
                abDic.Add(abName, tempAB);
            }
        }

        /// <summary> �첽������Դ ��ָ������ </summary>
        /// <param name="abName">Ŀ��AB����</param>
        /// <param name="resName">��Դ��</param>
        /// <param name="callBack">�ص������������Ǽ��ص���Դ</param>
        public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
        {
            StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
        }
        /// �첽������Դ ��ָ������ ��ʵ��Э�̺���
        private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
        {
            //�첽����AB��
            yield return StartCoroutine(LoadABAsync(abName));
            //������Դ
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                //�첽�����ʲ�
                AssetBundleRequest abr = ab.LoadAssetAsync(resName);
                yield return abr;
                //�첽���ؽ����� ͨ��ί�в������ݸ��ⲿʹ��
                if (abr.asset is GameObject)
                    callBack(Instantiate(abr.asset));
                else
                    callBack(abr.asset);
            }
            else
            {
                Debug.Log($"û��{abName}���AB��");
            }
        }

        /// <summary> �첽������Դ ���� </summary>
        /// <param name="abName">Ŀ��AB����</param>
        /// <param name="resName">��Դ��</param>
        /// <param name="callBack">�ص������������Ǽ��ص���Դ</param>
        public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
        }
        /// �첽������Դ ���� ��ʵ��Э�̺���
        private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            //�첽����AB��
            yield return StartCoroutine(LoadABAsync(abName));
            //������Դ
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                //�첽�����ʲ�
                AssetBundleRequest abr = ab.LoadAssetAsync(resName);
                yield return abr;
                //�첽���ؽ����� ͨ��ί�в������ݸ��ⲿʹ��
                if (abr.asset is GameObject)
                    callBack(Instantiate(abr.asset) as T);
                else
                    callBack(abr.asset as T);
            }
            else
            {
                Debug.Log($"û��{abName}���AB��");
            }
        }

        /// <summary> �첽������Դ ָ��type </summary>
        /// <param name="abName">Ŀ��AB����</param>
        /// <param name="resName">��Դ��</param>
        /// <param name="callBack">�ص������������Ǽ��ص���Դ</param>
        public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
        {
            StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack));
        }
        /// �첽������Դ ָ��type ��ʵ��Э�̺���
        private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
        {
            //�첽����AB��
            yield return StartCoroutine(LoadABAsync(abName));
            //������Դ
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                //�첽�����ʲ�
                AssetBundleRequest abr = ab.LoadAssetAsync(resName, type);
                yield return abr;
                //�첽���ؽ����� ͨ��ί�в������ݸ��ⲿʹ��
                if (abr.asset is GameObject)
                    callBack(Instantiate(abr.asset));
                else
                    callBack(abr.asset);
            }
            else
            {
                Debug.Log($"û��{abName}���AB��");
            }
        }
        #endregion

        #region ж��
        /// <summary>
        /// ������ж��
        /// </summary>
        /// <param name="abName">ж�صİ���</param>
        /// <param name="unloadAllLoadedObjects">�Ƿ��ͨ��AB�����ص���ԴҲж��</param>
        public void Unload(string abName, bool unloadAllLoadedObjects = false)
        {
            //����������
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                ab.Unload(unloadAllLoadedObjects);
                abDic.Remove(abName);
            }
        }

        /// <summary>
        /// ж������AB��
        /// </summary>
        /// <param name="unloadAllLoadedObjects">�Ƿ��ͨ��AB�����ص���ԴҲж��</param>
        public void ClearAB(bool unloadAllLoadedObjects = false)
        {
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
            abDic.Clear();
            mainAB = null;
            mainfest = null;
        }
        #endregion
    }
}