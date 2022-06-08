using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI descriptionText;
    public string NameText { get { return nameText.text; } set { nameText.text = value; } }
    public string DescriptionText {  get { return descriptionText.text; } set { descriptionText.text = value; } }
}
