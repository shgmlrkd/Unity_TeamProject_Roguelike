using DG.Tweening;
using UnityEngine;
using System.Collections;

public class BossDoor : Doorinstall
{
    [Header("보스방 연출 설정")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private float bossRoomCameraSize = 9f; // 보스방 카메라 사이즈
    [SerializeField] private float normalRoomCameraSize = 5f; // 일반방 복귀 시 카메라 사이즈
    protected override void ExecuteDoorAction()
    {
        RoomRuleChecker.Instance.MaxDoorCount++;
        RoomRuleChecker.Instance.CanGenerateMoreRooms = true;
        // 일반 이동 대신 연출 및 강제 이동 호출
        StartCoroutine(BossEnterSequence());
    }
    private IEnumerator BossEnterSequence()
    {
        SoundManager.Instance.PlaySFX(SoundKey.BossDoorOpen);

        // 암전
        if (blackScreen != null)
            yield return blackScreen.DOFade(1f, 2.0f).WaitForCompletion();

        // 보스방 이동 좌표 계산 
        Vector3 bossCenterPos = transform.position + new Vector3(0, 500, 0);
        Vector3 spawnPos = bossCenterPos + new Vector3(0, -3f, 0);
        Vector2Int bossGridPos = new Vector2Int(myRoomPos.x, myRoomPos.y + 1);

        // 이동 처리 (플레이어와 카메라를 함께 이동)
        RoomManager.Instance.ForceMoveToBossRoom(spawnPos, bossGridPos);

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(spawnPos.x, spawnPos.y, Camera.main.transform.position.z);
        }
        // 대기
        yield return new WaitForSeconds(0.5f);

        // 화면 밝아짐 및 카메라 확장
        if (blackScreen != null)
        {
            blackScreen.DOFade(0f, 2.0f);
            yield return new WaitForSeconds(0.5f);
            Camera.main.DOOrthoSize(bossRoomCameraSize, 1.0f);
            SoundManager.Instance.PlayBGM(SoundKey.BoosRoomBGM);
        }
    }
    public void OnBossDoorTriggered(Doorinstall door)
    {
        RoomManager.Instance.SpawnBossRoomOnly(door);
    }
}
