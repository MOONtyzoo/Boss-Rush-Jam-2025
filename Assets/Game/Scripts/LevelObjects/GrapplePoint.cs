using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    private void Start() {
        SetRotation(Random.Range(0f, 360f));
    }

    private void Update() {
        float newRotation = transform.rotation.eulerAngles.z + 45f * Time.deltaTime;
        transform.localScale = (1f + 0.1f * Mathf.Sin(2f*Mathf.PI*Time.deltaTime * 0.5f)) * Vector3.one;
        SetRotation(newRotation);
    }

    private void SetRotation(float rotationAngle) {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle));
    }
}
