using UnityEngine;

namespace OctanGames.Extensions
{
    public static class CameraExtensions
    {
        public static Bounds OrthographicBounds(this Camera camera)
        {
            float screenAspect = (float)Screen.width / Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            var bounds = new Bounds(camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

            return bounds;
        }
    }
}