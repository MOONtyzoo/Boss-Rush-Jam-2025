using UnityEngine;

/// <summary>
/// <para>Helps you to snap your aim to a valid target based on it's settings</para>
/// </summary>
[CreateAssetMenu(menuName="ScriptableObjects/AimAssisterSO", fileName ="AimAssisterSO")]
public class AimAssisterSO : ScriptableObject
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float searchConeRadius;
    [SerializeField] private float searchConeAngle;

    /// <summary>
    /// <para> Will search for valid targets in a cone, and return the one closest to aimPoint </para>
    /// </summary>
    public bool TryGetTargetPositionInDirection(Vector2 searchOrigin, Vector2 aimPoint, out Vector2 targetPosition) {
        Transform closestValidTarget = null;
        float closestValidTargetDistance = Mathf.Infinity;

        Vector2 searchDirection = (aimPoint - searchOrigin).normalized;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(searchOrigin, searchConeRadius, Vector2.up, 0f, targetLayerMask);
        foreach(RaycastHit2D hit in hits) {
            Transform target = hit.collider.gameObject.transform;
            Vector2 directionToTarget = ((Vector2)target.position - searchOrigin).normalized;
            bool isValidTarget = Vector2.Angle(searchDirection, directionToTarget) <= searchConeAngle/2;
            
            float distanceToAimPoint = Vector2.Distance((Vector2)target.position, aimPoint);
            if (isValidTarget && distanceToAimPoint < closestValidTargetDistance) {
                closestValidTarget = target;
                closestValidTargetDistance = distanceToAimPoint;
            }
        }

        if (closestValidTarget != null) {
            targetPosition = closestValidTarget.position;
            return true;
        } else {
            targetPosition = Vector2.zero;
            return false;
        }
    }
}
