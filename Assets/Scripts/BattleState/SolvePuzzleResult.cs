using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/State/SolvePuzzleResult")]

public class SolvePuzzleResult : BattleStateBase
{
    [SerializeField] private BattleStateBase nextState;
    [SerializeField] private float targetY = 0f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float interval = 0.2f;

    public override void Enter(BattleController controller)
    {
        base.Enter(controller);

        Debug.Log("SolvePuzzleResult");

        controller.StartCoroutine(AnimatePieces());
    }

    private IEnumerator AnimatePieces()
    {
        yield return new WaitForSeconds(0.5f);
        var pieces = controller.GetPieceListCopy();

        foreach (var piece in pieces)
        {
            //ピースをグリッドから削除.
            foreach (var c in piece.GetCellInfoCopy())
            {
                var x = c.offset.x + piece.PosX;
                var y = c.offset.y + piece.PosY;

                controller.puzzleGrid.GetCell(x, y).OccupiedObject = null;
            }


            controller.StartCoroutine(MoveToY(piece, targetY, moveDuration));
            yield return new WaitForSeconds(interval);

            //敵被ダメージ処理.
            var damageMap = controller.CountDamageToEnemy(piece);
            foreach (var damage in damageMap)
            {
                controller.uIController.CreateDamegeText(damage.Key, damage.Value);
                damage.Key.TakeDamage(damage.Value);
            }
        }


        var battleEnemies = controller.GetBattleEnemiesCopy();
        //敵死亡処理.
        foreach (var enemy in controller.GetBattleEnemiesCopy())
        {
            if (enemy.IsDead)
            {
                Debug.Log($"{enemy.EnemyData.enemyName}は死亡した");
                //enemyをFieldGridから削除.
                enemy.SetOccupiedCells(controller.fieldGrid, enemy.body.PosX, enemy.body.PosY, null);

                battleEnemies.Remove(enemy);
                Destroy(enemy.bodyCell.gameObject);
            }
        }

        controller.SetBattleEnemies(battleEnemies);

        yield return new WaitForSeconds(moveDuration);
        controller.ChangeState(nextState);
    }

    private IEnumerator MoveToY(CharacterPiece piece, float y, float duration)
    {
        Vector3 start = piece.transform.position;
        Vector3 end = new Vector3(start.x, y, start.z);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            piece.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        piece.transform.position = end;

        if (controller.PieceHitAnyFieldObject(piece))
        {
            Destroy(piece.gameObject);
        }
        else
        {
            //FieldObject作成.
            var remainPiece = piece.CreateRemainPiece();

            //FieldGridに配置.
            foreach(var c in piece.CellInfos)
            {
                var fieldX = c.offset.x + piece.PosX;
                var fieldY = c.offset.y + piece.PosY;

                controller.fieldGrid.GetCell(fieldX, fieldY).OccupiedObject = remainPiece;
            }

            //PuzzleGridから削除.
            foreach(var c in piece.CellInfos)
            {
                var puzzleX = c.offset.x + piece.PosX;
                var puzzleY = c.offset.y + piece.PosY;

                controller.puzzleGrid.GetCell(puzzleX, puzzleY).OccupiedObject = null;
            }
        }
    }

    public override void Exit()
    {
        controller.ClearPieceList();
    }
}