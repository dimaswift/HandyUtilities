using UnityEngine;
using System.Collections;
namespace HandyUtilities
{
    public class SoundManager : MonoBehaviour
    {
        AudioSource m_musicSource, m_effectSource;
        public float volume { get; set; }
        public void Init()
        {
            var cam = Camera.main.gameObject;
            m_musicSource = cam.AddComponent<AudioSource>();
            m_effectSource = cam.AddComponent<AudioSource>();
        }

        public void Play(AudioClip clip, float volume = 1f)
        {
            m_effectSource.PlayOneShot(clip, this.volume * volume);
        }

        public void PlayMusic(AudioClip clip, float volume = 1f)
        {
            m_musicSource.clip = clip;
            m_musicSource.volume = this.volume * volume;
            m_musicSource.loop = true;
            m_musicSource.Play();
        }

        public void Mute(bool mute)
        {
            m_musicSource.mute = mute;
            m_effectSource.mute = mute;
        }
    }
}
