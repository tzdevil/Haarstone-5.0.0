using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public List<CardSO> cards;
    public TextMeshProUGUI deckCount;

    public void ShuffleDeck()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            CardSO temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    private void Awake()
    {
        ShuffleDeck();
    }

    private void Update()
    {
        deckCount.text = "Cards left: " + cards.Count;
    }

}
