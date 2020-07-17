using UnityEngine;

public class ParticlesBySize : MonoBehaviour
{
    public Camera playerCamera;
    private ParticleSystem _particleSystem;
    private float _startSize;
    private float _startY;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _startSize = _particleSystem.shape.scale.x;
        _startY = _particleSystem.transform.position.y;
    }

    private void Update()
    {
        ParticleSystem.ShapeModule particleSystemShape = _particleSystem.shape;
        float orthographicSize = playerCamera.orthographicSize;
        particleSystemShape.scale = new Vector3(_startSize + orthographicSize, 1, 1);
        _particleSystem.transform.position = new Vector3(0, _startY+playerCamera.transform.position.y+orthographicSize, 11);
    }
}