using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ColliderAxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

[System.Serializable]
public class VisibleAxleInfo
{
    public GameObject leftWheel;
    public GameObject rightWheel;
    public bool motor;
    public bool steering;
}

/// <summary>
/// https://docs.unity3d.com/Manual/WheelColliderTutorial.html
/// </summary>
public class SimpleCarController : MonoBehaviour
{
    [SerializeField] private List<ColliderAxleInfo> axleInfos;
    [SerializeField] private List<VisibleAxleInfo> visibleWheels;

    [SerializeField] private float maxMotorTorque;
    [SerializeField] private float maxSteeringAngle;

    [SerializeField] private float _maxDownPressureForce;

    [SerializeField] private Transform _lookAtPoint;

    public Vector3 CameraLookAtPoint
    {
        get
        {
            return _lookAtPoint != null ?
                _lookAtPoint.position :
                transform.position;
        }
    }

    private void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        PerformWheelCollidersLogic(motor, steering);
    }

    #region WheelCollidersLogic
    
    private void PerformWheelCollidersLogic(float motor, float steering)
    {
        foreach (ColliderAxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    /// <summary>
    /// finds the corresponding visual wheel
    /// correctly applies the transform
    /// </summary>
    /// <param name="collider"></param>
    private void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    #endregion WheelCollidersLogic

    #region VisibleAxleInfoLogic



    #endregion VisibleAxleInfoLogic
}