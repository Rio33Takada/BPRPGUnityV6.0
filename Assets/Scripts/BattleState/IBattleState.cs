using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleState
{
    void Enter(BattleController controller);
    void Update();
    void Exit();

    void OnConfirmPlacedPiece();
}
