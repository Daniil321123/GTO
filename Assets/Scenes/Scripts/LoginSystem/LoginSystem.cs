using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginSystem : MonoBehaviour
{
    public enum CurrentWindow { Login, Register }
    public CurrentWindow currentWindow = CurrentWindow.Login;

    [SerializeField] private HelloWorldManager helloWorldManager;
    string loginEmail = "";
    string loginPassword = "";
    string registerEmail = "";
    string registerPassword1 = "";
    string registerPassword2 = "";
    string registerUsername = "";
    string errorMessage = "";

    bool isWorking = false;
    bool registrationCompleted = false;
    bool isLoggedIn = false;

    //Logged-in user data
    string userName = "";
    string userEmail = "";

    //string rootURL = "http://92.63.101.74/"; //Path where php files are located
    string rootURL = "http://192.168.0.168/"; //Path where php files are located

    private GUILayoutOption heightField = GUILayout.Height(85);
    private GUILayoutOption buttonSizeWidth = GUILayout.Width(125);
    private GUILayoutOption buttonSizeHeight = GUILayout.Height(85);

    public void Start()
    {
        helloWorldManager.transform.gameObject.SetActive(false);
    }

    void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.label.fontSize = 30;
        GUI.skin.button.fontSize = 30;
        GUI.skin.window.fontSize = 30;
        GUI.skin.textField.fontSize = 30;
        if (!isLoggedIn)
        {
            if (currentWindow == CurrentWindow.Login)
            {
                GUI.Window(0, new Rect(Screen.width / 2 - 450, Screen.height / 2 - 430, 900, 860), LoginWindow, "Login");
            }
            if (currentWindow == CurrentWindow.Register)
            {
                GUI.Window(0, new Rect(Screen.width / 2 - 450, Screen.height / 2 - 430, 900, 860), RegisterWindow, "Register");
            }
        }

        GUI.Label(new Rect(50, 50, 500, 80), "Status: " + (isLoggedIn ? "Logged-in Username: " + userName + " Email: " + userEmail : "Logged-out"));
        if (isLoggedIn)
        {
            if (GUI.Button(new Rect(50, 50, 125, 80), "Log Out"))
            {
                isLoggedIn = false;
                userName = "";
                userEmail = "";
                currentWindow = CurrentWindow.Login;
                helloWorldManager.StopClient();
                helloWorldManager.gameObject.SetActive(false);
            }
        }
    }

    void LoginWindow(int index)
    {
        if (isWorking)
        {
            GUI.enabled = false;
        }

        if (errorMessage != "")
        {
            GUI.color = Color.red;
            GUILayout.Label(errorMessage);
        }
        if (registrationCompleted)
        {
            GUI.color = Color.green;
            GUILayout.Label("Registration Completed!");
        }

        GUI.color = Color.white;
        GUI.skin.label.fontSize = 30;
        GUILayout.Label("Email:");
        loginEmail = GUILayout.TextField(loginEmail, heightField);
        GUILayout.Label("Password:", GUILayout.Height(50));
        loginPassword = GUILayout.PasswordField(loginPassword, '*', heightField);

        GUILayout.Space(5);

        if (GUILayout.Button("Submit", buttonSizeWidth, buttonSizeHeight))
        {
            StartCoroutine(LoginEnumerator());
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label("Do not have account?");
        if (GUILayout.Button("Register", GUILayout.Width(150), buttonSizeHeight))
        {
            ResetValues();
            currentWindow = CurrentWindow.Register;
        }
    }

    void RegisterWindow(int index)
    {
        if (isWorking)
        {
            GUI.enabled = false;
        }

        if (errorMessage != "")
        {
            GUI.color = Color.red;
            GUILayout.Label(errorMessage);
        }

        GUI.color = Color.white;
        GUILayout.Label("Email:");
        registerEmail = GUILayout.TextField(registerEmail, 254, heightField);
        GUILayout.Label("Username:");
        registerUsername = GUILayout.TextField(registerUsername, 15, heightField);
        GUILayout.Label("Password:");
        registerPassword1 = GUILayout.PasswordField(registerPassword1, '*', 19, heightField);
        GUILayout.Label("Password Again:");
        registerPassword2 = GUILayout.PasswordField(registerPassword2, '*', 19, heightField);

        GUILayout.Space(5);

        if (GUILayout.Button("Submit", buttonSizeWidth, buttonSizeHeight))
        {
            StartCoroutine(RegisterEnumerator());
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label("Already have an account?");
        if (GUILayout.Button("Login", buttonSizeWidth, buttonSizeHeight))
        {
            ResetValues();
            currentWindow = CurrentWindow.Login;
        }
    }

    IEnumerator RegisterEnumerator()
    {
        isWorking = true;
        registrationCompleted = false;
        errorMessage = "";

        WWWForm form = new WWWForm();
        form.AddField("email", registerEmail);
        form.AddField("username", registerUsername);
        form.AddField("password1", registerPassword1);
        form.AddField("password2", registerPassword2);

        if (registerPassword1 != registerPassword2)
        {
            errorMessage = "Пароли не совпадают";
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequest.Get(rootURL + "reg/" + registerUsername + "&" + registerPassword1 + "&" + registerEmail))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    errorMessage = "Не все поля заполнены";
                }
                else
                {
                    string responseText = www.downloadHandler.text;
                    if (responseText.StartsWith("res"))
                    {
                        string[] dataChunks = responseText.Split('|');
                        switch (dataChunks[1])
                        {
                            case "1":
                                currentWindow = CurrentWindow.Login;
                                break;
                            case "2":
                                errorMessage = "Длинный ник";
                                break;
                            case "3":
                                errorMessage = "Русские символы в нике";
                                break;
                            case "4":
                                errorMessage = "Неправильный формат емаила";
                                break;
                            case "5":
                                errorMessage = "Такой пользователь уже существует";
                                break;
                            case "6":
                                errorMessage = "Такой email существует";
                                break;
                        }
                    }
                }
            }
        }
        isWorking = false;
    }

    IEnumerator LoginEnumerator()
    {
        isWorking = true;
        registrationCompleted = false;
        errorMessage = "";

        using (UnityWebRequest www = UnityWebRequest.Get(rootURL + "auth/" + loginEmail + "&" + loginPassword))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                errorMessage = www.error;
            }
            else
            {
                string responseText = www.downloadHandler.text;
                if (responseText.StartsWith("res"))
                {
                    string[] dataChunks = responseText.Split('|');
                    if (dataChunks[1] == "2")
                    {
                        errorMessage = "Игрок уже онлайн";
                    }
                    else if (dataChunks[1] == "1")
                    {
                        if (dataChunks[2] == "currentCar")
                        {
                            PlayerInfo.currentCar = Int32.Parse(dataChunks[3]);
                        }
                        else if(dataChunks[4] == "money")
                        {
                            PlayerInfo.money = dataChunks[5];
                        }
                        PlayerInfo.nicName = loginEmail;
                        PlayerInfo.password = loginPassword;
                        isLoggedIn = true;
                        ResetValues();
                        helloWorldManager.transform.gameObject.SetActive(true);
                        helloWorldManager.StartClient();
                    }
                }
                else
                {
                    errorMessage = responseText;
                }
            }
        }
        isWorking = false;
    }

    void ResetValues()
    {
        errorMessage = "";
        loginEmail = "";
        loginPassword = "";
        registerEmail = "";
        registerPassword1 = "";
        registerPassword2 = "";
        registerUsername = "";
    }
}
