using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCardEffects : MonoBehaviour
{
    public GameObject cardOnBoardPrefab;

    public async void Wrenchcalibur(List<GameObject> target, HeroScript hero)
    {
        HeroScript oppHero = hero.transform.root.name == "Player"
                                                         ? GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject.GetComponent<HeroScript>()
                                                         : GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject.GetComponent<HeroScript>();

        GlobalCardEffects.ShuffleToDeck(hero, oppHero.deck, hero.weaponSlot.tokens[0], -1, 1);

        GameObject bombCard = Instantiate(cardOnBoardPrefab, hero.weapon.transform.position, Quaternion.identity, hero.transform.parent);
        bombCard.transform.localScale = 0.5f * Vector3.one;
        bombCard.GetComponent<RawImage>().texture = hero.weaponSlot.tokens[0].cardSprite.texture;
        bombCard.GetComponent<CardOnBoardScript>().card = hero.weaponSlot.tokens[0];
        bombCard.transform.DOScale(0.8f * Vector3.one, .5f);
        await Task.Delay(500);
        bombCard.transform.DOMove(oppHero.deck.transform.Find("Canvas").Find("DeckDrop").position, .5f);
        await Task.Delay(500);
        Destroy(bombCard);
    }

    public void CursedAxe(List<GameObject> target, HeroScript hero)
    {
        HeroScript oppHero = hero.transform.root.name == "Player"
                                                         ? GameObject.Find("Opponent").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject.GetComponent<HeroScript>()
                                                         : GameObject.Find("Player").transform.Find("Hero").transform.Find("Canvas").transform.Find("Hero").gameObject.GetComponent<HeroScript>();

        GlobalCardEffects.ShuffleToDeck(hero, oppHero.deck, hero.weaponSlot.tokens[0], -1, 2);

        for (int i = -1; i <= 1; i++)
        {
            if (i == 0) continue;

            GameObject bombCard = Instantiate(cardOnBoardPrefab, hero.weapon.transform.position, Quaternion.identity, hero.transform.parent);
            bombCard.transform.position = hero.weapon.transform.position + (i) * 1.25f * Vector3.right;
            bombCard.transform.localScale = 0.5f * Vector3.one;
            bombCard.GetComponent<RawImage>().texture = hero.weaponSlot.tokens[0].cardSprite.texture;
            bombCard.GetComponent<CardOnBoardScript>().card = hero.weaponSlot.tokens[0];
            CursedAxeTweening(oppHero, bombCard);
        }
    }
    async void CursedAxeTweening(HeroScript hero, GameObject bombCard)
    {
        bombCard.transform.DOScale(0.8f * Vector3.one, .5f);
        await Task.Delay(500);
        bombCard.transform.DOMove(hero.deck.transform.Find("Canvas").Find("DeckDrop").position, .5f);
        await Task.Delay(500);
        Destroy(bombCard);
    }

}
