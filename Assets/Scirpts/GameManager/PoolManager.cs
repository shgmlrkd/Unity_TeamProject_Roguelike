using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : ScenesSingleton<PoolManager>
{
    private Dictionary<Type, Queue<Component>> poolDictionary = new Dictionary<Type, Queue<Component>>(); // 각 타입 Queue 저장고 생성

    private Dictionary<Type, Transform> poolParents = new Dictionary<Type, Transform>(); // 타입별 부모 오브젝트용

    private Transform poolRoot; // 모든 풀링 오브젝트 최상위 부모

    private Transform uiPoolRoot; // UI 용

    private bool isUI = false;

    public void SetCreatePool(Transform transform = null, bool isUI = false)
    {
        this.isUI = isUI;

        if (isUI)
        {
            CreateUIPoolRoot(transform);
        }
        else
        {
            CreatePoolRoot();
        }
    }

    private void CreateUIPoolRoot(Transform parentTransform)
    {
        // 이미 생성되어 있다면 중복 생성 방지
        if (uiPoolRoot != null) return;

        // 1. UI용 최상위 루트 생성 (반드시 RectTransform 포함)
        GameObject uiRootObj = new GameObject("UIPoolRoot", typeof(RectTransform));

        // 2. 넘겨받은 transform(보통 호출한 UI 패널)을 부모로 설정
        // 이렇게 하면 UIPoolRoot가 Canvas 하위 계층에 놓이게 됩니다.
        uiRootObj.transform.SetParent(parentTransform, false);

        uiPoolRoot = uiRootObj.transform;
    }

    private void CreatePoolRoot()
    {
        GameObject rootObj = new GameObject("PoolRoot"); // PoolRoot 빈 게임 오브젝트 생성

        rootObj.transform.SetParent(transform); // PoolRoot를 PoolManager 오브젝트의 자식 설정

        poolRoot = rootObj.transform; // 생성된 poolRoot의 트랜스폼을 저장 
    }

    public void PreloadPool<T>(T prefab, int count) where T : Component // 오브젝트를 미리 생성
    {
        // 현재 프리팹의 타입을 가져옴
        Type type = typeof(T);

        CreatePool(type);

        // count만큼 미리 생성
        for (int i = 0; i < count; i++)
        {
            T obj = Instantiate(prefab); // 프리팹을 이용해서 생성

            obj.gameObject.SetActive(false); // 바로 사용 안하니 비활성화

            obj.transform.SetParent(poolParents[type]); // 하이어라키 정리용 (부모 밑으로 정리)

            poolDictionary[type].Enqueue(obj); // 생성한 오브젝트 큐에 저장
        }
    }

    public T GetPool<T>(T prefab) where T : Component // 오브젝트 꺼내기
    {
        Type type = typeof(T);

        CreatePool(type);

        T obj = null; // 반환한 오브젝트 변수

        if (poolDictionary[type].Count > 0) // 큐에 대기중인 오브젝트가 있다면 쓴다
        {
            obj = poolDictionary[type].Dequeue() as T;
        }
        else
        {
            obj = Instantiate(prefab); // 대기중인 오브젝트가 없다면 새로 만들기

            obj.transform.SetParent(poolParents[type]); // 새로 생성한것도 부모 밑으로
        }

        obj.gameObject.SetActive(true); // 꺼냈으니 활성화 

        return obj; // 오브젝트 반환

    }

    public void ReturnPool<T>(T obj) where T : Component // 사용이 끝난 오브젝트 다시 넣기
    {
        Type type = typeof(T);

        CreatePool(type); 

        obj.gameObject.SetActive(false); // 사용 안하니까 비활성화 상태 전환

        if(obj.transform.parent != poolParents[type]) // 다른 부모 밑에 있다면 타입별 부모 밑으로 이동
        {
            obj.transform.SetParent(poolParents[type]);
        }

        poolDictionary[type].Enqueue(obj); // 다시 큐에 넣기
    }

    private void CreatePool(Type type)  // 큐와 부모 오브젝트 생성
    {
        if(poolDictionary.ContainsKey(type)) // 추가할려는 타입이 이미 있다면 하지말고 리턴
        {
            return;
        }

        poolDictionary.Add(type, new Queue<Component>()); // 해당 타입의 큐를 생성

        CreatePoolParent(type); // 부모 생성 호출
    }

    private void CreatePoolParent(Type type) // 각 타입별 부모 오브젝트 생성
    {
        GameObject parentObj = null;

        if (isUI)
        {
            parentObj = new GameObject(type.Name, typeof(RectTransform));   // UI용
        }
        else
        {
            parentObj = new GameObject(type.Name);  // 타입 이름으로 오브젝트 생성
        }

        if (poolRoot != null)
        {
            parentObj.transform.SetParent(poolRoot); // poolRoot의 자식으로 설정
        }
        else
        {
            parentObj.transform.SetParent(uiPoolRoot); // poolRoot가 null이라면 UI용임
        }

        poolParents.Add(type, parentObj.transform); // 타입별 부모 Transform에 저장
    }
}
