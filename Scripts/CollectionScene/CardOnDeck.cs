using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardOnDeck : MonoBehaviour
{
    public CardSO card;
    public int mana;
    public string cardName;
    public bool legendary; // legendary ise * yaparýz countu lol
    public int count; // 2 tane varsa 2 lol

    public int numberOnDeck;

    public CollectionManager cm;

    private void Start()
    {
        cm = GameObject.Find("CollectionManager").GetComponent<CollectionManager>();
        mana = (int)card.mana;
        cardName = card.name;
        count = 1;
        legendary = card.legendary;
    }

    public int CardInDeckCount(CardSO thisCard, int count = 0)
    {
        if (cm.currDeck.Count > 0)
        {
            IEnumerable<CardOnDeck> data =
                from _card in cm.currDeck
                where (thisCard == _card.card)
                select _card;
            count = data.ToList().Count;
        }
        return count;
    }

    public void UpdateStats()
    {
        mana = (int)card.mana;
        cardName = card.name;
        count = 1;
        legendary = card.legendary;
    }

    private void Update()
    {
        transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = mana.ToString();
        transform.Find("Name").GetComponent<TextMeshProUGUI>().text = cardName;
        transform.Find("Count").GetComponent<TextMeshProUGUI>().text = legendary ? "*" : count.ToString();
        transform.Find("Mask").Find("MinionImage").GetComponent<SpriteRenderer>().sprite = card.cardSprite;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(646, 400 - numberOnDeck * 47);
    }

    public void RemoveFromDeck()
    {
        if (cm.creatingDeck)
        {
            if (CardInDeckCount(card) == 2)
            {
                count = 1;
            }
            else
            {
                cm.currDeck.Remove(this);
                Destroy(gameObject);
            }
            PlayerDatabase.currentDeck.Remove(card);
            cm.SortCardsInDeck();
        }
    }
}
