using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscoverAttribute : MonoBehaviour
{
    public CardSO card;

    public float mana;
    // Minion ise
    public float attack;
    public float hp;

    private void Update()
    {
        GetComponent<RawImage>().texture = card.cardSprite.texture;
        transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = card.mana.ToString();
        transform.Find("Attack").gameObject.SetActive(card.cardType == CardType.MINION);
        if (card.cardType == CardType.MINION) transform.Find("Attack").GetComponent<TextMeshProUGUI>().text = card.attack.ToString();
        transform.Find("Health").gameObject.SetActive(card.cardType == CardType.MINION);
        if (card.cardType == CardType.MINION) transform.Find("Health").GetComponent<TextMeshProUGUI>().text = card.hp.ToString();
    }

    public void ClickOnCard()
    {
        transform.parent.parent.Find("Hero").GetComponent<HeroScript>().StopDiscoveringCards();
        Type scriptType = typeof(SpellCardEffects);
        MethodInfo info = scriptType.GetMethod(transform.parent.parent.Find("Hero").GetComponent<HeroScript>().discoverFunction);
        info?.Invoke(GameObject.Find("GameplayManager").GetComponent<SpellCardEffects>(), new object[] { new List<GameObject> { gameObject }, gameObject });
    }
}
