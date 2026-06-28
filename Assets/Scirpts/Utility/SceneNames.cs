using System.Collections.Generic;

// 씬 이름을 관리하는 정적 클래스
public static class SceneNames
{
    private static readonly Dictionary<SceneType, string> sceneTable
        = new Dictionary<SceneType, string>()
        {
            {SceneType.Title, "TiltleScene"},
            {SceneType.InGame, "InGameScene"},
        };

    // 씬 타입별 이름을 반환해주는 메서드
    public static string GetSceneName(SceneType type)
    {
        return sceneTable[type];
    }
}
