using UnityEngine;

public abstract class BattleStateBase : ScriptableObject, IBattleState
{
    protected BattleController controller;

    public virtual void Enter(BattleController controller)
    {
        this.controller = controller;
    }

    public virtual void Update() { }
    public virtual void Exit() { }

    public virtual void OnConfirmPlacedPiece() { }
}
