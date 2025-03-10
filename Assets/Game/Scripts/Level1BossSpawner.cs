using UnityEngine;

public class Level1BossSpawner : MonoBehaviour
{
    [SerializeField] private TutorialObserver tutorialObserver;
    [SerializeField] private MadMole moleBoss;

    private void Start() {
        tutorialObserver.OnTutorialCompleted += TutorialObserver_OnTutorialCompleted;
    }

    private void TutorialObserver_OnTutorialCompleted() {
        moleBoss.gameObject.SetActive(true);
    }
}
