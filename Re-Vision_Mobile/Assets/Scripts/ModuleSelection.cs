using ReVision.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ReVision;
using System.IO;

// /!\ TODO : For now, we assume that the script is linked to the "Container" of the buttons (this.gameObject == GameObject.Find("Content"))

public class ModuleSelection : MonoBehaviour
{
    private const string RESOURCE_FOLDER = "images/";
    private int loadedCount = 0;

    [SerializeField] private bool canLoadGame = false; // Jonas tmp fix : to avoid loading game from skin selection
    [SerializeField] private GameObject scrollBar;

    float scroll_pos = 0;
    float[] pos;

    // Start is called before the first frame update
    void Start()
    {
        if (canLoadGame)
        {
            // For all existing buttons, we link their action with a default quizz + skin
            foreach (Transform child in this.gameObject.transform)
            {
                child.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnStartModule(null); // Start the game with default data of game
                });
            }

            LoadExistingModules();
        }
    }

    // Update is called once per frame
    void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1);

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollBar.GetComponent<Scrollbar>().value;
        }
        else
        {
            //Stop on a module and not in the middle
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }

        //Change scale button isn't in the middle
        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                for (int j = 0; j < pos.Length; j++)
                {
                    if (j != i)
                    {
                        transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(0.7f, 0.7f), 0.1f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Perform the read data of a json file and create a button if read success
    /// </summary>
    /// <param name="storedJsonPath">Absolute path of the json data</param>
    private IEnumerator GetData(string storedJsonPath)
    {
        Debug.Log("Processing Data, wait");

        WWW www = new WWW(storedJsonPath);
        yield return www;
        if (www.error == null)
        {
            JSONData jsonData = ProcessJsonData(www.text);

            Debug.Log("Data Processed");
            OnDataLoaded(jsonData);
        }
        else
        {
            Debug.Log("Something went wrong");
        }
    }

    /// <summary>
    /// Perform the start of "read json data" and import it to the module selection button
    /// </summary>
    /// <param name="storedJsonPath">Absolute path of the json data</param>
    public void StartGetData(string storedJsonPath)
    {
        StartCoroutine(GetData(storedJsonPath));
    }

    /// <summary>
    /// Read the data of the json file
    /// </summary>
    /// <param name="stringJsonData">json data inline string</param>
    /// <returns>The serialised data of the file</returns>
    private JSONData ProcessJsonData(string stringJsonData)
    {
        JSONData jsnData = JsonUtility.FromJson<JSONData>(stringJsonData);
        Debug.Log(jsnData.name);

        // Download the data if needed
        string imageDlPath = Utils.MergeWithPersistantPath(RESOURCE_FOLDER + jsnData.name);
        if (!Directory.Exists(Utils.MergeWithPersistantPath(imageDlPath)))
            {
                Debug.Log("Downloading module images");

                StartCoroutine(Utils.DownloadZipContent(jsnData.url, imageDlPath,
                    () => { Debug.Log("Success download module images"); },
                    () => { Debug.Log("Failed download module images"); })
                );
            }

        foreach (QuestionsList x in jsnData.questions)
        {
            Debug.Log("question: " + x.question);
            Debug.Log("answer1: " + x.answers1);
            Debug.Log("image: " + x.image);
            Debug.Log("wich one correcte: " + x.correct);
        }

        foreach (RewardsList x in jsnData.rewards)
        {
            Debug.Log("type: " + x.type);
            Debug.Log("criteria: " + x.criteria);
        }

        return jsnData;
    }

    /// <summary>
    /// Start (switch to scene Level) the module with the specified data.
    /// </summary>
    /// <param name="jsonData">The data of the module</param>
    /// <remarks>if <paramref name="jsonData"/> is null, the game will use defaults data hard coded in the game script</remarks>

    private void OnStartModule(JSONData jsonData)
    {
        var quizz = new Quizz();
        quizz.Questions = new List<Question>();
        Player pData;

        if (jsonData == null)
        {
            // Use the default generated data of the game
            pData = null;
            quizz = null;
        }
        else
        {
            pData = new Player()
            {
                HeadPath = RESOURCE_FOLDER + "/" + jsonData.name + "/" + "/Head/Head_Cosm2.png",
                BodyPath = RESOURCE_FOLDER + "/" + jsonData.name + "/" + "/Body/Body_Cosm2.png",
                FlagPath = RESOURCE_FOLDER + "/" + jsonData.name + "/" + "/Flag/Flag_Cosm2.png",
                Title = "Player Pro"
            };

            foreach (QuestionsList x in jsonData.questions)
            {
                string imgPath = x.image;
                if (!string.IsNullOrEmpty(imgPath))
                { // Use the full path from the persistant
                    imgPath = RESOURCE_FOLDER + "/" + jsonData.name + "/" + imgPath;
                }
                quizz.Questions.Add(new Question()
                {
                    Sentence = x.question,
                    Answers = new List<Answer>() {
                        new Answer() { Sentence = x.answers1, IsCorrect = x.correct == 1 },
                        new Answer() { Sentence = x.answers2, IsCorrect = x.correct == 2 },
                        new Answer() { Sentence = x.answers3, IsCorrect = x.correct == 3 },
                        new Answer() { Sentence = x.answers4, IsCorrect = x.correct == 4 }
                },
                    ImagePath = imgPath
                }) ;
            }
        }

        if (DataPersistent.Instance != null) //  Jonas tmp fix : to be able to start the game (with defaults) even if persistant wasn't load
        {
            DataPersistent.Instance.quizzForGame = quizz;
            DataPersistent.Instance.playerForGame = pData;
        }

        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// Create a button which can start the game with the specified data
    /// </summary>
    /// <param name="data">The data of the module</param>
    private void OnDataLoaded(JSONData data)
    {
        // Instantiate Prefab "BtnModule" and add it to "this.gameObject"
        GameObject btnModulePrefab = Instantiate(Resources.Load("Prefabs/BtnModule") as GameObject);

        string name = loadedCount + "_" + data.name; // TODO : TMP code to prove the dl + load modules works
        loadedCount++; // TMP code

        btnModulePrefab.GetComponentInChildren<TextMeshProUGUI>().text = name;
        btnModulePrefab.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnStartModule(data);
        });

        btnModulePrefab.transform.SetParent(this.gameObject.transform);
    }
    
    /// <summary>
    /// Load all the modules stored in the JSON folder
    /// </summary>
    private void LoadExistingModules()
    {
        string storeFolder = Utils.MergeWithPersistantPath("/JSON/");

        string[] files = Directory.GetFiles(storeFolder, "*.json");

        foreach (string file in files)
        {
            StartGetData(file);
        }
    }
}
