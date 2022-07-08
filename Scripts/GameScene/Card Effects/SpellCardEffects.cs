using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SpellCardEffects : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject minionPrefab;

    private GameplayManager gameplayManager => GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

    public int ChildCount(int _childCount = 0)
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

    public int OpponentChildCount(int _childCount = 0)
    {
        if (GameplayManager.turn == Turn.OPPONENT)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Player Minion"))
            {
                _childCount++;
            }
        }
        else if (GameplayManager.turn == Turn.PLAYER)
        {
            foreach (GameObject taunt in GameObject.FindGameObjectsWithTag("Opponent Minion"))
            {
                _childCount++;
            }
        }
        return _childCount;
    }

    public CurrentEffects ExistingEffect(string effectKey, GameObject _root, CurrentEffects def = default)
    {
        foreach (CurrentEffects eff in _root.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
        {
            if (eff.effectName == effectKey)
            {
                def = eff;
            }
        }
        return def;
    }

    #region Priest Spells
    public void BoardClearForMirror(List<GameObject> target, CardScript card)
    {
        // hero
        GameObject _target = card.transform.root.name == "Player" ? GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        float armordanSonraGidecekHealth = 3 > _target.GetComponent<HeroScript>().armor ? Mathf.Abs(3 - _target.GetComponent<HeroScript>().armor) : 0;
        _target.GetComponent<HeroScript>().armor -= 3;
        _target.GetComponent<HeroScript>().currentHP -= armordanSonraGidecekHealth;
        if (_target.GetComponent<HeroScript>().currentHP <= 0)
        {
            _target.GetComponent<SpriteRenderer>().color = new Color32(118, 46, 46, 255);
            print(_target.name + " oyunu kaybetti.");
        }

        // minions
        foreach (GameObject minion in (card.transform.root.name == "Player" ? GameObject.FindGameObjectsWithTag("Opponent Minion") : GameObject.FindGameObjectsWithTag("Player Minion")))
        {
            if (!minion.GetComponent<MinionScript>().hasDivineShield)
            {
                minion.GetComponent<MinionScript>().currentHP -= 3;
                minion.GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
                minion.GetComponent<MinionScript>().damageTakenTime = 1f;
                minion.GetComponent<MinionScript>().damageTakenValue = 3;
            }
            else if (minion.GetComponent<MinionScript>().hasDivineShield)
            {
                minion.GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
                minion.GetComponent<MinionScript>().damageTakenTime = 1f;
                minion.GetComponent<MinionScript>().damageTakenValue = 0;
                minion.GetComponent<MinionScript>().hasDivineShield = false;
                foreach (CurrentEffects effect in transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
                {
                    if (effect.effectName == "MotivatedTauren")
                    {
                        foreach (GameObject min in GameObject.FindGameObjectsWithTag("Player Minion"))
                        {
                            if (min.name == "Motivated Tauren")
                            {
                                min.GetComponent<MinionScript>().currentAttack += 3;
                                min.GetComponent<MinionScript>().currentHP += 3;
                            }
                        }
                        foreach (GameObject min in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                        {
                            if (min.name == "Motivated Tauren")
                            {
                                min.GetComponent<MinionScript>().currentAttack += 3;
                                min.GetComponent<MinionScript>().currentHP += 3;
                            }
                        }
                    }
                    if (effect.effectName == "CarielRoame")
                    {
                        if (minion.transform.root.name == "Player")
                        {
                            List<MinionScript> effected = new List<MinionScript>();
                            foreach (GameObject min in GameObject.FindGameObjectsWithTag("Player Minion"))
                            {
                                if (min != minion.gameObject)
                                {
                                    if (!min.GetComponent<MinionScript>().hasDivineShield) effected.Add(min.GetComponent<MinionScript>());
                                }
                            }
                            if (effected.Count > 0) effected[UnityEngine.Random.Range(0, effected.Count)].hasDivineShield = true;
                        }
                        else
                        {
                            List<MinionScript> effected = new List<MinionScript>();
                            foreach (GameObject min in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                            {
                                if (min != minion.gameObject)
                                {
                                    if (!min.GetComponent<MinionScript>().hasDivineShield) effected.Add(min.GetComponent<MinionScript>());
                                }
                            }
                            if (effected.Count > 0) effected[Random.Range(0, effected.Count)].hasDivineShield = true;
                        }
                    }
                }
            }
            if (minion.GetComponent<MinionScript>().currentHP <= 0)
            {
                minion.GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
                Destroy(minion, .7f);
            }
        }
    }

    public void AWayOut(List<GameObject> target, CardScript card)
    {
        gameplayManager.DrawCard(card.gameObject, card.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
    }
    public async void AWayOut1(List<GameObject> target, CardScript card)
    {
        for (int i = 0; i < 2; i++)
        {
            gameplayManager.DrawCard(card.gameObject, card.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
            await Task.Delay(200);
        }
    }
    public async void AWayOut2(List<GameObject> target, CardScript card)
    {
        for (int i = 0; i < 3; i++)
        {
            gameplayManager.DrawCard(card.gameObject, card.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
            await Task.Delay(200);
        }
    }
    #endregion

    #region Paladin Spells
    public void BlessingOfPatience(List<GameObject> target, CardScript card)
    {
        target[0].GetComponent<MinionScript>().currentAttack += 3;
        target[0].GetComponent<MinionScript>().currentHP += 3;
    }
    public void BlessingOfPatienceCorrupted(List<GameObject> target, CardScript card)
    {
        target[0].GetComponent<MinionScript>().currentAttack += 1;
        target[0].GetComponent<MinionScript>().currentHP += 1;
    }

    public void LastManStanding(List<GameObject> target, CardScript card)
    {
        target[0].GetComponent<MinionScript>().currentAttack += 5;
        target[0].GetComponent<MinionScript>().currentHP += 5;
        target[0].GetComponent<MinionScript>().hasDivineShield = true;
        target[0].GetComponent<MinionScript>().hasTaunt = true;
    }

    public void DivineStudies(List<GameObject> target, CardScript card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        GameplayManager gm = gameplayManager;
        hero.discovering = true;
        IEnumerable<CardSO> __cards =
            from _card in gm.allPlayableCards
            where _card.cardType == CardType.MINION && _card.minionAbilites.Any(a => a.minionAbility == MinionAbility.DivineShield)
            select _card;

        List<CardSO> _cards = __cards.ToList();
        //List < CardSO > _cards = new List<CardSO>();
        //foreach (CardSO _card in gm.allPlayableCards)
        //{
        //    if (_card.cardType == CardType.MINION)
        //    {
        //        foreach(MinionAbilites ability in _card.minionAbilites)
        //        {
        //            if (ability.minionAbility == MinionAbility.DivineShield)
        //            {
        //                _cards.Add(_card);
        //            }
        //        }
        //    }
        //}
        CardSO card1 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card1);
        CardSO card2 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card2);
        CardSO card3 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card3);

        hero.discoverCards[0].GetComponent<DiscoverAttribute>().card = card1;
        hero.discoverCards[1].GetComponent<DiscoverAttribute>().card = card2;
        hero.discoverCards[2].GetComponent<DiscoverAttribute>().card = card3;
        hero.discoverFunction = "DivineStudies_AfterDiscovering";
        hero.DiscoverCards();
        CurrentEffects eff;
        eff.effectName = "DivineShieldCosts1Less";
        if (ExistingEffect("DivineShieldCosts1Less", card.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("DivineShieldCosts1Less", card.gameObject);
            card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Remove(ExistingEffect("DivineShieldCosts1Less", card.gameObject));
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            hero._currentEffects.Add(eff);
        }
    }

    public void DivineStudies_AfterDiscovering(List<GameObject> target, GameObject card)
    {
        HeroScript whoseTurn = target[0].transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        if (whoseTurn.hand.Count < 10)
        {
            GameObject newCard = Instantiate(cardPrefab);
            newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            newCard.GetComponent<CardScript>().card = target[0].GetComponent<DiscoverAttribute>().card;
            newCard.name = target[0].GetComponent<DiscoverAttribute>().card.cardName;
            whoseTurn.hand.Add(newCard);
        }
    }

    public void DivineJudgement(List<GameObject> target, CardScript card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "DivineShieldMinionsCosts2LessThisTurn";
        if (ExistingEffect("DivineShieldMinionsCosts2LessThisTurn", card.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("DivineShieldMinionsCosts2LessThisTurn", card.gameObject);
            card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Remove(ExistingEffect("DivineShieldCosts1Less", card.gameObject));
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            hero._currentEffects.Add(eff);
        }
    }

    public void BornAChampion(List<GameObject> target, CardScript card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "BornAChampion";
        if (ExistingEffect("BornAChampion", card.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("BornAChampion", card.gameObject);
            card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Remove(ExistingEffect("DivineShieldCosts1Less", card.gameObject));
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            hero._currentEffects.Add(eff);
        }
        hero.currentQuest = card.card;
        hero.currentQProgress = 0;
        hero.maxQProgress = 6;
        hero.AssignNewQuest();
    }
    #endregion

    #region Mage Spells
    public void BurstWave(List<GameObject> target, CardScript card)
    {
        GameObject targetedHero = card.transform.root.name == "Player" ? GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript _hero = targetedHero.GetComponent<HeroScript>();

        if (_hero.battleground.Count == 0) return;

        System.Random rnd = new System.Random();

        IEnumerable<MinionScript> _minions =
            from _minion in _hero.battleground
            orderby rnd.Next()
            select _minion.GetComponent<MinionScript>();
        List<MinionScript> minions = _minions.ToList();
        for (int i = 0; i < (minions.Count <= 3 ? minions.Count : 3); i++)
        {
            var excessDamage = minions[i].currentHP > 3 ? 0 : Mathf.Abs(minions[i].currentHP - 3);
            minions[i].currentHP -= 3;
            if (!minions[i].GetComponent<MinionScript>().hasDivineShield)
            {
                minions[i].GetComponent<MinionScript>().currentHP -= 3;
                minions[i].GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
                minions[i].GetComponent<MinionScript>().damageTakenTime = 1f;
                minions[i].GetComponent<MinionScript>().damageTakenValue = 2;
            }
            else if (minions[i].GetComponent<MinionScript>().hasDivineShield)
            {
                minions[i].GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
                minions[i].GetComponent<MinionScript>().damageTakenTime = 1f;
                minions[i].GetComponent<MinionScript>().damageTakenValue = 0;
                minions[i].GetComponent<MinionScript>().hasDivineShield = false;
            }
            if (excessDamage > 0) return;
            _hero.TakeDamage(excessDamage);
        }
    }
    #endregion

    #region Druid Spells
    public void BananaCrates(List<GameObject> target, GameObject card)
    {
        // minions
        foreach (GameObject minion in (card.gameObject.transform.root.name == "Player" ? GameObject.FindGameObjectsWithTag("Player Minion") : GameObject.FindGameObjectsWithTag("Opponent Minion")))
        {
            minion.GetComponent<MinionScript>().currentAttack += 1;
            minion.GetComponent<MinionScript>().currentHP += 1;
        }
    }

    public async void CallOfKindness(List<GameObject> target, CardScript card)
    {
        GameObject targetedHero = GameObject.Find(card.transform.root.name).transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript _hero = targetedHero.GetComponent<HeroScript>();

        for (int i = 0; i < 3; i++)
        {
            gameplayManager.DrawCard(card.gameObject, card.transform.root.name == "Player" ? Turn.OPPONENT : Turn.PLAYER);

            if (i == 2) continue;

            await Task.Delay(1300);
        }
    }
    #endregion

    #region Warrior Spells
    public void SuchAPride(List<GameObject> target, CardScript card)
    {
        foreach (HeroScript hero in FindObjectsOfType<HeroScript>())
        {
            hero.armor += 10;
        }
    }

    public void ExperimentalBrawl(List<GameObject> target, CardScript card)
    {
        foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Player Minion"))
        {
            if (minion != target[0])
            {
                minion.GetComponent<MinionScript>().skullPanel.SetActive(true);
                minion.tag = "Dead Player Minion";
                Destroy(minion, .7f);
            }
        }
        foreach (GameObject minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
        {
            if (minion != target[0])
            {
                minion.GetComponent<MinionScript>().skullPanel.SetActive(true);
                minion.tag = "Dead Opponent Minion";
                Destroy(minion, .7f);
            }
        }
    }

    public void Hearthstone(List<GameObject> target, CardScript card)
    {
        GameObject targetedHero = card.transform.root.name == "Player" ? GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = targetedHero.GetComponent<HeroScript>();

        float totalDamage = (hero.currentMana * 2);
        float excessDamage = Mathf.Abs(target[0].GetComponent<MinionScript>().currentHP - totalDamage);
        print(excessDamage);
        target[0].GetComponent<MinionScript>().TakeDamage(totalDamage);
        hero.armor += excessDamage;
        print(hero.armor);
    }

    public void TickingBomb(List<GameObject> target, CardScript card)
    {
        HeroScript hero = card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>();
        CurrentEffects eff;
        eff.effectName = "TickingBomb";
        if (ExistingEffect("TickingBomb", card.gameObject).effectName == eff.effectName)
        {
            CurrentEffects val = ExistingEffect("TickingBomb", card.gameObject);
            eff.howManyRounds = 999;
            eff.howManyStacked = val.howManyStacked + 1;
            card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects.Add(eff);
        }
        else
        {
            eff.howManyRounds = 999;
            eff.howManyStacked = 1;
            hero._currentEffects.Add(eff);
        }
        hero.currentQuest = card.card;
        hero.currentQProgress = 0;
        hero.maxQProgress = 6;
        hero.AssignNewQuest();
    }
    #endregion

    #region Shaman Spells 
    public void NotSoChargedCall(List<GameObject> target, CardScript card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        GameplayManager gm = gameplayManager;
        hero.discovering = true;
        List<CardSO> _cards = new List<CardSO>();
        foreach (CardSO _card in gm.manaCards[(int)hero.currentMana].cards)
        {
            if (_card.cardType == CardType.MINION) _cards.Add(_card);
        }
        CardSO card1 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card1);
        CardSO card2 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card2);
        CardSO card3 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card3);

        hero.discoverCards[0].GetComponent<DiscoverAttribute>().card = card1;
        hero.discoverCards[1].GetComponent<DiscoverAttribute>().card = card2;
        hero.discoverCards[2].GetComponent<DiscoverAttribute>().card = card3;
        hero.discoverFunction = "NotSoChargedCall_AfterDiscovering";
        hero.DiscoverCards();
    }

    public void NotSoChargedCall_AfterDiscovering(List<GameObject> target, GameObject card)
    {
        HeroScript whoseTurn = target[0].transform.root.Find("Hero").transform.Find("Canvas").transform.Find("Hero").GetComponent<HeroScript>();
        if (whoseTurn.hand.Count < 10)
        {
            GameObject newCard = Instantiate(cardPrefab);
            newCard.transform.SetParent(whoseTurn.gameObject.transform.parent.parent.parent.Find("Hand").transform.Find("Canvas").transform.Find("HandUI").transform);
            newCard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            newCard.GetComponent<CardScript>().card = target[0].GetComponent<DiscoverAttribute>().card;
            newCard.name = target[0].GetComponent<DiscoverAttribute>().card.cardName;
            whoseTurn.hand.Add(newCard);
        }
    }

    public void Emre(List<GameObject> target, CardScript minion)
    {
        target[0].GetComponent<MinionScript>().EvolveMinion(5);
    }
    #endregion

    #region Hunter Spells 
    public void LateToTheParty(List<GameObject> target, CardScript card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        GameplayManager gm = gameplayManager;
        List<CardSO> _cards = new List<CardSO>();
        foreach (CardSO _card in hero.deck.cards)
        {
            if (_card.cardType == CardType.MINION) _cards.Add(_card);
        }
        CardSO card1 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card1);
        CardSO card2 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card2);
        CardSO card3 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card3);
        hero.discovering = true;
        hero.discoverCards[0].GetComponent<DiscoverAttribute>().card = card1;
        hero.discoverCards[1].GetComponent<DiscoverAttribute>().card = card2;
        hero.discoverCards[2].GetComponent<DiscoverAttribute>().card = card3;
        hero.discoverFunction = "LateToTheParty_AfterDiscovering1";
        hero.DiscoverCards();
    }

    public void LateToTheParty_AfterDiscovering1(List<GameObject> target, GameObject card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        GameplayManager gm = gameplayManager;
        gm.DrawCard(card.gameObject, card.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, target[0].GetComponent<DiscoverAttribute>().card);
        List<CardSO> _cards = new List<CardSO>();
        foreach (CardSO _card in hero.deck.cards)
        {
            if (_card.cardType == CardType.MINION) _cards.Add(_card);
        }
        CardSO card1 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card1);
        CardSO card2 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card2);
        CardSO card3 = _cards[Random.Range(0, _cards.Count)];
        _cards.Remove(card3);
        hero.discovering = true;
        hero.discoverCards[0].GetComponent<DiscoverAttribute>().card = card1;
        hero.discoverCards[1].GetComponent<DiscoverAttribute>().card = card2;
        hero.discoverCards[2].GetComponent<DiscoverAttribute>().card = card3;
        hero.discoverFunction = "LateToTheParty_AfterDiscovering2";
        hero.DiscoverCards();
    }

    public void NotSoChargedCall_AfterDiscovering2(List<GameObject> target, GameObject card)
    {
        gameplayManager.DrawCard(card, card.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, target[0].GetComponent<DiscoverAttribute>().card);
    }
    #endregion

    #region Rogue Spells
    public void IDareYou(List<GameObject> target, CardScript card)
    {
        GameObject targetedHero = GameObject.Find(card.transform.root.name == "Player" ? "Opponent" : "Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript _hero = targetedHero.GetComponent<HeroScript>();

        var spikyCoin = card.card.tokens[0];
        gameplayManager.DrawCard(_hero.gameObject, card.transform.root.name == "Player" ? Turn.OPPONENT : Turn.PLAYER, spikyCoin);
    }
    public void SpikyCoin(List<GameObject> target, CardScript card)
    {
        HeroScript hero = card.gameObject.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>();
        hero.TakeDamage(5);
        hero.currentMana++;
    }
    #endregion

    #region Warlock Spells
    public void AbyssalEgg(List<GameObject> target, CardScript card)
    {
        GameObject targetedHero = GameObject.Find(card.transform.root.name).transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript _hero = targetedHero.GetComponent<HeroScript>();

        if (ChildCount() >= 7) return;

        var dragon = card.card.tokens[0];
        GameObject minion = Instantiate(minionPrefab, GameObject.Find(card.transform.root.name).transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
        minion.GetComponent<MinionScript>().Card = dragon;
        minion.name = card.name;
        minion.tag = $"{minion.transform.root.name} Minion";
        minion.GetComponent<MinionScript>().numberOnBoard = ChildCount();
        minion.GetComponent<MinionScript>().UpdateStats();
    }
    #endregion

    #region Neutral Spells (Tokens)
    public async void Bomb(List<GameObject> target, GameObject card)
    {
        HeroScript hero = card.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>();
        await Task.Delay(1000);
        hero.deck.cards.RemoveAt(0);
        hero.TakeDamage(5);
        //gameplayManager.DrawCard(hero.gameObject, card.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT);
    }

    public void ZeCoin(List<GameObject> target, CardScript card)
    {
        HeroScript hero = card.gameObject.transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>();
        hero.currentMana++;
    }
    #endregion

    #region Demon Hunter Spells 
    public void SliceAndDice(List<GameObject> target, CardScript card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();

        if (!card.cardDrawnThisTurn)
        {
            if (!target[0].GetComponent<MinionScript>().hasDivineShield)
            {
                target[0].GetComponent<MinionScript>().currentHP -= 4;
                target[0].GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
                target[0].GetComponent<MinionScript>().damageTakenTime = 1f;
                target[0].GetComponent<MinionScript>().damageTakenValue = 4;
            }
            else if (target[0].GetComponent<MinionScript>().hasDivineShield)
            {
                target[0].GetComponent<MinionScript>().damageTakenGameObject.SetActive(true);
                target[0].GetComponent<MinionScript>().damageTakenTime = 1f;
                target[0].GetComponent<MinionScript>().damageTakenValue = 0;
                target[0].GetComponent<MinionScript>().hasDivineShield = false;
                foreach (CurrentEffects effect in transform.root.Find("Hero").Find("Canvas").Find("Hero").GetComponent<HeroScript>()._currentEffects)
                {
                    if (effect.effectName == "MotivatedTauren")
                    {
                        foreach (GameObject min in GameObject.FindGameObjectsWithTag("Player Minion"))
                        {
                            if (min.name == "Motivated Tauren")
                            {
                                min.GetComponent<MinionScript>().currentAttack += 3;
                                min.GetComponent<MinionScript>().currentHP += 3;
                            }
                        }
                        foreach (GameObject min in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                        {
                            if (min.name == "Motivated Tauren")
                            {
                                min.GetComponent<MinionScript>().currentAttack += 3;
                                min.GetComponent<MinionScript>().currentHP += 3;
                            }
                        }
                    }
                    if (effect.effectName == "CarielRoame")
                    {
                        if (target[0].transform.root.name == "Player")
                        {
                            List<MinionScript> effected = new List<MinionScript>();
                            foreach (GameObject min in GameObject.FindGameObjectsWithTag("Player Minion"))
                            {
                                if (min != target[0].gameObject)
                                {
                                    if (!min.GetComponent<MinionScript>().hasDivineShield) effected.Add(min.GetComponent<MinionScript>());
                                }
                            }
                            if (effected.Count > 0) effected[UnityEngine.Random.Range(0, effected.Count)].hasDivineShield = true;
                        }
                        else
                        {
                            List<MinionScript> effected = new List<MinionScript>();
                            foreach (GameObject min in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                            {
                                if (min != target[0].gameObject)
                                {
                                    if (!min.GetComponent<MinionScript>().hasDivineShield) effected.Add(min.GetComponent<MinionScript>());
                                }
                            }
                            if (effected.Count > 0) effected[Random.Range(0, effected.Count)].hasDivineShield = true;
                        }
                    }
                }
            }
            if (target[0].GetComponent<MinionScript>().currentHP <= 0)
            {
                target[0].GetComponent<RawImage>().color = new Color32(118, 46, 46, 255);
                Destroy(target[0], .7f);
            }
        }
        else
        {
            target[0].GetComponent<MinionScript>().skullPanel.SetActive(true);
            target[0].tag = "Dead Opponent Minion";
            Destroy(target[0], .7f);
        }
    }

    public void BecomeStronger(List<GameObject> target, CardScript card)
    {
        GameObject targetedHero = GameObject.Find(card.transform.root.name).transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript _hero = targetedHero.GetComponent<HeroScript>();

        var card1 = _hero.deck.cards[0];
        gameplayManager.DrawCard(card.gameObject, card.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, card1);

        var card2 = _hero.deck.cards[1];
        gameplayManager.DrawCard(card.gameObject, card.transform.root.name == "Player" ? Turn.PLAYER : Turn.OPPONENT, card2);
        
        _hero.heroAttack += card1.mana + card2.mana;
    }
    #endregion

    #region Heroes
    #region Priest HP

    public void XyrellaTheDevout(List<GameObject> target, CardScript card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        hero.hero = card.card;
        hero.armor += card.card.armorGiven;
        hero.GetComponent<RawImage>().texture = card.card.portrait.texture;
        if (ChildCount() < 7)
        {
            GameObject newMinion = Instantiate(minionPrefab, card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
            newMinion.GetComponent<MinionScript>().Card = card.card.tokens[0];
            newMinion.name = card.name;
            newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
            newMinion.GetComponent<MinionScript>().numberOnBoard = ChildCount();
            newMinion.GetComponent<MinionScript>().UpdateStats();

            if (newMinion.transform.root.name == "Player")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
            else if (newMinion.transform.root.name == "Opponent")
            {
                foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                {
                    if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                    {
                        if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                    }
                    _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                }
            }
        }
    }
    public void XyrellaTheDevoutCorrupted(List<GameObject> target, CardScript card)
    {
        GameObject _hero = card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject : GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject;
        HeroScript hero = _hero.GetComponent<HeroScript>();
        hero.hero = card.card;
        hero.armor += card.card.armorGiven;
        hero.GetComponent<RawImage>().texture = card.card.portrait.texture;
        for (int i = 0; i < 2; i++)
        {
            if (ChildCount() < 7)
            {
                GameObject newMinion = Instantiate(minionPrefab, card.transform.root.name == "Player" ? GameObject.Find("Player").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform : GameObject.Find("Opponent").transform.Find("Battleground").transform.Find("Canvas").transform.Find("BattlegroundUI").transform);
                newMinion.GetComponent<MinionScript>().Card = card.card.tokens[0];
                newMinion.name = card.name;
                newMinion.tag = newMinion.transform.root.name == "Player" ? "Player Minion" : "Opponent Minion";
                newMinion.GetComponent<MinionScript>().numberOnBoard = ChildCount();
                newMinion.GetComponent<MinionScript>().UpdateStats();

                if (newMinion.transform.root.name == "Player")
                {
                    foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Player Minion"))
                    {
                        if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                        {
                            if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                        }
                        _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                    }
                }
                else if (newMinion.transform.root.name == "Opponent")
                {
                    foreach (GameObject _minion in GameObject.FindGameObjectsWithTag("Opponent Minion"))
                    {
                        if (newMinion.GetComponent<MinionScript>().numberOnBoard <= _minion.GetComponent<MinionScript>().numberOnBoard)
                        {
                            if (newMinion != _minion) _minion.GetComponent<MinionScript>().numberOnBoard++;
                        }
                        _minion.GetComponent<RectTransform>().DOAnchorPos(_minion.GetComponent<MinionScript>().BasePosition(), .5f);
                    }
                }
            }
        }
    }

    #endregion

    #endregion
}
