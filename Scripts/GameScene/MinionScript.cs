using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.Threading.Tasks;

public class MinionScript : MonoBehaviour
{
    private GameplayManager _gameplayManager => GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

    public CardSO Card;

    public float mana;
    public float defaultMana;
    public float currentAttack;
    public float defaultAttack;
    public float currentHP;
    public float defaultHP;
    public float maxHP;

    public GameObject minionPrefab;
    public GameObject cardPrefab;
    private GameObject playerBattleground, opponentBattleground;

    public bool canAttack; // bu turn saldýrabilir mi, (didAttack tarzý saldýrdý mý diye bir þeye gerek yok, canAttack'ý refreshleriz)
    public bool canAttackHeroes; // herolara saldýrabilir mi, canAttack = true ve canAttackHeroes = false ise Rush'týr.

    private GameObject targetArrow;
    public GameObject targetPrefab;
    public bool holding;

    public bool hasTaunt; // player -> opponent minion ya da opponent hero'ya saldýrmak istediðinde herhangi bir minionda taunt varsa saldýramayacak. sadece tauntlara saldýrabilecek.
    public bool hasDivineShield;
    public bool hasDeathrattleKeyword;
    public bool hasAttackEffectKeyword;
    public bool hasEndOfEachTurnKeyword;
    public bool hasEndOfYourTurnKeyword;
    public bool silenced;

    public GameObject divineShieldPanel;
    public GameObject sleepyEffect;
    public GameObject tauntEffect;
    public GameObject skullPanel; // damage yemeden ölürse bu çýkacak
    public GameObject deathrattleKeyword;
    public GameObject attackEffectKeyword;
    public GameObject silencedPanel;
    public GameObject canAttackWithMinion;

    public GameObject damageTakenGameObject;
    public float damageTakenValue;
    public float damageTakenTime;

    public bool attackedThisTurn;

    public GameObject newMinion;

    public int numberOnBoard; // boardda soldan kaçýncý sýrada

    public GameObject gm;

    public bool hasAwaken;

    public bool deathrattleEffect; // false ise henüz çalýþtýrýlmamýþ demek.

    public bool hovering;
    public float hoverTime;
    private GameObject cardOnBoard;
    public GameObject cardOnBoardPrefab;

    private void Awake()
    {
        gm = GameObject.Find("GameplayManager");
    }

    public int GetBoardCount()
    {
        return GameObject.FindGameObjectsWithTag($"{transform.root.name} Minion").ToList().Count;
    }

    public Vector3 BasePosition()
    {
        float xValue = 0;
        // önce numberOnBoard'ý hesaplayacaðýz, ardýndan basePosition'a dotweenleyeceðiz.
        switch (GetBoardCount())
        {
            case 1:
                xValue = 670;
                break;
            case 2:
                if (numberOnBoard == 1) xValue = 585f;
                if (numberOnBoard == 2) xValue = 760f;
                break;
            case 3:
                if (numberOnBoard == 1) xValue = 495;
                if (numberOnBoard == 2) xValue = 670;
                if (numberOnBoard == 3) xValue = 845;
                break;
            case 4:
                if (numberOnBoard == 1) xValue = 410f;
                if (numberOnBoard == 2) xValue = 585f;
                if (numberOnBoard == 3) xValue = 760f;
                if (numberOnBoard == 4) xValue = 935f;
                break;
            case 5:
                if (numberOnBoard == 1) xValue = 320;
                if (numberOnBoard == 2) xValue = 495;
                if (numberOnBoard == 3) xValue = 670;
                if (numberOnBoard == 4) xValue = 845;
                if (numberOnBoard == 5) xValue = 1020;
                break;
            case 6:
                if (numberOnBoard == 1) xValue = 265;
                if (numberOnBoard == 2) xValue = 410f;
                if (numberOnBoard == 3) xValue = 585f;
                if (numberOnBoard == 4) xValue = 760f;
                if (numberOnBoard == 5) xValue = 935f;
                if (numberOnBoard == 6) xValue = 1110f;
                break;
            case 7:
                if (numberOnBoard == 1) xValue = 145;
                if (numberOnBoard == 2) xValue = 320;
                if (numberOnBoard == 3) xValue = 495;
                if (numberOnBoard == 4) xValue = 670;
                if (numberOnBoard == 5) xValue = 845;
                if (numberOnBoard == 6) xValue = 1020;
                if (numberOnBoard == 7) xValue = 1195;
                break;
        }
        return new Vector3(xValue, transform.root.name == "Player" ? -255 : -75, 0); // localposiiton olmalý.
    }

