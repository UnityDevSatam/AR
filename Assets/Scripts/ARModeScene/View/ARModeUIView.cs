using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ARModeUIView : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject subCatContainer;
    public GameObject itemContainer;
    public GameObject subCatPrefab;
    public GameObject itemPrefab;
    public SubCatUIItemList subCatUIItemList;
    public ModelUIItemList modelUIItemList;

    private void Start()
    {
        FillSubCategoryDetail();
        FillModelListDetail();
        AssignModelPrefabs();
        PopulateSubCategories("City");

    }

    public void FillSubCategoryDetail()
    {
        SubCatDetails subCatDetails = DownloadController.Instance.subCatDetails;
        int count = subCatDetails.SubCategory.Count;
        for (int i=0;i< count; i++)
        {
            SubCatUIItem item = new SubCatUIItem();
            item.ItemName = subCatDetails.SubCategory[i].ItemName;
            item.Icon = subCatDetails.SubCategory[i].icon;
            item.ModelCat = subCatDetails.SubCategory[i].catName;
            if (!subCatUIItemList.CheckAlreadyExist(item.ItemName))
            {
                subCatUIItemList.subCatList.Add(item);
            }
        }
        Debug.Log("FillSubCategoryDetail-------");
    }

    public void FillModelListDetail()
    {
        ModelDetails modelDetails = DownloadController.Instance.modelDetails;
        int catCount = modelDetails.Category.Count;
        string catName, subCatName;
        for (int i = 0; i < catCount; i++)
        {
            catName = modelDetails.Category[i].catName;
            int subcatCount = modelDetails.Category[i].SubCategory.Count;
            for (int j = 0; j < subcatCount; j++)
            {
                subCatName = modelDetails.Category[i].SubCategory[j].subCatName;
                int modelCount = modelDetails.Category[i].SubCategory[j].ObjectList.Count;
                for (int k = 0; k < modelCount; k++)
                {
                    ModelUIItem item = new ModelUIItem();
                    item.ItemName = modelDetails.Category[i].SubCategory[j].ObjectList[k].ItemName;
                    item.ModelCat = catName;
                    item.ModelSubCat = subCatName;
                    item.Icon = modelDetails.Category[i].SubCategory[j].ObjectList[k].icon;
                    if (!modelUIItemList.CheckAlreadyExist(item.ItemName))
                    {
                        modelUIItemList.modelList.Add(item);
                    }
                }
            }
        }
        Debug.Log("FillModelListDetail-------");
    }

    public void AssignModelPrefabs()
    {
        int count = DownloadController.Instance.assetList.Count;
        for(int i=0;i< count; i++)
        {
            modelUIItemList.modelList[i].ModelPrefab = DownloadController.Instance.assetList[i];
        }
        Debug.Log("AssignModelPrefabs-------");
    }

    public void PopulateSubCategories(string cat)
    {
        DeleteAllChildrens(subCatContainer);
        Toggle firstSubCat = null;
        List<SubCatUIItem> subCatUIItems = subCatUIItemList.GetSpecificCategory(cat);

        foreach (SubCatUIItem subCatUIItem in subCatUIItems)
        {
            GameObject objItem = Instantiate(subCatPrefab, subCatContainer.transform);
            if (!string.IsNullOrEmpty(subCatUIItem.Icon))
            {
                StartCoroutine(GetTexture(objItem.transform.Find("Background").transform.Find("Icon").GetComponent<Image>(), subCatUIItem.Icon));
            }
            
            objItem.transform.Find("Background").transform.Find("Title").GetComponent<TextMeshProUGUI>().text = subCatUIItem.ItemName;
            objItem.GetComponent<Toggle>().group = itemContainer.GetComponent<ToggleGroup>();
            if (firstSubCat == null)
            {
                firstSubCat = objItem.GetComponent<Toggle>();
                PopulateSubCatItems(subCatUIItem.ItemName);
            }

            EventTrigger trigger = objItem.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) =>
            {
                PopulateSubCatItems(subCatUIItem.ItemName);
            });
            trigger.triggers.Add(entry);
        }
        firstSubCat.isOn = true;
        Debug.Log("PopulateSubCategories-------");
    }

    public void PopulateSubCatItems(string subCatName)
    {
        DeleteAllChildrens(itemContainer);

        List<ModelUIItem> modelUIItems = modelUIItemList.GetSpecificSubCategory(subCatName);

        foreach(ModelUIItem modelUIItem in modelUIItems)
        {
            GameObject objItem = Instantiate(itemPrefab, itemContainer.transform);
            if (!string.IsNullOrEmpty(modelUIItem.Icon))
            {
                StartCoroutine(GetTexture(objItem.transform.Find("Background").transform.Find("Icon").GetComponent<Image>(), modelUIItem.Icon));
            }
            
            objItem.transform.Find("Background").transform.Find("Title").GetComponent<TextMeshProUGUI>().text = modelUIItem.ItemName;
            objItem.GetComponent<Toggle>().group = itemContainer.GetComponent<ToggleGroup>();
            objItem.GetComponent<Toggle>().isOn = modelUIItem.IsSelected;

            EventTrigger trigger = objItem.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) =>
            {
                Debug.Log("modelUIItem");
                modelUIItemList.SetSelected(modelUIItem);
                ItemClick(modelUIItem);
            });
            trigger.triggers.Add(entry);
        }
        Debug.Log("PopulateSubCatItems-------"+ subCatName);
    }

    public void ItemClick(ModelUIItem modelUIItem)
    {
        if (ARModeManager.Instance.selectedModel)
            Destroy(ARModeManager.Instance.selectedModel);
        //ARModeManager.Instance.selectedModel = modelUIItem.ModelPrefab;
        Vector3 startPos = ARModeManager.Instance.planeObject.transform.Find("StartPointPlane").transform.position;
        ARModeManager.Instance.selectedModel = Instantiate(modelUIItem.ModelPrefab, startPos, Quaternion.identity);
    }

    IEnumerator GetTexture(Image image,string imageUri)    {        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);        yield return www.SendWebRequest();        if (www.isNetworkError || www.isHttpError)        {            Debug.Log(www.error);        }        else        {            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;            image.sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));        }    }

    public void DeleteAllChildrens(GameObject obj)
    {
        foreach(Transform child in obj.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
