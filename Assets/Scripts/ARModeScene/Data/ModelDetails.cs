using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectList
{
    public string ItemName;
    public string icon;
}

[Serializable]
public class SubCategory
{
    public string subCatName;
    public List<ObjectList> ObjectList;
}

[Serializable]
public class Category
{
    public string catName;
    public List<SubCategory> SubCategory;
}

[Serializable]
public class ModelDetails
{
    public List<Category> Category;
}
