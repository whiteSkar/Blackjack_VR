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
        Restarting,
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
        
        state = GameState.Restarting;
        StartCoroutine(ResetRound(0.0f));
    }
    
    void Update()
    {
        if (state == GameState.PlayerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DealPlayerCard();
                if (GetPlayerSum() > 21)    // magic number
                    state = GameState.Over;
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
                DealDealerCard(true);
        }
        else if (state == GameState.Over)
        {
            state = GameState.Restarting;
            StartCoroutine(ResetRound(1.0f));
        }
    }
    
    int GetPlayerSum()
    {
        int sum = 0;
        for (int i = 0; i < playerCards.Count; i++)
        {
            sum += playerCards[i].getValue();
        }
        
        for (int i = 0; i < playerCards.Count && sum > 21; i++)
        {
            int cardVal = playerCards[i].getValue();
            if (cardVal == 11)  // magic number
            {
                playerCards[i].setValue(1);   // magic number
                sum -= 10;  // magic number
            }
        }
        
        return sum;
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
            sum += cardVal;
        }
        
        if (sum > 21 && isSoft)   // magic number
        {
            for (int i = 0; i < dealerCards.Count; i++)
            {
                int cardVal = dealerCards[i].getValue();
                if (cardVal == 11)  // magic number
                {
                    dealerCards[i].setValue(1);   // magic number
                    sum -= 10;  // magic number
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

    void DealInitialRound()
    {
        DealPlayerCard();
        DealDealerCard(true);
        DealPlayerCard();
        DealDealerCard(false);
    }
    
    void DealPlayerCard()
    {
        var nextCard = deckController.GetNextCard().GetComponent<CardController>();
        Vector3 cardPos = new Vector3(playerCardSpot.position.x + (horiSpaceBetweenPlayerCards * playerCards.Count), 
                                      playerCardSpot.position.y + (cardDepth * playerCards.Count),  
                                      playerCardSpot.position.z + (VertSpaceBetweenPlayerCards * playerCards.Count));
        nextCard.transform.position = cardPos;
        nextCard.flip();
        playerCards.Add(nextCard);
    }
    
    void DealDealerCard(bool showCard)
    {
        var nextCard = deckController.GetNextCard().GetComponent<CardController>();
        Vector3 cardPos = new Vector3(dealerCardSpot.position.x - (horiSpaceBetweenDealerCards * dealerCards.Count), dealerCardSpot.position.y, dealerCardSpot.position.z);
        nextCard.transform.position = cardPos;
        if (showCard)
            nextCard.flip();
        dealerCards.Add(nextCard);
    }
    
    IEnumerator ResetRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        foreach (var card in playerCards)
            Destroy(card.gameObject);
        playerCards.Clear();        
        
        foreach (var card in dealerCards)
            Destroy(card.gameObject);
        dealerCards.Clear();
        
        DealInitialRound();
        state = GameState.PlayerTurn;
    }
}
