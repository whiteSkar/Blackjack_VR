using UnityEngine;
using System.Collections;

public class CardController : MonoBehaviour
{    
    public int value;
    
    public void flip()
    {
        transform.Rotate(180.0f, 0, 0);
    }
    
    public int getValue()
    {
        return value;
    }
}
