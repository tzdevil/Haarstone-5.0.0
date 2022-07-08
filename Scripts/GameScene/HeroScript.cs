using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PlayerType { PLAYER, OPPONENT }

[System.Serializable]
public struct CurrentEffects
{
    public string effectName;
    public int howManyRounds; // 1 = bir round, 2 = hem sen hem rakibin end turn'lemiþ
    public int howManyStacked; // kaç kere art arda efekt bitmeden atýlmýþ
}

public class HeroScript : MonoBehaviour
{
    private GameplayManager _gameplayManager => GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

    public CardSO hero;

    public string heroName;
    public PlayerType playerType;

    public TextMeshProUGUI attackText, healthText;

    public float currentHP; // hero power heal veriyorsa mesela, currentHP + verilenHP olur, sýnýr da maxHP. MinionScript'de de böyle olacak ama Card'da olmayacak.
    public float maxHP;
    public float armor; // þimdilik hp + armor þeklinde gösteririm mesela 30+15
    public float currentMana;
    public float maxMana;
    public List<GameObject> hand;
    public List<GameObject> battleground;
    public DeckScript deck;

    public float currentFatigueDamage;

    public GameObject minionPrefab;
    private GameObject playerBattleground, opponentBattleground;

    public GameObject armorIcon;

    public float externalMinionManaDebuff;
    public float externalSpellManaDebuff;
    public float debuffTimer;

    public bool hasTaunt;

    public bool warmaThePissed, warmaTheBeliever, warmaTheExhausted, warmaTheAnnoyed;

    public GameObject emotePanel;
    public List<Sprite> emotes; // greetings -> well played -> thanks -> wow -> oops -> threaten
    public GameObject usedEmote;
    public bool emoteUsed;
    public float emoteUsedTime;

    public Vector2 heroPos;
    public Vector2 handUIPos;
    public Vector2 bgUIPos;
    public Vector2 heroPowerPos;
    public Vector2 deckPos;
    public Vector2 oppheroPos;
    public Vector2 opphandUIPos;
    public Vector2 oppbgUIPos;
    public Vector2 oppheroPowerPos;
    public Vector2 oppdeckPos;

    public GameObject newMinion;

    public HeroScript player, opponent;

    public bool firstPlayer;

    public GameObject cardPrefab;

    public List<CurrentEffects> _currentEffects;
    //public Dictionary<string, int> currentEffects = new Dictionary<string, int>();

    public bool discovering;
    public string discoverFunction; // discoverladýðýn karta týklayýnca bu fonksiyon çalýþacak.

    public CardSO currentQuest;

    public TextMeshProUGUI questProgress;
    public int currentQProgress, maxQProgress;
    public bool holdingQuest;
    public GameObject questCard;

    public GameObject mulliganPhase;

    public List<CardSO> graveyard = new();


    public float heroAttack; // weaponAttack harici heronun da attack'ý olabilir.
    public bool holding;
    public GameObject weapon;
    public RawImage weaponImage;
    public float weaponAttack, weaponDurability;
    public TextMeshProUGUI weaponAttackText, weaponDurabilityText;
    public CardSO weaponSlot;
    public int remainingHeroAttacks;
    private GameObject targetArrow;
    public GameObject targetPrefab;


