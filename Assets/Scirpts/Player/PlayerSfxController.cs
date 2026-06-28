
﻿using UnityEngine;

public class PlayerSfxController : MonoBehaviour
{
    [Header("Audio Pool")]
    [SerializeField] private int poolSize = 8;
    [SerializeField] private AudioSource audioSourcePrefab;

    [Header("Move Loop Audio Source")]
    [SerializeField] private AudioSource moveLoopAudioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip moveLoopClip;

    [Header("Volumes")]
    [SerializeField, Range(0f, 1f)] private float masterSfxVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float attackVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float moveVolume = 0.3f;

    [Header("Move Check")]
    [SerializeField] private CharacterInputManager inputManager;
    [SerializeField] private float minMoveInput = 0.01f;

    private AudioSource[] audioPool;

    private void Awake()
    {
        if (inputManager == null)
        {
            inputManager = GetComponent<CharacterInputManager>();
        }

        CreateAudioPool();
        SetupMoveLoopAudioSource();
    }

    private void Update()
    {
        UpdateMoveSfx();
    }
    //오디오 풀생성
    private void CreateAudioPool()
    {
        audioPool = new AudioSource[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source;

            if (audioSourcePrefab != null)
            {
                source = Instantiate(audioSourcePrefab, transform);
            }
            else
            {
                GameObject audioObject = new GameObject($"PooledAudioSource_{i}");
                audioObject.transform.SetParent(transform);
                source = audioObject.AddComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.loop = false;

            audioPool[i] = source;
        }
    }
    //플레이어 움직임 사운드 루프 설정
    private void SetupMoveLoopAudioSource()
    {
        if (moveLoopAudioSource == null)
        {
            GameObject moveAudioObject = new GameObject("MoveLoopAudioSource");
            moveAudioObject.transform.SetParent(transform);
            moveLoopAudioSource = moveAudioObject.AddComponent<AudioSource>();
        }

        moveLoopAudioSource.playOnAwake = false;
        moveLoopAudioSource.loop = true;
        moveLoopAudioSource.volume = moveVolume;

        if (moveLoopClip != null)
        {
            moveLoopAudioSource.clip = moveLoopClip;
        }
    }

    public void PlayAttackSfx()
    {
        PlayOneShotFromPool(attackClip, attackVolume);
    }

    public void PlayPickupSfx()
    {//이거 아이템 픽업하는 스크립트에 넣어주세요 이세호
        PlayOneShotFromPool(pickupClip, pickupVolume);
    }

    private void PlayOneShotFromPool(AudioClip clip, float volume)
    {
        if (clip == null || audioPool == null || audioPool.Length == 0)
        {
            return;
        }

        AudioSource source = GetAvailableAudioSource();

        if (source == null)
        {
            return;
        }

        source.volume = volume;
        source.PlayOneShot(clip, volume);
    }
    //이동 SFX 통제
    private AudioSource GetAvailableAudioSource()
    {
        for (int i = 0; i < audioPool.Length; i++)
        {
            if (audioPool[i] != null && !audioPool[i].isPlaying)
            {
                return audioPool[i];
            }
        }

        // 전부 재생 중이면 첫 번째 AudioSource 재사용
        return audioPool[0];
    }

    private void UpdateMoveSfx()
    {
        if (inputManager == null || moveLoopAudioSource == null || moveLoopClip == null)
        {
            return;
        }

        bool isMoving = inputManager.enabled &&
                        inputManager.MoveInput.sqrMagnitude > minMoveInput * minMoveInput;

        if (isMoving)
        {
            if (!moveLoopAudioSource.isPlaying)
            {
                moveLoopAudioSource.clip = moveLoopClip;
                moveLoopAudioSource.volume = moveVolume;
                moveLoopAudioSource.Play();
            }
        }
        else
        {
            if (moveLoopAudioSource.isPlaying)
            {
                moveLoopAudioSource.Stop();
            }
        }
    }

    public void StopMoveSfx()
    {
        if (moveLoopAudioSource != null && moveLoopAudioSource.isPlaying)
        {
            moveLoopAudioSource.Stop();
        }
    }
}