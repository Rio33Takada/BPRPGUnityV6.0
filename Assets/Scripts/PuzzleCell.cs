public class PuzzleCell
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public PuzzleObject OccupiedObject { get; set; }

    public PuzzleCell(int x, int y)
    {
        X = x;
        Y = y;
        OccupiedObject = null;
    }

    public bool IsOccupied => OccupiedObject != null;
}
