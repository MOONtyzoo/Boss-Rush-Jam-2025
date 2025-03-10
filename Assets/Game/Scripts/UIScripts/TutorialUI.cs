using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [System.Serializable]
    public class Lesson
    {
        public string name;
        public GameObject lessonScreen;
        public bool isCompleted;
    }

    [SerializeField] private List<Lesson> lessons;

    private int currentLessonIndex = 0;
    private bool isButtonClicked = false;

    private void Start()
    {
        DisplayCurrentLesson();
    }

    public string GetCurrentLesson()
    {
        return currentLessonIndex < lessons.Count ? lessons[currentLessonIndex].name : null;
    }

    public void CompleteLesson(string lessonName)
    {
        Lesson lesson = lessons.Find(l => l.name == lessonName);
        if (lesson != null && !lesson.isCompleted)
        {
            lesson.isCompleted = true;

            if (lesson.lessonScreen != null)
                lesson.lessonScreen.SetActive(false);


            currentLessonIndex++;
            if (currentLessonIndex < lessons.Count)
            {
                DisplayCurrentLesson();
            }
        }

        isButtonClicked = false;
    }

    private void DisplayCurrentLesson()
    {
        if (currentLessonIndex < lessons.Count)
        {
            Lesson currentLesson = lessons[currentLessonIndex];
            if (currentLesson.lessonScreen != null)
                currentLesson.lessonScreen.SetActive(true);

        }
    }

    public bool OnContinueButtonClicked()
    {
        bool wasClicked = isButtonClicked;
        isButtonClicked = false; 
        return wasClicked;
    }

    public bool AreAllLessonCompleted() {
        bool allCompleted = true;
        foreach (Lesson lesson in lessons) {
            allCompleted = allCompleted && lesson.isCompleted;
        }
        return allCompleted;
    }

    public void OnButtonClick()
    {
        isButtonClicked = true;
    }
}