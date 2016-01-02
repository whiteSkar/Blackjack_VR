using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DealerController : MonoBehaviour
{
    public enum GameState
    {
        PlayerBetting,
        PlayerTurn,
        JustBecameDealerTurn,
        DealerTurn,
        PlayerWin,
        DealerWin,
        Push,
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
    public float vertSpaceBetweenPlayerCards;
    public float horiSpaceBetweenDealerCards;
    public ChipManager chipManager;
    public float stayHoldTime;
    
    private IList<CardController> dealerCards;
    private IList<CardController> playerCards;
    private float cardDepth = 0.001f;
    private GameState state;
    private float touchStartedTime;
    private CrosshairController crosshairController;
    
    // Use this for initialization
    void Start()
    {
        crosshairController = GameObject.FindObjectOfType<CrosshairController>();
        
        dealerCards = new List<CardController>();
        playerCards = new List<CardController>();
        
        state = GameState.Restarting;
        StartCoroutine(ResetRound(0.0f));
    }
    
    void Update()
    {
        // change to switch
        if (state == GameState.PlayerBetting)
        {   
            // It makes sense to put the state machine in DealerController because
            //  the dealer is the one who controls the actual game state in real life.
            // However, it does not make sense to use crosshair controller in dealer controller since
            //  it's the player who moves the chips not the dealer.
            // What should I do? hm
            GameObject touchingObject = crosshairController.GetTouchingObject();
            if (touchingObject != null && touchingObject.CompareTag("Chip"))
            {
                chipManager.SelectChipsAbove(touchingObject);
            }
            else
            {
                chipManager.DeselectChips();
            }
            
            if (Input.GetMouseButton(0))
            {
                crosshairController.SetShouldDetectNewObject(false);
                chipManager.MoveSelectedChips(crosshairController.transform.position);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // TODO: when moving is done, stick on chips or bet chip spot rather than the exact mouse up position
                // should not be able to put chips on any other places rather than those two.
                crosshairController.SetShouldDetectNewObject(true);
                chipManager.DeselectChips();
                
                // TODO: when putting the chip on chip spot, it should be correctly placed in the data strcuture
                // and also be removed from player chip data structure
                
                // TODO: verify that at least one chip is bet before changing state
                
                state = GameState.PlayerTurn;
            }
        }
        else if (state == GameState.PlayerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartedTime = Time.time;
            }
            else if (Input.GetMouseButton(0))
            {
                if (Time.time - touchStartedTime >= stayHoldTime)
                {
                    // If the player doesn't lift his finger up until the next round,
                    // the player will stay immediately. It's okay like this for now.
                    state = GameState.JustBecameDealerTurn;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Time.time - touchStartedTime >= stayHoldTime)
                {
                    state = GameState.JustBecameDealerTurn;
                }
                else
                {
                    DealPlayerCard();
                    if (GetPlayerSum() > 21)    // magic number
                        state = GameState.DealerWin;
                }
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
            if (dealerState == DealerState.MustStay)
            {
                int dealerSum = GetDealerSum();
                int playerSum = GetPlayerSum();
                
                if (playerSum < dealerSum)
                    state = GameState.DealerWin;
                else if (playerSum > dealerSum)
                    state = GameState.PlayerWin;
                else
                    state = GameState.Push;
            }
            else if (dealerState == DealerState.Busted)
            {
                state = GameState.PlayerWin;
            }
            else
            {
                DealDealerCard(true);
            }
        }
        else if (state == GameState.DealerWin)  // TODO: Add some delay between chip movements
        {
            chipManager.DealerGetsChips();
            
            state = GameState.Restarting;
            StartCoroutine(ResetRound(1.0f));
        }
        else if (state == GameState.PlayerWin)
        {
            chipManager.DealerGivesChips(IsPlayerBlackJack());
            chipManager.PlayerGetsChips();  // shouldn't be in DealerController but PlayerController
            
            state = GameState.Restarting;
            StartCoroutine(ResetRound(1.0f));
        }
        else if (state == GameState.Push)
        {
            chipManager.PlayerGetsChips();  
            
            state = GameState.Restarting;
            StartCoroutine(ResetRound(1.0f));
        }
    }
    
    // is it tie when one has blackjack and one has 21?
    bool IsPlayerBlackJack()
    {
        if (playerCards.Count != 2) return false;
        
        if ((playerCards[0].getValue() == 11 && playerCards[1].getValue() == 10) || 
            (playerCards[0].getValue() == 10 && playerCards[1].getValue() == 11))
            return true;
        
        return false;
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
        int sum = GetDealerSum();
        
        if (sum < 17)   // magic number
            return DealerState.MustHit;
        else if (sum >= 17 && sum <= 21)    // magic number
            return DealerState.MustStay;
        else
            return DealerState.Busted;
    }
    
    int GetDealerSum()
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
        
        return sum;
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
                                      playerCardSpot.position.z + (vertSpaceBetweenPlayerCards * playerCards.Count));
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
        
        chipManager.BetChips(1);    // should not be inside DealerController. should be in something like PlayerController
        
        DealInitialRound();
        state = GameState.PlayerBetting;
    }
}
