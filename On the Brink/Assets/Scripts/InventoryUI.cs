using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    public Inventory inventoryScript;

    public GameObject inventoryItemPrefab;

    private Dictionary<GameObject, GameObject> inventoryItems = new Dictionary<GameObject, GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Used to show the inventory UI.
    public void EnableUI()
    {
        inventoryScript.isActive = true;
        gameObject.SetActive(true);
    }

    // Used to hide the inventory UI.
    public void DisableUI()
    {
        inventoryScript.isActive = false;
        gameObject.SetActive(false);
    }

    // Creates slots in the inventory for all items.
    public void Initialize()
    {
        // Create new items.

        // The parent.
        Transform itemsTransform = transform.Find("Items").GetComponent<Transform>();

        // The position for the first slot.
        Vector3 position = new Vector3(0, 120, 0);

        // Veriables for keeping track of all slots and positions.
        int index = 0;
        int itemsPerRow = 10;
        int slotSize = 31;

        // Instantiate a slot for all item types.
        foreach (GameObject itemType in inventoryScript.inventoryItems.Keys)
        {
            GameObject newInventoryItem = Instantiate(inventoryItemPrefab, itemsTransform.localPosition + position, inventoryItemPrefab.transform.rotation, itemsTransform);

            // Change the position to a position relative to the parent.
            newInventoryItem.transform.localPosition = position;

            inventoryItems[itemType] = newInventoryItem;

            // Get the correct information to the item at this slot.
            newInventoryItem.GetComponent<InventoryItem>().SetItemType(itemType);

            // Next item.
            index++;

            // Next spot.
            if (index % itemsPerRow == 0)
            {
                // New row.
                position = new Vector3(0, position.y - slotSize, 0);
            }
            else
            {
                // New column.
                position += new Vector3(slotSize, 0, 0);
            }
        }
    }

    // Update the information about an item.
    public void RefreshItem(GameObject itemType, ItemData itemData)
    {
        inventoryItems[itemType].GetComponent<InventoryItem>().Refresh(itemData);
    }
}
