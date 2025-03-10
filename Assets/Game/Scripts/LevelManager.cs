using System.Collections;
using BRJ.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private MadMole boss;
    [SerializeField] private GameObject winGamePopup;
    [SerializeField] private GameObject loseGamePopup;

    private Music previousMusicTrack;

    void Start()
    {
        player.OnDied += Player_OnDied;
        boss.OnSpawnStarted += Boss_OnSpawned;
        boss.OnDied += Boss_OnDied;
        boss.OnSuperJumpSequenceStarted += Boss_OnSuperJumpSequenceStarted;
        RoomSpinner.Instance.OnRoomSpinEnded += RoomSpinner_OnRoomSpinEnded;

        AudioManager.SwapMusicTrack(Music.Level1_A, 0.5f);
    }

    private void Player_OnDied() {
        loseGamePopup.gameObject.SetActive(true);
        StartCoroutine(ReturnToMainMenuCoroutine());
    }

    private void Boss_OnSpawned() {
        AudioManager.SwapMusicTrack(Music.Level1_C, 0.5f);
    }

    private void Boss_OnDied() {
        winGamePopup.gameObject.SetActive(true);
        StartCoroutine(ReturnToMainMenuCoroutine());
    }

    private void Boss_OnSuperJumpSequenceStarted() {
        previousMusicTrack = AudioManager.GetCurrentMusicTrack();
        AudioManager.SwapMusicTrack(Music.Level1_Spin, 0.5f);
    }

    private void RoomSpinner_OnRoomSpinEnded() {
        //AudioManager.SwapMusicTrack(previousMusicTrack, 0.5f);
    }

    private IEnumerator ReturnToMainMenuCoroutine() {
        float timer = 0;
        while (timer < 3) {
            timer += Time.deltaTime;
            yield return null;
        }

        BRJSceneManager.LoadSceneAsync(Scenes.MainMenuFinalOutline);
    }
}
