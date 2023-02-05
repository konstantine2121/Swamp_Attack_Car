using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class SpeedRegistrator : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    private float _cooficient = 3.6f;
    private TMP_Text _textContainer;

    private bool TargetExists => _rigidbody;
    private float Speed => _rigidbody.velocity.magnitude;

    private void Awake()
    {
        _textContainer = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        UpdateSpeedText();
    }

    private void UpdateSpeedText()
    {
        var speedValue = TargetExists ? (Speed * _cooficient).ToString("0.00") : "none";
        _textContainer.text = "Speed: " + speedValue;
    }
}
