using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SubCatUIItem
{
    [SerializeField] private string itemName = "";
    [SerializeField] private string modelCat = "";
    [SerializeField] private string icon;

    //Getters and Setters
    public string ItemName { get => itemName; set => itemName = value; }
    public string ModelCat { get => modelCat; set => modelCat = value; }
    public string Icon { get => icon; set => icon = value; }
}
