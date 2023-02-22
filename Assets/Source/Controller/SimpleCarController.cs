using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
/// 
[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    [SerializeField] private List<ColliderAxleInfo> axleInfos;

    [SerializeField] private Transform _lookAtPoint;
    [SerializeField] private Rigidbody _rigidbody;

    /// <summary>
    /// Клавиша тормоза.
    /// </summary>
    [SerializeField] private KeyCode _hitTheBreakKey;

    [SerializeField] private float _maxForwardSpeed;
    [SerializeField] private float _maxBackwardSpeed;

    /// <summary>
    /// Массимальная мощность мотора.
    /// </summary>
    [SerializeField] private float _maxMotorTorque;

    /// <summary>
    /// Максималный угол поворота.
    /// </summary>
    [SerializeField] private float _maxSteeringAngle;

    /// <summary>
    /// Сила торможения.
    /// </summary>
    [SerializeField] private float _breakTorque;

    /// <summary>
    /// Нажат тормоз
    /// </summary>
    private bool _hitTheBreak = false;

    public Vector3 CameraLookAtPoint
    {
        get
        {
            return _lookAtPoint != null ?
                _lookAtPoint.position :
                transform.position;
        }
    }

    /// <summary>
    /// Нажат тормоз
    /// </summary>
    public bool HitTheBreakPressed => _hitTheBreak;

    /// <summary>
    /// Скорость относительно локального вектора "вперед".
    /// </summary>
    private float ForwardSpeed
    {
        get
        {
            var localSpeed = transform.InverseTransformVector(_rigidbody.velocity);

            return localSpeed.z;
        }
    }

    /// <summary>
    /// Крутящий момент мотора.
    /// </summary>
    private float MotorTorque
    {
        get
        {
            var axleInfo = axleInfos.FirstOrDefault(info => info.motor);
            var torque = axleInfo == null ?
                0 :
                Mathf.Min(
                        axleInfo.leftWheel.motorTorque,
                        axleInfo.rightWheel.motorTorque
                        );

            return torque;
        }
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_hitTheBreakKey))
        {
            _hitTheBreak = true;
        }
        if (Input.GetKeyUp(_hitTheBreakKey))
        {
            _hitTheBreak = false;
        }
    }

    private void FixedUpdate()
    {
        PerformWheelCollidersLogic();
        Debug.Log(ForwardSpeed);
    }

    #region WheelCollidersLogic

    private void PerformWheelCollidersLogic()
    {
        float vertical = _maxMotorTorque * Input.GetAxis(InputAxis.Vertical);
        float steering = _maxSteeringAngle * Input.GetAxis(InputAxis.Horizontal);

        int middleIndex = (axleInfos.Count - 1) / 2;

        DecreaseSideSpeed();

        for (int i = 0; i < axleInfos.Count; i++)
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
                var forwardTorque = CalculateForwardTorque();

                axleInfo.leftWheel.motorTorque = forwardTorque;
                axleInfo.rightWheel.motorTorque = forwardTorque;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    private void DecreaseFrontSpeed()
    {
        const float maxDelta = 0.5f;
        const float frontSpeedCoefficient = 0.99f;

        var speed = _rigidbody.velocity;
        var localVector = transform.InverseTransformDirection(speed);

        localVector.z *= frontSpeedCoefficient;

        if (Mathf.Abs(localVector.z) < maxDelta)
        {
            localVector.z = 0;
        }

        _rigidbody.velocity = transform.TransformDirection(localVector);        
    }

    private void DecreaseSideSpeed()
    {
        const float maxSideDelta = 0.01f;
        const float sideSpeedCoefficient = 0.8f;

        var speed = _rigidbody.velocity;
        var localVector = transform.InverseTransformDirection(speed);

        if (Mathf.Abs(localVector.x) > maxSideDelta)
        {
            localVector.x *= sideSpeedCoefficient;

            _rigidbody.velocity = transform.TransformDirection(localVector);
        }
    }

    private float CalculateForwardTorque()
    {
        float verticaltInput = Input.GetAxis(InputAxis.Vertical);
        //float verticaltInput = 1;

        if (verticaltInput == 0)
        {
            return 0;
        }

        var tryMoveForward =  verticaltInput > 0;
        var tryMoveBackward = verticaltInput < 0;

        var forwardSpeed = ForwardSpeed;
        var isMaxSpeed = 
            tryMoveForward && Mathf.Abs(forwardSpeed)  >= _maxForwardSpeed ||
            tryMoveBackward && Mathf.Abs(forwardSpeed) >= _maxBackwardSpeed;

        var motorTorque = MotorTorque;

        var movingForward = forwardSpeed > 0;
        var movingBackward = forwardSpeed < 0;

        var hitTheBreak = HitTheBreakPressed  ||
            (movingBackward & tryMoveForward) ||
            (movingForward & tryMoveBackward);
                
        if (hitTheBreak)
        {
            DecreaseFrontSpeed();
            return 0;
        }

        if (tryMoveForward == tryMoveBackward && Mathf.Abs(forwardSpeed) <= 0.5f)
        {
            return 0;
        }

        if (isMaxSpeed)
        {
            return -motorTorque * 0.1f;
        }

        if (tryMoveForward & movingForward)
        {
            var torque = _maxMotorTorque * ( Mathf.Abs(_maxForwardSpeed - forwardSpeed) / _maxForwardSpeed);
            return torque;
        }

        if(tryMoveBackward & movingBackward)
        {
            var torque = _maxMotorTorque * (Mathf.Abs(_maxBackwardSpeed + forwardSpeed) / _maxBackwardSpeed);
            return -torque;
        }

        return -motorTorque * 0.5f;
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

    private void OnValidate()
    {
        if (_maxForwardSpeed == 0)
        {
            _maxForwardSpeed = 1;
        }

        if (_maxBackwardSpeed == 0)
        {
            _maxBackwardSpeed = 1;
        }
    }
}