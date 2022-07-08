using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardOnBoardScript : MonoBehaviour
{
    public CardSO card;
    public GameObject cardPrefab;

    public float mana;
    // Minion ise
    public float attack;
    public float hp;
    public float maxHP;

    public bool canTarget; // e�er bir kart targetlayabiliyosa, kart� ta��mak yerine karttan bir �ey ��kacak ve onu bir targeta g�t�rece�iz. ard�ndan onpointerdrop ile fonksiyonu �al��t�raca��z.

    public float externalManaChange; // cult neopythe gibi kartlardan gelen de�er
    public float debuffTimer;

    private void Update()
    {
        transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = card.mana.ToString();
        transform.Find("Attack").gameObject.SetActive(card.cardType == CardType.MINION);
        if (card.cardType == CardType.MINION) transform.Find("Attack").GetComponent<TextMeshProUGUI>().text = card.attack.ToString();
        transform.Find("Health").gameObject.SetActive(card.cardType == CardType.MINION);
        if (card.cardType == CardType.MINION) transform.Find("Health").GetComponent<TextMeshProUGUI>().text = card.hp.ToString();
        transform.Find("Mana").GetComponent<TextMeshProUGUI>().GetComponent<RectTransform>().anchoredPosition = !card.legendary ? new Vector3(-100.6f, 167.9f, 0) : new Vector3(-100.6f, 151, 0);
    }
}
