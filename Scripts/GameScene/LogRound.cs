using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public enum Action
{
    Play, // kartý oynadý
    Attack, // hero ya da minyon saldýrdý
    HeroPower // hero powera bastý
}

[System.Serializable]
public class LogRound
{
    public int RoundNumber; // kaçýncý round?
    public Turn Turn;

    // Effecting Card
    public CardScript Card;
    public CardSO OriginalCard => Card.card;
    public CardType CardType => OriginalCard.cardType;
    public float CardMana => Card.mana;
    public float CardAttack => Card.attack;
    public float CardHealth => Card.hp;

    // IF Targeting a Minion
    public MinionScript TargetedCard;
    public CardSO OriginalTargetedCard => TargetedCard.Card;
    public CardType TargetedCardType => OriginalTargetedCard.cardType;
    public float TargetedCardMana => TargetedCard.mana;
    public float TargetedCardAttack => TargetedCard.currentAttack;
    public float TargetedCardHealth => TargetedCard.currentHP;

    // IF Targeting a Hero
    public HeroScript TargetedHero;
    public float TargetedHeroHealth => TargetedHero.currentHP;

    public string Log() {
        StringBuilder log = new();

        log.Append($"{Turn} played {CardType} {OriginalCard.cardName} ({CardMana} Mana{(CardType == CardType.MINION || CardType == CardType.WEAPON ? $", {CardAttack} Attack" : "")}{(CardType == CardType.MINION || CardType == CardType.WEAPON ? $", {CardHealth} Health)" : ")")}");
        if (TargetedCard != null) log.Append($" targeting {TargetedCardType} {OriginalTargetedCard.cardName} ({TargetedCardMana} Mana{(TargetedCardType == CardType.MINION || TargetedCardType == CardType.WEAPON ? $", {TargetedCardAttack} Attack" : "")}{(TargetedCardType == CardType.MINION || TargetedCardType == CardType.WEAPON ? $", {TargetedCardHealth} Health)" : ")")}");
        if (TargetedHero != null) log.Append($" targeting HERO {TargetedHero.heroName} ({TargetedHeroHealth} Health)");
        log.Append($" at Turn {RoundNumber}.");

        return log.ToString();
    }

    public string AttackLog()
    {
        StringBuilder log = new();

        log.Append($"{Turn} played {CardType} {OriginalCard.cardName} ({CardMana} Mana{(CardType == CardType.MINION || CardType == CardType.WEAPON ? $", {CardAttack} Attack" : "")}{(CardType == CardType.MINION || CardType == CardType.WEAPON ? $", {CardHealth} Health)" : ")")}");
        if (TargetedCard != null) log.Append($" targeting {TargetedCardType} {OriginalTargetedCard.cardName} ({TargetedCardMana} Mana{(TargetedCardType == CardType.MINION || TargetedCardType == CardType.WEAPON ? $", {TargetedCardAttack} Attack" : "")}{(TargetedCardType == CardType.MINION || TargetedCardType == CardType.WEAPON ? $", {TargetedCardHealth} Health)" : ")")}");
        if (TargetedHero != null) log.Append($" targeting HERO {TargetedHero.heroName} ({TargetedHeroHealth} Health)");
        log.Append($" at Turn {RoundNumber}.");

        return log.ToString();
    }
}