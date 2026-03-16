using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPiece : PuzzleObject
{
    public static event Action<CharacterPiece, Vector2Int> OnPiecePlaced;

    [SerializeField] private PuzzleGrid puzzleGrid;
    [SerializeField] private float snapHeight = 5f;

    private Camera mainCamera;
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

        if (puzzleGrid == null)
        {
            puzzleGrid = GameObject.Find("PuzzleGrid").GetComponent<PuzzleGrid>();
        }
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
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;
        }
    }

    private void TryPlace()
    {
        isDragging = false;

        if (!TryGetPlacement(out int x, out int y, out Vector3 snappedPos))
        {
            Destroy(gameObject);
            return;
        }

        if (!CanPlaceAt(x, y))
        {
            Destroy(gameObject);
            return;
        }

        PlaceAt(x, y, snappedPos);
    }

    private bool TryGetPlacement(out int x, out int y, out Vector3 snappedPos)
    {
        x = 0;
        y = 0;
        snappedPos = Vector3.zero;

        if (puzzleGrid == null)
        {
            Debug.LogWarning("PuzzleGrid が見つからないため、ピースを配置できません。");
            return false;
        }

        Vector3 pos = transform.position;

        x = Mathf.RoundToInt(pos.x / puzzleGrid.CellSize);
        y = Mathf.RoundToInt(pos.z / puzzleGrid.CellSize);

        snappedPos = new Vector3(x * puzzleGrid.CellSize, snapHeight, y * puzzleGrid.CellSize);
        return true;
    }

    private bool CanPlaceAt(int x, int y)
    {
        foreach (var cellInfo in CellInfoList)
        {
            var checkX = x + cellInfo.offset.x;
            var checkY = y + cellInfo.offset.y;
            var targetCell = puzzleGrid.GetCell(checkX, checkY);

            if (targetCell == null)
            {
                Debug.Log($"このピースは {x},{y} に配置できません（範囲外）。");
                return false;
            }

            if (targetCell.IsOccupied && targetCell.OccupiedObject is not CharacterPiece)
            {
                Debug.Log($"このピースは {x},{y} に配置できません（占有済み）。");
                return false;
            }
        }

        return true;
    }

    private void PlaceAt(int x, int y, Vector3 snappedPos)
    {
        transform.position = snappedPos;
        posX = x;
        posY = y;

        foreach (var cellInfo in CellInfoList)
        {
            var placeX = x + cellInfo.offset.x;
            var placeY = y + cellInfo.offset.y;
            var targetCell = puzzleGrid.GetCell(placeX, placeY);

            if (targetCell?.OccupiedObject is CharacterPiece piece)
            {
                RemoveOverlappedCell(piece, placeX, placeY);
            }

            if (targetCell != null)
            {
                targetCell.OccupiedObject = this;
            }
        }

        character.PiecePlaced();
        OnPiecePlaced?.Invoke(this, new Vector2Int(x, y));
    }

    private void RemoveOverlappedCell(CharacterPiece piece, int replaceX, int replaceY)
    {
        var copyList = piece.GetCellInfoCopy();

        foreach (var cell in piece.CellInfoList)
        {
            var targetPos = cell.offset + new Vector2Int(piece.posX, piece.PosY);
            var replacePos = new Vector2Int(replaceX, replaceY);

            if (targetPos == replacePos)
            {
                copyList.Remove(cell);
                Destroy(cell.gameObject);

                var replacedCell = puzzleGrid.GetCell(replacePos.x, replacePos.y);
                if (replacedCell != null)
                {
                    replacedCell.OccupiedObject = this;
                }

                break;
            }
        }

        piece.CellInfoList = copyList;
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

        Vector3 axis = Vector3.Cross(dir, Vector3.up).normalized;

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            Vector3 pos = Vector3.Lerp(start, end, progress);

            float y = 4 * height * progress * (1 - progress);

            pos.y += y;
            transform.position = pos;

            float rotateSpeed = -120f;
            transform.Rotate(axis, rotateSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        transform.position = end;
        Destroy(gameObject);
    }
}
