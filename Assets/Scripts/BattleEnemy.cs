using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy
{
    private static readonly Vector2Int[] SurroundingDirections =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };

    private readonly EnemySpawnInfo spawnInfo;

    public EnemyData EnemyData { get { return spawnInfo.enemyData; } }

    public Vector2Int SpawnPos { get { return spawnInfo.spawnPos; } }

    public GameObject bodyCell;

    public EnemyBody body;

    private ParticleSystem stunParticle;

    public int CurrentHp { get; private set; }

    public int MaxHp => EnemyData.maxHp;

    public bool IsDead => CurrentHp <= 0;

    public bool IsStun { get; private set; }

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
                Debug.Log($"Enemy cannot be placed at {position.x}, {position.y} (out of range)");
                return false;
            }

            var occupiedObject = cell.OccupiedObject;
            if (occupiedObject != null && occupiedObject != body)
            {
                Debug.Log($"Enemy cannot be placed at {position.x}, {position.y} (already occupied)");
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
        if (!CanOccupy(grid, SpawnPos.x, SpawnPos.y))
        {
            return false;
        }

        body = new EnemyBody(this, SpawnPos.x, SpawnPos.y);
        Debug.Log($"Enemy spawned at {SpawnPos}");

        InitializeStatus();
        SetOccupiedCells(grid, SpawnPos.x, SpawnPos.y, body);

        return true;
    }

    public void StartTurn()
    {
        if (IsStun && stunParticle != null)
        {
            stunParticle.Stop();
        }

        IsStun = false;
        surroundingPieces.Clear();
    }

    public void Move(FieldGrid grid)
    {
        if (IsStun)
        {
            return;
        }

        var targetPosX = body.PosX;
        var targetPosY = body.PosY + 1;

        if (!CanOccupy(grid, targetPosX, targetPosY))
        {
            Debug.Log($"Enemy cannot move to {targetPosX}, {targetPosY}");
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

        var checkPositions = CollectSurroundingPositions(grid);
        if (!TryCollectSurroundingPieces(grid, checkPositions))
        {
            surroundingPieces.Clear();
            return;
        }

        IsStun = true;
        if (stunParticle != null)
        {
            stunParticle.Play();
        }

        Debug.Log($"{EnemyData.enemyName} is stunned");
    }

    private List<Vector2Int> CollectSurroundingPositions(FieldGrid grid)
    {
        var checkPositions = new List<Vector2Int>();

        foreach (var position in GetBodyPositions(body.PosX, body.PosY))
        {
            foreach (var direction in SurroundingDirections)
            {
                var checkPos = direction + position;
                var cell = grid.GetCell(checkPos.x, checkPos.y);
                if (cell == null)
                {
                    continue;
                }

                if (!checkPositions.Contains(checkPos) && cell.OccupiedObject != body)
                {
                    checkPositions.Add(checkPos);
                }
            }
        }

        return checkPositions;
    }

    private bool TryCollectSurroundingPieces(FieldGrid grid, List<Vector2Int> checkPositions)
    {
        foreach (var position in checkPositions)
        {
            if (grid.GetCell(position.x, position.y).OccupiedObject is not RemainPieceObject piece)
            {
                return false;
            }

            surroundingPieces.Add(piece);
            piece.nearestEnemy = this;
        }

        return true;
    }
}
