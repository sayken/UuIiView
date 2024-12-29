using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float rotationSpeed = 30f; // 回転速度

    void Start()
    {
        // Cubeの初期角度を設定
        transform.rotation = Quaternion.Euler(45, 45, 45);
    }

    void Update()
    {
        // 水平方向(Y軸)に回転させる
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}