    public int CalculateNumberOnBoard(float playedPosX, int order = 0) // playedPosX = mouseun droplandýðý x koordinatý.
    {
        switch (GetBoardCount())
        {
            case 1:
                order = 1;
                break;
            case 2:
                if (playedPosX < 675) order = 1;
                if (playedPosX >= 675) order = 2;
                break;
            case 3:
                if (playedPosX < 587.5f) order = 1;
                if (playedPosX >= 587.5f && playedPosX < 762.5f) order = 2;
                if (playedPosX >= 762.5f) order = 3;
                break;
            case 4:
                if (playedPosX < 500) order = 1;
                if (playedPosX >= 500 && playedPosX < 675) order = 2;
                if (playedPosX >= 675 && playedPosX < 850) order = 3;
                if (playedPosX >= 850) order = 4;
                break;
            case 5:
                if (playedPosX < 412.5f) order = 1;
                if (playedPosX >= 412.5f && playedPosX < 587.5f) order = 2;
                if (playedPosX >= 587.5f && playedPosX < 762.5f) order = 3;
                if (playedPosX >= 762.5f && playedPosX < 937.5f) order = 4;
                if (playedPosX >= 937.5f) order = 5;
                break;
            case 6:
                if (playedPosX < 325) order = 1;
                if (playedPosX >= 325 && playedPosX < 500) order = 2;
                if (playedPosX >= 500 && playedPosX < 675) order = 3;
                if (playedPosX >= 675 && playedPosX < 850) order = 4;
                if (playedPosX >= 850 && playedPosX < 1025) order = 5;
                if (playedPosX >= 1025) order = 6;
                break;
            case 7:
                if (playedPosX < 262.5f) order = 1;
                if (playedPosX >= 262.5f && playedPosX < 412.5f) order = 2;
                if (playedPosX >= 412.5f && playedPosX < 587.5f) order = 3;
                if (playedPosX >= 587.5f && playedPosX < 762.5f) order = 4;
                if (playedPosX >= 762.5f && playedPosX < 937.5f) order = 5;
                if (playedPosX >= 937.5f && playedPosX < 1087.5) order = 6;
                if (playedPosX >= 1087.5) order = 7;
                break;
        }
        return order;
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

    private void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drop;
        entry.callback.AddListener((data) => { OnPointerDropDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);

        playerBattleground = GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").gameObject;
        opponentBattleground = GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").gameObject;
    }

    private void Update()
    {
        if (holding && canAttack && targetArrow && currentAttack != 0)
            targetArrow.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
        //transform.Find("Stats").GetComponent<TextMeshProUGUI>().text = card.cardName + "\nMana: " + mana + "\nAttack: " + currentAttack + "\nHP: " + currentHP + "\nDefault HP: " + card.hp + "\nMax HP: " + maxHP;
        transform.Find("Mask").transform.Find("AttackIcon").transform.Find("AttackValue").GetComponent<TextMeshProUGUI>().text = currentAttack.ToString();
        transform.Find("Mask").transform.Find("HealthIcon").transform.Find("HealthValue").GetComponent<TextMeshProUGUI>().text = currentHP.ToString();
        if (currentAttack < defaultAttack)
            transform.Find("Mask").transform.Find("AttackIcon").transform.Find("AttackValue").GetComponent<TextMeshProUGUI>().color = new Color32(109, 36, 36, 255);
        else if (currentAttack == defaultAttack)
            transform.Find("Mask").transform.Find("AttackIcon").transform.Find("AttackValue").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        else if (currentAttack > defaultAttack)
            transform.Find("Mask").transform.Find("AttackIcon").transform.Find("AttackValue").GetComponent<TextMeshProUGUI>().color = new Color32(36, 108, 65, 255);

        if (currentHP < defaultHP)
            transform.Find("Mask").transform.Find("HealthIcon").transform.Find("HealthValue").GetComponent<TextMeshProUGUI>().color = new Color32(109, 36, 36, 255);
        else if (currentHP == defaultHP)
            transform.Find("Mask").transform.Find("HealthIcon").transform.Find("HealthValue").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        else if (currentHP > defaultHP)
            transform.Find("Mask").transform.Find("HealthIcon").transform.Find("HealthValue").GetComponent<TextMeshProUGUI>().color = new Color32(36, 108, 65, 255);

        if (currentHP <= 0)
        {
            GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
        }

        divineShieldPanel.SetActive(hasDivineShield);
        sleepyEffect.SetActive(!canAttack);
        tauntEffect.SetActive(hasTaunt);
        deathrattleKeyword.SetActive(hasDeathrattleKeyword);
        attackEffectKeyword.SetActive(hasAttackEffectKeyword || hasEndOfYourTurnKeyword || hasEndOfEachTurnKeyword);
        silencedPanel.SetActive(silenced);
        canAttackWithMinion.SetActive(canAttack && (canAttackHeroes || (!canAttackHeroes && ((transform.root.name == "Player" && GameObject.FindGameObjectsWithTag("Opponent Minion").Length > 0) || (transform.root.name == "Opponent" && GameObject.FindGameObjectsWithTag("Player Minion").Length > 0)))) && !attackedThisTurn && currentAttack != 0);

        if (damageTakenTime > 0)
        {
            damageTakenTime -= Time.deltaTime;
            damageTakenGameObject.transform.Find("DamageTakenValue").GetComponent<TextMeshProUGUI>().text = "-" + damageTakenValue;
            if (damageTakenTime <= 0) damageTakenGameObject.SetActive(false);
        }

        if (hovering)
        {
            hoverTime += Time.deltaTime;
        }
        if (cardOnBoard) cardOnBoard.SetActive(hoverTime > .35f && hovering);

        if (currentHP <= 0 && !deathrattleEffect) Deathrattle();

        
        transform.Find("Mask").Find("AwakenIcon").gameObject.SetActive(holding && !attackedThisTurn && Card.minionAbilites.Any(a => a.minionAbility == MinionAbility.Awaken) && !hasAwaken);
    }

    // next turn için, boarddaki bütün minionlarý araþtýr. eðer birinde endofeachturn varsa fonksiyon(true), endofyourturn varsa fonksiyon(false)
    public void EndOfTurn(bool each) // eðer each = true ise at the end of EACH turn, false ise at the end of YOUR turn.
    {
        foreach (MinionAbilites _card in Card.minionAbilites)
        {
            if ((!each && transform.root.name == "Player" && GameplayManager.turn == Turn.PLAYER) || (!each && transform.root.name == "Opponent" && GameplayManager.turn == Turn.OPPONENT) || each)
            {
                if (_card.minionAbility == MinionAbility.EndOfEveryTurn || _card.minionAbility == MinionAbility.EndOfYourTurn)
                {
                    Type scriptType = typeof(MinionCardEffects);
                    MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                    info?.Invoke(gm.GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, this });
                }
            }
        }
    }

