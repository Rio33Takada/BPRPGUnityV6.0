public class FieldCell
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public FieldObject OccupiedObject { get; set; }

    public FieldCell(int x, int y)
    {
        X = x;
        Y = y;
        OccupiedObject = null;
    }

    public bool IsOccupied => OccupiedObject != null;
}
