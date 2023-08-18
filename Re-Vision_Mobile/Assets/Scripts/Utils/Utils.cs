using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using UnityEngine.Networking;
using System.IO.Compression;

using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision
{
    /// <summary>
    /// A class with some utility functions. All of them are static.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Load a Sprite from a file stored in the persistant folder.
        /// If the file was not found, it will return the specified file in the built resource
        /// </summary>
        /// <param name="relativePath">The relative path of the file, from the persistant folder</param>
        /// <param name="resource">The path of the resource to use if loading of <paramref name="relativePath"/> failed</param>
        /// <returns>The sprite with the specified image</returns>
        /// <remarks>If failing to load, it will use the resource</remarks>
        public static Sprite LoadFromPeristantOrResource(String relativePath, String resource)
        {
            try
            {
                byte[] bytes = System.IO.File.ReadAllBytes(Utils.MergeWithPersistantPath(relativePath));
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            catch
            {
                return Resources.Load<Sprite>(resource);
            }
        }

        /// <summary>
        /// Download a zip from a direct download URL, extract it's content, and store it in the persistant folder
        /// </summary>
        /// <param name="url">Direct download URL of a zip file</param>
        /// <param name="relativeUnzipedPath">The relative path where save the unzip content, from the persistant</param>
        /// <param name="onFinish">Callback when the download+unzipage+store finished without issues</param>
        /// <param name="onError">Callback when the function failed to do all steps</param>
        /// <returns></returns>
        public static IEnumerator DownloadZipContent(String url, String relativeUnzipedPath, Action onFinish = null, Action onError = null)
        {
            // Download from direct download URL
            UnityWebRequest www = new UnityWebRequest(url);
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                onError?.Invoke();
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(MergeWithPersistantPath(relativeUnzipedPath)));

                // Get the zipped data
                var bytes = www.downloadHandler.data;

                // Unzip the data and save them
                ZipFile.UnZip(MergeWithPersistantPath(relativeUnzipedPath), bytes);

                onFinish?.Invoke();
            }
        }

        /// <summary>
        /// Alias for <code>Path.Combine(Application.persistentDataPath, string.TrimStart('/', '\\'))"</code>
        /// </summary>
        public static string MergeWithPersistantPath(string path)
        {
            return Path.Combine(Application.persistentDataPath, path.TrimStart('/', '\\'));
        }

        /// <summary>
        /// Download a file from a direct download URL, and store it in the persistant folder
        /// </summary>
        /// <param name="url">Direct download URL of a file</param>
        /// <param name="relativeSavePath">The relative path where save the file, from the persistant</param>
        /// <param name="onFinish">Callback when the download+store finished without issues</param>
        /// <param name="onError">Callback when the function failed to do all steps</param>
        public static IEnumerator DownloadAndSaveData(String url, String relativeSavePath, Action onFinish = null, Action onError = null)
        {
            // Download from direct download URL
            UnityWebRequest www = new UnityWebRequest(url);
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                onError?.Invoke();
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(MergeWithPersistantPath(relativeSavePath)));

                // Get the data and save them
                File.WriteAllBytes(MergeWithPersistantPath(relativeSavePath), www.downloadHandler.data);

                onFinish?.Invoke();
            }
        }
    }
}