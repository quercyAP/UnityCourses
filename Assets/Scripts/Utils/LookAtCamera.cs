using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera Cam;
    public bool LockX;
    public bool LockY;
    public bool LockZ;

    private void Start()
    {
        Cam = SwordSoul.GameManager.Camera;
    }

    private void Update()
    {
        Vector3 euler = transform.eulerAngles;
        transform.LookAt(Cam.transform);
        Vector3 rotation = transform.eulerAngles;
        if (LockX)
            rotation.x = euler.x;
        if (LockY)
            rotation.y = euler.y;
        if (LockZ)
            rotation.z = euler.z;
        transform.eulerAngles = rotation;
    }
}