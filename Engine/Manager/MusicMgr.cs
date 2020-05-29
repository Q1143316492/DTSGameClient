using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;

namespace CWLEngine.Core.Manager
{
    /**
     有个空物体。下面挂声音组件。播完删除
         */
    public class MusicMgr : Singleton<MusicMgr>
    {
        public class Sound
        {
            public int times;
            public AudioSource source;

            public Sound(AudioSource source, int times)
            {
                this.source = source;
                this.times = times;
            }
        }

        private GameObject musicFlodder = null;
        private List<Sound> soundList = new List<Sound>();
        private List<MusicBase> soundMultiList = new List<MusicBase>();

        public MusicMgr()
        {
            musicFlodder = GameObject.Find(EngineMacro.ENGINE_MUSIC);
            MonoMgr.Instance.AddUpdateEvent(Update);
        }

        private void Update()
        {
            for (int i = soundList.Count - 1; i >= 0; i--)
            {
                if (!soundList[i].source.isPlaying)
                {
                    if (soundList[i].times > 0)
                    {
                        soundList[i].source.Play();
                        soundList[i].times--;
                    }
                    else
                    {
                        GameObject.Destroy(soundList[i].source);
                        soundList.RemoveAt(i);
                    }
                }
            }
        }

        // 这里的times 是重复播放次数，会一遍播放完在重新播
        public void PlaySound(string name, int times = 1, UnityAction<Sound> callback = null)
        {
            ResourceMgr.Instance.LoadAsync<AudioClip>("sound/" + name, (clip) =>
            {
                AudioSource source = musicFlodder.AddComponent<AudioSource>();
                source.clip = clip;
                source.volume = 1.0f; // todo 可设置音量

                Sound sound = new Sound(source, times);
                soundList.Add(sound);
                sound.source.Play();
                sound.times--;
                callback?.Invoke(sound);
            });
        }

        public void StopSound(Sound sound)
        {
            if (soundList.Contains(sound))
            {
                soundList.Remove(sound);
                sound.source.Stop();
                GameObject.Destroy(sound.source);
            }
        }
    }
}
