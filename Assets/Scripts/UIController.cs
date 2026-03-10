using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private BattleController battleController;

    [SerializeField] GameObject charaUIPrefab;
    [SerializeField] Transform charaUIParent;

    [SerializeField] GameObject dicideButtonPrefab;
    [SerializeField] Transform dicideButtonPosition;

    [SerializeField] GameObject damageTextPrefab;

    [SerializeField] GameObject playerHpBar;
    [SerializeField] Transform playerHpBarPosition;
    private Slider playerHpBarSlider;

    private Dictionary<BattleCharacter, CharacterUI> charaUIList = new Dictionary<BattleCharacter, CharacterUI>();

    public void CreateUI(BattleController battleController)
    {
        this.battleController = battleController;

        foreach (var c in battleController.GetPlayerTeamCopy())
        {
            var uIObject = Instantiate(charaUIPrefab, charaUIParent).GetComponent<CharacterUI>();
            uIObject.InitializeUI(c);

            uIObject.GetComponentInChildren<PiecePanelUI>().controller = battleController;

            charaUIList.Add(c, uIObject);
        }

        var barObj = Instantiate(playerHpBar, playerHpBarPosition);
        playerHpBarSlider = barObj.GetComponent<Slider>();
    }

    public void CreateDicideButton()
    {
        var buttonObj = Instantiate(dicideButtonPrefab, dicideButtonPosition);
        var button = buttonObj.GetComponent<Button>();

        button.onClick.AddListener(() => battleController.PlayerConfirmPlacedPiece(buttonObj));
    }

    public void CreateDamegeText(BattleEnemy enemy, int damage)
    {
        var pos = new Vector3(enemy.body.PosX, 2.0f, enemy.body.PosY);
        var canvas = Instantiate(damageTextPrefab, pos, Quaternion.identity);
        canvas.GetComponentInChildren<Text>().text = damage.ToString();
        StartCoroutine(AnimateText(canvas));
    }

    private IEnumerator AnimateText(GameObject canvas)
    {
        Vector3 start = canvas.transform.position;
        Vector3 end = new Vector3(start.x, start.y + 2, start.z);

        float duration = 1;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = 1f - (1f - t) * (1f - t);

            canvas.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        canvas.transform.position = end;
        Destroy(canvas);
    }

    public void UpdateUI()
    {
        playerHpBarSlider.value = battleController.playerCurrentHp / battleController.playerMaxHp;
    }
}
