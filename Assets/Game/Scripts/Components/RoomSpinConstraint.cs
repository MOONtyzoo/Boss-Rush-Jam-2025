using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(ParentConstraint))]
public class RoomSpinConstraint : MonoBehaviour
{
    [Tooltip("If true, roomspins will not change the orientation of the transform")]
    [SerializeField] private bool keepOrientation = false;
    private ParentConstraint parentConstraint;
    
    private void Awake() {
        parentConstraint = GetComponent<ParentConstraint>();

        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = RoomSpinner.Instance.gameObject.transform;
        source.weight = 1;
        parentConstraint.AddSource(source);
        parentConstraint.constraintActive = false;

        SetKeepOrientation(keepOrientation);
    }

    private void OnEnable() {
        RoomSpinner.Instance.OnRoomSpinStarted += RoomSpinner_OnRoomSpinStarted;
        RoomSpinner.Instance.OnRoomSpinEnded += RoomSpinner_OnRoomSpinEnded;
    }

    private void OnDisable() {
        RoomSpinner.Instance.OnRoomSpinStarted -= RoomSpinner_OnRoomSpinStarted;
        RoomSpinner.Instance.OnRoomSpinEnded -= RoomSpinner_OnRoomSpinEnded;
    }

    private void RoomSpinner_OnRoomSpinStarted() {
        Transform roomTransform = RoomSpinner.Instance.gameObject.transform;
        Vector3 offsetLocalToRoom = roomTransform.InverseTransformPoint(transform.position);
        float rotationLocalToRoom = transform.eulerAngles.z - roomTransform.eulerAngles.z;
        parentConstraint.SetTranslationOffset(0, offsetLocalToRoom);
        parentConstraint.SetRotationOffset(0, new Vector3(0, 0, rotationLocalToRoom));
        parentConstraint.constraintActive = true;
    }

    private void RoomSpinner_OnRoomSpinEnded() {
        parentConstraint.constraintActive = false;
    }

    public void SetKeepOrientation(bool newKeepOrientation) {
        keepOrientation = newKeepOrientation;
        if (parentConstraint != null) {
            parentConstraint.rotationAxis = keepOrientation ? Axis.X | Axis.Y : Axis.X | Axis.Y |  Axis.Z ;
        }
    }
}
