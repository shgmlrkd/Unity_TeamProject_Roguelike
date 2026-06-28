using UnityEngine;

public class ScenesSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance; // ScenesSingleton<T> 의 T 라는 자리에 들어온 타입을 저장할 변수를 만든다. 
                               

    public static T Instance // 다른 클래스에서 Instance를 통해서 씬 싱글톤에 접근 가능, 퍼블릭 스태틱으로 저장 되기 때문에 어디서든 접근이 가능함
    {
        get
        {
            // instance가 null 이면
            if (instance == null)
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

    protected virtual void Awake()  // 설정된 객체들을 씬 싱글톤 인스턴스에 등록 (오버라이드)
    {
        instance = this as T;
    }

    protected virtual void OnDestroy() // 싱클톤 객체가 사라지면 instance참조 제거
    {
        if (instance == this)
            instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        if (instance != null)
        {
            // 종료 직전에 정리
            instance = null;
        }

        Destroy(gameObject);
    }
}
