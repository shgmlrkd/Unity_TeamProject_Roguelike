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

    private const float DEFAULT_VALUE = 0.5f;

    private float[] volumeValues = new float[3];

    private void Start()
    {
        // 옵션에 볼륨 수치가 변할 때 발생하는 이벤트 등록하기
        foreach (VolumeUI volume in volumeUIs)
        {
            // 값은 50으로 초기화
            volume.Slider.SetValueWithoutNotify(DEFAULT_VALUE);
            UpdateVolumeOptionValue(volume.Type, DEFAULT_VALUE);

            // AddListener 괄호안에는 value라는 매개변수가 OnVolumeChanged라는 함수에 매개변수로 들어간다는 뜻
            volume.Slider.onValueChanged.AddListener(value => OnVolumeChanged(volume.Type, value));
        }

        // 초기 볼륨 저장
        SaveVolumeOption();

        // 초기 볼륨 적용
        SoundManager.Instance.ApplyVolume();

        // 토글 옵션 이벤트 등록
        SubscribeToggleEvent();

        // 처음 시작 시 비활성화
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 파괴 시 이벤트 해제
        UnsubcribeToggleEvent();
    }

    private void SubscribeToggleEvent()
    {
        statToggle.OnToggleChanged += _ => SaveToggleOption();
        equipToggle.OnToggleChanged += _ => SaveToggleOption();
    }

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