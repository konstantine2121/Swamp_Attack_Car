using UnityEngine;

public class CameraCarFollow : MonoBehaviour
{
    [SerializeField]private SimpleCarController _target;
    [SerializeField]private Vector3 _offset;

    private Transform TargetTransform => _target.gameObject.transform;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (_target != null)
        {
            var rotatedVector = TargetTransform.localRotation * _offset;

            transform.position = TargetTransform.position + rotatedVector;
            transform.LookAt(_target.CameraLookAtPoint);
        }
    }
}
