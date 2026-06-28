using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundInfo
{
    public SoundKey Key;
    public AudioClip Clip;
}

public class SoundManager : GlobalSingleton<SoundManager>
{
    [Header("게임에 쓰일 오디오 모음")]
    [SerializeField] private List<SoundInfo> soundsInfo = new();

    private readonly Dictionary<SoundKey, AudioClip> soundData = new();

    [Header("BGM 오디오 소스")]
    [SerializeField] private AudioSource bgmSource;

    /*[Header("SFX 오디오 소스")]
    [SerializeField] private AudioSource[] sfxSources;*/
    
    [Header("SFX Pool")]
    [SerializeField] private int sfxPoolSize = 10;

    private readonly Queue<AudioSource> sfxPool = new();

    //private int currentSFXIndex;

    protected override void Awake()
    {
        base.Awake();

        foreach (SoundInfo info in soundsInfo)
        {
            soundData[info.Key] = info.Clip;
        }

        InitSFXPool();
    }

    private void InitSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject obj = new GameObject($"SFX_{i}");
            obj.transform.SetParent(transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;

            sfxPool.Enqueue(source);
        }
    }

    #region SFX

    public void PlaySFX(SoundKey key)
    {
        if (!soundData.TryGetValue(key, out AudioClip clip))
            return;

        AudioSource source = GetSFXSource();

        source.volume =
            OptionManager.Instance.MasterVolume *
            OptionManager.Instance.SFXVolume;

        source.clip = clip;
        source.Play();

        StartCoroutine(ReturnToPool(source));
    }

    private IEnumerator ReturnToPool(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);

        source.clip = null;
        sfxPool.Enqueue(source);
    }

    private AudioSource GetSFXSource()
    {
        if (sfxPool.Count > 0)
        {
            return sfxPool.Dequeue();
        }

        // 풀이 모두 사용 중이면 새로 생성
        GameObject obj = new GameObject($"SFX_{transform.childCount}");
        obj.transform.SetParent(transform);

        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;

        return source;
    }

    #endregion

    #region BGM

    public void PlayBGM(SoundKey key)
    {
        if (!soundData.TryGetValue(key, out AudioClip clip))
            return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        float finalBGMVolume = 
            OptionManager.Instance.MasterVolume * 
            OptionManager.Instance.BGMVolume;

        print($"마스터 볼륨 : {OptionManager.Instance.MasterVolume}");

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = finalBGMVolume;
        bgmSource.Play();
    }

    public void SetBGMVolume(float BGMVolume)
    {
        bgmSource.volume = BGMVolume * OptionManager.Instance.MasterVolume;
    }

    // 현재 키 값에 맞는 오디오 소스를 찾아서 플레이 중인지 확인
    public bool IsPlayingSFX(SoundKey key)
    {
        foreach (Transform child in transform)
        {
            AudioSource source = child.GetComponent<AudioSource>();

            if (source != null &&
                source.isPlaying &&
                source.clip != null &&
                soundData.TryGetValue(key, out AudioClip clip) &&
                source.clip == clip)
            {
                return true;
            }
        }

        return false;
    }

    /*public void StopBGM()
    {
        bgmSource.Stop();
    }*/

    /*public void PauseBGM(bool pause)
    {
        if (pause)
        {
            bgmSource.Pause();
        }
        else
        { 
            bgmSource.UnPause(); 
        }
    }*/

    #endregion

    /*public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }*/
}