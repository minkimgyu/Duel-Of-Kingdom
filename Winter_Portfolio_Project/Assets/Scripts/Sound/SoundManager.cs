using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WPP.SOUND
{
    [Serializable]
    public class Sound
    {
        public Sound(string name, AudioClip audioClip)
        {
            this.name = name;
            this.audioClip = audioClip;
        }

        [SerializeField]
        string name;
        public string Name { get { return name; } }

        [SerializeField]
        AudioClip audioClip;
        public AudioClip AudioClip { get { return audioClip; } }
    }

    public class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;

        [SerializeField] List<Sound> _sounds;
        [SerializeField] Transform _sfxTransform; // 여기 하위에 sfx 플레이어 넣어주기

        AudioSource _bgmPlayer;
        AudioSource[] _sfxPlayers;

        private void Awake()
        {
            _instance = this;
            _bgmPlayer = GetComponent<AudioSource>();
            _sfxPlayers = _sfxTransform.GetComponents<AudioSource>();
        }

        public static void StopBGM()
        {
            if (_instance._bgmPlayer.isPlaying == true) _instance._bgmPlayer.Stop();
        }

        public static void StopSFX()
        {
            for (int i = 0; i < _instance._sfxPlayers.Length; i++)
            {
                if (_instance._sfxPlayers[i].isPlaying == false) continue;
                _instance._sfxPlayers[i].Stop();
            }
        }

        public static void PlayBGM(string name, bool isLooping = false)
        {
            Sound sound = _instance._sounds.Find(x => x.Name == name);
            if (sound == null) return;

            StopBGM();

            _instance._bgmPlayer.clip = sound.AudioClip;
            _instance._bgmPlayer.loop = isLooping;

            _instance._bgmPlayer.Play();
        }

        public static void PlaySFX(string name)
        {
            Sound sound = _instance._sounds.Find(x => x.Name == name);
            if (sound == null) return;

            for (int i = 0; i < _instance._sfxPlayers.Length; i++)
            {
                if (_instance._sfxPlayers[i].isPlaying == true) continue;

                _instance._sfxPlayers[i].clip = sound.AudioClip;
                _instance._sfxPlayers[i].Play();
                break;
            }
        }

        public static void SetBGMVolume(float ratio)
        {
            _instance._bgmPlayer.volume = ratio;
        }

        public static void SetSFXVolume(float ratio)
        {
            for (int i = 0; i < _instance._sfxPlayers.Length; i++)
            {
                _instance._sfxPlayers[i].volume = ratio;
            }
        }
    }
}