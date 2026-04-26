using UnityEngine;

namespace CupkekGames.Luna.Ink.Demo
{
    /// <summary>
    /// Simple script that makes the GameObject face a target camera.
    /// </summary>
    public class FaceCamera : MonoBehaviour
    {
        [SerializeField] private Camera _targetCamera;

        private void Awake()
        {
            // If no camera assigned, try to find main camera
            if (_targetCamera == null)
            {
                _targetCamera = Camera.main;
            }
        }

        private void FixedUpdate()
        {
            if (_targetCamera == null) return;

            // Face the camera
            transform.rotation = _targetCamera.transform.rotation;
        }
    }
}
