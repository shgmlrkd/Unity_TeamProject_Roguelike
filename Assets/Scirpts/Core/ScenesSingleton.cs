using UnityEngine;

public class ScenesSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance; // ScenesSingleton<T> 의 T 라는 자리에 들어온 타입을 저장할 변수를 만든다. 
                               

    public static T Instance // 다른 클래스에서 Instance를 통해서 씬 싱글톤에 접근 가능, 퍼블릭 스태틱으로 저장 되기 때문에 어디서든 접근이 가능함
    {
        get
        {
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
}
