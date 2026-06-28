using UnityEngine.SceneManagement;

public class GameSceneManager : GlobalSingleton<GameSceneManager>
{
    // 씬 타입을 받아 해당 씬으로 이동(로드)
    public void LoadScene(SceneType type)
    {
        string sceneName = SceneNames.GetSceneName(type);
        SceneManager.LoadScene(sceneName);
    }

    // 현재 씬을 다시 로드함 (다시 하기 용도)
    public void ReloadCurScene()
    {
        // 현재 활성화된 씬 정보를 중 이름을 가져오고 그 씬을 로드함
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}