using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyData")]

public class EnemyData : ScriptableObject
{
    public string enemyName;

    public int maxHp;

    public List<Vector2Int> bodyCells;

    public GameObject enemyPrefab;

    public virtual void Attack(BattleController controller)
    {
        Debug.Log($"{enemyName}ÇÕçUåÇÇµÇΩ");
    }
}
