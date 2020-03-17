using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DownloadController : MonoBehaviour
{
    public static DownloadController Instance;

    [HideInInspector]
    public List<GameObject> assetList;
    [SerializeField]
    public string[] assetNames;
    [HideInInspector]
    public ModelDetails modelDetails;
    [HideInInspector]
    public SubCatDetails subCatDetails;
    private bool isInternet;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        DownLoadAssets();
    }

    public void DownLoadAssets()
    {
        foreach (string label in assetNames)
        {
            Addressables.DownloadDependenciesAsync(label).Completed += (op) =>
            {
                Debug.Log("Download Complete------------" + op.PercentComplete);
                OnDownloadCompleted(label);
            };
        }

        //Download Json
        Addressables.DownloadDependenciesAsync("ModelDetails").Completed += (op) =>
        {
            Debug.Log("ModelDetail Download Complete------------" + op.PercentComplete);
            Addressables.LoadAssetAsync<TextAsset>("ModelDetails").Completed += OnDownloadModelDetails;
            
        };

        //Download Json
        Addressables.DownloadDependenciesAsync("SubCategoryList").Completed += (op) =>
        {
            Debug.Log("SubCategoryList Download Complete------------" + op.PercentComplete);
            Addressables.LoadAssetAsync<TextAsset>("SubCategoryList").Completed += OnDownloadSubCategoryList;
        };

    }

    private void OnDownloadSubCategoryList(AsyncOperationHandle<TextAsset> obj)
    {
        TextAsset jsonString = obj.Result;
        string temp = jsonString.text;
        subCatDetails = JsonUtility.FromJson<SubCatDetails>(temp);
        Debug.Log("jsonString------------" + temp);
    }

    private void OnDownloadModelDetails(AsyncOperationHandle<TextAsset> obj)
    {
        TextAsset jsonString = obj.Result;
        string temp = jsonString.text;
        modelDetails = JsonUtility.FromJson<ModelDetails>(temp);
        Debug.Log("jsonString------------" + temp);
    }

    private void OnDownloadCompleted(string label)
    {
        Addressables.LoadAssetAsync<GameObject>(label).Completed += OnLoad;
    }

    private void OnLoad(AsyncOperationHandle<GameObject> obj)
    {
        assetList.Add(obj.Result);
    }

    void WaitForInternet()
    {
        if (isInternet)
        {
            StartCoroutine(CheckInternet());
            isInternet = false;
        }
    }

    IEnumerator CheckInternet()
    {
        yield return new WaitUntil(() => Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
        isInternet = true;
    }
}
