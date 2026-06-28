using UnityEngine;

public class SoundManager : GlobalSingleton<SoundManager>
{
   /* [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private AudioSource sfxSource;*/

    public void ApplyVolume()
    {
        // BGM 볼륨 적용
        //bgmSource.volume = OptionManager.Instance.MasterVolume * OptionManager.Instance.BGMVolume;

        // SFX 볼륨 적용
        //sfxSource.volume = OptionManager.Instance.MasterVolume * OptionManager.Instance.SFXVolume;

        print("사운드 적용 완료");

        print(
            $"Master : {OptionManager.Instance.MasterVolume}, " +
            $"BGM : {OptionManager.Instance.BGMVolume}, " +
            $"SFX : {OptionManager.Instance.SFXVolume}"
        );
    }
}
