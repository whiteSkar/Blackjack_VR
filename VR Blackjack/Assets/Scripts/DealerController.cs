using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DealerController : MonoBehaviour
{
    public DeckController deckController;
    public Transform dealerCardSpot;
    public Transform playerCardSpot;
    public float horiSpaceBetweenPlayerCards;
    public float VertSpaceBetweenPlayerCards;
    public float horiSpaceBetweenDealerCards;
    
    private IList<CardController> dealerCards;
    private IList<CardController> playerCards;
    private float cardDepth = 0.001f;
    
    // Use this for initialization
    void Start()
    {
        dealerCards = new List<CardController>();
        playerCards = new List<CardController>();
        
        StartCoroutine(DealCards());
    }
    
    IEnumerator DealCards()
    {
        print("HO");
        dealPlayerCard();
        dealDealerCard(true);
        dealPlayerCard();
        dealDealerCard(false);

        yield return null;
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
        Vector3 cardPos = new Vector3(dealerCardSpot.position.x - (horiSpaceBetweenDealerCards * playerCards.Count), dealerCardSpot.position.y, dealerCardSpot.position.z);
        nextCard.transform.position = cardPos;
        if (showCard)
            nextCard.flip();
        dealerCards.Add(nextCard);
    }
}
