public class OptionManager : GlobalSingleton<OptionManager>
{
    public float MasterVolume { get; private set; }
    public float BGMVolume { get; private set; }
    public float SFXVolume { get; private set; }

    public bool ShowStat { get; private set; }
    public bool ShowEquip { get; private set; }

    // 볼륨 값 저장
    public void SetVolume(float master, float bgm, float sfx)
    {
        MasterVolume = master;
        BGMVolume = bgm;
        SFXVolume = sfx;
    }

    // 인게임 UI 표시 여부 저장
    public void SetUIOption(bool showStat, bool showEquip)
    {
        ShowStat = showStat;
        ShowEquip = showEquip;

        print($"스탯 표시 : {ShowStat}\n장비 표시 : {showEquip}");
    }
}