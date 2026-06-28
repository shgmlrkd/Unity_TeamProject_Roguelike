using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionManager : GlobalSingleton<OptionManager>
{
    public float MasterVolume { get; private set; }
    public float BGMVolume { get; private set; }
    public float SFXVolume { get; private set; }

    public bool ShowStat { get; private set; }
    public bool ShowEquip { get; private set; }

    public event Action OnEquipShowEnabled;
    public event Action<bool, bool> OnUIOptionChanged;

    [Header("옵션 초기화 체크박스")]
    [SerializeField]
    private bool settingInit = false;

    private bool prevShowEquip = false;

    protected override void Awake()
    {
        base.Awake();

        // 옵션 세팅 초기화
        if (settingInit)
        { 
            PlayerPrefs.DeleteAll(); 
        }

        LoadOption();
    }

    // 볼륨 값 저장
    public void SetVolume(float master, float bgm, float sfx)
    {
        MasterVolume = master;
        BGMVolume = bgm;
        SFXVolume = sfx;

        SaveOption();
    }

    // 인게임 UI 표시 여부 저장
    public void SetUIOption(bool showStat, bool showEquip)
    {
        ShowStat = showStat;
        ShowEquip = showEquip;

        SaveOption();
        
        if(ShowEquip && !prevShowEquip)
        {
            OnEquipShowEnabled?.Invoke();
        }

        prevShowEquip = showEquip;

        OnUIOptionChanged?.Invoke(ShowStat, ShowEquip);

        if (SceneManager.GetActiveScene().name == SceneNames.GetSceneName(SceneType.InGame))
        { 
            PoolManager.Instance.SetPoolVisible(typeof(Image), ShowEquip);
        }
    }

    // 옵션을 PlayerPrefs로 저장
    public void SaveOption()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("BGMVolume", BGMVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);

        PlayerPrefs.SetInt("ShowStat", ShowStat ? 1 : 0);
        PlayerPrefs.SetInt("ShowEquipment", ShowEquip ? 1 : 0);

        PlayerPrefs.Save();
    }

    // 옵션을 PlayerPrefs로 로드
    private void LoadOption()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        ShowStat = PlayerPrefs.GetInt("ShowStat", 1) == 1;
        ShowEquip = PlayerPrefs.GetInt("ShowEquipment", 1) == 1;

        prevShowEquip = ShowEquip;  // 기존 장비 표시 옵션 저장

        // 저장된 표시 옵션 이벤트 발행
        OnUIOptionChanged?.Invoke(ShowStat, ShowEquip);
    }
}