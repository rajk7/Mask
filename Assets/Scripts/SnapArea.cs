using UnityEngine;

public class SnapArea : MonoBehaviour
{
    public float snapDistance = 50f;

    public Vector3 GetSnapPosition()
    {
        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, snapDistance);
    }
}
