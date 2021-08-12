using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject hostGameButton;
    [SerializeField] private GameObject joinGameButton;
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject backButton;

    [Header("Panels")]
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Text lobbyTitleText;
    [SerializeField] private GameObject currentPlayerPanel;
    [SerializeField] private GameObject playerStatusPrefab;

    [Header("Settings Panel")]
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown windowDropdown;

    [Header("Input Fields")]
    [SerializeField] private InputField hostServerPort;
    [SerializeField] private InputField hostName;
    [SerializeField] private InputField clientServerIP;
    [SerializeField] private InputField clientServerPort;
    [SerializeField] private InputField clientName;
    [SerializeField] private InputField hostPlayerName;

    bool serverStarted = false;
    bool playerRepresentation = false;
    private int connectionType = 0; // 1 = server, 2 = client

    private enum MenuState { MainMenu, HostMenu, JoinMenu, SettingsMenu, LobbyMenu}
    private MenuState menuState;

    private void Start()
    {
        menuState = MenuState.MainMenu;
        
        // Set screen settings at the start
        Screen.SetResolution(1600, 900, false);
    }

    private void Update()
    {
        switch (menuState)
        {
            case MenuState.MainMenu:
                SetMainMenuLayout();
                break;
            case MenuState.HostMenu:
                SetHostMenuLayout();
                break;
            case MenuState.JoinMenu:
                SetJoinMenuLayout();
                break;
            case MenuState.SettingsMenu:
                SetSettingsMenuLayout();
                break;
            case MenuState.LobbyMenu:
                SetLobbyMenuLayout();
                break;
            default:
                break;
        }

        // Close server if you back out of game lobby
        if (connectionType == 1 && menuState == MenuState.MainMenu)
        {
            GameObject server = GameObject.Find("Server");
            server.GetComponent<Server>().CloseServer();
            Destroy(server);
            serverStarted = false;

            Debug.Log("Server chickened out...");
        }

        // Delte client from connected clients on server if back out of game lobby
        if (connectionType == 2 && menuState == MenuState.MainMenu)
        {
            GameObject client = GameObject.Find("Client");
            client.GetComponent<Client>().DisconnectClient();
            Destroy(client);
            serverStarted = false;
        }
    }

    #region Buttons
    public void HostGameButton()
    {
        menuState = MenuState.HostMenu;
    }

    public void JoinGameButton()
    {
        menuState = MenuState.JoinMenu;
    }

    public void ClassGunnerButton()
    {
        GameObject client = GameObject.Find("Client");
        client.GetComponent<Client>().SetPlayerClass(PlayerClass.Gunner);
    }

    public void ClassKnightButton()
    {
        GameObject client = GameObject.Find("Client");
        client.GetComponent<Client>().SetPlayerClass(PlayerClass.Knight);
    }

    public void LobbyReadyButton()
    {
        GameObject client = GameObject.Find("Client");
        client.GetComponent<Client>().SetPlayerReady();
    }

    public void HostServerButton()
    {
        // Create Server
        GameObject serverObject = new GameObject("Server");
        int serverPort = 0;

        if (hostServerPort.text == "")
        {
            serverPort = Int32.Parse(hostServerPort.placeholder.GetComponent<Text>().text);
        }
        else
        {
            serverPort = Int32.Parse(hostServerPort.text);
        }
        serverObject.AddComponent<Server>().StartServer(serverPort);

        hostPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        serverStarted = true;
        connectionType = 1; // connected as server
        lobbyTitleText.text = "Lobby (Server)";

        //Create Client
        GameObject clientObject = new GameObject("Client");
        string hostName = "";
        // User name
        if (clientName.text == "")
            hostName = hostPlayerName.placeholder.GetComponent<Text>().text;
        else
            hostName = hostPlayerName.text;
        clientObject.AddComponent<Client>().StartClient("127.0.0.1", serverPort, hostName);
    }

    public void JoinServerButton()
    {
        // Create Client
        GameObject clientObject = new GameObject("Client");
        int serverPort = 0;
        string serverIP = "";
        string userName = "";

        // Server IP
        if (clientServerIP.text == "")
            serverIP = clientServerIP.placeholder.GetComponent<Text>().text;
        else
            serverIP = clientServerIP.text;

        // Server port
        if (clientServerPort.text == "")
            serverPort = Int32.Parse(clientServerPort.placeholder.GetComponent<Text>().text);
        else
            serverPort = Int32.Parse(clientServerPort.text);

        // User name
        if (clientName.text == "")
            userName = clientName.placeholder.GetComponent<Text>().text;
        else
            userName = clientName.text;

        clientObject.AddComponent<Client>().StartClient(serverIP, serverPort, userName);
        

        hostPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        connectionType = 2; // connected as client
        lobbyTitleText.text = "Lobby (Client)";
    }

    public void SettingsButton()
    {
        menuState = MenuState.SettingsMenu;
    }

    public void ResetMainMenu()
    {
        menuState = MenuState.MainMenu;
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ApplyButton()
    {
        // Screen settings
        string dropDownValue = resolutionDropdown.options[resolutionDropdown.value].text;
        string[] resolution = dropDownValue.Split('x');
        bool fullScreen = windowDropdown.value == 0 ? false : true;

        Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), fullScreen);
    }
    #endregion

    #region LayoutFunctions
    private void SetLobbyMenuLayout()
    {
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);

        lobbyPanel.SetActive(true);
    }

    private void SetSettingsMenuLayout()
    {
        settingsPanel.SetActive(true);
        hostButton.SetActive(false);
        joinButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
    }

    private void SetMainMenuLayout()
    {
        hostButton.SetActive(true);
        joinButton.SetActive(true);
        settingsButton.SetActive(true);
        quitButton.SetActive(true);
        backButton.SetActive(false);

        settingsPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);
    }

    private void SetHostMenuLayout()
    {
        hostPanel.SetActive(true);

        hostGameButton.SetActive(false);
        joinGameButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
    }

    private void SetJoinMenuLayout()
    {
        joinPanel.SetActive(true);

        hostGameButton.SetActive(false);
        joinGameButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
    }
    #endregion
}
