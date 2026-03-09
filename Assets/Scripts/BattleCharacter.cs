using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BattleCharacter
{
    private readonly CharacterData characterData;

    public CharacterData CharacterData { get { return characterData; } }

    private int pieceIndex;

    public bool IsMaxPieceIndex => pieceIndex >= CharacterData.OwnPieces.Count;

    public int Hp => characterData.hp;

    public string Name => characterData.characterName;

    public BattleCharacter(CharacterData characterData)
    {
        this.characterData = characterData;
        pieceIndex = 0;
    }

    public GameObject GetNextPiece()
    {
        if (pieceIndex < CharacterData.OwnPieces.Count)
        {
            return CharacterData.OwnPieces[pieceIndex];
        }
        return null;
    }

    public void PiecePlaced()
    {
        pieceIndex++;
    }

    public void StartTurn()
    {
        pieceIndex = 0;
    }
}
