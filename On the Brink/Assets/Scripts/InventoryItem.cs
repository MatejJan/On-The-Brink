using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    private GameObject itemType;

    private Inventory inventoryScript;

    public TextMeshProUGUI countText;
    public TextMeshProUGUI debugNameText;
    public Image image;

    private bool found;

    // Start is called before the first frame update
    void Start()
    {
        inventoryScript = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItemType(GameObject itemType)
    {
        this.itemType = itemType;

        var collectibleItem = itemType.GetComponent<CollectibleItem>();


        transform.Find("Silhouette").gameObject.SetActive(true);

        debugNameText.SetText(collectibleItem.name);

        countText.SetText("x" + inventoryScript.inventoryItems[itemType].Count);
        found = inventoryScript.inventoryItems[itemType].Found;

    }

    public void Refresh(ItemData itemData)
    {
        countText.SetText("x" + itemData.Count);
        found = itemData.Found;

        if (found)
        {
            transform.Find("Silhouette").gameObject.SetActive(false);
            transform.Find("Icon").gameObject.SetActive(true);

            if (itemData.Count > 0)
            {
                var colorAlpha = image.color;
                colorAlpha.a = 1f;
                image.color = colorAlpha;

                transform.Find("Icon").GetComponent<Image>().color = colorAlpha;
            }
            else
            {
                var colorAlpha = image.color;
                colorAlpha.a = 0.5f;
                image.color = colorAlpha;

                transform.Find("Icon").GetComponent<Image>().color = colorAlpha;
            }
        }
    }

    public void ShowToolTip()
    {
        transform.Find("Tool Tip").gameObject.SetActive(true);
        Debug.Log("show");
    }

    public void HideToolTip()
    {
        transform.Find("Tool Tip").gameObject.SetActive(false);
        Debug.Log("hide");

    }
}