    public void EvolveMinion(int neKadarEvolve)
    {
        int evolvelanacakMana = (int)mana + neKadarEvolve;
        GameplayManager _gm = gm.GetComponent<GameplayManager>();
        CardSO evolvelanacakKart = _gm.manaCards[evolvelanacakMana].cards[UnityEngine.Random.Range(0, _gm.manaCards[evolvelanacakMana].cards.Count)];
        print(Card + " -> " + evolvelanacakKart);
        Card = evolvelanacakKart;
        UpdateStats();
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

        _gameplayManager.AddNewLog(card, hero.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, this);

        Destroy(card.gameObject);
    }

    public async void OnPointerDropDelegate(PointerEventData data)
    {
        if (data.pointerDrag.CompareTag("Card"))
        {
            if ((GameplayManager.turn == Turn.PLAYER && data.pointerDrag.transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && data.pointerDrag.transform.root.name == "Opponent"))
            {
                if (GetBoardCount() < 7)
                {
                    HeroScript hero = data.pointerDrag.transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
                    CardScript card = data.pointerDrag.GetComponent<CardScript>();
                    // go = targetlanan minion
                    // data = kart
                    if ((card.card.cardTarget == CardTarget.ALL || card.card.cardTarget == CardTarget.FRIENDLYALL || card.card.cardTarget == CardTarget.FRIENDLYMINION || card.card.cardTarget == CardTarget.MINIONS || card.card.cardTarget == CardTarget.OPPONENTALL || card.card.cardTarget == CardTarget.OPPONENTMINION || card.card.cardTarget == CardTarget.SELF))
                    {
                        if (card.mana <= hero.currentMana && card.canTarget)
                        {
                            if (card.card.cardType == CardType.MINION && GetBoardCount() < 7)
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
                        hero.RotateAllCardsInHand();
                    }
                }
            }
        }

        else if ((data.pointerDrag.CompareTag("Player Minion") && gameObject.CompareTag("Opponent Minion")) || (data.pointerDrag.CompareTag("Opponent Minion") && gameObject.CompareTag("Player Minion")))
        {
            if (data.pointerDrag.GetComponent<MinionScript>().canAttack && data.pointerDrag.gameObject.GetComponent<MinionScript>().currentAttack != 0)
            {
                // eðer bir minionda taunt varsa ve o minion buysa YA DA hiçbir minionda taunt yoksa, saldýr.
                if ((GameplayManager.turn == Turn.PLAYER && data.pointerDrag.transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && data.pointerDrag.transform.root.name == "Opponent"))
                {
                    if ((TauntCount() > 0 && hasTaunt) || TauntCount() == 0)
                    {
                        // burasý saldýrý phasei. minionlarda canAttack olacak eðer saldýrabiliyosa bum bum
                        HeroScript hero = transform.root.transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
                        MinionScript minion = data.pointerDrag.GetComponent<MinionScript>();
                        // go = targetlanan minion
                        // data = kart
                        if (!minion.attackedThisTurn)
                        {
                            await MinionHitTweening(minion);
                        }
                    }
                }
            }
        }

        else if (data.pointerDrag.gameObject.tag == "Hero")
        {
            AttackWithWeapon(data.pointerDrag);
        }


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

        target.GetComponent<HeroScript>().TakeDamage(currentAttack);
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
                                                        ? _gameplayManager.weaponIconOff.texture
                                                        : _gameplayManager.weaponIconOn.texture;

    public async Task MinionHitTweening(MinionScript minion)
    {
        minion.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .2f);
        minion.attackedThisTurn = true;
        await Task.Delay(400);
        Vector3 basePos = minion.transform.position;
        minion.transform.DOMove(gameObject.transform.position + (GameplayManager.turn == Turn.PLAYER ? new Vector3(0, -0.35f, 0) : new Vector3(0, 0.35f, 0)), .125f);
        await Task.Delay(160);
        TakeDamage(minion.gameObject);
        minion.TakeDamage(gameObject);
        await Task.Delay(100);
        minion.transform.DOScale(new Vector3(1f, 1f, 1f), .2f);
        //yield return new WaitForSeconds(.1f);
        minion.transform.DOMove(basePos, .2f);
        foreach (MinionAbilites _card in minion.Card.minionAbilites)
        {
            if (_card.minionEffect != null && _card.minionAbility == MinionAbility.AttackEffect && (minion.Card.cardTarget == CardTarget.ALL || minion.Card.cardTarget == CardTarget.FRIENDLYALL || minion.Card.cardTarget == CardTarget.FRIENDLYMINION || minion.Card.cardTarget == CardTarget.MINIONS || minion.Card.cardTarget == CardTarget.OPPONENTALL || minion.Card.cardTarget == CardTarget.OPPONENTMINION || minion.Card.cardTarget == CardTarget.SELF))
            {
                Type scriptType = typeof(MinionCardEffects);
                MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                info?.Invoke(gm.GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, minion });
            }
        }
    }

