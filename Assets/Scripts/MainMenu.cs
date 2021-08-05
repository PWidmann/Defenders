using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Interactibles")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject backButton;
    
    [Header("Panels")]
    [SerializeField] private RectTransform menuPanelRectTransform;
    [SerializeField] private GameObject settingsMenuPanel;

    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown windowDropdown;

    private enum MenuState { MainMenu, SettingsMenu, Lobby}
    private MenuState menuState;
    //private float currentMenuWidth = 250;

    private void Start()
    {
        //menuPanelRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 250);
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
            case MenuState.SettingsMenu:
                SetSettingsMenuLayout();
                break;
            case MenuState.Lobby:
                SetPlayMenuLayout();
                break;
            default:
                break;
        }
    }

    private void SetPlayMenuLayout()
    {
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
    }

    private void SetSettingsMenuLayout()
    {
        //currentMenuWidth = Mathf.Lerp(currentMenuWidth, 350, Time.deltaTime * 10);
        //menuPanelRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentMenuWidth);
        //menuPanelRectTransform.sizeDelta = new Vector2(currentMenuWidth, 0);

        settingsMenuPanel.SetActive(true);
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
    }

    private void SetMainMenuLayout()
    {
        //currentMenuWidth = Mathf.Lerp(currentMenuWidth, 250, Time.deltaTime * 10);
        //menuPanelRectTransform.sizeDelta = new Vector2(currentMenuWidth, 0);

        playButton.SetActive(true);
        settingsButton.SetActive(true);
        quitButton.SetActive(true);
        backButton.SetActive(false);
        settingsMenuPanel.SetActive(false);
    }

    public void PlayButton()
    {
        menuState = MenuState.Lobby;
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
        string dropDownValue = resolutionDropdown.options[resolutionDropdown.value].text;
        string[] resolution = dropDownValue.Split('x');
        bool fullScreen = windowDropdown.value == 0 ? false : true;

        Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), fullScreen);
    }
}
