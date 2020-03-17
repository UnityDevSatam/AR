using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[CreateAssetMenu(fileName = "ModelAssetsList", menuName = "ModelAssests/ModelAssetsList", order = 1)]
[Serializable]
public class ModelUIItemList : ScriptableObject
{
    [SerializeField]
    public List<ModelUIItem> modelList;


    public ModelUIItem GetItem(string id)
    {
        foreach (ModelUIItem m_item in modelList)
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
        foreach (ModelUIItem m_item in modelList)
        {
            if (m_item.ItemName.Equals(id))
            {
                return true;
            }
        }
        return false;
    }

    public List<ModelUIItem> GetSpecificCategory(string specifiedCategory)
    {
        List<ModelUIItem> item = new List<ModelUIItem>();
        foreach (ModelUIItem i in modelList)
        {
            if (i.ModelCat.Equals(specifiedCategory))
            {
                item.Add(i);
            }
        }
        return item;
    }

    public List<ModelUIItem> GetSpecificSubCategory(string specifiedSubCategory)
    {
        List<ModelUIItem> item = new List<ModelUIItem>();
        foreach (ModelUIItem i in modelList)
        {
            if (i.ModelSubCat == specifiedSubCategory)
            {
                item.Add(i);
            }
        }
        return item;
    }

    public void SetSelected(ModelUIItem item)
    {
        foreach(ModelUIItem modelItem in modelList)
        {
            if (modelItem == item)
            {
                modelItem.IsSelected = true;
            }
            else
            {
                modelItem.IsSelected = false;
            }
        }
    }
}
