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

    private Inventory inventoryScript;
    private InventoryUI inventoryUIScript;

    CollectibleItem collectibleItem;

    Vector3 inventoryPosition;
    Vector3 inventoryPositionOffset = new Vector3(0, 27.5f, 0);

    bool canCraft;
    string craftThis;
    string useThisItem1;
    string useThisItem2;

    // Start is called before the first frame update
    void Start()
    {
        inventoryScript = GameObject.Find("Inventory").GetComponent<Inventory>();
        inventoryUIScript = GameObject.Find("Canvas").transform.Find("Inventory UI").gameObject.GetComponent<InventoryUI>();

        active = false;
        inventoryPosition = inventoryUIScript.transform.localPosition;
        canCraft = false;

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
            Craft(useThisItem1, useThisItem2, craftThis);
            Debug.Log("Craft it!");
            canCraft = false;
            //transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("Craft").gameObject.SetActive(false);
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
                if (workbenchSlot[1] != itemType)
                {
                    workbenchSlot[0] = itemType;
                    parentSlot = transform.Find("WorkbenchItem").Find("Slot1");
                    GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("DestroySlot1").gameObject.SetActive(true);
                    CheckForRecipes(itemType);
                }
            }
            else if (workbenchSlot[1] == null)
            {
                if (workbenchSlot[0] != itemType)
                {
                    workbenchSlot[1] = itemType;
                    parentSlot = transform.Find("WorkbenchItem").Find("Slot2");
                    GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("DestroySlot2").gameObject.SetActive(true);
                }
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

    public void RemoveItemSlot1()
    {
        if (workbenchSlot[0] != null)
        {
            RemoveItem(workbenchSlot[0]);
        }
    }

    // Goes through all recipes and creates a list with all items that can be combined with this item.
    public void CheckForRecipes(string itemType)
    {
        GameObject ingredient = inventoryScript.itemTypePrefabs[itemType];
        var secondIngredient = new List<GameObject>();
        for (int index = 0; index < recipes.Length / 3; index++)
        {
            if (recipes[index, 0] == ingredient)
            {
                secondIngredient.Add(recipes[index, 1]);
            }
        }
        HighlightSecondIngredient(secondIngredient);
    }

    // Highlights every ingredient that can be combined with the current ingredient.
    public void HighlightSecondIngredient(List<GameObject> secondIngredient)
    {
        foreach (GameObject ingredient in secondIngredient)
        {
            inventoryScript.HighlightItem(ingredient.GetComponent<CollectibleItem>().name);
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
                    string item1 = recipes[mainIngredient, 0].GetComponent<CollectibleItem>().name;
                    string item2 = recipes[mainIngredient, 1].GetComponent<CollectibleItem>().name;
                    string resultItemType = recipes[mainIngredient, 2].GetComponent<CollectibleItem>().name;
                    //transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(true);
                    GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("Craft").gameObject.SetActive(true);
                    Debug.Log("Craftable!");
                    canCraft = true;
                    craftThis = resultItemType;
                    useThisItem1 = item1;
                    useThisItem2 = item2;
                    return;
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

    // Combines two items, remove them from the inventory and add the result of the items.
    public void Craft(string ingredient1, string ingredient2, string result)
    {
        DestroySlot1();
        DestroySlot2();
        PlaceItem(result);

        inventoryScript.RemoveItem(ingredient1);
        inventoryScript.RemoveItem(ingredient2);
        inventoryScript.AddItem(result);
    }

    // Activate the workbench, display it.
    public void Activate()
    {
        inventoryUIScript.EnableUI();

        var workbenchCamera = transform.Find("Workbench Camera").gameObject;
        workbenchCamera.SetActive(true);
        
        var canvas = GameObject.Find("Canvas");
        canvas.transform.Find("WorkbenchUI").gameObject.SetActive(true);

        canvas.GetComponent<Canvas>().worldCamera = workbenchCamera.GetComponent<Camera>();

        GameObject.Find("Character renderer").GetComponent<MeshRenderer>().enabled = false;
        inventoryUIScript.transform.localPosition = inventoryPosition + inventoryPositionOffset;
        active = true;
    }

    // Closes the workbench UI and clears the slots.
    public void Deactivate()
    {
        DestroySlot1();
        DestroySlot2();
        inventoryScript.isActive = false;
        inventoryUIScript.gameObject.SetActive(false);
        transform.Find("Workbench Camera").gameObject.SetActive(false);

        var canvas = GameObject.Find("Canvas");
        canvas.transform.Find("WorkbenchUI").gameObject.SetActive(false);

        canvas.transform.Find("WorkbenchUI").gameObject.SetActive(true);

        canvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Scene Camera").GetComponent<Camera>();

        GameObject.Find("Character renderer").GetComponent<MeshRenderer>().enabled = true;
        inventoryUIScript.transform.localPosition = inventoryPosition;
        active = false;
        //transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("Craft").gameObject.SetActive(false);
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

        if (workbenchSlot[1] != null)
        {
            CheckForRecipes(workbenchSlot[1]);
        }

        //transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("Craft").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("DestroySlot1").gameObject.SetActive(false);
    }

    // Completely clears what is in slot 2 on the workbench.
    public void DestroySlot2()
    {
        if (transform.Find("WorkbenchItem").Find("Slot2").childCount > 0)
        {
            Destroy(transform.Find("WorkbenchItem").Find("Slot2").GetChild(0).gameObject);
        }

        workbenchSlot[1] = null;

        foreach (string itemType in inventoryScript.itemTypePrefabs.Keys)
        {
            inventoryScript.RemoveHighlight(itemType);
        }

        if (workbenchSlot[0] != null)
        {
            CheckForRecipes(workbenchSlot[0]);
        }

        //transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("Craft").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("WorkbenchUI").Find("DestroySlot2").gameObject.SetActive(false);
    }

    public void PressCraftButton()
    {
        if (canCraft)
        {
            Craft(useThisItem1, useThisItem2, craftThis);
        }
    }
}
