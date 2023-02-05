using UnityEngine;

public class CameraCarFollow : MonoBehaviour
{
    [SerializeField]private GameObject _target;
    [SerializeField]private Vector3 _cameraPositionOffset;
    [SerializeField]private Vector3 _cameraLookPointOffset;

    private Transform TargetTransform => _target.gameObject.transform;

    private void Start()
    {
        
    }

    private void LateUpdate()
    {
        if (_target != null)
        {
            var rotatedVector = TargetTransform.rotation * _cameraPositionOffset;

            transform.position = TargetTransform.position + rotatedVector;
            transform.LookAt(TargetTransform.position + _cameraLookPointOffset);
        }
    }
}
