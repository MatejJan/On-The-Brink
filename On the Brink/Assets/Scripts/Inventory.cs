using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Information stored about all collectible items. If it has been found at least ones and how many there is in the inventory.
public class ItemData
{
    public bool found = false;
    public int count = 0;
}

public class Inventory : MonoBehaviour
{
    Dictionary<GameObject, ItemData> inventoryItems = new Dictionary<GameObject, ItemData>();

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

    // Add item to inventory and mark it as found.
    void AddItem(GameObject itemType)
    {
        var itemData = inventoryItems[itemType];
        itemData.found = true;
        itemData.count++;
    }

    // Remove and add the correct items after aplying a recipe.
    void ApplyRecipe(GameObject recipeVariant)
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
    void RemoveItem(GameObject itemType)
    {
        var itemData = inventoryItems[itemType];
        itemData.count--;
    }
}
