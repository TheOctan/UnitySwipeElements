using UnityEngine;

namespace OctanGames.Extensions
{
    public static class CameraExtensions
    {
        public static Bounds OrthographicBounds(this Camera camera)
        {
            float screenAspect = (float)Screen.width / Screen.height;
            float cameraHeight = camera.orthographicSize * 2;

            Vector3 cameraCenter = camera.transform.position;
            var cameraSize = new Vector3(cameraHeight * screenAspect, cameraHeight, 0);

            var bounds = new Bounds(cameraCenter, cameraSize);

            return bounds;
        }
    }
}