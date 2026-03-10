using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterPiece : PuzzleObject
{
    public static event Action<CharacterPiece, Vector2Int> OnPiecePlaced;

    private Camera mainCamera;
    private PuzzleGrid puzzleGrid;
    public BattleCharacter character;
    private bool isDragging;

    public List<PieceCellInfo> CellInfoList = new List<PieceCellInfo>();

    public List<PieceCellInfo> GetCellInfoCopy()
    {
        return new List<PieceCellInfo>(CellInfoList);
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        puzzleGrid = FindFirstObjectByType<PuzzleGrid>();
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

        if (puzzleGrid == null)
        {
            Debug.LogError("PuzzleGrid not found.");
            Destroy(gameObject);
            return;
        }

        Vector3 pos = transform.position;

        int x = Mathf.RoundToInt(pos.x / puzzleGrid.CellSize);
        int y = Mathf.RoundToInt(pos.z / puzzleGrid.CellSize);

        Vector3 snappedPos = new Vector3(x * puzzleGrid.CellSize, 5, y * puzzleGrid.CellSize);

        bool canPlace = true;

        foreach (var c in CellInfoList)
        {
            var checkX = x + c.offset.x;
            var checkY = y + c.offset.y;

            var checkCell = puzzleGrid.GetCell(checkX, checkY);

            if (checkCell == null)
            {
            else if (checkCell.IsOccupied && checkCell.OccupiedObject is not CharacterPiece)
                canPlace = false;
            }
                var placeCell = puzzleGrid.GetCell(placeX, placeY);

                if (placeCell.OccupiedObject is CharacterPiece piece)
                            puzzleGrid.GetCell(replacePos.x, replacePos.y).OccupiedObject = this;
                placeCell.OccupiedObject = this;
                canPlace = false;
            }
        }

        if (canPlace)
        {
            transform.position = snappedPos;
            posX = x;
            posY = y;

            foreach (var c in CellInfoList)
            {
                var placeX = x + c.offset.x;
                var placeY = y + c.offset.y;

                if (grid.GetCell(placeX, placeY).OccupiedObject is CharacterPiece piece)
                {
                    var copyList = piece.GetCellInfoCopy();
                    foreach (var cell in piece.CellInfoList)
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
                    piece.CellInfoList = copyList;
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

    public void PopOutPiece(int enemyX, int enemyY, float cellSize)
    {
        var pos = new Vector3(posX, 0, posY);
        var enemyPos = new Vector3(enemyX, 0, enemyY);

        var dir = ((pos - enemyPos) * cellSize).normalized;

        StartCoroutine(PopAnimation(dir));
    }

    IEnumerator PopAnimation(Vector3 dir)
    {
        float duration = 0.8f;
        float height = 2.0f;
        float distance = 3.0f;

        Vector3 start = transform.position;
        Vector3 end = start + dir * distance;

        // dir と垂直（xz平面）
        Vector3 axis = Vector3.Cross(dir, Vector3.up).normalized;

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            // 水平方向
            Vector3 pos = Vector3.Lerp(start, end, progress);

            // 放物線
            float y = 4 * height * progress * (1 - progress);

            pos.y += y;
            transform.position = pos;

            // 回転
            float rotateSpeed = -120f;
            transform.Rotate(axis, rotateSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        transform.position = end;
        Destroy(gameObject);
    }
}
