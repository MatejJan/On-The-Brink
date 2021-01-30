using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{

    public bool active;
    GameObject[] workbenchSlot = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaceItem(GameObject itemType)
    {
        if (workbenchSlot[0] == null)
        {
            workbenchSlot[0] = itemType;
        }
        else if (workbenchSlot[1] == null)
        {
            workbenchSlot[1] = itemType;
        }
    }

    public void RemoveItem(GameObject itemType)
    {
        if (workbenchSlot[0] == itemType)
        {
            workbenchSlot[0] = null;
        }
        else if (workbenchSlot[1] == itemType)
        {
            workbenchSlot[1] = null;
        }
    }

    public void Craft(GameObject itemType)
    {
        RemoveItem(workbenchSlot[0]);
        RemoveItem(workbenchSlot[1]);
        PlaceItem(itemType);
    }

    public void Activate()
    {

    }

    public void Deactivate()
    {
        workbenchSlot[0] = null;
        workbenchSlot[1] = null;
    }
}
