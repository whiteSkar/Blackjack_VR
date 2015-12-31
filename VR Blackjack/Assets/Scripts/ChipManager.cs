using UnityEngine;
using System.Collections.Generic;

public class ChipManager : MonoBehaviour
{
    public int buyInMoney;
    public Transform playerChipSpot;
    public Transform playerChipBetSpot;
    public GameObject chip;
    public float spaceBetweenChips;
    
    private Stack<ChipController> playerChips;
    private IList<ChipController> playerBetChips;

    // Use this for initialization
    void Start()
    {
        playerChips = new Stack<ChipController>();
        playerBetChips = new List<ChipController>();
        
        int numChips = buyInMoney / chip.GetComponent<ChipController>().GetValue();
        Vector3 chipPos = playerChipSpot.position;
        
        for (int i = 0; i < numChips; i++)
        {
            GameObject chipObject = Instantiate(chip, chipPos, Quaternion.identity) as GameObject;
            chipPos = new Vector3(chipPos.x, chipPos.y + chipObject.GetComponent<MeshRenderer>().bounds.size.y + spaceBetweenChips, chipPos.z);
            playerChips.Push(chipObject.GetComponent<ChipController>());
        }
        
        Instantiate(chip, playerChipBetSpot.position, Quaternion.identity);
    }
}
