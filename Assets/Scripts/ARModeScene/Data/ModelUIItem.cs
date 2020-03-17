using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ModelCategory
{
    NONE,
    CITY,
    HOME
}

[Serializable]
public class ModelUIItem
{
    [SerializeField] private string itemName = "";
    [SerializeField] private string modelCat = "";
    [SerializeField] private string modelSubCat = "";
    [SerializeField] private string icon;
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private bool isSelected;

    //Getters and Setters
    public string ItemName { get => itemName; set => itemName = value; }
    public string ModelCat { get => modelCat; set => modelCat = value; }
    public string ModelSubCat { get => modelSubCat; set => modelSubCat = value; }
    public string Icon { get => icon; set => icon = value; }
    public GameObject ModelPrefab { get => modelPrefab; set => modelPrefab = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }
}
