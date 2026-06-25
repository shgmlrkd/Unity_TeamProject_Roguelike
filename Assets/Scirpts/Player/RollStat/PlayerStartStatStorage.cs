public static class PlayerStartStatStorage
{
    //static 저장 씬변경대비
    public static bool HasRolledStat { get; private set; }
    public static PlayerRolledStat RolledStat { get; private set; }

    public static void SetRolledStat(PlayerRolledStat stat)
    {
        RolledStat = stat;
        HasRolledStat = stat != null;
    }

    public static void Clear()
    {
        RolledStat = null;
        HasRolledStat = false;
    }
}