using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // This script is for Both Player and Dealer

    // get other scripts
    public CardScript cardScript;
    public DeckScript deckScript;

    // Total value of Plaer/Dealer's hand
    public int handValue = 0;

    // Betting money
    private int money = 1000;

    // Array of card objects on table
    public GameObject[] hand;
    // index of next card to be turned over
    public int cardIndex = 0;

    //Tracking aces for 1 to 11 conversionts
    List<CardScript> aceList = new List<CardScript>();

    public void StartHand()
    {
        GetCard();
        GetCard();
    }

    // Add a card to the Player nd Dealer's hand
    public int GetCard()
    {
        // Get a card, use deal card to assign sprite and value to card on table
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
        // Show card on game screen
        hand[cardIndex].GetComponent<Renderer>().enabled = true;
        // Add card value to running total of hand
        handValue += cardValue;
        // If value is 1, it is an ace
        if (cardValue == 1)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        // check if we should use an 11 instead of a 1
        AceCheck();
        cardIndex++;
        return handValue;
    }
    
    // Search for needed ace conversions, 1 to 11 or vice versa
    public void AceCheck()
    {
        // for each ace in the list check
        foreach (CardScript ace in aceList)
        {            
            if (handValue + 10 < 22 && ace.GetValueOfCard() == 1)
            {
                // if converting, adjust card object value and hand
                ace.SetValue(11);
                handValue += 10;
            } else if (handValue > 21 && ace.GetValueOfCard() == 11)
            {
                // if converting, adjust gameobject value and hand value
                ace.SetValue(1);
                handValue -= 10;
            }
        }
    }

    // Add or subtract from money, for bets
    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    // Output players current money amount
    public int GetMoney()
    {
        return money;
    }

    //  Hides all cards, resets the needed variables
    public void ResetHand()
    {
       for(int i = 0; i < hand.Length; i++)
        {
            hand[i].GetComponent<CardScript>().ResetCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        cardIndex = 0;
        handValue = 0;
        aceList = new List<CardScript>();
    }
}
