using System.Collections;
using UnityEngine;

public class EnemyBody : FieldObject
{
    private readonly BattleEnemy battleEnemy;

    public override BattleEnemy GetParentEnemy() => battleEnemy;

    public float moveDuration = 0.25f;

    public EnemyBody(BattleEnemy enemy, int x, int y)
    {
        battleEnemy = enemy;
        posX = x;
        posY = y;
    }

    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }
}