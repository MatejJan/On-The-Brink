using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    private GameObject itemTypePrefab;
    private string itemType;
    private Color defaultBackgroundColor;

    public Image iconImage;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI debugNameText;
    public TextMeshProUGUI toolTipText;

    public RectTransform toolTipBackground;

    private bool found;
    private int count;

    public Workbench workbenchScript;

    // Start is called before the first frame update
    void Start()
    {
        workbenchScript = GameObject.Find("Workbench").GetComponent<Workbench>();
        defaultBackgroundColor = transform.Find("Background").GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Sets the information about the item in this spot.
    public void SetItemType(GameObject itemTypePrefab)
    {
        this.itemTypePrefab = itemTypePrefab;

        var collectibleItem = itemTypePrefab.GetComponent<CollectibleItem>();

        itemType = collectibleItem.name;

        debugNameText.SetText(collectibleItem.name);

        toolTipText.SetText(collectibleItem.name);

        float textPaddingSize = 2f;
        Vector2 toolTipBackgroundSize = new Vector2(toolTipText.preferredWidth + textPaddingSize * 2, toolTipText.preferredHeight + textPaddingSize);
        toolTipBackground.sizeDelta = toolTipBackgroundSize;
    }

    // Update the information about the item in this slot.
    public void Refresh(ItemData itemData)
    {
        countText.SetText(itemData.Count + "");
        count = itemData.Count;

        // If there is no items of this type in the inventory don't show the item count.
        if (itemData.Count < 1)
        {
            transform.Find("Count").gameObject.SetActive(false);
        }
        else
        {
            // Show the item count.
            transform.Find("Count").gameObject.SetActive(true);
        }

        found = itemData.Found;

        // When an item is found hide the silhouette and show the icon instead.
        if (found)
        {
            transform.Find("Silhouette").gameObject.SetActive(false);
            transform.Find("Icon").gameObject.SetActive(true);

            // If there is any of this item type in the inventory show the icon.
            if (itemData.Count > 0)
            {
                var colorAlpha = iconImage.color;
                colorAlpha.a = 1f;
                iconImage.color = colorAlpha;

                transform.Find("Icon").GetComponent<Image>().color = colorAlpha;
            }
            else
            {
                // If there is no items of this type make the icon transparent.
                var colorAlpha = iconImage.color;
                colorAlpha.a = 0.5f;
                iconImage.color = colorAlpha;

                transform.Find("Icon").GetComponent<Image>().color = colorAlpha;
            }
        }

        bool highlight = itemData.Highlight;
        if (highlight)
        {
            transform.Find("Background").GetComponent<Image>().gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Background").GetComponent<Image>().gameObject.SetActive(false);
        }
    }

    // When the mouse is over the item show the tool tip, the name of the item.
    public void ShowToolTip()
    {
        if (found)
        {
            transform.Find("Tool Tip").gameObject.SetActive(true);
        }
    }

    // When the mouse is no longer over the item, hide the tool tip.
    public void HideToolTip()
    {
        transform.Find("Tool Tip").gameObject.SetActive(false);
    }

    public void PlaceItem()
    {
        if (count > 0)
        {
            workbenchScript.PlaceItem(itemType);
        }
    }
}
