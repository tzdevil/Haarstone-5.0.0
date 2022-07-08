using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnCollection : MonoBehaviour
{
    public CardSO card;
    public float mana;
    // Minion ise
    public float attack;
    public float hp;
    public float maxHP;

    [HideInInspector]
    public CollectionManager cm;

    private void Start()
    {
        cm = GameObject.Find("CollectionManager").GetComponent<CollectionManager>();

        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    public void UpdateStats()
    {
        mana = card.mana;
        attack = card.attack;
        hp = card.hp;
        maxHP = card.hp;
    }

    private void Update()
    {
        transform.Find("CardImage").GetComponent<RawImage>().texture = card.cardSprite.texture;
        if (card.HasBugs) transform.Find("CardImage").GetComponent<RawImage>().color = new Color32(188, 84, 84, 255);
        else if (card.NotReleased) transform.Find("CardImage").GetComponent<RawImage>().color = new Color32(95, 150, 239, 255);
        else transform.Find("CardImage").GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);

        transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = mana.ToString();
        transform.Find("Mana").GetComponent<TextMeshProUGUI>().GetComponent<RectTransform>().anchoredPosition = card.legendary ? new Vector3(-54.7f, 86.3f, 0) : new Vector3(-55f, 93.8f, 0);

        transform.Find("Attack").gameObject.SetActive(card.cardType == CardType.MINION || card.cardType == CardType.WEAPON);
        if (card.cardType == CardType.MINION || card.cardType == CardType.WEAPON) 
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().text = card.cardType == CardType.MINION ? attack.ToString() : card.weaponAttack.ToString();
        
        transform.Find("Health").gameObject.SetActive(card.cardType == CardType.MINION || card.cardType == CardType.WEAPON);
        if (card.cardType == CardType.MINION || card.cardType == CardType.WEAPON)
            transform.Find("Health").GetComponent<TextMeshProUGUI>().text = card.cardType == CardType.MINION ? hp.ToString() : card.weaponDurability.ToString();
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

    public GameObject cardOnDeckPrefab;

    public void AddToDeck()
    {
        if (card.HasBugs || card.NotReleased) return;

        if (PlayerDatabase.currentDeck.Count < 30)
        {
            if (cm.creatingDeck && ((card.legendary && CardInDeckCount(card) < 1) || (!card.legendary && CardInDeckCount(card) < 2)))
            {
                PlayerDatabase.currentDeck.Add(card);
                if (CardInDeckCount(card) != 1)
                {
                    GameObject cardOnDeck = Instantiate(cardOnDeckPrefab, GameObject.Find("NewDeck").transform);
                    cm.currDeck.Add(cardOnDeck.GetComponent<CardOnDeck>());
                    cardOnDeck.GetComponent<CardOnDeck>().card = card;
                }
                else
                {
                    CardOnDeck _card = cm.currDeck.FirstOrDefault(a => a.card == card);
                    if (_card != null) _card.count = 2;
                }
                cm.SortCardsInDeck();
            }
        }
    }

    public async void CheckCard()
    {
        if (cm.creatingDeck) return;

        cm.cardOnCollectionBigPrefab.GetComponent<CardOnCollectionBig>().card = card;
        await Task.Delay(50);
        cm.cardOnCollectionBigPrefab.SetActive(true);
    }
}
