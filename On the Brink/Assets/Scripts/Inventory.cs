using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Information stored about all collectible items. If it has been found at least ones and how many there is in the inventory.
public class ItemData
{
    public bool Found { get; set; }
    public int Count { get; set; }
}

public class Inventory : MonoBehaviour
{
    Dictionary<GameObject, ItemData> inventoryItems = new Dictionary<GameObject, ItemData>();

    // Gets the number of found collectible items.
    public int FoundCount
    {
        get
        {
            int count = 0;

            // Goes through the inventory dictionary and count how many items are found.
            foreach (KeyValuePair<GameObject, ItemData> item in inventoryItems)
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
        // Getting all item prefabs from the resources folder.
        var allItemTypes = Resources.LoadAll("Collectible Items");

        // Create ItemData objects for every item prefab.
        foreach (GameObject itemType in allItemTypes)
        {
            inventoryItems[itemType] = new ItemData();
        }
    }

    private void Update()
    {
        // Debug messages to see that everything works.
        Debug.Log("Found " + FoundCount);

        foreach (KeyValuePair<GameObject, ItemData> item in inventoryItems)
        {
            Debug.Log(item.Key.name + " count: " + item.Value.Count);
        }

    }

    // Add item to inventory and mark it as found.
    public void AddItem(GameObject itemType)
    {
        var itemData = inventoryItems[itemType];
        itemData.Found = true;
        itemData.Count++;
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
    public void RemoveItem(GameObject itemType)
    {
        var itemData = inventoryItems[itemType];
        itemData.Count--;
    }
}
