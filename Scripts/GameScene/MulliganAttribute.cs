using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MulliganAttribute : MonoBehaviour
{
    public CardSO card;

    public float mana;
    // Minion ise
    public float attack;
    public float hp;

    public bool mulligan; // true = kartý shufflelayacaðýz
    public GameObject mulliganImage;

    private void Update()
    {
        GetComponent<RawImage>().texture = card.cardSprite.texture;
        transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = card.mana.ToString();
        transform.Find("Mana").GetComponent<RectTransform>().localPosition = card.legendary ? new Vector3(-100.6f, 153.1f, 0) : new Vector3(-100.6f, 167.9f, 0);
        transform.Find("Attack").gameObject.SetActive(card.cardType == CardType.MINION || card.cardType == CardType.WEAPON);
        if (card.cardType == CardType.MINION) transform.Find("Attack").GetComponent<TextMeshProUGUI>().text = card.cardType == CardType.MINION ? card.attack.ToString() : card.weaponAttack.ToString();
        transform.Find("Health").gameObject.SetActive(card.cardType == CardType.MINION || card.cardType == CardType.WEAPON);
        if (card.cardType == CardType.MINION) transform.Find("Health").GetComponent<TextMeshProUGUI>().text = card.cardType == CardType.MINION ? card.hp.ToString() : card.weaponDurability.ToString();
        mulliganImage.SetActive(mulligan);
    }

    public void ClickOnCard()
    {
        mulligan = !mulligan;
    }
}
