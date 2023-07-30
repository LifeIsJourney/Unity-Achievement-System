using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Slider progressSlider;
    [SerializeField] TextMeshProUGUI infoText;
   
    public void Refresh(int currentValue,int totalValue)
    {
        progressSlider.value = (float)currentValue/totalValue;
        infoText.text = $"{currentValue}/{totalValue}";
    }
}
