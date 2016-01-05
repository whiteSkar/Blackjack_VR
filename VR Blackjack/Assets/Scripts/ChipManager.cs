using UnityEngine;
using System.Collections.Generic;

public class ChipManager : MonoBehaviour
{
    public int buyInMoney;
    public Transform playerChipSpot;
    public Transform playerChipBetSpot;
    public GameObject chip;
    public float spaceBetweenChips;
    public Color selectedChipColor;
    
    private IList<GameObject> playerChips;
    private IList<GameObject> playerBetChips;
    private Color originalChipColor;

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
        DeselectChips();
        
        for (int i = 0; i < playerChips.Count; i++)
        {
            if (playerChips[i].transform.position.x == bottomChip.transform.position.x &&
                playerChips[i].transform.position.y >= bottomChip.transform.position.y &&
                playerChips[i].transform.position.z == bottomChip.transform.position.z)
            {
                playerChips[i].GetComponent<MeshRenderer>().material.color = selectedChipColor;
            }
        }
    }
    
    public void DeselectChips()
    {
        foreach (var playerChip in playerChips)
        {
            playerChip.GetComponent<MeshRenderer>().material.color = originalChipColor;
            playerChip.GetComponent<CapsuleCollider>().enabled = true;  // was disabled when moving
        }
    }
    
    public void DeselectChip(GameObject chip)
    {
        chip.GetComponent<MeshRenderer>().material.color = originalChipColor;
        chip.GetComponent<CapsuleCollider>().enabled = true;  // was disabled when moving
    }
    
    public void MoveSelectedChips(Vector3 dest)
    {
        float lowestY = -1.0f;
        for (int i = 0; i < playerChips.Count; i++)
        {
            GameObject chip = playerChips[i];
            if (chip.GetComponent<MeshRenderer>().material.color == selectedChipColor)
            {
                if (lowestY == -1.0f)
                    lowestY = chip.transform.position.y;
                
                // This chip moves, crosshair repositions, this chip repositions, the crosshair repositions.....so on
                //  hence, disable collider so that raycast in crosshair doesn't collide with this.
                // It does also make sense that what I am moving should not be a collidable object.
                chip.GetComponent<CapsuleCollider>().enabled = false;
                chip.transform.position = new Vector3(dest.x, (chip.transform.position.y - lowestY) + dest.y, dest.z);
            }
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
        
        Vector3 chipPos = playerChipSpot.position;
        if (playerChips.Count > 0)
            chipPos = playerChips[playerChips.Count-1].transform.position;
                 
        foreach (var chip in playerBetChips)
        {
            chipPos = new Vector3(chipPos.x, chipPos.y + chip.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, chipPos.z);
            chip.transform.position = chipPos;
            playerChips.Add(chip);
        }
        
        playerBetChips.Clear();
    }
    
    public bool BetSelectedChips()
    {
        bool isThereAtLeastOneChipSelected = false;
        
        Vector3 chipDestPos = playerChipBetSpot.position;
        for (int i = playerChips.Count - 1; i >= 0; i--)
        {
            GameObject chip = playerChips[i];
            if (chip.GetComponent<MeshRenderer>().material.color == selectedChipColor)
            {
                playerChips.RemoveAt(i);
                chip.transform.position = chipDestPos;
                chipDestPos = new Vector3(chipDestPos.x, 
                                          chipDestPos.y + chip.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, 
                                          chipDestPos.z);
                playerBetChips.Add(chip);
                DeselectChip(chip);
                isThereAtLeastOneChipSelected = true;
            }
        }
        
        return isThereAtLeastOneChipSelected;
    }
}
