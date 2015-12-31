using UnityEngine;
using System.Collections.Generic;

public class ChipManager : MonoBehaviour
{
    public int buyInMoney;
    public Transform playerChipSpot;
    public Transform playerChipBetSpot;
    public GameObject chip;
    public float spaceBetweenChips;
    
    private Stack<GameObject> playerChips;
    private IList<GameObject> playerBetChips;

    // Use this for initialization
    void Start()
    {
        playerChips = new Stack<GameObject>();
        playerBetChips = new List<GameObject>();
        
        int numChips = buyInMoney / chip.GetComponent<ChipController>().GetValue();
        Vector3 chipPos = playerChipSpot.position;
        
        for (int i = 0; i < numChips; i++)
        {
            GameObject chipObject = Instantiate(chip, chipPos, Quaternion.identity) as GameObject;
            chipPos = new Vector3(chipPos.x, chipPos.y + chipObject.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, chipPos.z);
            playerChips.Push(chipObject);
        }
    }
    
    public void DealerGetsChips()
    {
        foreach (var chip in playerBetChips)
            Destroy(chip);
        playerBetChips.Clear();
    }
    
    public void DealerGivesChips(bool isBlackjack)
    {
        int numChips = playerBetChips.Count;
        if (numChips <= 0) return;
        
        if (isBlackjack)
            numChips = (int) Mathf.Ceil(numChips * 1.5f);
        
        Vector3 chipPos = new Vector3(playerBetChips[0].transform.position.x - playerBetChips[0].GetComponent<MeshRenderer>().bounds.size.x, 
                                      playerChipBetSpot.position.y, 
                                      playerChipBetSpot.position.z);
        for (int i = 0; i < numChips; i++)
        {
            GameObject chipObject = Instantiate(chip, chipPos, Quaternion.identity) as GameObject;
            chipPos = new Vector3(chipPos.x, chipPos.y + chipObject.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, chipPos.z);
            playerBetChips.Add(chipObject);
        }
    }
    
    public void PlayerGetsChips()
    {
        if (playerBetChips.Count <= 0) return;
        
        Vector3 chipPos = playerChips.Peek().transform.position;        
        foreach (var chip in playerBetChips)
        {
            chipPos = new Vector3(chipPos.x, chipPos.y + chip.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, chipPos.z);
            chip.transform.position = chipPos;
            playerChips.Push(chip);
        }
        
        playerBetChips.Clear();
    }
    
    public void BetChips(int numChips)
    {
        Vector3 chipPos = playerChipBetSpot.position;
        for (int i = 0; i < numChips && playerChips.Count > 0; i++)
        {
            GameObject chipObject = playerChips.Pop().gameObject;
            chipObject.transform.position = chipPos;
            chipPos = new Vector3(chipPos.x, chipPos.y + chipObject.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, chipPos.z);
            playerBetChips.Add(chipObject);
        }
    }
}
