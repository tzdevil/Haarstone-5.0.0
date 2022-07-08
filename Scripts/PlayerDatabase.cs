using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Deck
{
    public string deckName;
    public List<CardSO> cards;
    public int deckNumber;
}

public class PlayerDatabase
{
    public static List<Deck> decks = new List<Deck>();
    public static List<CardSO> currentDeck = new List<CardSO>();

    //public static List<CardSO> currentDeck = new List<CardSO>();
}
