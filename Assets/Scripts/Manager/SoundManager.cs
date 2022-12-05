using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using WE.Support;
using WE.Pooling;
using WE.Unit;
using Sirenix.OdinInspector;
using TigerForge;
using WE.Utils;

namespace WE.Manager
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        [SerializeField]
        private AudioSource music_1, music_2;
        [SerializeField]
        private AudioClip musicHome;

        [SerializeField]
        private AudioClip[] musicBattle;
        [SerializeField]
        private AudioSourcePoolingObj audioPrefabs;
        //Dictionary<int, AudioSourcePoolingObj> dictSoundFx = new Dictionary<int, AudioSourcePoolingObj>();
        private CompositeDisposable disposables = new CompositeDisposable();
        public float ValueFXSound => 1;
        //{
        //    get
        //    {
        //        return PlayerPrefs.GetFloat("FXSound_Vol", 1);
        //    }
        //    set
        //    {
        //        PlayerPrefs.SetFloat("FXSound_Vol", value);
        //    }
        //}
        public float ValueBGMusic => 1;
        //{
        //    get 
        //    {
        //        return PlayerPrefs.GetFloat("BGMusic_Vol", 1);
        //    }
        //    set
        //    {
        //        PlayerPrefs.SetFloat("BGMusic_Vol", value);
        //    }
        //}
        public bool MusicOn => Player.Instance.GetMusicSetting();
        public bool SoundOn => Player.Instance.GetSoundSetting();
        public int maxSfxPerSound = 20;
        public Dictionary<string, int> playingSoundDic;


        [FoldoutGroup("Sound Fx")]
        public AudioClip buttonSfx, openChestSfx, revicedItemSfx, revicedSkillSfx, upgradeSfx, revicedCoinSfx, coinMultiSfx, revicedExpSfx, reviveSfx, dieSfx, winSfx, loseSfx, warningSfx
            ;

        public void OnStart()
        {
            playingSoundDic = new Dictionary<string, int>();
            //dictSoundFx = new Dictionary<int, AudioSourcePoolingObj>();
            //PlayMusicHome();
            if (Player.Instance.GetSoundSetting())
            {
                if (music_1.isPlaying)
                {
                    music_1.Pause();
                    //music_1.mute = true;
                }
                if (music_2.isPlaying)
                {
                    music_2.Pause();
                    //music_2.mute = true;
                }

            }
            EventManager.StartListening(Constant.ON_SOUND_SETTING_CHANGE, OnToggleMusic);
            PlayMusicHome();
        }

        private void OnDisable()
        {
            EventManager.StopListening(Constant.ON_SOUND_SETTING_CHANGE, OnToggleMusic);
            //if (DOTween.IsTweening(music_1))
            DOTween.Kill(music_1);
            // if (DOTween.IsTweening(music_2))
            DOTween.Kill(music_2);
            //dictSoundFx.Clear();
            disposables.Clear();
        }

        //public void SettingFxSound(float _value)
        //{
        //    ValueFXSound = _value;
        //    foreach (var item in dictSoundFx)
        //    {
        //        if (item.Value != null)
        //            item.Value.audioSource.volume = _value;
        //    }
        //}
        //public void SettingMusic(float _value)
        //{
        //    ValueBGMusic = _value;
        //    music_1.volume = _value;
        //    music_2.volume = _value;
        //    if (ValueBGMusic <= 0f)
        //    {
        //        if (music_1.isPlaying)
        //        {
        //            music_1.Pause();
        //            music_1.mute = true;
        //        }
        //        if (music_2.isPlaying)
        //        {
        //            music_2.Pause();
        //            music_2.mute = true;
        //        }
        //    }
        //    else
        //    {
        //        if (music_1.mute)
        //        {
        //            music_1.mute = false;
        //            music_1.UnPause();
        //        }
        //        if (music_2.mute)
        //        {
        //            music_2.mute = false;
        //            music_2.UnPause();
        //        }
        //    }
        //}

        public void OnToggleMusic()
        {
            music_1.mute = !MusicOn;
            music_2.mute = !MusicOn;
        }
        public void PlayMusic(AudioClip audio, bool isLoop = true)
        {
            if (audio == null)
                return;
            OnToggleMusic();
            //if (DOTween.IsTweening(music_1))
            DOTween.Kill(music_1);
            //if (DOTween.IsTweening(music_2))
            DOTween.Kill(music_2);
            if (!music_1.isPlaying && !music_2.isPlaying)
            {
                music_1.clip = audio;
                music_1.Play();
                music_1.volume = ValueBGMusic;
                music_1.loop = isLoop;
            }
            else
            {
                float timerMixer = 0.5f;
                if (music_1.isPlaying)
                {
                    music_1.volume = ValueBGMusic;
                    music_2.volume = 0f;
                    music_2.loop = isLoop;
                    music_2.clip = audio;
                    music_2.Play();
                    music_1.DOFade(0f, timerMixer).SetEase(Ease.Linear).OnComplete(() =>
                     {
                         music_1.Stop();
                         music_1.clip = null;
                     }).SetUpdate(true);
                    music_2.DOFade(ValueBGMusic, timerMixer).SetEase(Ease.Linear).SetUpdate(true);
                }
                else
                {
                    music_1.volume = 0f;
                    //music_2.volume = 1f;
                    music_2.volume = ValueBGMusic;
                    music_1.loop = isLoop;
                    music_1.clip = audio;
                    music_1.Play();
                    music_1.DOFade(ValueBGMusic, timerMixer).SetEase(Ease.Linear).SetUpdate(true);
                    music_2.DOFade(0f, timerMixer).SetEase(Ease.Linear).OnComplete(() =>
                     {
                         music_2.clip = null;
                         music_2.Stop();
                     }).SetUpdate(true);
                }
            }
        }
        public void PlayMusicBattle()
        {
            PlayMusic(musicBattle[Random.Range(0, musicBattle.Length)]);
        }
        public void PlayMusicHome()
        {
            PlayMusic(musicHome);
        }


        public void PlaySoundFx(AudioClip clip, bool loop = false)
        {
            if (ValueFXSound <= 0 || !SoundOn)
                return; 
            string code = clip.GetHashCode().ToString();
            if (playingSoundDic.ContainsKey(code))
            {
                if (playingSoundDic[code] > maxSfxPerSound)
                {
                    return;
                }
                else
                {
                    playingSoundDic[code]++;
                }
            }
            else
            {
                playingSoundDic.Add(code, 1);
            }
            SetAudio(Helper.SpawnAudioSource(audioPrefabs), clip, loop);
        }

        //public void PlaySoundEatCoin(AudioClip clip)
        //{
        //    if (ValueFXSound <= 0)
        //        return;

        //    AudioSourcePoolingObj source = Helper.SpawnAudioSource(audioPrefabs);
        //    AudioSource _source = source.audioSource;
        //    _source.clip = clip;
        //    _source.loop = false;
        //    _source.volume = ValueFXSound;
        //    //if(dictSoundFx.ContainsKey())
        //    int id = _source.GetInstanceID();
        //    _source.Play();
        //    if (!dictSoundFx.ContainsKey(id))
        //        dictSoundFx.Add(_source.GetInstanceID(), source);
        //    Observable.Timer(System.TimeSpan.FromSeconds(clip.length)).Subscribe(_ =>
        //    {
        //        Despawn(source);
        //    });
        //}
        //public AudioSourcePoolingObj GetSoundFx(AudioClip clip, bool loop, bool canDespawn = true)
        //{
        //    AudioSourcePoolingObj source = Helper.SpawnAudioSource(audioPrefabs);
        //    SetAudio(source, clip, loop, canDespawn);
        //    return source;
        //}

        //public AudioSource GetNewAudioSource()
        //{
        //    AudioSourcePoolingObj source = Helper.SpawnAudioSource(audioPrefabs);
        //    AudioSource _source = source.audioSource;
        //    _source.volume = ValueFXSound;
        //    int id = _source.GetInstanceID();
        //    if (!dictSoundFx.ContainsKey(id))
        //        dictSoundFx.Add(id, source);
        //    return _source;
        //}

        private void SetAudio(AudioSourcePoolingObj _source, AudioClip clip, bool loop, bool canDespawn = true)
        {

            //if (dictSoundFx)
            //{

            //}
            _source.currentClipPlaying = clip.GetHashCode().ToString();
            _source.audioSource.clip = clip;
            _source.audioSource.loop = loop;
            _source.audioSource.volume = ValueFXSound;
            //if(dictSoundFx.ContainsKey())
            int id = _source.audioSource.GetInstanceID();
            _source.audioSource.Play();
            //if (!dictSoundFx.ContainsKey(id))
            //    dictSoundFx.Add(_source.GetInstanceID(), _source);
            if (!loop && canDespawn)
            {
                Observable.Timer(System.TimeSpan.FromSeconds(clip.length)).Subscribe(_ =>
                {
                    Despawn(_source);
                });
            }

        }
        public void Despawn(AudioSourcePoolingObj audio)
        {
            if (audio == null)
                return;
            //int id = audio.GetInstanceID();
            //if (dictSoundFx.ContainsKey(id))
            //    dictSoundFx.Remove(id);
            audio.audioSource.Stop();
            audio.Despawn();
        }
        public void GetSoundFxCash()
        {

        }

        public void PauseMusic()
        {

        }
        public void UnPauseMusic()
        {

        }
        bool cachedExpFrame;
        public void PlayRevicedExpSound()
        {
            if (!cachedExpFrame)
            {
                cachedExpFrame = true;
                PlaySoundFx(revicedExpSfx);
            }
        }
        private void Update()
        {
            cachedExpFrame = false;
        }
    }
}