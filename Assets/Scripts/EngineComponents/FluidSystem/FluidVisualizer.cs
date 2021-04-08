using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FluidVisualizer : MonoBehaviour
{

    Fluidbody f;
    Image i;
         
    // Use this for initialization
    void Start()
    {
        f = GetComponent<Fluidbody>();
        i = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float percent = (float)f.dataBuffer[UpdateManager.LastBuffer].amount/f.maxamount;
        i.fillAmount = percent;
            
    }
}
