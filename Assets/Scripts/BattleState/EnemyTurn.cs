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

        //包囲ピースの破壊.
        var targetPieces = new List<RemainPieceObject>();
        foreach (var enemy in controller.GetBattleEnemies())
        {
            foreach (var piece in enemy.surroundingPieces)
            {
                if (!(targetPieces.Contains(piece)))
                {
                    targetPieces.Add(piece);
                }
            }
        }
        foreach (var piece in targetPieces)
        {
            foreach (var cell in piece.GetCharacterPiece().CellInfoList)
            {
                var targetPosX = piece.PosX + cell.offset.x;
                var targetPosY = piece.PosY + cell.offset.y;

                //ピースをグリッドから削除.
                controller.fieldGrid.GetCell(targetPosX, targetPosY).OccupiedObject = null;

                //アニメーション開始.
                piece.PopOutAnimation();
            }
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
