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
    public bool active;

    private Inventory inventoryScript;
    private InventoryUI inventoryUIScript;

    private string[] workbenchSlot = new string[2];

    private Dictionary<string, Dictionary<string, Recipe>> recipesForIngredients = new Dictionary<string, Dictionary<string, Recipe>>();
    private Recipe activeRecipe = null;

    private Vector3 inventoryPosition;
    private Vector3 inventoryPositionOffset = new Vector3(0, 27.5f, 0);
    private static string GetCollectibleItemType(GameObject collectibleItem)
    {
        return collectibleItem.GetComponent<CollectibleItem>().name;
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryScript = GameObject.Find("Inventory").GetComponent<Inventory>();
        inventoryUIScript = GameObject.Find("Canvas").transform.Find("Inventory UI").gameObject.GetComponent<InventoryUI>();

        active = false;
        inventoryPosition = inventoryUIScript.transform.localPosition;

        Object[] allRecipePrefabObjects = Resources.LoadAll("Recipes");

        // Create a map of recipe ingredients for fast retrieval.
        foreach (GameObject recipePrefab in allRecipePrefabObjects)
        {
            Recipe recipe = recipePrefab.GetComponent<Recipe>();

            void placeRecipeInMap(GameObject ingredient1, GameObject ingredient2)
            {
                string ingredient1Name = GetCollectibleItemType(ingredient1);
                string ingredient2Name = GetCollectibleItemType(ingredient2);

                if (!recipesForIngredients.ContainsKey(ingredient1Name))
                {
                    recipesForIngredients[ingredient1Name] = new Dictionary<string, Recipe>();
                }

                recipesForIngredients[ingredient1Name][ingredient2Name] = recipe;
            }

            placeRecipeInMap(recipe.ingredients[0], recipe.ingredients[1]);
            placeRecipeInMap(recipe.ingredients[1], recipe.ingredients[0]);
        }

        HandleSlotsChanged();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            RemoveItem(workbenchSlot[0]);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            RemoveItem(workbenchSlot[1]);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Craft();
        }
    }

    // PUBLIC METHODS FOR INTERACTING WITH THE WORKBENCH

    // Activate the workbench, display it.
    public void Activate()
    {
        inventoryUIScript.EnableUI();

        var workbenchCamera = transform.Find("Workbench Camera").gameObject;
        workbenchCamera.SetActive(true);

        var canvas = GameObject.Find("Canvas");
        canvas.GetComponent<Canvas>().worldCamera = workbenchCamera.GetComponent<Camera>();
        canvas.GetComponent<PixelArtCanvas>().Recalculate();

        GameObject.Find("Character renderer").GetComponent<MeshRenderer>().enabled = false;
        inventoryUIScript.transform.localPosition = inventoryPosition + inventoryPositionOffset;

        active = true;
    }

    // Closes the workbench UI and clears the slots.
    public void Deactivate()
    {
        DestroySlot1();
        DestroySlot2();
        HandleSlotsChanged();

        transform.Find("Workbench Camera").gameObject.SetActive(false);

        var canvas = GameObject.Find("Canvas");
        canvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Scene Camera").GetComponent<Camera>();
        canvas.GetComponent<PixelArtCanvas>().Recalculate();

        GameObject.Find("Character renderer").GetComponent<MeshRenderer>().enabled = true;
        inventoryUIScript.transform.localPosition = inventoryPosition;

        active = false;
    }

    // Put an instance of an item at one of the two workbench slots.
    public void PlaceItem(string itemType)
    {
        PlaceItemInSlot(itemType);
        HandleSlotsChanged();
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

        HandleSlotsChanged();
    }

    // Combines two items, remove them from the inventory and add the result of the items.
    public void Craft()
    {
        if (activeRecipe == null) return;

        // Update player inventory.
        inventoryScript.RemoveItem(GetCollectibleItemType(activeRecipe.ingredients[0]));
        inventoryScript.RemoveItem(GetCollectibleItemType(activeRecipe.ingredients[1]));
        inventoryScript.AddItem(GetCollectibleItemType(activeRecipe.result));

        // Update workbench UI.
        DestroySlot1();
        DestroySlot2();
        PlaceItemInSlot(GetCollectibleItemType(activeRecipe.result));
        HandleSlotsChanged();
    }

    // INTERNAL METHODS FOR HANDLING SLOTS

    // Completely clears what is in slot 1 on the workbench.
    private void DestroySlot1()
    {
        Transform slotTransform = transform.Find("WorkbenchItem").Find("Slot1");
        if (slotTransform.childCount > 0)
        {
            Destroy(slotTransform.GetChild(0).gameObject);
        }

        workbenchSlot[0] = null;
    }

    // Completely clears what is in slot 2 on the workbench.
    private void DestroySlot2()
    {
        if (transform.Find("WorkbenchItem").Find("Slot2").childCount > 0)
        {
            Destroy(transform.Find("WorkbenchItem").Find("Slot2").GetChild(0).gameObject);
        }

        workbenchSlot[1] = null;
    }

    private void PlaceItemInSlot(string itemType)
    {
        Transform parentSlot = null;

        if (workbenchSlot[0] == null)
        {
            if (workbenchSlot[1] != itemType)
            {
                workbenchSlot[0] = itemType;
                parentSlot = transform.Find("WorkbenchItem").Find("Slot1");
            }
        }
        else if (workbenchSlot[1] == null)
        {
            if (workbenchSlot[0] != itemType)
            {
                workbenchSlot[1] = itemType;
                parentSlot = transform.Find("WorkbenchItem").Find("Slot2");
            }
        }

        if (parentSlot != null)
        {
            var itemTypePrefab = inventoryScript.itemTypePrefabs[itemType];
            Instantiate(itemTypePrefab, parentSlot);
        }
    }

    private void HandleSlotsChanged()
    {
        // Find out if we have items for a valid recipe.
        DetermineActiveRecipe();

        // Show hammer if we have a valid recipe.
        transform.Find("WorkbenchItem").Find("Hammer").gameObject.SetActive(activeRecipe != null);

        // Highlight inventory items.
        RecalculateHighlights();
    }

    // Checks if the two items on the workbench can be combined or not.
    private void DetermineActiveRecipe()
    {
        activeRecipe = null;

        if (workbenchSlot[0] != null && workbenchSlot[1] != null)
        {
            if (recipesForIngredients.ContainsKey(workbenchSlot[0]))
            {
                Dictionary<string, Recipe> recipesForIngredient1 = recipesForIngredients[workbenchSlot[0]];

                if (recipesForIngredient1.ContainsKey(workbenchSlot[1]))
                {
                    activeRecipe = recipesForIngredient1[workbenchSlot[1]];
                }
            }
        }
    }

    private void RecalculateHighlights()
    {
        bool onlySlot1 = workbenchSlot[0] != null && workbenchSlot[1] == null;
        bool onlySlot2 = workbenchSlot[0] == null && workbenchSlot[1] != null;

        RemoveAllHighlights();

        if (onlySlot1 || onlySlot2)
        {
            HandleHighlightsForItem(workbenchSlot[0] ?? workbenchSlot[1]);
        }
    }

    private void RemoveAllHighlights()
    {
        foreach (string itemTypeKey in inventoryScript.itemTypePrefabs.Keys)
        {
            inventoryScript.SetHighlightForItem(itemTypeKey, false);
        }
    }

    // Highlights all potential ingredients.
    private void HandleHighlightsForItem(string itemType)
    {
        // Check if we have any recipes with this item.
        if (!recipesForIngredients.ContainsKey(itemType)) return;

        // Highlight all potential ingredients.
        foreach (string potentialIngredient in recipesForIngredients[itemType].Keys)
        {
            inventoryScript.SetHighlightForItem(potentialIngredient, true);
        }
    }
}
