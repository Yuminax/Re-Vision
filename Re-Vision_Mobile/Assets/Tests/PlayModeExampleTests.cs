using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ReVision.Data;
using ReVision;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Networking;
using System.IO.Compression;

public class PlayModeTests
{

    [Test]
    public void PlayModeTestsSimplePasses()
    {
        // Use the Assert class to test conditions.
        Assert.True(true);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator PlayModeTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        Assert.True(true);
        yield return null;
        Assert.True(true);

        Answer a = new Answer();
        a.IsCorrect = true;
        Assert.True(a.IsCorrect);
    }

    [UnityTest]
    public IEnumerator ZipDownloader()
    {
        string url = "https://drive.google.com/uc?export=download&id=18g-Beoo52G1K6ERbacv_vlj0bytdiOQ2";
        string unzipedPath = "/TEST_PlayMode/extract/";
        string fileExist = unzipedPath + "Question/swiss.png";



        yield return Utils.DownloadZipContent(
            url, unzipedPath,
            () => { Assert.True(File.Exists(Utils.MergeWithPersistantPath(fileExist))); },
            () => { Assert.Fail(); }
            );
    }
}
