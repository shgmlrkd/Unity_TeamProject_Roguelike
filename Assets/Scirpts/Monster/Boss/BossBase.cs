using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    protected const float FULL_CIRCLE_ANGLE = 360.0f;

    protected BossStateManager stateManager;
    protected BossContext bossContext;

    protected float stateTime;

    // 현재 State 내부 시간 (패턴 타이밍용)
    public float StateTime => stateTime;

    // 초기화 (Manager가 호출)
    public virtual void Init(BossStateManager manager, BossContext context)
    {
        stateManager = manager;
        bossContext = context;
    }

    // State 진입
    public virtual void Enter()
    {
        stateTime = 0.0f;
    }

    // State 종료
    public virtual void Exit() { }

    // 매 프레임 실행
    public abstract void Tick();

    // 물리 프레임 실행
    public virtual void FixedTick() { }

    // 공통 업데이트
    public void ManualUpdate()
    {
        stateTime += Time.deltaTime;
        Tick();
    }

    // 거리 계산
    protected float GetDistance()
    {
        return Vector2.Distance(bossContext.rb.position, bossContext.target.position);
    }

    // 제곱 거리 구하기
    protected float GetSqrDistance()
    {
        return ((Vector2)bossContext.target.position - bossContext.rb.position).sqrMagnitude;
    }

    // 방향 구하기
    protected Vector2 GetDirection()
    {
        return ((Vector2)bossContext.target.position - bossContext.rb.position).normalized;
    }

    // 상태 변경
    protected void ChangeState(BossStateEnum nextState)
    {
        stateManager.SetState(nextState);
    }

    protected Vector2 GetRadialDirection(int index, int count, float startAngle = 0.0f)
    {
        float angle = startAngle + (FULL_CIRCLE_ANGLE / count) * index;
        float rad = angle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    protected Vector3 GetRadialOffset(int index, int count, float radius)
    {
        return GetRadialDirection(index, count) * radius;
    }
}