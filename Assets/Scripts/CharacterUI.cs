using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public BattleCharacter BattleCharacter {  get; private set; }

    public Transform effectIconParent;
    public Image icon;
    public Text nameText;

    public void InitializeUI(BattleCharacter character)
    {
        BattleCharacter = character;
        GetComponentInChildren<PiecePanelUI>().character = BattleCharacter;

        UpdateUI();
    }

    public void UpdateUI()
    {
        icon.sprite = BattleCharacter.CharacterData.iconSprite;
        nameText.text = BattleCharacter.CharacterData.characterName;
    }
}
