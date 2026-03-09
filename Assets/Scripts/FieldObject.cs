using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FieldObject
{
    protected int posX;
    protected int posY;
    public int PosX => posX;
    public int PosY => posY;

    public virtual BattleEnemy GetParentEnemy() => null;
}
