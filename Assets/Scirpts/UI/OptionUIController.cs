using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class VolumeUI
{
    // 어떤 볼륨 UI인지 구분하기 위한 타입
    public VolumeType Type;

    // 볼륨 값을 조절하는 슬라이더
    public Slider Slider;

    // 슬라이더 값을 표시하는 텍스트
    public TextMeshProUGUI Text;
}

public class OptionUIController : MonoBehaviour
{
    [Header("볼륨 UI 리스트")]
    [SerializeField]
    private VolumeUI[] volumeUIs;

    [Header("UI 표시 체크 박스")]
    [SerializeField]
    private UIButtonToggle statToggle;

    [SerializeField]
    private UIButtonToggle equipToggle;

    private void Start()
    {
        // 저장된 옵션 값을 불러와 UI에 반영
        InitializeVolumeUI();

        // 저장된 UI 표시 옵션 상태 적용
        InitializeToggleUI();

        // 슬라이더 이벤트 등록
        SubscribeVolumeEvent();

        // 토글 이벤트 등록
        SubscribeToggleEvent();

        // 로드된 볼륨 값 즉시 적용
        SoundManager.Instance.ApplyVolume();

        // 처음 시작 시 비활성화
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 파괴 시 이벤트 해제
        UnsubcribeToggleEvent();
    }

    private void SubscribeVolumeEvent()
    {
        foreach (VolumeUI volume in volumeUIs)
        {
            // 반복문 변수 캡처 문제 방지를 위해 로컬 변수 사용
            VolumeType type = volume.Type;

            volume.Slider.onValueChanged.AddListener(
                value => OnVolumeChanged(type, value));
        }
    }

    // 저장된 토글 상태를 UI에 적용
    private void InitializeToggleUI()
    {
        statToggle.SetToggleWithoutNotify(
            OptionManager.Instance.ShowStat);

        equipToggle.SetToggleWithoutNotify(
            OptionManager.Instance.ShowEquip);
    }

    // 저장된 볼륨 값을 UI에 적용
    private void InitializeVolumeUI()
    {
        float master = OptionManager.Instance.MasterVolume;
        float bgm = OptionManager.Instance.BGMVolume;
        float sfx = OptionManager.Instance.SFXVolume;

        volumeUIs[(int)VolumeType.Master].Slider.SetValueWithoutNotify(master);
        volumeUIs[(int)VolumeType.BGM].Slider.SetValueWithoutNotify(bgm);
        volumeUIs[(int)VolumeType.SFX].Slider.SetValueWithoutNotify(sfx);

        UpdateVolumeOptionValue(VolumeType.Master, master);
        UpdateVolumeOptionValue(VolumeType.BGM, bgm);
        UpdateVolumeOptionValue(VolumeType.SFX, sfx);
    }

    // 토글 설정하는 부분 이벤트 등록
    private void SubscribeToggleEvent()
    {
        statToggle.OnToggleChanged += _ => SaveToggleOption();
        equipToggle.OnToggleChanged += _ => SaveToggleOption();
    }

    // 토글 설정하는 부분 이벤트 해제
    private void UnsubcribeToggleEvent()
    {
        statToggle.OnToggleChanged -= _ => SaveToggleOption();
        equipToggle.OnToggleChanged -= _ => SaveToggleOption();
    }

    // 볼륨값이 변하면 볼륨 관련 수치, 텍스트를 업데이트 시킴
    private void OnVolumeChanged(VolumeType volumeType, float value)
    {
        UpdateVolumeOptionValue(volumeType, value);

        // 현재 값 저장
        SaveVolumeOption();

        // 즉시 적용
        SoundManager.Instance.ApplyVolume();
    }

    private void UpdateVolumeOptionValue(VolumeType type, float value)
    {
        // 타입이 없으면 리턴
        if (type == VolumeType.None) return;

        // null이면 리턴
        if (volumeUIs[(int)type] == null) return;

        // 텍스트 갱신
        volumeUIs[(int)type].Text.text = $"{(value * 100).ToString("F0")}";
    }

    private void SaveVolumeOption()
    {
        // 옵션 매니저에 값들을 저장함
        OptionManager.Instance.SetVolume(
            volumeUIs[(int)VolumeType.Master].Slider.value,
            volumeUIs[(int)VolumeType.BGM].Slider.value,
            volumeUIs[(int)VolumeType.SFX].Slider.value
        );
    }

    private void SaveToggleOption()
    {
        // 옵션 매니저에 토글 값들을 저장함
        OptionManager.Instance.SetUIOption(statToggle.IsOn, equipToggle.IsOn);
    }
}