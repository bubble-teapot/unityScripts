using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class MusicManager : Singleton<MusicManager>,ManagerInit
    {
        //Ψһ�ı����������
        private AudioSource bkMusic = null;
        //���ִ�С
        private float bkValue;

        //��Ч��������
        private GameObject soundObj = null;
        //��Ч�б�
        private List<AudioSource> soundList = null;
        //��Ч��С
        private float soundValue;

        public void Init()
        {
            bkValue = 1;
            soundValue = 1;
            soundList = new List<AudioSource>();
            //��mono��update������ִ��Update����
            MonoProxy.Instance.AddUpdateListener(OnUpdate);

            Debug.Log("MusicManager ��ʼ�����...");
        }

        //���²����б�Ͳ���Դ���
        private void OnUpdate()
        {
            for (int i = soundList.Count - 1; i >= 0; --i)
            {
                if (!soundList[i].isPlaying)
                {
                    GameObject.Destroy(soundList[i]);
                    soundList.RemoveAt(i);
                }
            }
        }

        /// <summary> ���ű������� </summary>
        /// <param name="name"></param>
        public void PlayBkMusic(string name)
        {
            if (bkMusic == null)
            {
                GameObject obj = new GameObject();
                obj.name = "BkMusic";
                bkMusic = obj.AddComponent<AudioSource>();
            }
            //�첽���ر������� ������ɺ� ����
            ABManager.Instance.LoadResAsync<AudioClip>("music/bk", name, (clip) =>
            {
                bkMusic.clip = clip;
                bkMusic.loop = true;
                bkMusic.volume = bkValue;
                bkMusic.Play();
            });

        }

        /// <summary> ��ͣ�������� </summary>
        public void PauseBKMusic()
        {
            if (bkMusic == null)
                return;
            bkMusic.Pause();
        }

        /// <summary> ֹͣ�������� </summary>
        public void StopBKMusic()
        {
            if (bkMusic == null)
                return;
            bkMusic.Stop();
        }

        /// <summary> �ı䱳������ ������С </summary>
        /// <param name="v"></param>
        public void ChangeBKValue(float v)
        {
            bkValue = v;
            if (bkMusic == null)
                return;
            bkMusic.volume = bkValue;
        }

        /// <summary> ������Ч </summary>
        public void PlaySound(string name, bool isLoop, UnityAction<AudioSource> callBack = null)
        {
            if (soundObj == null)
            {
                soundObj = new GameObject();
                soundObj.name = "Sound";
            }
            //����Ч��Դ�첽���ؽ����� �����һ����Ч,"·��"Ϊ����
            ABManager.Instance.LoadResAsync<AudioClip>("music", name, (clip) =>
            {
                AudioSource source = soundObj.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = isLoop;
                source.volume = soundValue;
                source.Play();
                soundList.Add(source);
                if (callBack != null)
                    callBack(source);
            });
        }

        /// <summary> �ı���Ч������С </summary>
        /// <param name="value"></param>
        public void ChangeSoundValue(float value)
        {
            soundValue = value;
            for (int i = 0; i < soundList.Count; ++i)
                soundList[i].volume = value;
        }

        /// <summary> ֹͣ��Ч </summary>
        public void StopSound(AudioSource source)
        {
            if (soundList.Contains(source))
            {
                soundList.Remove(source);
                source.Stop();
                GameObject.Destroy(source);
            }
        }
    }
}
