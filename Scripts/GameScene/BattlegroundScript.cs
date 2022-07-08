using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class BattlegroundScript : MonoBehaviour
{
    public GameObject minionPrefab;
    public GameObject cardPrefab;
    public GameObject newMinion;

    private GameplayManager _gameplayManager => GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

    private void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drop;
        entry.callback.AddListener((data) => { OnPointerDropDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public float ChildCount()
    {
        float _childCount = GameObject.FindGameObjectsWithTag($"{(GameplayManager.turn == Turn.PLAYER ? "Player" : "Opponent")} Minion").Count();
        return _childCount;
    }

    public float OpponentChildCount()
    {
        float _childCount = GameObject.FindGameObjectsWithTag($"{(GameplayManager.turn == Turn.OPPONENT ? "Player" : "Opponent")} Minion").Count();
        return _childCount;
    }

    public void SpawnNewMinion(HeroScript hero, GameObject dataPointer, CardScript card)
    {
        hero.currentMana -= card.mana;
        newMinion = Instantiate(minionPrefab, transform);
        newMinion.GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.mousePosition.x - 514, -75);
        newMinion.GetComponent<MinionScript>().Card = card.card;
        newMinion.GetComponent<MinionScript>().UpdateStats();
        newMinion.GetComponent<MinionScript>().currentAttack = card.attack;
        newMinion.GetComponent<MinionScript>().currentHP = card.hp;
        newMinion.GetComponent<MinionScript>().defaultAttack = card.attack;
        newMinion.GetComponent<MinionScript>().defaultHP = card.hp;
        newMinion.name = card.card.cardName;
        newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
        newMinion.GetComponent<MinionScript>().numberOnBoard = newMinion.GetComponent<MinionScript>().CalculateNumberOnBoard(Input.mousePosition.x - 514);
        hero.battleground.Add(newMinion);

        IEnumerable<MinionAbility> abilityList =
            from _card in card.card.minionAbilites
            select _card.minionAbility;
        switch (abilityList)
        {
            case var _ when abilityList.Contains(MinionAbility.Charge):
                newMinion.GetComponent<MinionScript>().attackedThisTurn = false;
                newMinion.GetComponent<MinionScript>().canAttackHeroes = true;
                break;
            case var _ when abilityList.Contains(MinionAbility.Rush):
                newMinion.GetComponent<MinionScript>().attackedThisTurn = false;
                newMinion.GetComponent<MinionScript>().canAttackHeroes = false;
                break;
        }
        foreach (MinionAbilites _card in card.card.minionAbilites)
        {
            if (_card.minionEffect != null && _card.minionAbility == MinionAbility.Battlecry)
            {
                Type scriptType = typeof(MinionCardEffects);
                MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                info?.Invoke(GameObject.Find("GameplayManager").GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, newMinion.GetComponent<MinionScript>() });
            }
        }
        hero.hand.Remove(card.gameObject);
        foreach (GameObject item in GameObject.FindGameObjectsWithTag($"{(newMinion.transform.root.name == "Player" ? "Player" : "Opponent")} Minion"))
        {
            item.GetComponent<MinionScript>().numberOnBoard = item.GetComponent<MinionScript>().CalculateNumberOnBoard(item.GetComponent<RectTransform>().anchoredPosition.x);
            if (item.GetComponent<MinionScript>().numberOnBoard >= newMinion.GetComponent<MinionScript>().numberOnBoard && item != newMinion) item.GetComponent<MinionScript>().numberOnBoard++;
            item.GetComponent<RectTransform>().DOAnchorPos(item.GetComponent<MinionScript>().BasePosition(), .5f);
        }

        if (card.card.minionAbilites.Any(a => a.minionAbility == MinionAbility.DivineShield))
        {
            List<string> itemsToRemove = new();
            IEnumerable<string> effectList =
                from effect in transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects
                select effect.effectName;
            switch (effectList)
            {
                case var k when effectList.Contains("BornAChampion"):
                    hero.currentQProgress++;
                    if (hero.currentQProgress >= hero.maxQProgress)
                    {
                        if (hero.hand.Count < 10)
                        {
                            GameObject newCard = Instantiate(cardPrefab);
                            newCard.transform.SetParent(hero.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
                            newCard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                            newCard.GetComponent<CardScript>().card = hero.currentQuest.tokens[0];
                            newCard.name = hero.currentQuest.tokens[0].cardName;
                            hero.hand.Add(newCard);
                        }
                        hero.currentQuest = null;
                        transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Remove(transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.FirstOrDefault(a => a.effectName == "BornAChampion"));
                    }
                    break;
                case var k when effectList.Contains("DivineShieldCosts1Less"):
                    transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Remove(transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.FirstOrDefault(a => a.effectName == "DivineShieldCosts1Less"));
                    break;
            }
        }

        _gameplayManager.AddNewLog(card, hero.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);

        Destroy(card.gameObject);
    }

    public GameObject cardOnBoardPrefab;

    public void PlaySpell(HeroScript hero, GameObject dataPointer, CardScript card)
    {
        if ((card.card.cardTarget != CardTarget.MINIONS && card.card.cardTarget != CardTarget.FRIENDLYMINION && card.card.cardTarget != CardTarget.OPPONENTMINION) || (card.card.cardTarget == CardTarget.MINIONS && (ChildCount() > 0 || OpponentChildCount() > 0)) || (card.card.cardTarget == CardTarget.FRIENDLYMINION && ChildCount() > 0) || (card.card.cardTarget == CardTarget.OPPONENTMINION && OpponentChildCount() > 0))
        {
            hero.currentMana -= card.mana;
            GameObject cardOnBoard = Instantiate(cardOnBoardPrefab, GameObject.Find("GlobalCanvas").transform);
            cardOnBoard.GetComponent<CardOnBoardScript>().card = card.card;
            cardOnBoard.transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = card.mana.ToString();
            cardOnBoard.GetComponent<RawImage>().texture = card.card.cardSprite.texture;
            cardOnBoard.name = card.card.cardName;
            cardOnBoard.tag = card.transform.root.name == "Player" ? "Player Spell" : "Opponent Spell";
            Destroy(cardOnBoard, .9f);
            Type scriptType = typeof(SpellCardEffects);
            MethodInfo info = scriptType.GetMethod(card.card.spellEffect);
            info?.Invoke(GameObject.Find("GameplayManager").GetComponent<SpellCardEffects>(), new object[] { new List<GameObject> { gameObject }, card });
            hero.hand.Remove(card.gameObject);

            _gameplayManager.AddNewLog(card, hero.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
            
            Destroy(card.gameObject);
        }
    }

    void PlayWeapon(HeroScript hero, CardScript card)
    {
        IEnumerable<string> effectList =
            from effect in hero._currentEffects
            select effect.effectName;
        if (effectList.Contains("DivineShieldCosts1Less"))
            hero._currentEffects.Remove(transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.FirstOrDefault(a => a.effectName == "DivineShieldCosts1Less"));
    }

    public void OnPointerDropDelegate(PointerEventData data)
    {
        if (data.pointerDrag.CompareTag("Card"))
        {
            HeroScript hero = transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
            CardScript card = data.pointerDrag.GetComponent<CardScript>();
            if ((GameplayManager.turn == Turn.PLAYER && hero.transform.root.name == "Player" && gameObject.transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && hero.transform.root.name == "Opponent" && gameObject.transform.root.name == "Opponent"))
            {
                if (card.mana <= hero.currentMana && !(card.canTarget && (card.card.cardTarget == CardTarget.ALL || card.card.cardTarget == CardTarget.FRIENDLYALL || card.card.cardTarget == CardTarget.OPPONENTALL || (card.card.cardTarget == CardTarget.FRIENDLYMINION && card.GameObjectCount("Player Minion") > 0) || (card.card.cardTarget == CardTarget.OPPONENTMINION && card.GameObjectCount("Opponent Minion") > 0) || (card.card.cardTarget == CardTarget.MINIONS && (card.GameObjectCount("Player Minion") > 0 || card.GameObjectCount("Opponent Minion") > 0)))))
                {
                    if (card.card.cardType == CardType.MINION && ChildCount() < 7)
                    {
                        SpawnNewMinion(hero, data.pointerDrag, card);
                        IEnumerable<GameObject> _cards =
                                from _card in hero.hand
                                where (_card.GetComponent<CardScript>().mana < card.mana && _card.GetComponent<CardScript>().card.corruptable == Corruptable.TRUE)
                                select _card;
                        foreach (var item in _cards) item.GetComponent<CardScript>().card = item.GetComponent<CardScript>().card.corruptedVersion;
                    }

                    else if (card.card.cardType == CardType.SPELL || card.card.cardType == CardType.HERO)
                    {
                        PlaySpell(hero, data.pointerDrag, card);
                        IEnumerable<GameObject> _cards =
                                from _card in hero.hand
                                where (_card.GetComponent<CardScript>().mana < card.mana && _card.GetComponent<CardScript>().card.corruptable == Corruptable.TRUE)
                                select _card;
                        foreach (var item in _cards) item.GetComponent<CardScript>().card = item.GetComponent<CardScript>().card.corruptedVersion;
                    }

                    else if (card.card.cardType == CardType.WEAPON)
                    {
                        hero.currentMana -= card.mana;
                        if (hero.weaponSlot is { })
                        {
                            if (hero.weaponSlot.weaponAbilities.Any(a => a.weaponAbility == WeaponAbility.Deathrattle))
                            {
                                foreach (WeaponAbilities _card in card.card.weaponAbilities)
                                {
                                    if (_card.weaponEffect != null && _card.weaponAbility == WeaponAbility.Deathrattle)
                                    {
                                        Type scriptType = typeof(WeaponCardEffects);
                                        MethodInfo info = scriptType.GetMethod(_card.weaponEffect);
                                        info?.Invoke(GameObject.Find("GameplayManager").GetComponent<WeaponCardEffects>(), new object[] { new List<GameObject> { gameObject }, hero });
                                    }
                                }
                            }
                        }
                        hero.EquipWeapon(card.card);
                        foreach (WeaponAbilities _card in card.card.weaponAbilities)
                        {
                            if (_card.weaponEffect != null && _card.weaponAbility == WeaponAbility.Battlecry)
                            {
                                Type scriptType = typeof(WeaponCardEffects);
                                MethodInfo info = scriptType.GetMethod(_card.weaponEffect);
                                info?.Invoke(GameObject.Find("GameplayManager").GetComponent<WeaponCardEffects>(), new object[] { new List<GameObject> { gameObject }, hero });
                            }
                        }
                        PlayWeapon(hero, card);
                        hero.hand.Remove(card.gameObject);

                        _gameplayManager.AddNewLog(card, hero.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);

                        Destroy(card.gameObject);
                    }
                }
            }
            hero.RotateAllCardsInHand();
        }
    }
}