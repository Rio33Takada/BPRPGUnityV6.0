using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterPiece : PuzzleObject
{
    public static event Action<CharacterPiece, Vector2Int> OnPiecePlaced;

    private Camera mainCamera;
    public BattleCharacter character;
    private bool isDragging;

    public List<PieceCellInfo> CellInfos = new List<PieceCellInfo>();

    public List<PieceCellInfo> GetCellInfoCopy()
    {
        return new List<PieceCellInfo>(CellInfos);
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void BeginDrag(BattleCharacter character)
    {
        isDragging = true;
        this.character = character;
    }

    private void Update()
    {
        if (!isDragging) return;

        FollowMouse();

        if (Input.GetMouseButtonUp(0))
        {
            TryPlace();
        }
    }

    private void FollowMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;
        }
    }

    private void TryPlace()
    {
        isDragging = false;

        var grid = GameObject.Find("PuzzleGrid").GetComponent<PuzzleGrid>();

        Vector3 pos = transform.position;

        int x = Mathf.RoundToInt(pos.x / grid.CellSize);
        int y = Mathf.RoundToInt(pos.z / grid.CellSize);

        Vector3 snappedPos = new Vector3(x * grid.CellSize, 5, y * grid.CellSize);

        bool canPlace = true;

        foreach (var c in CellInfos)
        {
            var checkX = x + c.offset.x;
            var checkY = y + c.offset.y;

            if (grid.GetCell(checkX, checkY) == null)
            {
                Debug.Log($"このピースは{x},{y}の位置に配置できません(範囲外)");
                canPlace = false;
            }
            else if (grid.GetCell(checkX, checkY).IsOccupied && !(grid.GetCell(checkX, checkY).OccupiedObject is CharacterPiece))
            {
                Debug.Log($"このピースは{x},{y}の位置に配置できません(占有済み)");
                canPlace = false;
            }
        }

        if (canPlace)
        {
            transform.position = snappedPos;
            posX = x;
            posY = y;

            foreach (var c in CellInfos)
            {
                var placeX = x + c.offset.x;
                var placeY = y + c.offset.y;

                if (grid.GetCell(placeX, placeY).OccupiedObject is CharacterPiece piece)
                {
                    var copyList = piece.GetCellInfoCopy();
                    foreach (var cell in piece.CellInfos)
                    {
                        var targetPos = cell.offset + new Vector2Int(piece.posX, piece.PosY);
                        var replacePos = new Vector2Int(placeX, placeY);

                        if (targetPos == replacePos)
                        {
                            copyList.Remove(cell);
                            Destroy(cell.gameObject);
                            grid.GetCell(replacePos.x, replacePos.y).OccupiedObject = this;
                        }
                    }
                    piece.CellInfos = copyList;
                }

                grid.GetCell(placeX, placeY).OccupiedObject = this;
            }

            character.PiecePlaced();
            OnPiecePlaced?.Invoke(this, new Vector2Int(x, y));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public RemainPieceObject CreateRemainPiece()
    {
        var remainPiece = new RemainPieceObject(this);
        return remainPiece;
    }
}
