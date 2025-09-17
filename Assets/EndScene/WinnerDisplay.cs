using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
    
public class WinnerDisplay : MonoBehaviour
{
    public Text winnerText;

    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.Instance.winner == 0)
        {
            winnerText.text = "Congratulations Player 1!";
        }
        else
        {
            winnerText.text = "Congratulations Player 2!";
        }
    }
}
