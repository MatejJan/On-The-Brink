using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{

    public bool active;
    string[] workbenchSlot = new string[2];

    public Inventory inventoryScript;
    public InventoryUI inventoryUIScript;

    Vector3 inventoryPosition;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        inventoryPosition = inventoryUIScript.transform.position;
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
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DestroySlot2();
        }

    }

    public void PlaceItem(string itemType)
    {

        Transform parentSlot = null;

        if (workbenchSlot[0] == null)
        {
            workbenchSlot[0] = itemType;
            parentSlot = transform.Find("WorkbenchItem").Find("Slot1");
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

    // Removes items from the slot array.
    public void RemoveItem(string itemType)
    {
        if (workbenchSlot[0] == itemType)
        {
            workbenchSlot[0] = null;
            if (transform.Find("WorkbenchItem").Find("Slot1").childCount > 0)
            {
                Destroy(transform.Find("WorkbenchItem").Find("Slot1").GetChild(0).gameObject);
            }
        }
        else if (workbenchSlot[1] == itemType)
        {
            workbenchSlot[1] = null;
            if (transform.Find("WorkbenchItem").Find("Slot2").childCount > 0)
            {
                Destroy(transform.Find("WorkbenchItem").Find("Slot2").GetChild(0).gameObject);
            }
        }
    }

    public void Craft(string itemType)
    {
        RemoveItem(workbenchSlot[0]);
        RemoveItem(workbenchSlot[1]);
        PlaceItem(itemType);
    }

    public void Activate()
    {
        inventoryUIScript.EnableUI();
        transform.Find("WorkbenchItem").gameObject.SetActive(true);
        inventoryUIScript.transform.position = inventoryPosition + new Vector3(0, 40, 0);
        active = true;
    }

    public void Deactivate()
    {
        workbenchSlot[0] = null;
        workbenchSlot[1] = null;
        ClearSlots();
        inventoryScript.isActive = false;
        inventoryUIScript.gameObject.SetActive(false);
        transform.Find("WorkbenchItem").gameObject.SetActive(false);
        inventoryUIScript.transform.position = inventoryPosition;
        active = false;
    }

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
    }

    public void DestroySlot1()
    {
        if (transform.Find("WorkbenchItem").Find("Slot1").childCount > 0)
        {
            Destroy(transform.Find("WorkbenchItem").Find("Slot1").GetChild(0).gameObject);
        }

        workbenchSlot[0] = null;
    }

    public void DestroySlot2()
    {
        if (transform.Find("WorkbenchItem").Find("Slot2").childCount > 0)
        {
            Destroy(transform.Find("WorkbenchItem").Find("Slot2").GetChild(0).gameObject);
        }

        workbenchSlot[1] = null;
    }
}
