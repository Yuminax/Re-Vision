using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

public class Menu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI btnUser;
    [SerializeField] private GameObject btnUserObj;

    // Start is called before the first frame update
    void Start()
    {
        if (DataPersistent.Instance != null)
        {
            btnUser.text = DataPersistent.Instance.user;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadProfile()
    {
        SceneManager.LoadScene("Profile");
    }
    public void LoadTraining()
    {
        SceneManager.LoadScene("Training");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
