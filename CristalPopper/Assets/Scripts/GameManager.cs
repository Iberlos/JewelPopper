using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int startingRedJewels = 10;
    [SerializeField]
    private Text redJewelDisplay = default;
    [SerializeField]
    private int startingGreenJewels = 10;
    [SerializeField]
    private Text greenJewelDisplay = default;
    [SerializeField]
    private int startingBlueJewels = 10;
    [SerializeField]
    private Text blueJewelDisplay = default;
    [SerializeField]
    private int startingMagentaJewels = 10;
    [SerializeField]
    private Text magentaJewelDisplay = default;

    public Turret Turret { get;  set; }
    public int GetJewel
    {
        get
        {
            int index = Random.Range(0, 4);
            int returnValue = -1;
            for(int i = 0; i<4; i++)
            {
                if (jewelCounters[index] <= 0)
                    index = (index + 1) % 4;
                else
                {
                    jewelCounters[index]--;
                    returnValue = index;
                    break;
                }
            }

            RefreshJewelDisplays();

            return returnValue;
        }
    }

    private int[] jewelCounters = new int[4];

    public static GameManager instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        jewelCounters[0] = startingRedJewels;
        jewelCounters[1] = startingGreenJewels;
        jewelCounters[2] = startingBlueJewels;
        jewelCounters[3] = startingMagentaJewels;

        RefreshJewelDisplays();
    }

    public void SelectShot(int colorIndex)
    {
        if(colorIndex < 3 && colorIndex > -1)
        {
            bool canSelect = true;
            for (int i = 0; i < jewelCounters.Length; i++)
                if (jewelCounters[i] < 1 || (i == colorIndex && jewelCounters[i] <= 1))
                {
                    canSelect = false;
                    break;
                }

            if (canSelect) //if each jewel counter is at least 1 and the selected one is at least 2 the shot can be selected
            {
                jewelCounters[Turret.Unload()]++;
                for (int i = 0; i < jewelCounters.Length; i++)
                    jewelCounters[i]--;
                jewelCounters[colorIndex]--;
                Turret.Reload(colorIndex);
            }
            RefreshJewelDisplays();
        }
    }

    private void RefreshJewelDisplays()
    {
        redJewelDisplay.text = jewelCounters[0].ToString();
        greenJewelDisplay.text = jewelCounters[1].ToString();
        blueJewelDisplay.text = jewelCounters[2].ToString();
        magentaJewelDisplay.text = jewelCounters[3].ToString();
    }

    public void AddJewels(int colorIndex, int amount)
    {
        jewelCounters[colorIndex] += amount;
    }
}
