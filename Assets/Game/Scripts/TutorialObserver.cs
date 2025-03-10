using System;
using UnityEngine;

public class TutorialObserver : MonoBehaviour
{
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private PlayerController playerController;

    public event Action OnTutorialCompleted;

    private string activeLesson;
    private bool lessonComplete = false;

    private int grappleCount = 0;
    private int shootCount = 0; 

    private bool aKeyPressed = false;
    private bool sKeyPressed = false;
    private bool dKeyPressed = false;

    private void Start()
    {
        activeLesson = tutorialUI.GetCurrentLesson();

        // Register grapple callback
        playerController.OnGrappleStarted += OnGrappleStarted;
    }

    private void Update()
    {
        if (lessonComplete || string.IsNullOrEmpty(activeLesson))
            return;

        switch (activeLesson)
        {
            case "Intro":
            case "PreSpin":
            case "RoomSpin":
                //these lessons are completed by pressing a button
                if (tutorialUI.OnContinueButtonClicked())
                {
                    CompleteCurrentLesson();
                }
                break;

            case "Movement":
                TrackMovementKeys();
                if (aKeyPressed && sKeyPressed && dKeyPressed)
                {
                    CompleteCurrentLesson();
                }
                break;

            case "Jump":
                if (inputReader.GetInputJumpDown())
                {
                    CompleteCurrentLesson();
                }
                break;

            case "Grapple":
                if (grappleCount >= 2)
                {
                    CompleteCurrentLesson();
                }
                break;

            case "WallJump":
                if (playerController.CanWallJump())
                {
                    CompleteCurrentLesson();
                }
                break;

            case "Shooting":
                if (inputReader.GetInputShootDown())
                {
                    shootCount++;  //increment shoot count when the player shoots
                }
                
                if (shootCount >= 4)  //if the player has shot 4 times
                {
                    CompleteCurrentLesson();
                }
                break;

            default:
                Debug.LogWarning($"Unhandled lesson: {activeLesson}");
                break;
        }
    }

    private void TrackMovementKeys()
    {
        if (Input.GetKeyDown(KeyCode.A)) aKeyPressed = true;
        if (Input.GetKeyDown(KeyCode.S)) sKeyPressed = true;
        if (Input.GetKeyDown(KeyCode.D)) dKeyPressed = true;
    }

    private void CompleteCurrentLesson()
    {
        tutorialUI.CompleteLesson(activeLesson);
        lessonComplete = true;

        if (tutorialUI.AreAllLessonCompleted()) {
            OnTutorialCompleted?.Invoke();
        }

        Invoke(nameof(PrepareNextLesson), 0.5f); //added a slight delay before the next lesson
    }

    private void PrepareNextLesson()
    {
        activeLesson = tutorialUI.GetCurrentLesson();
        lessonComplete = false;

        ResetLessonTrackingVariables();
    }

    private void ResetLessonTrackingVariables()
    {
        aKeyPressed = false;
        sKeyPressed = false;
        dKeyPressed = false;
        grappleCount = 0;
        shootCount = 0;  
    }

    private void OnGrappleStarted()
    {
        grappleCount++;
    }

    private void OnDestroy()
    {
        playerController.OnGrappleStarted -= OnGrappleStarted;
    }
}