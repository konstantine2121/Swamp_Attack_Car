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

/// <summary>
/// https://docs.unity3d.com/Manual/WheelColliderTutorial.html
/// </summary>
public class SimpleCarController : MonoBehaviour
{
    [SerializeField] private List<ColliderAxleInfo> axleInfos;

    [SerializeField] private Transform _lookAtPoint;

    [SerializeField] private float maxMotorTorque;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float _maxDownPressureForce;

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
        int middleIndex = (axleInfos.Count-1) / 2;

        for (int i =0; i<  axleInfos.Count; i++)
        {
            var axleInfo = axleInfos[i];

            var invertRotation = i > middleIndex;

            if (axleInfo.steering)
            {
                var localSteering = invertRotation ? -steering : steering;

                axleInfo.leftWheel.steerAngle = localSteering;
                axleInfo.rightWheel.steerAngle = localSteering;
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

        visualWheel.position = position;
        visualWheel.rotation = rotation;
    }

    #endregion WheelCollidersLogic

    #region VisibleAxleInfoLogic



    #endregion VisibleAxleInfoLogic
}