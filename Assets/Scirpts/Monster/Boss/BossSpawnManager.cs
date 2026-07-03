using System;
using UnityEngine;

public class BossSpawnManager : ScenesSingleton<BossSpawnManager>
{
    [SerializeField]
    private BossStateManager bossPrefab;

    [SerializeField]
    private CameraRig cameraRig;

    private BossStateManager boss;

    public event Action OnShowBossHpUI;

    protected override void Awake()
    {
        base.Awake();

        GameObject bossObj = Instantiate(bossPrefab.gameObject);
        boss = bossObj.GetComponent<BossStateManager>();

        // 보스가 타겟 찾기
        boss.Context.target = GameObject.Find("Player").transform;

        BossHp bossHp = boss.GetComponent<BossHp>();

        InGameManager.Instance.SetBossHp(bossHp);

        boss.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        cameraRig.OnCameraMoveActive += SpawnBoss; 
        cameraRig.OnCameraMoveActive += StartFadeIn;
        cameraRig.OnCameraMoveFinished += StartBoss;
    }

    private void OnDisable()
    {
        if (cameraRig != null)
        {
            cameraRig.OnCameraMoveActive -= SpawnBoss;
            cameraRig.OnCameraMoveActive -= StartFadeIn;
            cameraRig.OnCameraMoveFinished -= StartBoss;
        }
    }

    private void StartBoss()
    {
        if (!RoomRuleChecker.Instance.IsInBossEntranceMode) return;

        if (!boss.gameObject.activeSelf) return;

        boss.SetState(BossStateEnum.Idle);
    }

    private void StartFadeIn()
    {
        if (!RoomRuleChecker.Instance.IsInBossEntranceMode) return;

        if (!boss.gameObject.activeSelf) return;

        boss.VisualController.FadeIn();

        OnShowBossHpUI?.Invoke();
    }

    public void SpawnBoss()
    {
        if (!RoomRuleChecker.Instance.IsInBossEntranceMode) return;

        InGameManager.Instance.RegisterBossSpawned();
        boss.transform.position = RoomManager.Instance.BossSpawnPos;
        boss.gameObject.SetActive(true);
    }
}