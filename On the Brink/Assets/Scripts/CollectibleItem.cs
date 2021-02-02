using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public new string name;

    public string ItemType
    {
        get
        {
            return name;
        }
    }
}
