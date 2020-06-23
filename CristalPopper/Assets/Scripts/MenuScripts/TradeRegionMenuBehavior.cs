using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeRegionMenuBehavior : MonoBehaviour
{
    [SerializeField]
    private Text toggleMenuButtonText = default;
    [SerializeField]
    private Button storeButton = default;

    private bool active;

    public void ToggleMenu()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.localPosition += active ? new Vector3(0f, 179f, 0f) : new Vector3(0f, -179f, 0f);
        active = !active;
        storeButton.gameObject.SetActive(active);
        toggleMenuButtonText.text = active ? "Close Tade Menu" : "Trade Menu";
    }
}
