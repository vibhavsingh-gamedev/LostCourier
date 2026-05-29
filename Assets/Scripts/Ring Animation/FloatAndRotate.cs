using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 60f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmount = 0.2f;

    private Vector3 _startPos;

    private void Start() => _startPos = transform.localPosition;

    private void Update()
    {
        // Rotate around Y axis
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.World);

        // Float up and down using sine wave
        float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.localPosition = new Vector3(_startPos.x, newY, _startPos.z);
    }
}