using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("SetInInspector")]
    public float moveSpeed;

    [Header("SetDynamically")]
    private Transform _transform;
    public Vector3 mapCentre;

    private void Awake ()
    {
        _transform = this.gameObject.transform;
    }
private void Update ()
    {
        if (GameManager.currentGameState == GameManager.GameState.Game || GameManager.currentGameState == GameManager.GameState.LevelRedactor)
        {
            if(Input.mousePosition.x >= Screen.width - 10 && _transform.position.x < mapCentre.x * 2)
            {
                _transform.position += moveSpeed * Time.deltaTime * Vector3.right;
            }
            if(Input.mousePosition.y >= Screen.height - 10 && _transform.position.y < mapCentre.y * 2)
            {
                _transform.position += moveSpeed * Time.deltaTime * Vector3.up;
            }
            if(Input.mousePosition.x <= 0 + 10 && _transform.position.x > 0)
            {
                _transform.position += moveSpeed * Time.deltaTime * Vector3.left;
            }
            if(Input.mousePosition.y <= 0 + 10 && _transform.position.y > 0)
            {
                _transform.position += moveSpeed * Time.deltaTime * Vector3.down;
            }
        }
    }
}
