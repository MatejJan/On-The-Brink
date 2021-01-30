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

    public void EnableUI()
    {
        gameObject.SetActive(true);
    }

    public void DisableUI()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        // Create new items.

        Transform itemsTransform = transform.Find("Items");

        Vector3 position = new Vector3(10, 100, 0);

        Debug.Log("start of UI");

        foreach (GameObject itemType in inventoryScript.inventoryItems.Keys)
        {

            GameObject newInventoryItem = Instantiate(inventoryItemPrefab, itemsTransform.position + position, inventoryItemPrefab.transform.rotation, itemsTransform);

            inventoryItems[itemType] = newInventoryItem;

            newInventoryItem.GetComponent<InventoryItem>().SetItemType(itemType);

            position += new Vector3(50, 0, 0);
        }
    }

    public void RefreshItem(GameObject itemType, ItemData itemData)
    {
        inventoryItems[itemType].GetComponent<InventoryItem>().Refresh(itemData);
    }
}
