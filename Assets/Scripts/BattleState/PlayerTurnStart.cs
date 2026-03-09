using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/State/PlayerTurnStart")]

public class PlayerTurnStart : BattleStateBase
{
    [SerializeField] private BattleStateBase nextState;

    public override void Enter(BattleController controller)
    {
        base.Enter(controller);

        Debug.Log("PlayerTurnStart");

        foreach (var c in controller.GetPlayerTeamCopy())
        {
            c.StartTurn();
        }

        controller.ChangeState(nextState);
    }

    public override void Exit()
    {
        
    }
}