    public void TakeDamage(GameObject damageDealer)
    {
        if (!hasDivineShield)
        {
            currentHP -= damageDealer.GetComponent<MinionScript>().currentAttack;
            damageTakenGameObject.SetActive(true);
            damageTakenTime = 1f;
            damageTakenValue = damageDealer.GetComponent<MinionScript>().currentAttack;
            damageDealer.GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
            damageDealer.GetComponent<MinionScript>().damageTakenTime = 1f;
            damageDealer.GetComponent<MinionScript>().damageTakenValue = damageDealer.GetComponent<MinionScript>().hasDivineShield ? 0 : currentAttack;
        }
        else if (hasDivineShield)
        {
            damageTakenGameObject.SetActive(true);
            damageTakenTime = 1f;
            damageTakenValue = 0;
            damageDealer.GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
            damageDealer.GetComponent<MinionScript>().damageTakenTime = 1f;
            damageDealer.GetComponent<MinionScript>().damageTakenValue = damageDealer.GetComponent<MinionScript>().hasDivineShield ? 0 : currentAttack;
            hasDivineShield = false;
            foreach (CurrentEffects v in transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
            {
                if (v.effectName == "MotivatedTauren")
                {
                    GameObject.FindGameObjectsWithTag("Player Minion").FirstOrDefault(a => a.name == "Motivated Tauren").GetComponent<MinionScript>().currentAttack += 3;
                    GameObject.FindGameObjectsWithTag("Player Minion").FirstOrDefault(a => a.name == "Motivated Tauren").GetComponent<MinionScript>().currentHP += 3;
                    GameObject.FindGameObjectsWithTag("Opponent Minion").FirstOrDefault(a => a.name == "Motivated Tauren").GetComponent<MinionScript>().currentAttack += 3;
                    GameObject.FindGameObjectsWithTag("Opponent Minion").FirstOrDefault(a => a.name == "Motivated Tauren").GetComponent<MinionScript>().currentHP += 3;
                }
                if (v.effectName == "CarielRoame")
                {
                    GameObject.FindGameObjectsWithTag($"{transform.root.name} Minion").Where(a => a != gameObject).ToList().ForEach(a => a.GetComponent<MinionScript>().hasDivineShield = true);
                }
                if (v.effectName == "TuralyonTheChampion")
                {
                    GetComponent<MinionScript>().currentAttack += 2;
                    GetComponent<MinionScript>().currentHP += 3;
                }
            }
        }
        if (currentHP <= 0)
        {
            tag = CompareTag("Player") ? "Dead Player Minion" : "Dead Opponent Minion";
            var hero = CompareTag("Player") ? GameObject.Find("Player").transform.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>() : GameObject.Find("Opponent").transform.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>();
            hero.graveyard.Add(Card);
            GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
            hero.battleground.Remove(gameObject);
            Destroy(gameObject, .7f);
        }
    }
    public void TakeDamage(float damage)
    {
        if (!hasDivineShield)
        {
            currentHP -= damage;
            damageTakenGameObject.SetActive(true);
            damageTakenTime = 1f;
            damageTakenValue = damage;
        }
        else if (hasDivineShield)
        {
            damageTakenGameObject.SetActive(true);
            damageTakenTime = 1f;
            damageTakenValue = 0;
            hasDivineShield = false;
            foreach (CurrentEffects v in transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
            {
                if (v.effectName == "MotivatedTauren")
                {
                    GameObject.FindGameObjectsWithTag("Player Minion").FirstOrDefault(a => a.name == "Motivated Tauren").GetComponent<MinionScript>().currentAttack += 3;
                    GameObject.FindGameObjectsWithTag("Player Minion").FirstOrDefault(a => a.name == "Motivated Tauren").GetComponent<MinionScript>().currentHP += 3;
                    GameObject.FindGameObjectsWithTag("Opponent Minion").FirstOrDefault(a => a.name == "Motivated Tauren").GetComponent<MinionScript>().currentAttack += 3;
                    GameObject.FindGameObjectsWithTag("Opponent Minion").FirstOrDefault(a => a.name == "Motivated Tauren").GetComponent<MinionScript>().currentHP += 3;
                }
                if (v.effectName == "CarielRoame")
                {
                    GameObject.FindGameObjectsWithTag($"{transform.root.name} Minion").Where(a => a != gameObject).ToList().ForEach(a => a.GetComponent<MinionScript>().hasDivineShield = true);
                }
                if (v.effectName == "TuralyonTheChampion")
                {
                    GetComponent<MinionScript>().currentAttack += 2;
                    GetComponent<MinionScript>().currentHP += 3;
                }
            }
        }
        if (currentHP <= 0)
        {
            tag = CompareTag("Player") ? "Dead Player Minion" : "Dead Opponent Minion";
            var hero = CompareTag("Player") ? GameObject.Find("Player").transform.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>() : GameObject.Find("Opponent").transform.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>();
            print(hero.name);
            hero.graveyard.Add(Card);
            GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
            hero.battleground.Remove(gameObject);
            Destroy(gameObject, .7f);
        }
    }

    private void OnDestroy()
    {
        DeathTweening();
    }

    public void Deathrattle()
    {
        deathrattleEffect = true;
        if (Card.minionAbilites.Any(a => a.minionAbility == MinionAbility.Deathrattle))
        {
            Type scriptType = typeof(MinionCardEffects);
            MethodInfo info = scriptType.GetMethod(Card.minionAbilites.FirstOrDefault(a => a.minionAbility == MinionAbility.Deathrattle).minionEffect);
            info?.Invoke(gm.GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, this }); // this = Assertion failed on expression: 'go.IsActive()' sorunu yaratýyor olmalý. ama her þey çalýþýyor.
        }
    }

    public void DeathTweening()
    {
        if (transform.root.name == "Player")
        {
            foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                if (numberOnBoard <= minion.GetComponent<MinionScript>().numberOnBoard)
                {
                    minion.GetComponent<MinionScript>().numberOnBoard--;
                }
                minion.GetComponent<RectTransform>().DOAnchorPos(minion.GetComponent<MinionScript>().BasePosition(), .5f);
            }
        }
        else if (transform.root.name == "Opponent")
        {
            foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                if (numberOnBoard <= minion.GetComponent<MinionScript>().numberOnBoard)
                {
                    minion.GetComponent<MinionScript>().numberOnBoard--;
                }
                minion.GetComponent<RectTransform>().DOAnchorPos(minion.GetComponent<MinionScript>().BasePosition(), .5f);
            }
        }
    }

    public void MouseDownMinion()
    {
        if (canAttack && currentAttack != 0 && !attackedThisTurn)
        {
            if ((GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player") || (GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent"))
            {
                foreach (GameObject target in GameObject.FindGameObjectsWithTag("TargetArrow"))
                    Destroy(target);
                holding = true;
                GetComponent<RawImage>().raycastTarget = false;
                transform.parent.parent.GetComponent<Canvas>().sortingOrder--;
                if (canAttack && currentAttack != 0)
                {
                    targetArrow = Instantiate(targetPrefab, transform);
                    targetArrow.GetComponent<Canvas>().overrideSorting = true;
                    targetArrow.GetComponent<Canvas>().sortingLayerName = "TargetIcon";
                }
            }
        }
    }

    public void AwakenUpMinion()
    {
        print("wassup");
        foreach (MinionAbilites _card in Card.minionAbilites)
        {
            if (_card.minionEffect != null && _card.minionAbility == MinionAbility.Awaken)
            {
                Type scriptType = typeof(MinionCardEffects);
                MethodInfo info = scriptType.GetMethod(_card.minionEffect);
                info?.Invoke(gm.GetComponent<MinionCardEffects>(), new object[] { new List<GameObject> { gameObject }, this });
            }
        }
        hasAwaken = true;
        attackedThisTurn = false;
        if (GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")
        {
            holding = false;
            GetComponent<RawImage>().raycastTarget = true;
            transform.parent.parent.GetComponent<Canvas>().sortingOrder++;
        }
        if (targetArrow) Destroy(targetArrow);
    }

    public void MouseUpMinion()
    {
        if (Card.minionAbilites.Any(a => a.minionAbility == MinionAbility.Awaken) && !hasAwaken) return;

        if (GameplayManager.turn == Turn.PLAYER && transform.root.name == "Player" || GameplayManager.turn == Turn.OPPONENT && transform.root.name == "Opponent")
        {
            holding = false;
            GetComponent<RawImage>().raycastTarget = true;
            transform.parent.parent.GetComponent<Canvas>().sortingOrder++;
        }
        if (targetArrow) Destroy(targetArrow);
    }

    public void UpdateStats()
    {
        mana = Card.mana;
        defaultMana = Card.mana;
        currentAttack = Card.attack;
        defaultAttack = Card.attack;
        currentHP = Card.hp;
        defaultHP = Card.hp;
        maxHP = Card.hp;
        name = Card.name;
        transform.Find("Mask").Find("MinionImage").GetComponent<SpriteRenderer>().sprite = Card.cardSprite;
        foreach (MinionAbilites _card in Card.minionAbilites)
        {
            if (_card.minionAbility == MinionAbility.Taunt) hasTaunt = true; else hasTaunt = false;
            if (_card.minionAbility == MinionAbility.DivineShield) hasDivineShield = true; else hasDivineShield = false;
            if (_card.minionAbility == MinionAbility.Deathrattle) hasDeathrattleKeyword = true; else hasDeathrattleKeyword = false;
            if (_card.minionAbility == MinionAbility.AttackEffect) hasAttackEffectKeyword = true; else hasAttackEffectKeyword = false;
            if (_card.minionAbility == MinionAbility.EndOfEveryTurn) hasEndOfYourTurnKeyword = true; else hasEndOfYourTurnKeyword = false;
            if (_card.minionAbility == MinionAbility.EndOfYourTurn) hasEndOfYourTurnKeyword = true; else hasEndOfYourTurnKeyword = false;
            if (_card.minionAbility == MinionAbility.Charge && ((transform.root.name == "Player" && GameplayManager.turn == Turn.PLAYER) || (transform.root.name == "Opponent" && GameplayManager.turn == Turn.OPPONENT))) { canAttack = true; canAttackHeroes = true; } else { canAttack = false; }
            if (_card.minionAbility == MinionAbility.Rush && ((transform.root.name == "Player" && GameplayManager.turn == Turn.PLAYER) || (transform.root.name == "Opponent" && GameplayManager.turn == Turn.OPPONENT))) { canAttack = true; canAttackHeroes = false; } else { canAttack = false; }
        }
    }

    public void MouseHoverEnterMinion()
    {
        hoverTime = 0;
        hovering = true;
        cardOnBoard = Instantiate(cardOnBoardPrefab, GameObject.Find("GlobalCanvas").transform);
        cardOnBoard.GetComponent<CardOnBoardScript>().card = Card;
        cardOnBoard.GetComponent<CardOnBoardScript>().mana = Card.mana;
        cardOnBoard.GetComponent<CardOnBoardScript>().attack = Card.attack;
        cardOnBoard.GetComponent<CardOnBoardScript>().hp = Card.hp;
        cardOnBoard.GetComponent<CardOnBoardScript>().maxHP = Card.hp;
        cardOnBoard.GetComponent<CardOnBoardScript>().canTarget = Card.canTarget;
        cardOnBoard.GetComponent<RawImage>().texture = Card.cardSprite.texture;
    }

    public GameObject _cardOnBoardPrefab;

    public void PlaySpell(HeroScript hero, GameObject dataPointer, CardScript card)
    {
        hero.currentMana -= card.mana;
        GameObject cardOnBoard = Instantiate(_cardOnBoardPrefab, GameObject.Find("GlobalCanvas").transform);
        cardOnBoard.GetComponent<CardOnBoardScript>().card = card.card;
        cardOnBoard.transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = card.mana.ToString();
        cardOnBoard.GetComponent<RawImage>().texture = card.card.cardSprite.texture;
        cardOnBoard.name = card.card.cardName;
        cardOnBoard.tag = card.transform.root.name == "Player" ? "Player Spell" : "Opponent Spell";
        Destroy(cardOnBoard, .9f);
        Type scriptType = typeof(SpellCardEffects);
        MethodInfo info = scriptType.GetMethod(card.card.spellEffect);
        info?.Invoke(gm.GetComponent<SpellCardEffects>(), new object[] { new List<GameObject> { gameObject }, card });
        hero.hand.Remove(card.gameObject);

        _gameplayManager.AddNewLog(card, hero.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, this);

        Destroy(card.gameObject);
    }

    public void MouseHoverExitMinion()
    {
        hovering = false;
        hoverTime = 0;
        if (cardOnBoard) Destroy(cardOnBoard);
    }
}
