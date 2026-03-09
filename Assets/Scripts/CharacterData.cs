using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CharaData")]

public class CharacterData : ScriptableObject
{
    public string characterName;
    public int hp;
    public int attack;
    public int defence;

    public List<GameObject> OwnPieces;

    public Sprite iconSprite;
}
