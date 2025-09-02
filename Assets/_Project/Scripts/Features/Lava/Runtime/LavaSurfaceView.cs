using UnityEngine;

public class LavaSurfaceView : MonoBehaviour, ILavaSurface
{
    private const string SURFACE_OBJECT_NAME = "LavaSurface";
    private const float MIN_THICKNESS = 0.001f;
    private const float UNIT_MESH_Y = 1f; 
    
    [SerializeField] private Transform _surfaceTransform;
    [SerializeField] private float _baseY = 0f;
    
    private float _baseLocalY;

    private void Awake()
    {
        if (_surfaceTransform == null)
            _surfaceTransform = transform;
        
        gameObject.name = SURFACE_OBJECT_NAME;
        _baseLocalY = _surfaceTransform.localPosition.y;
    }

    void ILavaSurface.ApplyHeight(float heightFromBaseY)
    {
        float thickness = Mathf.Max(heightFromBaseY, MIN_THICKNESS);
        
        Vector3 scale = _surfaceTransform.localScale;
        scale.y = thickness / UNIT_MESH_Y;
        _surfaceTransform.localScale = scale;
        
        Vector3 localPosition = _surfaceTransform.localPosition;
        localPosition.y = _baseLocalY + (thickness * 0.5f);
        _surfaceTransform.localPosition = localPosition;
    }
    
    public void SetBaseY(float baseY) => _baseY = baseY;
}