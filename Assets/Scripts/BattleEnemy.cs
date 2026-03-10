using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy
{
    private readonly EnemySpawnInfo spawnInfo;

    public EnemyData EnemyData { get { return spawnInfo.enemyData; } }

    public Vector2Int SpawnPos { get { return spawnInfo.spawnPos; } }

    public GameObject bodyCell;

    public EnemyBody body;

    private ParticleSystem stunParticle;

    public int CurrentHp { get; private set; }

    public int MaxHp => EnemyData.maxHp;

    public bool IsDead => CurrentHp <= 0;

    public bool IsStun {  get; private set; }

    public List<RemainPieceObject> surroundingPieces = new List<RemainPieceObject>();

    public BattleEnemy(EnemySpawnInfo spawnInfo)
    {
        this.spawnInfo = spawnInfo;
    }

    public void InitializeStatus()
    {
        CurrentHp = MaxHp;
        IsStun = false;
    }

    public void RegisterParticleSystem()
    {
        stunParticle = bodyCell.GetComponentInChildren<ParticleSystem>();
    }

    private IEnumerable<Vector2Int> GetBodyPositions(int originX, int originY)
    {
        foreach (var bodyCellOffset in EnemyData.bodyCells)
        {
            yield return new Vector2Int(originX + bodyCellOffset.x, originY + bodyCellOffset.y);
        }
    }

    private bool CanOccupy(FieldGrid grid, int originX, int originY)
    {
        foreach (var position in GetBodyPositions(originX, originY))
        {
            var cell = grid.GetCell(position.x, position.y);
            if (cell == null)
            {
                Debug.Log($"Enemyは{position.x},{position.y}の位置に配置できません(範囲外)");
                return false;
            }

            var occupiedObject = cell.OccupiedObject;
            if (occupiedObject != null && occupiedObject != body)
            {
                Debug.Log($"Enemyは{position.x},{position.y}の位置に配置できません(占有済み)");
                return false;
            }
        }

        return true;
    }

    public void SetOccupiedCells(FieldGrid grid, int originX, int originY, FieldObject fieldObject)
    {
        foreach (var position in GetBodyPositions(originX, originY))
        {
            grid.GetCell(position.x, position.y).OccupiedObject = fieldObject;
        }
    }

    public bool Spawn(FieldGrid grid)
    {
        if (!CanOccupy(grid, SpawnPos.x, SpawnPos.y)) return false;

        body = new EnemyBody(this, SpawnPos.x, SpawnPos.y);
        Debug.Log($"Enemyを{SpawnPos}にスポーンしました");

        InitializeStatus();
        SetOccupiedCells(grid, SpawnPos.x, SpawnPos.y, body);

        return true;
    }

    public void StartTurn()
    {
        if (IsStun && stunParticle != null) stunParticle.Stop();
        IsStun = false;
        surroundingPieces.Clear();
    }

    public void Move(FieldGrid grid)
    {
        if(IsStun) return;
        var targetPosX = body.PosX;
        var targetPosY = body.PosY + 1;

        if (!CanOccupy(grid, targetPosX, targetPosY))
        {
            Debug.Log($"Enemyは{targetPosX},{targetPosY}の位置に移動できません");
            return;
        }

        SetOccupiedCells(grid, body.PosX, body.PosY, null);
        SetOccupiedCells(grid, targetPosX, targetPosY, body);
        body.SetPosition(targetPosX, targetPosY);

        var pos = new Vector3(targetPosX * grid.CellSize, 0, targetPosY * grid.CellSize);
        bodyCell.transform.position = pos;
    }

    public void TakeDamage(int amount)
    {
        CurrentHp -= amount;
    }

    public void CheckStun(FieldGrid grid)
    {
        surroundingPieces.Clear();
        var CheckPosList = new List<Vector2Int>();
        var directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };
        foreach(var position in GetBodyPositions(body.PosX, body.PosY))
        {
            foreach(var pos in directions)
            {
                var checkPos = pos + position;
                var cell = grid.GetCell(checkPos.x, checkPos.y);
                if (cell == null) continue;

                if (!CheckPosList.Contains(checkPos) && cell.OccupiedObject != body)
                {
                    CheckPosList.Add(checkPos);
                }
            }
        }

        foreach(var position in CheckPosList)
        {
            if(grid.GetCell(position.x, position.y).OccupiedObject is RemainPieceObject obj)
            {
                surroundingPieces.Add(obj);
                obj.nearestEnemy = this;
            }
            else
            {
                surroundingPieces.Clear();
                return;
            }
        }

        IsStun = true;
        if (stunParticle != null) stunParticle.Play();
        Debug.Log($"{EnemyData.enemyName}はスタン状態になった");
    }
}