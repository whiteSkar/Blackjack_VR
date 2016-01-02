using UnityEngine;
using System.Collections.Generic;

public class ChipManager : MonoBehaviour
{
    public int buyInMoney;
    public Transform playerChipSpot;
    public Transform playerChipBetSpot;
    public GameObject chip;
    public float spaceBetweenChips;
    
    private IList<GameObject> playerChips;
    private IList<GameObject> playerBetChips;
    private Color originalChipColor;

    // Use this for initialization
    void Start()
    {
        originalChipColor = chip.GetComponent<MeshRenderer>().sharedMaterial.color;
        
        playerChips = new List<GameObject>();
        playerBetChips = new List<GameObject>();
        
        int numChips = buyInMoney / chip.GetComponent<ChipController>().GetValue();
        Vector3 chipPos = playerChipSpot.position;
        
        for (int i = 0; i < numChips; i++)
        {
            GameObject chipObject = Instantiate(chip, chipPos, Quaternion.identity) as GameObject;
            chipPos = new Vector3(chipPos.x, 
                                  chipPos.y + chipObject.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips,
                                  chipPos.z);
            playerChips.Add(chipObject);
        }
    }
    
    public void SelectChipsAbove(GameObject bottomChip)
    {
        for (int i = 0; i < playerChips.Count; i++)
        {
            if (playerChips[i].transform.position.x == bottomChip.transform.position.x &&
                playerChips[i].transform.position.y >= bottomChip.transform.position.y &&
                playerChips[i].transform.position.z == bottomChip.transform.position.z)
            {
                playerChips[i].GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }
    }
    
    public void DeselectChips()
    {
        foreach (var playerChip in playerChips)
            playerChip.GetComponent<MeshRenderer>().material.color = originalChipColor;
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
        
        Vector3 chipPos = playerChips[playerChips.Count-1].transform.position;        
        foreach (var chip in playerBetChips)
        {
            chipPos = new Vector3(chipPos.x, chipPos.y + chip.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, chipPos.z);
            chip.transform.position = chipPos;
            playerChips.Add(chip);
        }
        
        playerBetChips.Clear();
    }
    
    public void BetChips(int numChips)
    {
        Vector3 chipPos = playerChipBetSpot.position;
        for (int i = 0; i < numChips && playerChips.Count > 0; i++)
        {
            GameObject chipObject = playerChips[playerChips.Count-1].gameObject;
            playerChips.RemoveAt(playerChips.Count - 1);
            chipObject.transform.position = chipPos;
            chipPos = new Vector3(chipPos.x, chipPos.y + chipObject.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, chipPos.z);
            playerBetChips.Add(chipObject);
        }
    }
}
