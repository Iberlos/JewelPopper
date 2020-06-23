using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorValuePicker : MonoBehaviour
{
    [SerializeField]
    private int colorIndex = default;
    [SerializeField]
    private Image regionBG = default;
    [SerializeField]
    private Image amountTextBG = default;
    [SerializeField]
    private Text amountText = default;

    public virtual int ColorIndex
    {
        get => colorIndex;
        set
        {
            colorIndex = value;
            switch (colorIndex)
            {
                case 0:
                    {
                        regionBG.color = Color.red;
                        amountTextBG.color = Color.red;
                        break;
                    }
                case 1:
                    {
                        regionBG.color = Color.green;
                        amountTextBG.color = Color.green;
                        break;
                    }
                case 2:
                    {
                        regionBG.color = Color.blue;
                        amountTextBG.color = Color.blue;
                        break;
                    }
                case 3:
                    {
                        regionBG.color = Color.magenta;
                        amountTextBG.color = Color.magenta;
                        break;
                    }
            }
        }
    }

    private void OnValidate()
    {
        Amount = amount;
        ColorIndex = colorIndex;
    }

    public int Amount
    {
        get => amount;
        set
        {
            amount = value;
            amountText.text = amount.ToString();
        }
    }

    private int amount;

    public void ModifyAmountBy(int delta)
    {
        int newAmount = Mathf.Clamp(Amount + delta, 0, GameManager.GetJewelCountAt(colorIndex));
        Amount = newAmount;
    }
}
