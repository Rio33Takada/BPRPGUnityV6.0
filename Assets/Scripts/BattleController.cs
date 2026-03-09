using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleController : MonoBehaviour
{
    public UIController uIController;

    public FieldGrid fieldGrid;
    public PuzzleGrid puzzleGrid;

    private IBattleState currentState;
    public BattleStateBase startState;

    public List<CharacterData> teamData;
    private List<BattleCharacter> playerTeam = new List<BattleCharacter>();

    public List<EnemySpawnInfo> enemySpawns;
    private List<BattleEnemy> enemies = new List<BattleEnemy>();

    private List<CharacterPiece> pieceList = new List<CharacterPiece>();

    public CameraProjectionMover mainCamera;

    private void Awake()
    {
        CharacterPiece.OnPiecePlaced += HandlePiecePlaced;
    }

    private void Start()
    {
        PrepareBattle();
    }

    private void CreatePlayerTeam()
    {
        foreach(var c in teamData)
        {
            var chara = new BattleCharacter(c);
            playerTeam.Add(chara);
        }

    }

    public List<BattleCharacter> GetPlayerTeamCopy()
    {
        return new List<BattleCharacter>(playerTeam);
    }

    public List<CharacterPiece> GetPieceListCopy()
    {
        return new List<CharacterPiece>(pieceList);
    }

    public void ClearPieceList()
    {
        pieceList.Clear();
    }

    public List<BattleEnemy> GetBattleEnemiesCopy()
    {
        return new List<BattleEnemy>(enemies);
    }

    private void CreateEnemies()
    {
        foreach(var e in enemySpawns)
        {
            var enemy = new BattleEnemy(e);

            if (enemy.Spawn(fieldGrid))
            {
                enemies.Add(enemy);
            }
        }
    }

    public List<BattleEnemy> GetBattleEnemies()
    {
        return new List<BattleEnemy>(enemies);
    }

    public void SetBattleEnemies(List<BattleEnemy> battleEnemies)
    {
        enemies = battleEnemies;
    }

    private void GenerateEnemyCells()
    {
        foreach (var e in enemies)
        {
            if (e.bodyCell == null)
            {
                Vector3 pos = new Vector3(e.SpawnPos.x, 0, e.SpawnPos.y);

                GameObject b = Instantiate(e.EnemyData.enemyPrefab, pos, Quaternion.identity);
                e.bodyCell = b;
                e.RegisterParticleSystem();
            }
        }
    }

    public void PrepareBattle()
    {
        //データ作成.
        CreatePlayerTeam();
        CreateEnemies();

        //UI生成.
        uIController.CreateUI(this);

        //見た目生成.
        GenerateEnemyCells();

        //バトル開始.
        ChangeState(startState);
    }

    public void ToggleCamera()
    {
        mainCamera.Toggle();
    }

    public void ChangeState(IBattleState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    public void HandlePiecePlaced(CharacterPiece piece, Vector2Int origin)
    {
        pieceList.Add(piece);

        foreach(var c in playerTeam)
        {
            if (!c.IsMaxPieceIndex) return;
        }

        //決定ボタンUIを表示.
        Debug.Log("HandlePiecePlaced");
        uIController.CreateDicideButton();
    }

    public void PlayerConfirmPlacedPiece(GameObject obj)
    {
        Debug.Log("ConfirmPlacedPiece");

        currentState?.OnConfirmPlacedPiece();

        Destroy(obj);
    }

    public Dictionary<BattleEnemy, int> CountDamageToEnemy(CharacterPiece piece)
    {
        var result = new Dictionary<BattleEnemy, int>();

        foreach (var cell in piece.GetCellInfoCopy())
        {
            int posX = cell.offset.x + piece.PosX;
            int posY = cell.offset.y + piece.PosY;

            var fieldCell = fieldGrid.GetCell(posX, posY);

            var enemy = fieldCell.OccupiedObject?.GetParentEnemy();
            if (enemy != null)
            {
                int damage = piece.character.CharacterData.attack * cell.power;

                if (result.ContainsKey(enemy))
                    result[enemy] += damage;
                else
                    result[enemy] = damage;
            }
        }

        return result;
    }

    public bool PieceHitAnyFieldObject(CharacterPiece piece)
    {
        foreach(var cell in piece.GetCellInfoCopy())
        {
            var posX = cell.offset.x + piece.PosX;
            var posY = cell.offset.y + piece.PosY;
            if (fieldGrid.GetCell(posX, posY).OccupiedObject != null)
            {
                return true;
            }
        }
        return false;
    }
}
