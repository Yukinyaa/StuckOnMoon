using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : Singleton<GameUIManager>
{
    [SerializeField]
    InfoUI infoUI;

    public void Render(string infoText)
    {
        if (infoText == null)
        {
            infoUI.gameObject.SetActive(false);
        }
        
        else
        {
            infoUI.DescriptionText = infoText;
            infoUI.gameObject.SetActive(true);
        }
        
    }
}
