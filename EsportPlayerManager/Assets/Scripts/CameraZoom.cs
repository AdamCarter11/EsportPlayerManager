using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraZoom : MonoBehaviour
{
    public Transform target; // The point to zoom in on
    public float zoomSpeed = 5.0f; // The speed of the zoom
    public float zoomedOrthographicSize = 2.0f; // The desired orthographic size when zoomed in
    public float defaultOrthographicSize = 5.0f; // The default orthographic size of the camera

    private bool isZoomed = false; // Flag to indicate if currently zoomed in
    private Vector3 originalPosition; // The original position of the camera

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleZoom();
        }
    }

    public void ToggleZoom()
    {
        if (isZoomed)
        {
            ZoomOut();
        }
        else
        {
            ZoomIn();
        }
    }

    private void ZoomIn()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned for zooming.");
            return;
        }

        // Calculate the new orthographic size when zoomed in
        float targetOrthographicSize = zoomedOrthographicSize;

        // Smoothly adjust the camera's orthographic size
        StartCoroutine(AdjustOrthographicSize(targetOrthographicSize));

        isZoomed = true;
    }

    private void ZoomOut()
    {
        // Reset the camera's orthographic size to default
        StartCoroutine(AdjustOrthographicSize(defaultOrthographicSize));

        isZoomed = false;
    }

    private System.Collections.IEnumerator AdjustOrthographicSize(float targetOrthographicSize)
    {
        float currentOrthographicSize = Camera.main.orthographicSize;
        float elapsedTime = 0;

        while (elapsedTime < zoomSpeed)
        {
            // Adjust the camera's orthographic size using Lerp
            float newOrthographicSize = Mathf.Lerp(currentOrthographicSize, targetOrthographicSize, elapsedTime / zoomSpeed);
            Camera.main.orthographicSize = newOrthographicSize;

            // Calculate the new position based on the target position and orthographic size
            Vector3 newPosition = CalculateNewPosition(target.position, newOrthographicSize);
            transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera's orthographic size is set to the target value
        Camera.main.orthographicSize = targetOrthographicSize;
        SceneManager.LoadScene("CopyCombat");
        // Reset the camera's position to the original position
        //transform.position = originalPosition;
    }

    private Vector3 CalculateNewPosition(Vector3 targetPosition, float orthographicSize)
    {
        // Calculate the new camera position based on the target position and orthographic size
        Vector3 newPosition = targetPosition;
        newPosition.z = originalPosition.z; // Maintain the original z position
        newPosition += new Vector3(0, 0, -10); // Adjust the z position if needed
        return newPosition;
    }
}
