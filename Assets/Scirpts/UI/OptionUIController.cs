using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class VolumeUI
{
    public VolumeType type;
    public Slider slider;
    public TextMeshProUGUI text;
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
            volume.slider.SetValueWithoutNotify(DEFAULT_VALUE);
            UpdateVolumeOptionValue(volume.type, DEFAULT_VALUE);

            // AddListener 괄호안에는 value라는 매개변수가 OnVolumeChanged라는 함수에 매개변수로 들어간다는 뜻
            volume.slider.onValueChanged.AddListener(value => OnVolumeChanged(volume.type, value));
        }
        
        // 처음 시작 시 비활성화
        gameObject.SetActive(false);
    }

    // 볼륨값이 변하면 볼륨 관련 수치, 텍스트를 업데이트 시킴
    private void OnVolumeChanged(VolumeType volumeType, float value)
    {
        UpdateVolumeOptionValue(volumeType, value);
    }

    private void UpdateVolumeOptionValue(VolumeType type, float value)
    {
        // 타입이 없으면 리턴
        if (type == VolumeType.None) return;

        // VolumeUI을 찾고
        VolumeUI ui = GetUI(type);

        // null이면 리턴
        if (ui == null) return;

        // 텍스트 갱신
        ui.text.text = $"{(value * 100).ToString("F0")}";
    }

    private VolumeUI GetUI(VolumeType type)
    {
        foreach (VolumeUI volume in volumeUIs)
        {
            if(volume.type == type)
            {
                return volume;
            }    
        }

        return null;
    }

    public void GetOptionValue()
    {
        // 값만 따로 저장하는 foreach문을 돌림
        foreach (VolumeUI volume in volumeUIs)
        {
            volumeValues[(int)volume.type] = volume.slider.value;
        }
        
        // 마스터 볼륨값
        float masterVolume = volumeValues[(int)VolumeType.Master];

        // 최종 BGM 볼륨값
        float finalBGMValue = volumeValues[(int)VolumeType.BGM] * masterVolume;

        // 최종 SFX 볼륨값
        float finalSFXValue = volumeValues[(int)VolumeType.SFX] * masterVolume;

        // GameManager에서 받을 float 형 볼륨 수치

        // ex) GameManager.Instance.GetVolumeOption(finalBGMValue);
        // GameManager.Instance.GetVolumeOption(finalSFXValue);

        // GameManager에서 받을 bool형 토글

        // ex) GameManager.Instance.SetToggleOption(statToggle.IsOn);
        // GameManager.Instance.SetToggleOption(equipToggle.IsOn);
    }
}