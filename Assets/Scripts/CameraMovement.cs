using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("SetInInspector")]
    public float moveSpeed;
    public float maxZoom;
    public float minZoom;
    public float zoomSpeed;
    public Camera thisCamera;

    [Header("SetDynamically")]
    private Transform _transform;
    public Vector3 mapCentre;
    private float moveSpeedChanger = 1;

    private void Awake ()
    {
        _transform = this.gameObject.transform;
    }
private void Update ()
    {
        if (GameManager.currentGameState == GameManager.GameState.Game || GameManager.currentGameState == GameManager.GameState.LevelRedactor)
        {
            ZoomTheCamera();
            if(Input.mousePosition.x >= Screen.width - 10 && _transform.position.x < mapCentre.x * 2)
            {
                _transform.position += moveSpeed * Time.deltaTime * Vector3.right * moveSpeedChanger;
            }
            if(Input.mousePosition.y >= Screen.height - 10 && _transform.position.y < mapCentre.y * 2)
            {
                _transform.position += moveSpeed * Time.deltaTime * Vector3.up * moveSpeedChanger;
            }
            if(Input.mousePosition.x <= 0 + 10 && _transform.position.x > 0)
            {
                _transform.position += moveSpeed * Time.deltaTime * Vector3.left * moveSpeedChanger;
            }
            if(Input.mousePosition.y <= 0 + 10 && _transform.position.y > 0)
            {
                _transform.position += moveSpeed * Time.deltaTime * Vector3.down * moveSpeedChanger;
            }
        }
    }

    public void ZoomTheCamera ()
    {
        if (thisCamera.orthographicSize <= maxZoom || thisCamera.orthographicSize >= minZoom)
        {
            if(Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                thisCamera.orthographicSize += zoomSpeed * Time.deltaTime * -Input.GetAxis("Mouse ScrollWheel") * 100;
                if (thisCamera.orthographicSize < minZoom)
                {
                    thisCamera.orthographicSize = minZoom;
                }
                if(thisCamera.orthographicSize > maxZoom)
                {
                    thisCamera.orthographicSize = maxZoom;
                }
            }
            moveSpeedChanger = 1 + ((thisCamera.orthographicSize - minZoom) / (maxZoom - minZoom));
        }
    }
}
