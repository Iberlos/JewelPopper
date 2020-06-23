using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : ColorValuePicker
{
    [SerializeField]
    private ColorValuePicker[] valuePickers = default;

    public override int ColorIndex {
        get => base.ColorIndex;
        set
        {
            base.ColorIndex = Mathf.Clamp(value, 0, 3);
            Amount = 0;
            for(int i =0, valuePickerIndex = 0; i<4; i++)
            {
                if(i != ColorIndex)
                {
                    valuePickers[valuePickerIndex].ColorIndex = i;
                    valuePickers[valuePickerIndex].Amount = 0;
                    valuePickerIndex++;
                }
            }
        }
    }

    public void ModifyColorIndexBy(int delta)
    {
        ColorIndex += delta;
    }

    public void RefreshAmount()
    {
        Amount = 0;
        foreach(ColorValuePicker picker in valuePickers)
        {
            Amount += picker.Amount;
        }
    }

    public void ConfirmTrade()
    {
        int[] amounts = new int[4];
        foreach (ColorValuePicker picker in valuePickers)
        {
            amounts[picker.ColorIndex] = -picker.Amount;
            picker.Amount = picker.Amount;
        }
        amounts[ColorIndex] = Amount;
        RefreshAmount();

        GameManager.instance.TradeJewels(amounts);
    }
}
