using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SubCategoryList
{
    public string ItemName;
    public string icon;
    public string catName;
}

[Serializable]
public class SubCatDetails
{
    public List<SubCategoryList> SubCategory;
}
