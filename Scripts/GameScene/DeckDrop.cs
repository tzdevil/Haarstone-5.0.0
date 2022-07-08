using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckDrop : MonoBehaviour
{
    public DeckScript deck;

    private GameplayManager gameplayManager => GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drop;
        entry.callback.AddListener((data) => { OnPointerDropDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public void OnPointerDropDelegate(PointerEventData data)
    {
        if (data.pointerDrag.CompareTag("Card"))
        {
            HeroScript hero = transform.parent.parent.parent.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
            CardScript card = data.pointerDrag.GetComponent<CardScript>();
            if (hero.currentMana >= 1)
            {
                if (card.card.tradeable)
                {
                    hero.currentMana -= 1;
                    deck.cards.Add(card.card); 
                    hero.hand.Remove(card.gameObject);
                    Destroy(card.gameObject);
                    gameplayManager.DrawCard(card.gameObject, data.pointerDrag.gameObject.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
                    deck.ShuffleDeck();
                }
            }
        }
    }
}
