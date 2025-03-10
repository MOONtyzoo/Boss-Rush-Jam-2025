using UnityEngine;

public class MousePositionTracker : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private void Update() {
        transform.position = playerController.GetAimPosition();
    }
}
