using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    /// <summary> AB包管理器 让外部进行资源加载 </summary>
    public class ABManager : SingletonMono<ABManager>,ManagerInit
    {
        //AB包不能重复加载，否则会报错，需要用字典存起来进行判断
        //字典：存储加载过的AB包
        private Dictionary<string, AssetBundle> abDic = null;
        //主包
        private AssetBundle mainAB = null;
        //主包的配置文件：其中存储了依赖包信息
        private AssetBundleManifest mainfest = null;
        //AB包的存放路径
        private string PathUrl
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }
        //主包名
        private string MainABName
        {
            get
            {
                //根据平台返回不同的主包名
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
                return "PC";
#endif
            }
        }

        /// <summary> 初始化，需要先在外部初始化才能使用 </summary>
        public void Init()
        {
            abDic = new Dictionary<string, AssetBundle>();

            Debug.Log("ABManager 初始化完成...");
        }

        #region 同步加载资源
        /// <summary> 同步加载AB包 </summary>
        /// <param name="abName">AB包名</param>
        public void LoadAB(string abName)
        {
            //1.处理依赖包
            //通过加载主包，获取主包配置信息中依赖包，加载依赖包
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(System.IO.Path.Combine(PathUrl, MainABName));
                mainfest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            //获取依赖包名字数组
            AssetBundle tempAB = null;
            string[] strs = mainfest.GetAllDependencies(abName);
            //加载依赖包，添加进字典
            for (int i = 0; i < strs.Length; i++)
            {
                //如果未加载过
                if (!abDic.ContainsKey(strs[i]))
                {
                    tempAB = AssetBundle.LoadFromFile(System.IO.Path.Combine(PathUrl, strs[i]));
                    abDic.Add(strs[i], tempAB);
                }
            }

            //2.加载目标包
            //如果未加载过
            if (!abDic.ContainsKey(abName))
            {
                tempAB = AssetBundle.LoadFromFile(System.IO.Path.Combine(PathUrl, abName));
                abDic.Add(abName, tempAB);
            }
        }
        /// <summary> 同步加载资源 不指定类型 </summary>
        /// <param name="abName">目标AB包名</param>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName)
        {
            //加载AB包
            LoadAB(abName);
            //返回资源
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                Object obj = ab.LoadAsset(resName);
                //如果是GameObject，实例化后再返回
                if (obj is GameObject)
                    return Instantiate(obj);
                else
                    return obj;
            }
            else
            {
                Debug.Log($"没有{abName}这个AB包");
            }
            return null;
        }
        /// <summary> 同步加载资源 根据泛型指定类型 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">目标AB包名</param>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            //加载AB包
            LoadAB(abName);
            //返回资源
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                T obj = ab.LoadAsset<T>(resName);
                //如果是GameObject，实例化后再返回
                if (obj is GameObject)
                    return Instantiate(obj);
                else
                    return obj;
            }
            else
            {
                Debug.Log($"没有{abName}这个AB包");
            }
            return null;
        }
        /// <summary> 同步加载资源 根据Type指定类型 热更需要使用此方法 </summary>
        /// <param name="abName">目标AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, System.Type type)
        {
            //加载AB包
            LoadAB(abName);
            //返回资源
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                Object obj = ab.LoadAsset(resName, type);
                //如果是GameObject，实例化后再返回
                if (obj is GameObject)
                    return Instantiate(obj);
                else
                    return obj;
            }
            else
            {
                Debug.Log($"没有{abName}这个AB包");
            }
            return null;
        }
        #endregion

        #region 异步加载资源
        /// <summary> 异步加载AB包的协程 </summary>
        /// <param name="abName">AB包名</param>
        /// <returns></returns>
        public IEnumerator LoadABAsync(string abName)
        {
            //1.处理依赖包
            //通过加载主包，获取主包配置信息中依赖包，加载依赖包
            if (mainAB == null)
            {
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(PathUrl, MainABName));
                yield return abcr;
                mainAB = abcr.assetBundle;
                AssetBundleRequest abr = mainAB.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                yield return abr;
                mainfest = abr.asset as AssetBundleManifest;
            }
            //获取依赖包名字数组
            AssetBundle tempAB = null;
            string[] strs = mainfest.GetAllDependencies(abName);
            //加载依赖包，添加进字典
            for (int i = 0; i < strs.Length; i++)
            {
                //如果未加载过
                if (!abDic.ContainsKey(strs[i]))
                {
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(PathUrl, strs[i]));
                    yield return abcr;
                    tempAB = abcr.assetBundle;
                    abDic.Add(strs[i], tempAB);
                }
            }

            //2.加载目标包
            //如果未加载过
            if (!abDic.ContainsKey(abName))
            {
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(PathUrl, abName));
                yield return abcr;
                tempAB = abcr.assetBundle;
                abDic.Add(abName, tempAB);
            }
        }

        /// <summary> 异步加载资源 不指定类型 </summary>
        /// <param name="abName">目标AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数，参数是加载的资源</param>
        public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
        {
            StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
        }
        /// 异步加载资源 不指定类型 的实际协程函数
        private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
        {
            //异步加载AB包
            yield return StartCoroutine(LoadABAsync(abName));
            //返回资源
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                //异步加载资产
                AssetBundleRequest abr = ab.LoadAssetAsync(resName);
                yield return abr;
                //异步加载结束后 通过委托参数传递给外部使用
                if (abr.asset is GameObject)
                    callBack(Instantiate(abr.asset));
                else
                    callBack(abr.asset);
            }
            else
            {
                Debug.Log($"没有{abName}这个AB包");
            }
        }

        /// <summary> 异步加载资源 泛型 </summary>
        /// <param name="abName">目标AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数，参数是加载的资源</param>
        public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
        }
        /// 异步加载资源 泛型 的实际协程函数
        private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
        {
            //异步加载AB包
            yield return StartCoroutine(LoadABAsync(abName));
            //返回资源
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                //异步加载资产
                AssetBundleRequest abr = ab.LoadAssetAsync(resName);
                yield return abr;
                //异步加载结束后 通过委托参数传递给外部使用
                if (abr.asset is GameObject)
                    callBack(Instantiate(abr.asset) as T);
                else
                    callBack(abr.asset as T);
            }
            else
            {
                Debug.Log($"没有{abName}这个AB包");
            }
        }

        /// <summary> 异步加载资源 指定type </summary>
        /// <param name="abName">目标AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调函数，参数是加载的资源</param>
        public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
        {
            StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack));
        }
        /// 异步加载资源 指定type 的实际协程函数
        private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
        {
            //异步加载AB包
            yield return StartCoroutine(LoadABAsync(abName));
            //返回资源
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                //异步加载资产
                AssetBundleRequest abr = ab.LoadAssetAsync(resName, type);
                yield return abr;
                //异步加载结束后 通过委托参数传递给外部使用
                if (abr.asset is GameObject)
                    callBack(Instantiate(abr.asset));
                else
                    callBack(abr.asset);
            }
            else
            {
                Debug.Log($"没有{abName}这个AB包");
            }
        }
        #endregion

        #region 卸载
        /// <summary>
        /// 单个包卸载
        /// </summary>
        /// <param name="abName">卸载的包名</param>
        /// <param name="unloadAllLoadedObjects">是否把通过AB包加载的资源也卸载</param>
        public void Unload(string abName, bool unloadAllLoadedObjects = false)
        {
            //如果有这个包
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                ab.Unload(unloadAllLoadedObjects);
                abDic.Remove(abName);
            }
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否把通过AB包加载的资源也卸载</param>
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