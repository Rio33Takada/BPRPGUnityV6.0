using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemySpawnInfo")]

public class EnemySpawnInfo : ScriptableObject
{
    public EnemyData enemyData;

    public Vector2Int spawnPos;
}
