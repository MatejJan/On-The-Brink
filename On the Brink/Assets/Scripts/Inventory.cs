using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Information stored about all collectible items. If it has been found at least ones and how many there is in the inventory.
public class ItemData
{
    public bool Found { get; set; }
    public int Count { get; set; }
    public bool Highlight { get; set; }
}

public class Inventory : MonoBehaviour
{
    public Dictionary<string, ItemData> inventoryItems = new Dictionary<string, ItemData>();

    public Dictionary<string, GameObject> itemTypePrefabs = new Dictionary<string, GameObject>();

    public InventoryUI inventoryUIScript;

    public bool isActive;

    public Object[] allItemTypePrefabObjects;

    // Gets the number of found collectible items.
    public int FoundCount
    {
        get
        {
            int count = 0;

            // Goes through the inventory dictionary and count how many items are found.
            foreach (KeyValuePair<string, ItemData> item in inventoryItems)
            {
                if (item.Value.Found)
                {
                    count++;
                }
            }

            // Returns the number of found items.
            return count;
        }
    }

    // Start is called before the first frame update.
    void Start()
    {
        isActive = false;

        // Getting all item prefabs from the resources folder.
        allItemTypePrefabObjects = Resources.LoadAll("Collectible Items");

        // Create ItemData objects for every item prefab.
        foreach (GameObject itemTypePrefab in allItemTypePrefabObjects)
        {
            string itemType = itemTypePrefab.GetComponent<CollectibleItem>().name;
            inventoryItems[itemType] = new ItemData();
            itemTypePrefabs[itemType] = itemTypePrefab;
        }

        GameObject.Find("Canvas").transform.Find("Inventory UI").GetComponent<InventoryUI>().Initialize();
    }

    private void Update()
    {
        // If the player press 'I' on the keyboard, the inventory opens or close.
        if (Input.GetKeyDown(KeyCode.I))
        {
            Workbench workbenchScript = GameObject.Find("Workbench")?.GetComponent<Workbench>();

            if (workbenchScript)
            {
                if (isActive)
                {
                    inventoryUIScript.DisableUI();

                    if (workbenchScript.active)
                    {
                        workbenchScript.Deactivate();
                    }
                }
                else
                {
                    inventoryUIScript.EnableUI();
                }
            }
        }
    }

    // Add item to inventory and mark it as found.
    public void AddItem(string itemType)
    {
        var itemData = inventoryItems[itemType];
        itemData.Found = true;
        itemData.Count++;

        RefreshItem(itemType);
    }

    public void AddItem(GameObject itemTypePrefab)
    {
        AddItem(itemTypePrefab.GetComponent<CollectibleItem>().name);
    }

    public void HighlightItem(string itemType)
    {
        var itemData = inventoryItems[itemType];
        itemData.Highlight = true;
        RefreshItem(itemType);
    }

    public void RemoveHighlight(string itemType)
    {
        var itemData = inventoryItems[itemType];
        itemData.Highlight = false;
        RefreshItem(itemType);
    }

    // Remove and add the correct items after aplying a recipe.
    public void ApplyRecipe(GameObject recipeVariant)
    {
        // Get ingredients (the two items we are combining) and the resulting item from the recipe prefab.
        Recipe recipeVariantScript = recipeVariant.GetComponent<Recipe>();

        // Remove ingredients from inventory.
        RemoveItem(recipeVariantScript.ingredients[0]);
        RemoveItem(recipeVariantScript.ingredients[1]);

        // Add resulting item to inventory.
        AddItem(recipeVariantScript.result);
    }

    // Removing item from inventory.
    public void RemoveItem(string itemType)
    {
        var itemData = inventoryItems[itemType];
        itemData.Count--;

        RefreshItem(itemType);
    }

    public void RemoveItem(GameObject itemTypePrefab)
    {
        RemoveItem(itemTypePrefab.GetComponent<CollectibleItem>().name);
    }

    // Update information about an item.
    public void RefreshItem(string itemType)
    {
        inventoryUIScript.RefreshItem(itemType, inventoryItems[itemType]);
    }

    public bool HaveItem(string itemType)
    {
        if (inventoryItems[itemType].Count > 0)
        {
            return true;
        }
        return false;
    }

    public bool HaveItem(GameObject itemTypePrefab)
    {
        return HaveItem(itemTypePrefab.GetComponent<CollectibleItem>().name);
    }
}
