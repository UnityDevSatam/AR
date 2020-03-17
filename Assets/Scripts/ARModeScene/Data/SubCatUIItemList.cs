using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SubCatList", menuName = "SubCategory/SubCategoryList", order = 1)]
[Serializable]
public class SubCatUIItemList : ScriptableObject
{
    [SerializeField]
    public List<SubCatUIItem> subCatList;


    public SubCatUIItem GetItem(string id)
    {
        foreach (SubCatUIItem m_item in subCatList)
        {
            if (m_item.ItemName.Equals(id))
            {
                return m_item;
            }
        }
        return null;
    }

    public bool CheckAlreadyExist(string id)
    {
        foreach (SubCatUIItem m_item in subCatList)
        {
            if (m_item.ItemName.Equals(id))
            {
                return true;
            }
        }
        return false;
    }

    public List<SubCatUIItem> GetSpecificCategory(string specifiedCategory)
    {
        List<SubCatUIItem> item = new List<SubCatUIItem>();
        foreach (SubCatUIItem i in subCatList)
        {
            if (i.ModelCat.Equals(specifiedCategory))
            {
                item.Add(i);
            }
        }
        return item;
    }
}
