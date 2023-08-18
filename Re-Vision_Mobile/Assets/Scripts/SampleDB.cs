using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using ReVision;
using UnityEditor;


public class SampleDB : MonoBehaviour
{
    // Name of the DB
    private string dbName = "";
    private string dbPathName = "";
    private string ModulesTable = "";


    [SerializeField] private TMP_InputField pseudoCreate;
    [SerializeField] private TMP_InputField passwordCreate;
    [SerializeField] private TMP_InputField pseudoLogin;
    [SerializeField] private TMP_InputField passwordLogin;
    [SerializeField] private TMP_InputField passwordCheck;
    private ModuleSelection jsonController;

    string path;


    // Start is called before the first frame update
    void Start()
    {
        dbPathName = Utils.MergeWithPersistantPath("/Test.db");
        dbName = "URI=file:" + dbPathName;
        //run the method to create the table
        CreateDB();
        //CreateTableModule();

        jsonController = gameObject.GetComponent<ModuleSelection>();

    }

    //No control on the SQL query, because they are in text
    public void CreateDB()
    {
        //Create de db connection
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //set up an object called "command" to allow db control
            using (var command = connection.CreateCommand())
            {

                command.CommandText = "CREATE TABLE IF NOT EXISTS users (id INTEGER primary key AUTOINCREMENT, pseudo VARCHAR(20), password VARCHAR(50), UNIQUE(pseudo));";
                command.ExecuteNonQuery();

            }

            connection.Close();
        }
    }

    public void AddUser()
    {
        string register = pseudoCreate.text;
        string passwordClear = passwordCreate.text;
        string ConfirmPassword = passwordCheck.text;

        if (register != string.Empty && passwordClear != string.Empty && ConfirmPassword != string.Empty)
        {
            if (ConfirmPassword == passwordClear)
            {
                //Create de db connection
                using (var connection = new SqliteConnection(dbName))
                {
                    connection.Open();
                    //set up an object called "command" to allow db control
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO users (pseudo, password) VALUES ('" + register + "', '" + passwordClear + "');";

                        command.ExecuteNonQuery(); // runs the SQL command
                    }

                    connection.Close();
                }
                DisplayUsers();
            }
            else
            {
                Debug.LogWarning("Wrong password check");
            }

        }
        else
        {
            Debug.LogError("Empty filed");
        }

    }
    public void DisplayUsers()
    {

        //display.text = "";
        List<string> listDisplay = new List<string>();
        //Create de db connection
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //set up an object called "command" to allow db control
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM users;";

                using (IDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        Debug.Log("ID : " + reader["id"] + " Pseudo : " + reader["pseudo"] + "  Password : " + reader["password"]);

                    }
                    reader.Close();
                }

            }

            connection.Close();
        }
    }

    public void Connection()
    {
        string register = pseudoLogin.text;
        string passwordClear = passwordLogin.text;

        //Create de db connection
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //set up an object called "command" to allow db control
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM users WHERE pseudo=\"" + register + "\" LIMIT 1;";

                using (IDataReader reader = command.ExecuteReader())
                {
                    //Check if the user exist
                    if (!reader.IsDBNull(0))
                    {
                        if (reader["password"].ToString() == passwordClear)
                        {
                            Debug.Log("bon mdp");
                            DataPersistent.Instance.user = register;

                            //if user exist and password correct
                            SceneManager.LoadScene("MainMenu");
                        }
                        else
                        {
                            Debug.LogError("mauvais mdp");
                        }

                    }
                    else
                    {
                        Debug.LogError("Pas d'utilisateur avec ce pseudo");
                    }

                    reader.Close();

                }
            }

            connection.Close();
        }
    }

    public void CreateTableModule()
    {

        ////Create de db connection
        //using (var connection = new SqliteConnection(ModulesTable))
        //{
        //    connection.Open();

        //    //set up an object called "command" to allow db control
        //    using (var command = connection.CreateCommand())
        //    {

        //        command.CommandText = "CREATE TABLE IF NOT EXISTS tableModules (id INTEGER primary key, theme VARCHAR(20), name VARCHAR(50));";
        //        command.ExecuteNonQuery();
        //    }

        //    connection.Close();
        //}

        string jsonDDL = "https://drive.google.com/uc?export=download&id=1B2asa8WPXa0LT493rUFvOXnYb8os5fZM"; // TODO for now it's hard coded, should be changed by the "available data on the download site"
        string storeFolder = "/JSON/";
        string dateName = System.DateTime.Now.ToString("yyyyMMdd-HHmmss"); // Store the file with the current date and time
        string relativePath = storeFolder + dateName + ".json";
        StartCoroutine(Utils.DownloadAndSaveData(jsonDDL, relativePath,
            () =>
            {
                Debug.Log("JSON Downloaded");
                jsonController.StartGetData(Utils.MergeWithPersistantPath(relativePath));
            },
            () => { Debug.Log("JSON Download Failed"); }));
    }
}
