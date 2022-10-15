using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public Vector3 TargetOffset = new Vector3(0f, 0.5f, 0f);
    public float Speed;
    public Vector3 locationOffset;
    public bool DirectMovement;
    public bool DirectRotation;

    void LateUpdate()
    {
        Vector3 desiredPosition = Target.position + SwordSoul.GameManager.Player.transform.rotation * locationOffset;
        if (DirectMovement)
            transform.position = desiredPosition;
        else
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Speed * Time.deltaTime);
            transform.position = smoothedPosition;
        }

        Quaternion targetRotation = Quaternion.LookRotation((Target.position + TargetOffset) - transform.position);
        if (DirectRotation)
            transform.rotation = targetRotation;
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Speed * Time.deltaTime);
    }
}