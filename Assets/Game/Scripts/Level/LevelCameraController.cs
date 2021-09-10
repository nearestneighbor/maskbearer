using UnityEngine;

public class LevelCameraController : MonoBehaviour
{
    private Camera _camera;
    private Player _player;
    private LevelBounds _bounds;

    private Vector2 _cameraPosition;
    private float _cameraDefaultHLerpSpeed = 5;
    private float _cameraDefaultVLerpSpeed = 8;

    private void Start()
    {
        // FIXME: Pass these value through method after level initialization
        Setup(Camera.main,
            FindObjectOfType<Player>(),
            FindObjectOfType<LevelBounds>()
        );
    }

    private void Setup(Camera camera, Player player, LevelBounds bounds)
    {
        _camera = camera;
        _player = player;
        _bounds = bounds;

        // Instant movement to target
        _cameraPosition = _player.transform.position + new Vector3(0, 1.5f, 0);
    }

    private void Update()
    {
        if (_camera == null)
            return;

        UpdateFollow();
        UpdateBoudns();
    }

    private void UpdateFollow()
    {
        if (_player == null)
            return;

        var targetPos = _player.transform.position;
        targetPos.y += 1.5f;

        // Lerp
        _cameraPosition.x = Mathf.Lerp(_cameraPosition.x, targetPos.x, _cameraDefaultHLerpSpeed * Time.deltaTime);
        _cameraPosition.y = Mathf.Lerp(_cameraPosition.y, targetPos.y, _cameraDefaultVLerpSpeed * Time.deltaTime);
    }

    private void UpdateBoudns()
    {
        if (_bounds == null)
            return;

        var bPos = (Vector2)_bounds.transform.position;
        var bHalfSize = (Vector2)_bounds.Size / 2f;
        var cHalfSize = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);

        var min = bPos - bHalfSize + cHalfSize;
        var max = bPos + bHalfSize - cHalfSize;

        _cameraPosition = new Vector2(
            bHalfSize.x < cHalfSize.x ? bPos.x : Mathf.Clamp(_cameraPosition.x, min.x, max.x),
            bHalfSize.y < cHalfSize.y ? bPos.y : Mathf.Clamp(_cameraPosition.y, min.y, max.y)
        );
    }

    private void LateUpdate()
    {
        if (_camera == null)
            return;

        _camera.transform.position = new Vector3(
            _cameraPosition.x,
            _cameraPosition.y,
            _camera.transform.position.z
        );
    }
}