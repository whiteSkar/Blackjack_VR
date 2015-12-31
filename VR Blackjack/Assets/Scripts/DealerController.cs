using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DealerController : MonoBehaviour
{
    public enum GameState
    {
        PlayerTurn,
        JustBecameDealerTurn,
        DealerTurn,
        Over,
    };
    
    public enum DealerState
    {
        MustHit,
        MustStay,
        Busted,
    };
    
    public DeckController deckController;
    public Transform dealerCardSpot;
    public Transform playerCardSpot;
    public float horiSpaceBetweenPlayerCards;
    public float VertSpaceBetweenPlayerCards;
    public float horiSpaceBetweenDealerCards;
    
    private IList<CardController> dealerCards;
    private IList<CardController> playerCards;
    private float cardDepth = 0.001f;
    private GameState state;
    
    
    // Use this for initialization
    void Start()
    {
        dealerCards = new List<CardController>();
        playerCards = new List<CardController>();
        
        dealInitialRound();
        state = GameState.PlayerTurn;
    }
    
    void Update()
    {
        if (state == GameState.PlayerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dealPlayerCard();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                state = GameState.JustBecameDealerTurn;              
            }
        }
        else if (state == GameState.JustBecameDealerTurn)
        {
            dealerCards[dealerCards.Count-1].flip();
            state = GameState.DealerTurn;
        }
        else if (state == GameState.DealerTurn)
        {
            DealerState dealerState = GetDealerState();
            if (dealerState == DealerState.MustStay || dealerState == DealerState.Busted)
                state = GameState.Over; // testting purpose
            else
                dealDealerCard(true);
        }
    }
    
    DealerState GetDealerState()
    {
        int sum = 0;
        bool isSoft = false;
        for (int i = 0; i < dealerCards.Count; i++)
        {
            int cardVal = dealerCards[i].getValue();
            if (cardVal == 11)  // magic number
                isSoft = true;
            sum += dealerCards[i].getValue();
        }
        
        if (sum > 21 && isSoft)   // magic number
        {
            for (int i = 0; i < dealerCards.Count; i++)
            {
                int cardVal = dealerCards[i].getValue();
                if (cardVal == 11)  // magic number
                {
                    dealerCards[i].setValue(1);   // magic number
                    break;
                }
            }
        }
        
        if (sum < 17 || (sum == 17 && isSoft))   // magic number
            return DealerState.MustHit;
        else if (sum >= 17 && sum <= 21)    // magic number
            return DealerState.MustStay;
        else
            return DealerState.Busted;
    }

    void dealInitialRound()
    {
        dealPlayerCard();
        dealDealerCard(true);
        dealPlayerCard();
        dealDealerCard(false);
    }
    
    void dealPlayerCard()
    {
        var nextCard = deckController.GetNextCard().GetComponent<CardController>();
        Vector3 cardPos = new Vector3(playerCardSpot.position.x + (horiSpaceBetweenPlayerCards * playerCards.Count), 
                                      playerCardSpot.position.y + (cardDepth * playerCards.Count),  
                                      playerCardSpot.position.z + (VertSpaceBetweenPlayerCards * playerCards.Count));
        nextCard.transform.position = cardPos;
        nextCard.flip();
        playerCards.Add(nextCard);
    }
    
    void dealDealerCard(bool showCard)
    {
        var nextCard = deckController.GetNextCard().GetComponent<CardController>();
        Vector3 cardPos = new Vector3(dealerCardSpot.position.x - (horiSpaceBetweenDealerCards * dealerCards.Count), dealerCardSpot.position.y, dealerCardSpot.position.z);
        nextCard.transform.position = cardPos;
        if (showCard)
            nextCard.flip();
        dealerCards.Add(nextCard);
    }
}
