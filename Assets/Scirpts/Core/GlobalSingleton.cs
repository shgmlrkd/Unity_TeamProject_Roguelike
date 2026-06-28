using UnityEngine;

public class GlobalSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            // instance가 null 이면
            if(instance == null)
            {
                // 현재 씬에 instance가 있는지 찾기
                instance = FindFirstObjectByType<T>();

                // 그래도 null 이면
                if (instance == null)
                {
                    // GameObject를 싱글톤으로 만들 클래스 이름으로 만들고 그 오브젝트에 스크립트 넣어주기
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        // 아직 싱글톤 인스턴스가 등록되지 않았다면
        if (instance == null)
        {
            // 현재 객체를 인스턴스로 등록
            instance = this as T;
        }
        // 이미 다른 인스턴스가 존재한다면
        else if (instance != this)
        {
            // 중복 생성된 객체이므로 제거
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        if(instance != null)
        {
            // 종료 직전에 정리
            instance = null;
        }

        Destroy(gameObject);
    }
}