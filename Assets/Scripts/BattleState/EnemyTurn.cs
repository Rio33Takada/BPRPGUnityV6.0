using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/State/EnemyTurn")]

public class EnemyTurn : BattleStateBase
{
    [SerializeField] private BattleStateBase nextState;

    public override void Enter(BattleController controller)
    {
        base.Enter(controller);

        Debug.Log("EnemyTurnStart");

        //ターン開始時の処理.
        foreach (var enemy in controller.GetBattleEnemies())
        {
            enemy.StartTurn();
        }

        //スタン判定.
        foreach (var enemy in controller.GetBattleEnemies())
        {
            enemy.CheckStun(controller.fieldGrid);
        }

        //移動.
        foreach(var e in controller.GetBattleEnemies())
        {
            e.Move(controller.fieldGrid);
        }

        //攻撃.

        controller.ChangeState(nextState);
    }
}