    private void Start()
    {
        if (firstPlayer) _gameplayManager.player = gameObject;
        else _gameplayManager.opponent = gameObject;
        name = "Hero";
        // transform.SetParent(view.IsMine ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform);
        UpdatePos();
        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        warmaThePissed = false;
        warmaTheBeliever = false;
        warmaTheExhausted = false;
        warmaTheAnnoyed = false;

        emotePanel.SetActive(false);

        currentFatigueDamage = 1;

        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drop;
        entry.callback.AddListener((data) => { OnPointerDropDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);

        playerBattleground = GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").gameObject;
        opponentBattleground = GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").gameObject;
        deck = transform.parent.parent.parent.Find("Deck").GetComponent<DeckScript>();

        //ToggleWeapon();
    }

    public void UpdatePos()
    {
        transform.position = transform.parent.parent.parent.name == "Player" ? heroPos : oppheroPos;
        transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform.position = transform.parent.parent.parent.name == "Player" ? handUIPos : opphandUIPos;
        //transform.parent.parent.parent.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform.position = transform.parent.parent.parent.name == "Player" ? bgUIPos : oppbgUIPos;
        //transform.parent.parent.parent.Find("Hero Power").transform.Find("Canvas").transform.Find("Hero Power").transform.position = transform.parent.parent.parent.name == "Player" ? heroPowerPos : oppheroPowerPos;
        transform.parent.parent.parent.Find("Deck").transform.position = transform.parent.parent.parent.name == "Player" ? deckPos : oppdeckPos;
    }

    public void ClickedOnHero()
    {
        if (!holding)
        {
            emotePanel.SetActive(true);
            if (usedEmote) usedEmote.SetActive(false);
        }
    }

    #region Counts
    public float ChildCount(float _childCount = 0)
    {
        if (GameplayManager.turn == Turn.PLAYER)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                _childCount++;
            }
        }
        else if (GameplayManager.turn == Turn.OPPONENT)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                _childCount++;
            }
        }
        return _childCount;
    }

    public float TauntCount(float tauntCount = 0)
    {
        if (GameplayManager.turn == Turn.PLAYER)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                if (taunt.GetComponent<MinionScript>().hasTaunt)
                {
                    tauntCount++;
                }
            }
            if (GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().hasTaunt) tauntCount++;
        }
        if (GameplayManager.turn == Turn.OPPONENT)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                if (taunt.GetComponent<MinionScript>().hasTaunt)
                {
                    tauntCount++;
                }
            }
            if (GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>().hasTaunt) tauntCount++;
        }
        return tauntCount;
    }
    #endregion

    public void RotateAllCardsInHand()
    {
        foreach (GameObject card in hand)
            card.GetComponent<CardScript>().SetRotation(hand.Count);
    }

    private void Update()
    {
        if (holding && targetArrow)
            targetArrow.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);

        if (debuffTimer < 0) debuffTimer = 0;
        if (currentMana > 10) currentMana = 10;
        if (armor < 0) armor = 0;
        transform.parent.Find("Mana").GetComponent<TextMeshProUGUI>().text = currentMana + "/" + maxMana;
        healthText.text = currentHP.ToString();
        attackText.transform.parent.gameObject.SetActive(heroAttack + weaponAttack > 0);
        attackText.text = (heroAttack + weaponAttack).ToString();
        armorIcon.SetActive(armor > 0);
        if (armorIcon) armorIcon.transform.Find("Armor").GetComponent<TextMeshProUGUI>().text = armor.ToString();
        usedEmote.SetActive(emoteUsed);
        if (emoteUsed)
        {
            emoteUsedTime -= Time.deltaTime;
            if (emoteUsedTime <= 0) emoteUsed = false;
        }
        if (damageTakenTime > 0)
        {
            damageTakenTime -= Time.deltaTime;
            damageTaken.transform.Find("DamageTakenValue").GetComponent<TextMeshProUGUI>().text = "-" + damageTakenValue;
            if (damageTakenTime <= 0) damageTaken.SetActive(false);
        }
        questProgress.gameObject.transform.parent.gameObject.SetActive(currentQuest != null);
        if (currentQuest != null) questProgress.text = currentQProgress + "/" + maxQProgress;
        questCard.SetActive(holdingQuest);
        if (currentQuest != null) questCard.GetComponent<RawImage>().texture = currentQuest.cardSprite.texture;

        weapon.transform.parent.gameObject.SetActive(weaponSlot != null);
        if (weaponSlot is { })
        {
            weaponAttackText.text = weaponAttack.ToString();
            weaponDurabilityText.text = weaponDurability.ToString();

            if (weaponAttack < weaponSlot.weaponAttack)
                weaponAttackText.color = new Color32(109, 36, 36, 255);
            if (weaponAttack == weaponSlot.weaponAttack)
                weaponAttackText.color = new Color32(255, 255, 255, 255);
            if (weaponAttack > weaponSlot.weaponAttack)
                weaponAttackText.color = new Color32(36, 108, 65, 255);

            if (weaponDurability < weaponSlot.weaponDurability)
                weaponDurabilityText.color = new Color32(109, 36, 36, 255);
            if (weaponDurability == weaponSlot.weaponDurability)
                weaponDurabilityText.color = new Color32(255, 255, 255, 255);
            if (weaponDurability > weaponSlot.weaponDurability)
                weaponDurabilityText.color = new Color32(36, 108, 65, 255);
        }
    }

    public void SpawnNewMinion(HeroScript hero, GameObject dataPointer, CardScript card)
    {
        hero.currentMana -= card.mana;
        newMinion = Instantiate(minionPrefab, GameObject.Find(GameplayManager.turn == Turn.PLAYER ? "Player" : "Opponent").transform.Find("Battleground").Find("Canvas").Find("BattlegroundUI"));
        newMinion.GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.mousePosition.x - 514, -75);
        newMinion.GetComponent<MinionScript>().Card = card.card;
        newMinion.GetComponent<MinionScript>().UpdateStats();
        newMinion.GetComponent<MinionScript>().currentAttack = card.attack;
        newMinion.GetComponent<MinionScript>().currentHP = card.hp;
        newMinion.GetComponent<MinionScript>().defaultAttack = card.attack;
        newMinion.GetComponent<MinionScript>().defaultHP = card.hp;
        newMinion.name = card.card.cardName;
        newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
        newMinion.GetComponent<MinionScript>().numberOnBoard = newMinion.GetComponent<MinionScript>().GetBoardCount() + 1;
        foreach (MinionAbilites _card in card.card.minionAbilites)
        {
            if (_card.minionEffect != null && _card.minionAbility == MinionAbility.Battlecry)
            {
                Type scriptType = typeof(MinionCardEffects);
                MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                info?.Invoke(GameObject.Find("GameplayManager").GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, newMinion.GetComponent<MinionScript>() });
            }
            if (_card.minionAbility == MinionAbility.Charge)
            {
                newMinion.GetComponent<MinionScript>().attackedThisTurn = false;
                newMinion.GetComponent<MinionScript>().canAttackHeroes = true;
            }
            if (_card.minionAbility == MinionAbility.Rush)
            {
                newMinion.GetComponent<MinionScript>().attackedThisTurn = false;
                newMinion.GetComponent<MinionScript>().canAttackHeroes = false;
            }
        }
        hero.hand.Remove(card.gameObject);

        foreach (GameObject item in GameObject.FindGameObjectsWithTag($"{(newMinion.transform.root.name == "Player" ? "Player" : "Opponent")} Minion"))
        {
            item.GetComponent<MinionScript>().numberOnBoard = item.GetComponent<MinionScript>().CalculateNumberOnBoard(item.GetComponent<RectTransform>().anchoredPosition.x);
            if (item.GetComponent<MinionScript>().numberOnBoard >= newMinion.GetComponent<MinionScript>().numberOnBoard && item != newMinion) item.GetComponent<MinionScript>().numberOnBoard++;
            item.GetComponent<RectTransform>().DOAnchorPos(item.GetComponent<MinionScript>().BasePosition(), .5f);
        }
        foreach (MinionAbilites _card in card.card.minionAbilites)
        {
            if (_card.minionAbility == MinionAbility.DivineShield)
            {
                List<string> itemsToRemove = new List<string>();
                foreach (CurrentEffects effect in _currentEffects)
                {
                    if (effect.effectName == "BornAChampion")
                    {
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
                            itemsToRemove.Add(effect.effectName);
                            //transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>().currentEffects.Remove(effect.Key);
                        }
                    }
                    if (effect.effectName == "DivineShieldCosts1Less")
                    {
                        itemsToRemove.Add(effect.effectName);
                    }
                }
                foreach (string str in itemsToRemove)
                {
                    foreach (CurrentEffects effect in _currentEffects)
                    {
                        if (effect.effectName == str) _currentEffects.Remove(effect);
                    }
                }
            }
        }


        _gameplayManager.AddNewLog(card, hero.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, this);

        Destroy(card.gameObject);
    }

    public GameObject cardOnBoardPrefab;

    public void PlaySpell(HeroScript hero, GameObject dataPointer, CardScript card)
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

        _gameplayManager.AddNewLog(card, hero.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, this);

        Destroy(card.gameObject);
    }

    public async void OnPointerDropDelegate(PointerEventData data)
    {
        if (data.pointerDrag.CompareTag("Card"))
        {
            HeroScript hero = data.pointerDrag.transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
            CardScript card = data.pointerDrag.GetComponent<CardScript>();
            if ((GameplayManager.turn == Turn.PLAYER && hero.transform.parent.parent.parent.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && hero.transform.parent.parent.parent.name == "Player"))
            {
                if (card.mana <= hero.currentMana && (card.canTarget && (card.card.cardTarget == CardTarget.ALL || card.card.cardTarget == CardTarget.FRIENDLYALL || card.card.cardTarget == CardTarget.OPPONENTALL || (card.card.cardTarget == CardTarget.FRIENDLYMINION && card.GameObjectCount("Player Minion") > 0) || (card.card.cardTarget == CardTarget.OPPONENTMINION && card.GameObjectCount("Opponent Minion") > 0) || (card.card.cardTarget == CardTarget.MINIONS && (card.GameObjectCount("Player Minion") > 0 || card.GameObjectCount("Opponent Minion") > 0)))))
                {
                    if (card.card.cardType == CardType.MINION && ChildCount() < 7)
                    {
                        SpawnNewMinion(hero, data.pointerDrag, card);
                        foreach (GameObject _card in hero.hand)
                        {
                            if (_card.GetComponent<CardScript>().mana < card.mana && _card.GetComponent<CardScript>().card.corruptable == Corruptable.TRUE)
                            {
                                _card.GetComponent<CardScript>().card = _card.GetComponent<CardScript>().card.corruptedVersion;
                            }
                        }
                    }
                    else if (card.card.cardType == CardType.SPELL || card.card.cardType == CardType.HERO)
                    {
                        PlaySpell(hero, data.pointerDrag, card);
                        foreach (GameObject _card in hero.hand)
                        {
                            if (_card.GetComponent<CardScript>().mana < card.mana && _card.GetComponent<CardScript>().card.corruptable == Corruptable.TRUE)
                            {
                                _card.GetComponent<CardScript>().card = _card.GetComponent<CardScript>().card.corruptedVersion;
                            }
                        }
                    }
                }
            }
            hero.RotateAllCardsInHand();
        }
        else if ((data.pointerDrag.CompareTag("Player Minion") && gameObject.transform.parent.parent.parent.name == "Opponent" || (data.pointerDrag.CompareTag("Opponent Minion") && gameObject.transform.parent.parent.parent.name == "Player")))
        {
            if ((GameplayManager.turn == Turn.PLAYER && data.pointerDrag.transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && data.pointerDrag.transform.root.name == "Opponent"))
            {
                if ((TauntCount() > 0 && hasTaunt) || TauntCount() == 0)
                {
                    // burasý saldýrý phasei. minionlarda canAttack olacak eðer saldýrabiliyosa bum bum
                    HeroScript hero = data.pointerDrag.transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
                    MinionScript minion = data.pointerDrag.GetComponent<MinionScript>();
                    // go = targetlanan minion
                    // data = kart
                    if (!minion.attackedThisTurn && minion.canAttackHeroes)
                    {
                        await HeroitTweening(minion);
                    }
                }
            }
        }
        else if (data.pointerDrag.tag == "Hero")
        {
            AttackWithWeapon(data.pointerDrag);
        }
    }

    public async Task HeroitTweening(MinionScript minion)
    {
        minion.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .2f);
        minion.attackedThisTurn = true;
        await Task.Delay(400);
        Vector3 basePos = minion.transform.position;
        minion.transform.DOMove(gameObject.transform.position + (GameplayManager.turn == Turn.PLAYER ? new Vector3(0, -0.35f, 0) : new Vector3(0, 0.35f, 0)), .125f);
        await Task.Delay(160);
        TakeDamage(minion.gameObject);
        await Task.Delay(100);
        minion.transform.DOScale(new Vector3(1f, 1f, 1f), .2f);
        minion.transform.DOMove(basePos, .2f);
        foreach (MinionAbilites _card in minion.Card.minionAbilites)
        {
            if (_card.minionEffect != null && _card.minionAbility == MinionAbility.AttackEffect && (minion.Card.cardTarget == CardTarget.ALL || minion.Card.cardTarget == CardTarget.FRIENDLYALL || minion.Card.cardTarget == CardTarget.OPPONENTALL || minion.Card.cardTarget == CardTarget.HEROES || minion.Card.cardTarget == CardTarget.PLAYER || minion.Card.cardTarget == CardTarget.OPPONENT))
            {
                Type scriptType = typeof(MinionCardEffects);
                MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                info?.Invoke(GameObject.Find("GameplayManager").GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, minion });
            }
        }
    }

    #region Taking Damage
    public void TakeDamage(GameObject damageDealer) => TakeDamage(damageDealer.GetComponent<MinionScript>().currentAttack);

    public void TakeDamage(float damage)
    {
        var toplamExtraDamage = GetLessOrMore(true);
        var toplamLessDamage = GetLessOrMore(false);

        float damageValue = damage + toplamExtraDamage - toplamLessDamage;
        float armordanSonraGidecekHealth = Math.Clamp(damageValue - armor, 0, float.MaxValue);

        armor -= damageValue;
        currentHP -= armordanSonraGidecekHealth;

        ShowTakeDamage(damageValue);

        if (currentHP <= 0)
        {
            GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
            print(gameObject.transform.parent.parent.parent.name + " oyunu kaybetti.");
        }

        int GetPlayerData(bool isPlayer, bool more) => (isPlayer ? player : opponent)._currentEffects
                                                                                     .FirstOrDefault(a => a.effectName == (more ? "ShadowcuAbla" : "ExcitedRookie")).howManyStacked;
        int GetLessOrMore(bool more) => GetPlayerData(true,more) + GetPlayerData(false, more);
    }

    public GameObject damageTaken;
    public float damageTakenTime;
    public float damageTakenValue;

    public void ShowTakeDamage(float value)
    {
        damageTaken.SetActive(true);
        damageTakenTime = 1f;
        damageTakenValue = value;
    }
    #endregion

    #region Discover
    public GameObject discoverMenu;
    public List<GameObject> discoverCards;
    public GameObject showHideButton;
    public Sprite showButton, hideButton;
    public bool showingDiscoverCards;

    public void DiscoverCards()
    {
        discovering = true;
        showHideButton.SetActive(true);
        showingDiscoverCards = true;
        discoverMenu.SetActive(true);
        foreach (GameObject dis in discoverCards)
            dis.SetActive(true);
    }

    public void StopDiscoveringCards()
    {
        discovering = false;
        showHideButton.SetActive(false);
        discoverMenu.SetActive(false);
        foreach (GameObject dis in discoverCards)
            dis.SetActive(false);
    }

    public void ShowHideDiscoverCards()
    {
        showingDiscoverCards = !showingDiscoverCards;
        showHideButton.GetComponent<Image>().sprite = showingDiscoverCards ? hideButton : showButton;
        foreach (GameObject dis in discoverCards)
            dis.SetActive(showingDiscoverCards);
    }
    #endregion

    #region Quest
    public void HoldingQuest()
    {
        holdingQuest = true;
    }

    public void StopHoldingQuest()
    {
        holdingQuest = false;
    }

    public void AssignNewQuest() => questProgress.transform.parent.GetComponent<RawImage>().color = currentQuest.classColor;
    #endregion

    #region Weapon & Attacking with Hero
    public void EquipWeapon(CardSO card)
    {
        weaponSlot = card;
        weaponImage.texture = card.cardSprite.texture;
        weaponAttack = card.weaponAttack;
        weaponDurability = card.weaponDurability;
    }

    async void AttackWithWeapon(GameObject target)
    {
        if ((target.GetComponent<HeroScript>().weaponSlot == null && target.GetComponent<HeroScript>().heroAttack <= 0) || target.GetComponent<HeroScript>().remainingHeroAttacks <= 0) return;

        // Attacking
        if (target.GetComponent<HeroScript>().weaponSlot.weaponAbilities.Any(a => a.weaponAbility == WeaponAbility.Attacking))
        {
            foreach (WeaponAbilities _card in target.GetComponent<HeroScript>().weaponSlot.weaponAbilities)
            {
                if (_card.weaponEffect != null && _card.weaponAbility == WeaponAbility.Attacking)
                {
                    Type scriptType = typeof(WeaponCardEffects);
                    MethodInfo info = scriptType.GetMethod(_card.weaponEffect);
                    info?.Invoke(GameObject.Find("GameplayManager").GetComponent<WeaponCardEffects>(), new object[] { new List<GameObject> { gameObject }, target.GetComponent<HeroScript>() });
                }
            }
        }

        var basePos = target.transform.position;
        await Task.Delay(350);
        target.transform.DOMove(transform.position, .3f);

        target.GetComponent<HeroScript>().weaponDurability--;
        target.GetComponent<HeroScript>().remainingHeroAttacks--;

        await Task.Delay(300);
        target.transform.DOMove(basePos, .35f);

        TakeDamage(target.GetComponent<HeroScript>().heroAttack + target.GetComponent<HeroScript>().weaponAttack);

        // Attacked
        if (target.GetComponent<HeroScript>().weaponSlot.weaponAbilities.Any(a => a.weaponAbility == WeaponAbility.Attacked))
        {
            foreach (WeaponAbilities _card in target.GetComponent<HeroScript>().weaponSlot.weaponAbilities)
            {
                if (_card.weaponEffect != null && _card.weaponAbility == WeaponAbility.Attacked)
                {
                    Type scriptType = typeof(WeaponCardEffects);
                    MethodInfo info = scriptType.GetMethod(_card.weaponEffect);
                    info?.Invoke(GameObject.Find("GameplayManager").GetComponent<WeaponCardEffects>(), new object[] { new List<GameObject> { gameObject }, target.GetComponent<HeroScript>() });
                }
            }
        }
        if (target.GetComponent<HeroScript>().weaponDurability == 0)
        {
            await Task.Delay(250);
            target.GetComponent<HeroScript>().weaponSlot = null;
        }

        ChangeWeaponIcon(target);
    }

    void ChangeWeaponIcon(GameObject target) => target.GetComponent<HeroScript>().weapon.GetComponent<RawImage>().texture = (target.GetComponent<HeroScript>().remainingHeroAttacks == 0
                                                  || ((GameplayManager.turn == Turn.PLAYER && target.GetComponent<HeroScript>().playerType == PlayerType.PLAYER)
                                                     || (GameplayManager.turn == Turn.OPPONENT && target.GetComponent<HeroScript>().playerType == PlayerType.OPPONENT)))
                                                        ? target.GetComponent<HeroScript>()._gameplayManager.weaponIconOff.texture
                                                        : _gameplayManager.weaponIconOn.texture;

    public void MouseDownHero()
    {
        if ((weaponSlot == null && heroAttack <= 0) || remainingHeroAttacks <= 0) return;

        HeroEffectWhileHolding();
        if (heroAttack + weaponAttack > 0 && remainingHeroAttacks > 0)
        {
            if ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent"))
            {
                foreach (GameObject target in GameObject.FindGameObjectsWithTag("TargetArrow"))
                    Destroy(target);
                holding = true;
                GetComponent<RawImage>().raycastTarget = false;
                //transform.parent.parent.GetComponent<Canvas>().sortingOrder--;
                targetArrow = Instantiate(targetPrefab, transform);
                targetArrow.GetComponent<Canvas>().overrideSorting = true;
                targetArrow.GetComponent<Canvas>().sortingLayerName = "TargetIcon";
            }
        }
    }

    async void HeroEffectWhileHolding()
    {
        transform.DOScale(Vector3.one * 1.6f, .45f);
        await Task.Delay(450);
        transform.DOScale(Vector3.one * 1.5f, .15f);
    }

    public void MouseUpHero()
    {
        if (targetArrow is null) return;

        DOTween.KillAll();
        transform.DOScale(Vector3.one, .4f);
        if (GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")
        {
            holding = false;
            GetComponent<RawImage>().raycastTarget = true;
            //transform.parent.parent.GetComponent<Canvas>().sortingOrder++;
        }
        if (targetArrow) Destroy(targetArrow);
    }

    #endregion
}
