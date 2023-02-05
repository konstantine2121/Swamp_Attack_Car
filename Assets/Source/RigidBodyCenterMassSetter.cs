using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyCenterMassSetter : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;

    private void Awake()
    {
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            SetOffset(rigidbody);
        }
    }

    private void SetOffset(Rigidbody rigidbody)
    {
        if (rigidbody)
        {
            rigidbody.centerOfMass = _offset;
        }
    }
}
