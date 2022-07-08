using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class GlobalCardEffects
{
    public static GameplayManager gameplayManager => GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

    #region Shuffle card to the chosen deck.
    // Choose random index if position = -1.
    public static void ShuffleToDeck(HeroScript hero, DeckScript targetDeck, int currentPosition, int newPosition)
    {
        CheckIfItsRelatedToTickingBombQuest(hero, targetDeck.cards[currentPosition]);

        if (newPosition == -1) newPosition = Random.Range(0, targetDeck.cards.Count - 1);
        targetDeck.cards.Swap(currentPosition, newPosition);
    }

    public static async void ShuffleToDeck(HeroScript hero, DeckScript targetDeck, CardSO card, int position, int howManyTimes)
    {
        for (int i = 0; i < howManyTimes; i++)
        {
            ShuffleToDeck(hero, targetDeck, card, position);
            await Task.Delay(500);
        }
    }

    // Choose random index if position = -1.
    // To add to the bottom of the deck, you have to say position = deck.cards.Count.
    private static void ShuffleToDeck(HeroScript hero, DeckScript targetDeck, CardSO card, int position)
    {
        CheckIfItsRelatedToTickingBombQuest(hero, card);

        // Insert the card in a fixed or random position.
        if (position == -1) position = Random.Range(0, targetDeck.cards.Count - 1);
        targetDeck.cards.Insert(position, card);
    }

    // Check if it's related to Warrior Quest: Ticking Bomb.
    static void CheckIfItsRelatedToTickingBombQuest(HeroScript hero, CardSO card)
    {
        if (card.name == "Bomb" && hero._currentEffects.Any(a => a.effectName == "TickingBomb"))
        {
            hero.currentQProgress++;
            if (hero.currentQProgress >= hero.maxQProgress)
            {
                gameplayManager.AddNewCardToHand(hero, gameplayManager.BombaciMulayim);
                hero.currentQuest = null;
                hero._currentEffects.Remove(hero._currentEffects.FirstOrDefault(a => a.effectName == "TickingBomb"));
            }
        }
    }
    #endregion
}
