using UnityEngine;
using UnityEngine.UI;
using BRJ.SceneManagement;

public class UserInterfaceManager : MonoBehaviour
{
    [SerializeField] private Scenes startButtonSceneDestination;
    [SerializeField] private Scenes mainMenuScene; // Add a serialized field for the Main Menu scene

    public Button speedRunButton;
    public Button hardCoreButton;

    private bool isSpeedRunSelected = false;
    private bool isHardCoreSelected = false;
    private bool ifButtonTriggered = false;

    private void Start()
    {
        // Add listeners to ensure that the correct toggle methods are connected
        speedRunButton?.onClick.AddListener(ToggleSpeedRun);
        hardCoreButton?.onClick.AddListener(ToggleHardCore);
    }

    private void ToggleSpeedRun()
    {
        isSpeedRunSelected = !isSpeedRunSelected;
    }

    private void ToggleHardCore()
    {
        isHardCoreSelected = !isHardCoreSelected;
    }

    public void OnStartButtonClick()
    {
        if (isSpeedRunSelected)
        {
            // Enable SpeedRun timer
            if (SpeedRunTimer.Instance != null)
            {
                SpeedRunTimer.Instance.StartTimer();
            }
        }
        
        if (isHardCoreSelected)
        {
            // Do any hardcore stuff here if needed
        }

        BRJSceneManager.LoadSceneAsync(startButtonSceneDestination, isSpeedRunSelected, isHardCoreSelected);
    }

    public void OnMainMenuButtonClick()
    {
        BRJSceneManager.LoadSceneAsync(mainMenuScene);
    }
}

/*
 * Casual mode
 * - no modifiers
 * - loads game scene
 * Speed Run mode
 * - calls speed run script which will show the timer (observer or singleton?)
 * - then loads game scene
 * Hardcore mode
 * - Causes player death to restart to level one
 * Level selector
 * 
 */
