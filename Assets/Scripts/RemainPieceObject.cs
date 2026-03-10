using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainPieceObject : FieldObject
{
    private readonly BattleCharacter character;
    private readonly CharacterPiece characterPiece;
    public int DurabilityPoint { get; private set; }

    public BattleEnemy nearestEnemy;

    public RemainPieceObject(CharacterPiece piece)
    {
        posX = piece.PosX;
        posY = piece.PosY;

        character = piece.character;
        characterPiece = piece;

        DurabilityPoint = piece.GetCellInfoCopy().Count;
    }

    public CharacterPiece GetCharacterPiece() { return characterPiece; }

    public void PopOutAnimation(float cellSize)
    {
        characterPiece.PopOutPiece(nearestEnemy.body.PosX, nearestEnemy.body.PosY, cellSize);
    }
}
