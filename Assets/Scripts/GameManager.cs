using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // Game Buttons
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button betBtn;    

    private int standClicks = 0;
    private int hitClicks = 0;

    // Access the player and dealer's script
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    // Public Text to access and update - HUD
    public Text scoreText;
    public Text dealerScoreText;
    public Text betsText;
    public Text cashText;
    public Text mainText;
    public Text standBtnText;

    // Card Hiding dealers 2nd card
    public GameObject hideCard;

    // How much is bet
    int pot = 0;

    // Start is called before the first frame update
    public void Start()
    {
        // Add on click listeners to the buttons
        dealBtn.onClick.AddListener(() => DealClicked());
        hitBtn.onClick.AddListener(() => HitClicked());
        standBtn.onClick.AddListener(() => StandClicked());
        betBtn.onClick.AddListener(() => BetClicked());
    } 

    private void DealClicked()
    {
        // Reset round, hide text, prep for new hand
        playerScript.ResetHand();
        dealerScript.ResetHand();
        
        // Hide dealers hand score at the start of deal
        mainText.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();

        // Update the scores displayed 
        scoreText.text = "Player: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Dealer: " + dealerScript.handValue.ToString();

        // Enable to hide one of the Dealer's cards
        hideCard.GetComponent < Renderer>().enabled = true;

        // Adjust visibility of buttons
        dealBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(true);
        standBtn.gameObject.SetActive(true);
        standBtnText.text = "Stand";

        // Set standard pot size
        pot = 40;
        betsText.text = "Bets: $" + pot.ToString();
        playerScript.AdjustMoney(-20);
        cashText.text = "$" + playerScript.GetMoney().ToString();

        // Player Black Jack 21!!!
        if (playerScript.handValue == 21)
        {
            //mainText.text = "Black Jack!";
            //standClicks++;
            //standClicks++;

            RoundOver();
        }
    }

    private void HitClicked()
    {
        // I'm using hitClicks to display the BlackJack message if the player hasn't hit yet...
        hitClicks++;

        // Check that there is still room on the table
        if (playerScript.cardIndex <= 10)  // lmao, in his video this was GetCard() instead of cardIndex
        {
            playerScript.GetCard();
            scoreText.text = "Player: " + playerScript.handValue.ToString();
            if(playerScript.handValue > 20) RoundOver();
        }
    }

    private void StandClicked()
    {
        standClicks++;
        if (standClicks > 1) RoundOver();

        // Dealer's turn :D
        HitDealer();        
    }
    private void HitDealer()
    {
        while (dealerScript.handValue < 17 && dealerScript.cardIndex < 10)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Dealer: " + dealerScript.handValue.ToString();
            if (dealerScript.handValue > 20) RoundOver();
        }
        // !!!! I had to add these 2 lines of code, so I didn't have to click Stand twice, which was super annoying !!! 
        // irl the dealer just flips it, you don't have to tell them
        standClicks++;
        RoundOver();
    }

    // Check for winner and loser, hand is over
    void RoundOver()
    {
        // Boolean (true/false) for bust and blackjack/21
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;

        // If Stand has been clicked less than twice, no 21s or bust, quit function
        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;

        // if you get to this point, we will evaluate?
        bool roundOver = true;

        // All Bust, bets returned
        if (playerBust && dealerBust)
        {
            mainText.text = "Bust";
            playerScript.AdjustMoney(pot / 2);
        }

        // BlackJack test!!!!
        else if (hitClicks == 0 && player21)
        {
            mainText.text = "BlackJack!";
        }

        // if playr busts, dealer didn't, or if dealer has more points, deal wins
        else if (!dealerBust && dealerScript.handValue > playerScript.handValue)
        {
            mainText.text = "Dealer Wins!";
        }

        // if playr busts... 
        else if (playerBust)
        {
            mainText.text = "Bust! You Lose!";
        }

        // if dealer busts, playr didn't , or player has more points, player wins
        else if (dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            mainText.text = "You win!";
            playerScript.AdjustMoney(pot);
        }

        // Check for tie, return bets
        else if (playerScript.handValue == dealerScript.handValue)
        {
            mainText.text = "Push";
            playerScript.AdjustMoney(pot/2);
        }

        else
        {
            roundOver = false;
        }

        // Set Ui up for next move / hand / turn
        if (roundOver)
        {
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(true);
            mainText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            cashText.text = "$" + playerScript.GetMoney().ToString();
            standClicks = 0;
            hitClicks = 0;
        }
    }

    // Add money to pot if bet clicked
    void BetClicked()
    {
        Text newBet = betBtn.GetComponentInChildren(typeof(Text)) as Text;
        int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));
        playerScript.AdjustMoney(-intBet);
        cashText.text = "$" + playerScript.GetMoney().ToString();
        pot += (intBet * 2);
        betsText.text = "Bets: $" + pot.ToString();
    }
}
