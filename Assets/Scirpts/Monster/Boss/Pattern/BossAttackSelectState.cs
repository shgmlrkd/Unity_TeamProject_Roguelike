using System.Collections.Generic;
using UnityEngine;

public class BossAttackSelectState : BossBase
{
    [SerializeField] private AttackWeight[] attacks;

    private bool phaseTwoRequested = false;
    private bool selected = false;

    private void Start()
    {
        bossContext.OnPhaseTwoRequested += OnPhaseTwoRequested;
    }

    private void OnDisable()
    {
        bossContext.OnPhaseTwoRequested -= OnPhaseTwoRequested;
    }

    private void OnPhaseTwoRequested()
    {
        phaseTwoRequested = true;
    }

    public override void Enter()
    {
        selected = false;
    }

    public override void Tick()
    {
        if (selected) return;

        if (phaseTwoRequested)
        {
            phaseTwoRequested = false;
            ChangeState(BossStateEnum.PhaseTransition);
            return;
        }

        // 어떤 공격을 할건지 고름
        TrySelectAttack();

        selected = true;
    }

    // 랜덤으로 선택된 공격 상태로 넘어감
    private void TrySelectAttack()
    {
        float distance = GetDistance();

        List<AttackWeight> validList = GetValidAttacks(distance);

        ChangeState(GetRandom(validList));
    }

    // 유효한 공격 패턴 중 랜덤으로 하나 뽑음
    private BossStateEnum GetRandom(List<AttackWeight> validList)
    {
        if (validList.Count == 0)
        {
            return BossStateEnum.Idle;
        }

        float totalWeight = 0.0f;

        foreach (AttackWeight valid in validList)
        {
            totalWeight += valid.weight;
        }

        float random = Random.value * totalWeight;

        float current = 0.0f;

        foreach (AttackWeight valid in validList)
        {
            current += valid.weight;

            if (random <= current)
            {
                print(valid.state.ToString());
                return valid.state;
            }
        }

        print(validList[validList.Count - 1].state);
        return validList[validList.Count - 1].state;
    }

    // 유효한 공격 리스트를 뽑음
    private List<AttackWeight> GetValidAttacks(float distance)
    {
        List<AttackWeight> result = new();

        foreach (AttackWeight attack in attacks)
        {
            if (distance >= attack.minDistance && distance <= attack.maxDistance)
            {
                result.Add(attack); 
            }
        }

        return result;
    }
}