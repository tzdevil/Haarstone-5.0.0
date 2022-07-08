using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefabBigScript : MonoBehaviour
{
    public CardSO card;
    public GameObject cardPrefab;
    public GameObject OGcard;

    public float mana;
    // Minion ise
    public float attack;
    public float hp;
    public float maxHP;

    public bool canTarget; // eðer bir kart targetlayabiliyosa, kartý taþýmak yerine karttan bir þey çýkacak ve onu bir targeta götüreceðiz. ardýndan onpointerdrop ile fonksiyonu çalýþtýracaðýz.

    public float externalManaChange; // cult neopythe gibi kartlardan gelen deðer
    public float debuffTimer;

    public GameObject canPlayGameObject;

    private void Update()
    {
        if (mana < 0) mana = 0;
        if (debuffTimer < 0) debuffTimer = 0;
        mana = card.mana + externalManaChange;
        mana += (card.cardType == CardType.MINION) ? transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().externalMinionManaDebuff : transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().externalSpellManaDebuff;

        foreach (MinionAbilites _card in card.minionAbilites)
        {
            if (_card.minionEffect == "ArmorGiant" && _card.minionAbility == MinionAbility.CostsLess)
            {
                mana -= transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().armor;
            }
        }
        if (card.spellEffect == "CallOfKindness")
            mana -= Mathf.Clamp((GameObject.Find(transform.root.name)).transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().hand.Count, 0, card.mana);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(GetComponent<RawImage>().enabled);
        }
        if (GetComponent<RawImage>().enabled)
        {
            canPlayGameObject.SetActive(transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().currentMana >= mana && ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")));
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = mana.ToString();
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().text = attack.ToString();
            transform.Find("Health").GetComponent<TextMeshProUGUI>().text = hp.ToString();
            transform.Find("Attack").gameObject.SetActive(card.cardType == CardType.MINION || card.cardType == CardType.WEAPON);
            transform.Find("Health").gameObject.SetActive(card.cardType == CardType.MINION || card.cardType == CardType.WEAPON); 
            canPlayGameObject.GetComponent<Image>().color = (card.hasCardDrawnThisTurnEffect && OGcard.GetComponent<CardScript>().cardDrawnThisTurn) ? new Color32(210, 207, 50, 255) : new Color32(122, 255, 80, 255);
        }
        if ((card.cardType == CardType.MINION && attack < card.attack)|| (card.cardType == CardType.WEAPON && attack < card.weaponAttack))
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().color = new Color32(109, 36, 36, 255);
        if ((card.cardType == CardType.MINION && attack == card.attack) || (card.cardType == CardType.WEAPON && attack == card.weaponAttack))
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        if ((card.cardType == CardType.MINION && attack > card.attack) || (card.cardType == CardType.WEAPON && attack > card.weaponAttack))
            transform.Find("Attack").GetComponent<TextMeshProUGUI>().color = new Color32(36, 108, 65, 255);

        if ((card.cardType == CardType.MINION && hp < card.hp) || (card.cardType == CardType.WEAPON && hp < card.weaponDurability))
            transform.Find("Health").GetComponent<TextMeshProUGUI>().color = new Color32(109, 36, 36, 255);
        if ((card.cardType == CardType.MINION && hp == card.hp) || (card.cardType == CardType.WEAPON && hp == card.weaponDurability))
            transform.Find("Health").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        if ((card.cardType == CardType.MINION && hp > card.hp) || (card.cardType == CardType.WEAPON && hp > card.weaponDurability))
            transform.Find("Health").GetComponent<TextMeshProUGUI>().color = new Color32(36, 108, 65, 255);

        if (mana > card.mana)
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().color = new Color32(109, 36, 36, 255);
        else if (mana == card.mana)
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        else if (mana < card.mana)
            transform.Find("Mana").GetComponent<TextMeshProUGUI>().color = new Color32(36, 108, 65, 255); 
        transform.Find("Mana").GetComponent<TextMeshProUGUI>().GetComponent<RectTransform>().anchoredPosition = card.legendary ? new Vector3(-88.5f, 134.5f, 0) : new Vector3(-88.5f, 149, 0);
    }

    //public void MouseHoverExitCard()
    //{
    //    OGcard.GetComponent<RawImage>().enabled = true;
    //    Destroy(gameObject);
    //}

    //public void MouseDownCard()
    //{
    //    OGcard.SetActive(true);
    //    OGcard.GetComponent<CardScript>().holding = true;
    //    Destroy(gameObject);
    //}
}
