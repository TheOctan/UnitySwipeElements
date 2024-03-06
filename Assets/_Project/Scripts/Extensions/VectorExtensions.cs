using UnityEngine;

namespace OctanGames.Extensions
{
    public struct CornerTuple
    {
        public CornerTuple(Vector3 leftUp, Vector3 rightUp, Vector3 leftDown, Vector3 rightDown)
        {
            LeftUpCorner = leftUp;
            RightUpCorner = rightUp;
            LeftDownCorner = leftDown;
            RightDownCorner = rightDown;
        }

        public Vector3 LeftUpCorner;
        public Vector3 RightUpCorner;
        public Vector3 LeftDownCorner;
        public Vector3 RightDownCorner;
    }
    public static class VectorExtensions
    {
        public static CornerTuple GetCornersFromCenter(this Vector3 center, float height, float width)
        {
            return center.GetCornersFromSides(height / 2f, height / 2f, width / 2f, width / 2f);
        }

        public static CornerTuple GetCornersFromLeftUpCorner(this Vector3 leftUp, float height, float width)
        {
            Vector3 rightUp = leftUp;
            rightUp.x += width;

            Vector3 leftDown = leftUp;
            leftDown.y -= height;
            
            Vector3 rightDown = leftUp;
            rightDown.x += width;
            rightDown.y -= height;

            return new CornerTuple(leftUp, rightUp, leftDown, rightDown);
        }

        public static CornerTuple GetAllCornersFrom2Corners(this Vector3 leftUp, Vector3 rightDown)
        {
            var rightUp = new Vector3(rightDown.x, leftUp.y);
            var leftDown = new Vector3(leftUp.x, rightDown.y);

            return new CornerTuple(leftUp, rightUp, leftDown, rightDown);
        }

        public static CornerTuple GetCornersFromSides(this Vector3 center, float up, float down, float left, float right)
        {
            Vector3 leftUp = center;
            leftUp.x -= left;
            leftUp.y += up;
            
            Vector3 rightUp = center;
            rightUp.x += right;
            rightUp.y += up;
            
            Vector3 leftDown = center;
            leftDown.x -= left;
            leftDown.y -= down;
            
            Vector3 rightDown = center;
            rightDown.x += right;
            rightDown.y -= down;
            
            return new CornerTuple(leftUp, rightUp, leftDown, rightDown);
        }

        public static Vector3 GetCenter(this Vector3 leftUpCorner, Vector2 size)
        {
            Vector3 center = leftUpCorner;
            center.y -= size.y / 2f;
            center.x += size.x / 2f;
            return center;
        }
    }
}