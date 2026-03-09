using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/State/WaitPlayerInput")]

public class WaitPlayerInput : BattleStateBase
{
    [SerializeField] private BattleStateBase nextState;

    public override void Enter(BattleController controller)
    {
        base.Enter(controller);

        controller.ToggleCamera();

        Debug.Log("WaitPlayerInput");
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        controller.ToggleCamera();
    }

    public override void OnConfirmPlacedPiece()
    {
        controller.ChangeState(nextState);
    }
}
