using UnityEngine;

[CreateAssetMenu(menuName ="Data/TestEnemy")]

public class TestEnemy : EnemyData
{
    public override void Attack(BattleController controller)
    {
        controller.playerCurrentHp -= 100;
        base.Attack(controller);
    }
}
