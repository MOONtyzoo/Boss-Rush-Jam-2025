using UnityEngine;

public class PrototypePlayerVisual : MonoBehaviour
{
    [Tooltip("The script will rotate this transform to point at the mouse")]
    [SerializeField] private Transform aimableArm;
    [Tooltip("The position of the arm when player is facing left")]
    [SerializeField] private Transform armLeftPoint;
    [Tooltip("The position of the arm when player is facing right")]
    [SerializeField] private Transform armRightPoint;

    [Tooltip("Affects how quickly the rotates toward the cursor")]
    [SerializeField] private float armRotationSpeed;

    private Vector2 armAimDirection;

    private void Update() {
        AnimateAimableArm();
    }

    private void AnimateAimableArm() {
        aimableArm.transform.position = IsMouseRightOfPlayer() ? armRightPoint.transform.position : armLeftPoint.transform.position;

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 armPivotPosition = aimableArm.position;

        Vector2 targetAimDirection =  (mouseWorldPosition - armPivotPosition).normalized;
        armAimDirection = Vector3.Slerp(armAimDirection, targetAimDirection, armRotationSpeed * Time.deltaTime);
        
        aimableArm.transform.up = armAimDirection;
    }

    private bool IsMouseRightOfPlayer() {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorldPosition.x >= transform.position.x;
    }
}
