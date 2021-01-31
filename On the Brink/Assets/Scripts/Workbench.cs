using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeData
{
    public GameObject mainIngredient;
    public GameObject secondIngredient;
    public GameObject result;
}

public class Workbench : MonoBehaviour
{
    public GameObject[,] recipes;

    public bool active;
    string[] workbenchSlot = new string[2];

    public Inventory inventoryScript;
    public InventoryUI inventoryUIScript;

    CollectibleItem collectibleItem;

    Vector3 inventoryPosition;
    Vector3 hammerPosition;

    bool canCraft;
    string craftThis;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        inventoryPosition = inventoryUIScript.transform.position;

        Object[] allRecipePrefabObjects = Resources.LoadAll("Recipes");

        recipes = new GameObject[allRecipePrefabObjects.Length * 2, 3];

        int index = 0;

        // Create an 2dimensional array with all ingredients and results.
        foreach (GameObject recipePrefab in allRecipePrefabObjects)
        {
            for (int item = 0; item < 2; item++)
            {
                // Ingredient 1.
                GameObject recipeIngredient = recipePrefab.GetComponent<Recipe>().ingredients[item];
                recipes[index, 0] = recipeIngredient;

                // Ingredient 2.
                if (item == 0)
                {
                    recipes[index, 1] = recipePrefab.GetComponent<Recipe>().ingredients[1];
                }

                if (item == 1)
                {
                    recipes[index, 1] = recipePrefab.GetComponent<Recipe>().ingredients[0];
                }

                // Ingredient 3.
                recipes[index, 2] = recipePrefab.GetComponent<Recipe>().result;
                index++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (active)
            {
                Deactivate();
            }
            else if (!active && !inventoryScript.isActive)
            {
                Activate();
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            DestroySlot1();
            if (workbenchSlot[1] != null)
            {
                CheckForRecipes(workbenchSlot[1]);
            }

            if (workbenchSlot[0] == null && workbenchSlot[1] == null)
            {
                foreach (string itemTypeKey in inventoryScript.itemTypePrefabs.Keys)
                {
                    inventoryScript.RemoveHighlight(itemTypeKey);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DestroySlot2();
            if (workbenchSlot[0] != null)
            {
                CheckForRecipes(workbenchSlot[0]);
            }

            if (workbenchSlot[0] == null && workbenchSlot[1] == null)
            {
                foreach (string itemTypeKey in inventoryScript.itemTypePrefabs.Keys)
                {
                    inventoryScript.RemoveHighlight(itemTypeKey);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && canCraft)
        {
            Craft(craftThis);
            Debug.Log("Craft it!");
            canCraft = false;
            transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(false);
        }

    }

    // Put an instance of an item at one of the two workbench slots.
    public void PlaceItem(string itemType)
    {
        if (active)
        {
            Transform parentSlot = null;

            if (workbenchSlot[0] == null)
            {
                workbenchSlot[0] = itemType;
                parentSlot = transform.Find("WorkbenchItem").Find("Slot1");
                CheckForRecipes(itemType);
            }
            else if (workbenchSlot[1] == null)
            {
                workbenchSlot[1] = itemType;
                parentSlot = transform.Find("WorkbenchItem").Find("Slot2");
            }

            if (parentSlot == null)
            {
                return;
            }
            var itemTypePrefab = inventoryScript.itemTypePrefabs[itemType];
            Instantiate(itemTypePrefab, parentSlot);
        }

        if (workbenchSlot[0] != null && workbenchSlot[1] != null)
        {
            Craftable(workbenchSlot[0], workbenchSlot[1]);

            foreach (string itemTypeKey in inventoryScript.itemTypePrefabs.Keys)
            {
                inventoryScript.RemoveHighlight(itemTypeKey);
            }
        }
    }

    // Remove an item from the correct spot on the workbench.
    public void RemoveItem(string itemType)
    {
        if (workbenchSlot[0] == itemType)
        {
            DestroySlot1();
        }
        else if (workbenchSlot[1] == itemType)
        {
            DestroySlot2();
        }
    }

    // Goes through all recipes and creates a list (of arrays with two items in each array) with all items that can be combined with this item, and what result they produce.
    public void CheckForRecipes(string itemType)
    {
        GameObject ingredient = inventoryScript.itemTypePrefabs[itemType];
        var secondIngredientAndResult = new List<GameObject[]>();
        for (int index = 0; index < recipes.Length / 3; index++)
        {
            if (recipes[index, 0] == ingredient)
            {
                secondIngredientAndResult.Add(new GameObject[2] { recipes[index, 1], recipes[index, 2] });
            }
        }
        HighlightSecondIngredient(secondIngredientAndResult);
    }

    // Highlights every ingredient (the first item in every array in the list) that can be combined with the current ingredient.
    public void HighlightSecondIngredient(List<GameObject[]> secondIngredientAndResult)
    {
        foreach (GameObject[] recipe in secondIngredientAndResult)
        {
            inventoryScript.HighlightItem(recipe[0].GetComponent<CollectibleItem>().name);
        }
    }

    // Checks if the two items on the workbench can be combined or not.
    public void Craftable(string itemSlot1, string itemSlot2)
    {
        GameObject ingredientOne = inventoryScript.itemTypePrefabs[itemSlot1];
        GameObject ingredientTwo = inventoryScript.itemTypePrefabs[itemSlot2];
        for (int mainIngredient = 0; mainIngredient < recipes.Length / 3; mainIngredient++)
        {
            if (recipes[mainIngredient, 0] == ingredientOne)
            {
                if (recipes[mainIngredient, 1] == ingredientTwo)
                {
                    string resultItemType = recipes[mainIngredient, 2].GetComponent<CollectibleItem>().name;
                    transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(true);
                    Debug.Log("Craftable!");
                    canCraft = true;
                    craftThis = resultItemType;
                }
                else
                {
                    canCraft = false;
                }
            }
            else
            {
                canCraft = false;
            }
        }
    }

    // Combines two items //TODO Remove the two items from the inventory, add the new item to the inventory.
    public void Craft(string itemType)
    {
        DestroySlot1();
        DestroySlot2();
        PlaceItem(itemType);
        inventoryScript.AddItem(itemType);
    }

    // Activate the workbench, display it.
    public void Activate()
    {
        inventoryUIScript.EnableUI();
        transform.Find("WorkbenchItem").gameObject.SetActive(true);
        inventoryUIScript.transform.position = inventoryPosition + new Vector3(0, 40, 0);
        active = true;
    }

    // Closes the workbench UI and clears the slots.
    public void Deactivate()
    {
        DestroySlot1();
        DestroySlot2();
        inventoryScript.isActive = false;
        inventoryUIScript.gameObject.SetActive(false);
        transform.Find("WorkbenchItem").gameObject.SetActive(false);
        inventoryUIScript.transform.position = inventoryPosition;
        active = false;
        transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(false);
    }

    // Not sure I need this one.
    public void ClearSlots()
    {
        if (transform.Find("WorkbenchItem").Find("Slot1").childCount > 0)
        {
            Destroy(transform.Find("WorkbenchItem").Find("Slot1").GetChild(0).gameObject);
        }

        if (transform.Find("WorkbenchItem").Find("Slot2").childCount > 0)
        {
            Destroy(transform.Find("WorkbenchItem").Find("Slot2").GetChild(0).gameObject);
        }

        foreach (string itemType in inventoryScript.itemTypePrefabs.Keys)
        {
            inventoryScript.RemoveHighlight(itemType);
        }
    }

    // Completely clears what is in slot 1 on the workbench.
    public void DestroySlot1()
    {
        if (transform.Find("WorkbenchItem").Find("Slot1").childCount > 0)
        {
            Destroy(transform.Find("WorkbenchItem").Find("Slot1").GetChild(0).gameObject);
        }

        workbenchSlot[0] = null;

        foreach (string itemType in inventoryScript.itemTypePrefabs.Keys)
        {
            inventoryScript.RemoveHighlight(itemType);
        }

        transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(false);
    }

    // Completely clears what is in slot 2 on the workbench.
    public void DestroySlot2()
    {
        if (transform.Find("WorkbenchItem").Find("Slot2").childCount > 0)
        {
            Destroy(transform.Find("WorkbenchItem").Find("Slot2").GetChild(0).gameObject);
        }

        workbenchSlot[1] = null;

        transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(false);
    }
}
