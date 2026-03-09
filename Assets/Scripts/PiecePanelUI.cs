using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PiecePanelUI : MonoBehaviour, IPointerDownHandler
{
    public BattleCharacter character;
    public BattleController controller;

    public void OnPointerDown(PointerEventData eventData)
    {
        var prefab = character.GetNextPiece();
        if (prefab == null) return;
        GameObject obj = Instantiate(prefab);
        var drag = obj.GetComponent<CharacterPiece>();

        drag.BeginDrag(character);
    }
}
