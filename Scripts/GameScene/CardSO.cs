using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardClass
{
    Neutral,
    DemonHunter, Druid, Hunter, Mage, Paladin, Priest, Rogue, Shaman, Warrior, Warlock
}
public enum CardType { MINION, SPELL, HERO, WEAPON }
public enum MinionAbility { Battlecry, Deathrattle, AttackEffect, Charge, Taunt, CostsLess, DivineShield, Rush, EndOfYourTurn, EndOfEveryTurn, Tradeable, Awaken, Mark, Transform, Form }
public enum CardTarget { SELF, PLAYER, OPPONENT, HEROES, FRIENDLYMINION, FRIENDLYALL, OPPONENTMINION, OPPONENTALL, MINIONS, ALL }

public enum Corruptable { FALSE, TRUE }
public enum CanSpawnTokens { FALSE, TRUE }

public enum WeaponAbility { Battlecry, Attacking, Attacked, Deathrattle}

[System.Serializable]
public struct MinionAbilites
{
    public MinionAbility minionAbility;
    public string minionEffect;
}

[System.Serializable]
public struct WeaponAbilities
{
    public WeaponAbility weaponAbility;
    public string weaponEffect;
}

[CreateAssetMenu(fileName = "New Card", menuName = "New Card", order = 51)]
public class CardSO : ScriptableObject
{
    [Header("Card Settings")]
    public string cardName;
    public CardType cardType;
    public CardClass cardClass;
    public bool HasBugs; // eðer kart buglýysa, rengi kýrmýzý olsun ve oynanamasýn.
    public bool NotReleased; // gelecekte kodlanacak.

    public Sprite cardSprite;
    //public bool hasDoubleClass;
    //public List<ClassSO> cardClass;

    public bool legendary; // legendary ise her desteye bir tane yerleþtirebiliyorsun.

    public bool isToken = false;

    [Header("Script Related")]
    public bool canTarget;
    public CardTarget cardTarget;
    public bool quest;
    public bool hasCardDrawnThisTurnEffect;

    [Header("Stats")]
    public float mana;

    // Spell ise
    public string spellEffect;
    public bool castsWhenDrawn;

    // Quest ise
    public Color classColor;

    // Minion ise
    public float attack;
    public float hp;
    public List<MinionAbilites> minionAbilites;

    // Hero ise
    public HeroPowerSO heroPower;
    public Sprite portrait;
    public float armorGiven;

    // Weapon ise
    public float weaponAttack;
    public float weaponDurability;
    public List<WeaponAbilities> weaponAbilities;

    [Header("Additional Keywords")]
    public bool tradeable;

    public Corruptable corruptable;
    public CardSO corruptedVersion;

    public List<CardSO> tokens;

    [TextArea] public string DesignerNotes = string.Empty;
}
