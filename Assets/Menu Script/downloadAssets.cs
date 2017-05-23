using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class downloadAssets : MonoBehaviour {

    public GameObject loadingUI;

    public string BundleURL;
    public int version;
    public string assetName;

    // Use this for initialization
    void Start () {
        assetName = "CompleteTank";
        BundleURL = "https://s3-us-west-2.amazonaws.com/articsp/AssetBundle/texture";
        version = 1;

        StartCoroutine(DownloadAndCache());
    }
	
    public IEnumerator DownloadAndCache()
    {
        loadingUI.SetActive(true);
        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
        {
            yield return www;
            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);
            AssetBundle bundle = www.assetBundle;
            // Unload the AssetBundles compressed contents to conserve memory
            bundle.Unload(false);
            loadingUI.SetActive(false);
        } // memory is freed from the web stream (www.Dispose() gets called implicitly)
    }
}
