using UnityEngine;
using System.Collections;

public class DeckController : MonoBehaviour
{
    public GameObject[] cards;
    public float spaceBetweenCards;
    public Vector3 cardScale;
    
    void Start()
    {
        for (int i = 1; i < cards.Length; i++)
        {
            var curCardPos = cards[i].transform.position;
            cards[i].transform.position = new Vector3(curCardPos.x, cards[i-1].transform.position.y + spaceBetweenCards, curCardPos.z);
        }
    }
    
    public GameObject GetNextCard()
    {
        GameObject nextCard = cards[Random.Range(0, cards.Length)];
        nextCard = GameObject.Instantiate(nextCard);
        nextCard.transform.localScale = cardScale;
        return nextCard;
    }
}
