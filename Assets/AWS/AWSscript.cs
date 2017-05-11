/// <Written By>
/// Xie Yuan Shan
/// </Written By>

#region using
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using Amazon.CognitoSync;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync.SyncManager;
using System.Text;
#endregion

public class AWSscript : MonoBehaviour
{
    #region AWS Variables
    string IdentityPoolId = "us-west-2:070d16bc-2902-4ac4-940a-1ea8f56d7a06";
    string CognitoIdentityRegion = RegionEndpoint.USWest2.SystemName;
    private RegionEndpoint _CognitoIdentityRegion
    {
        get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
    }
    string S3Region = RegionEndpoint.USWest2.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }

    private static AWSscript instance = null;

    public static AWSscript GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    private IAmazonS3 _s3Client;
    private AWSCredentials _credentials;

    private AWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
            return _credentials;
        }
    }

    private IAmazonS3 Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(Credentials, _S3Region);
            }
            //test comment
            return _s3Client;
        }
    }
    #endregion

    #region Variables
    [System.NonSerialized]
    public bool uploadedXML = false;
    [System.NonSerialized]
    public bool uploadingXML = false;
    [System.NonSerialized]
    public bool downloadingStuff = false;
    [System.NonSerialized]
    public bool downloadedStuff = false;
    [System.NonSerialized]
    public List<string> xmlList = new List<string>();
    [System.NonSerialized]
    public bool fetchingList = false;
    [System.NonSerialized]
    public bool fetchedList = false;

    [Header("LevelCreator Values")]
    [Tooltip("The name of the AmazonWebService bucket")]
    public string S3BucketName = null;

    [Tooltip("The name of the folder for \'LEVELs\' in the bucket")]
    public string levelFolderName = "level";
    [Tooltip("The name of the folder for \'AssetBundles\' in the bucket")]
    public string assetFolderName = "AssetBundle";
    
    #endregion

    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
    }

    #region Download
    /// <summary>
    /// Download from S3 Bucket
    /// </summary>
    /// 

    public void AWSDownload(string fileName, string folderName)
    {
        downloadingStuff = true;

        Client.GetObjectAsync(S3BucketName, folderName + "/" + fileName, (responseObj) =>
                {
                    //Debug.Log("Folder : " +folderName + "\nFile : " +fileName);
                    int progress;
                    var response = responseObj.Response;
                    if (response.ResponseStream != null)
                    {
#if UNITY_ANDROID
    string aeDir = Application.persistentDataPath;
#else
                        string aeDir = Application.dataPath;
#endif
                        if (!Directory.Exists(aeDir + "/Serialization/XMLa/" + folderName))
                        {
                            Directory.CreateDirectory(aeDir + "/Serialization/XMLa/" + folderName);
                        }
                        using (BinaryReader bReader = new BinaryReader(response.ResponseStream))
                        {
                            byte[] buffer = bReader.ReadBytes((int)response.ResponseStream.Length);
                            progress = (int)response.ResponseStream.Length;
                            bReader.ReadBytes((int)response.ResponseStream.Length);
                            File.WriteAllBytes(aeDir + "/Serialization/XMLa/" + folderName + "/" + fileName, buffer);
                            downloadedStuff = true;
                        }
                    }
                });
    }
#endregion

    #region UploadXML
        /// <summary>
        /// UploadXML to S3 Bucket. 
        /// </summary>
        public void AWSUploadXML(string fileName)
        {
            uploadingXML = true;
		
            var stream = new FileStream(Application.dataPath + "/Serialization/XMLa/temp/" + fileName + ".xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            var request = new PostObjectRequest()
            {
                Bucket = S3BucketName,
                Key = levelFolderName + "/" + fileName + ".xml",
                InputStream = stream,
                CannedACL = S3CannedACL.Private
            };

            Client.PostObjectAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                {
                    stream.Close();
                    uploadedXML = true;
                }
            });
        }
    #endregion

    #region List XMLs
        /// <summary>
        /// List XMLs in S3 Bucket
        /// </summary>
        public void AWSListLevels()
        {
            fetchingList = true;

            var request = new ListObjectsRequest
            {
                Prefix = levelFolderName + "/",
                BucketName = S3BucketName
            };

            Client.ListObjectsAsync(request, (responseObject) =>
            {
                if (responseObject.Exception == null)
                {
                    responseObject.Response.S3Objects.ForEach((o) =>
                    {
                        if (o.Key != levelFolderName + "/")
                        {
                        xmlList.Add(o.Key.Replace(levelFolderName + "/", ""));
                        }
                    }
                    );
                    fetchedList = true;
                }
                else
                {

                }
            });
        }
        #endregion

    #region Gets the objects from AWS S3
    public void GetObject(string fileName)
    {
        Client.GetObjectAsync(S3BucketName, levelFolderName + "/" + fileName, (responseObj) =>
        {
            var response = responseObj.Response;
            int progress;
            if (response.ResponseStream != null)
            {
                string aeDir = Application.dataPath;
                if (!Directory.Exists(aeDir + "/Serialization/XML/"))
                {
                    Directory.CreateDirectory(aeDir + "/Serialization/XML/");
                }
                using (BinaryReader bReader = new BinaryReader(response.ResponseStream))
                {
                    byte[] buffer = bReader.ReadBytes((int)response.ResponseStream.Length);
                    progress = (int)response.ResponseStream.Length;
                    bReader.ReadBytes((int)response.ResponseStream.Length);
                    File.WriteAllBytes(aeDir + "/Serialization/XML/" + fileName, buffer);
                    downloadedStuff = true;
                }
                UIManager.GetInstance().Status.text = "Downloaded.";
                Debug.Log("The object selected has been downloaded.");
            }
        });
    }
        #endregion

    #region List the objects from the bucket in AWS S3
    public List<string> getListOfBucketObjects(string redirect)
    {
        List<string> list = new List<string>();
        bool skippedFirst = false;
        ListObjectsRequest request = new ListObjectsRequest
        {
            BucketName = S3BucketName,
            Prefix = levelFolderName + "/"
        };

        Client.ListObjectsAsync(request, (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                UIManager.GetInstance().Status.text = "Connected to AWS S3 Database.";
                Debug.Log("Connected to AWS S3 Database.");
                responseObject.Response.S3Objects.ForEach((o) =>
                {
                    // Because getting a list of object returns the folder as the first array
                    if (skippedFirst)
                    {
                        list.Add(string.Format("{0}\n", o.Key));
                    }
                    else
                    {
                        skippedFirst = true;
                    }
                });
                if (redirect == "LevelManager")
                    LevelManager.GetInstance().loadDownloadsInContainer();
                else if (redirect == "LoadManager")
                    LoadManager.GetInstance().loadDownloadsInContainer();
            }
            else
            {
                Debug.Log("Got Exception \n");
            }
        });
        return list;
    }
        #endregion
}