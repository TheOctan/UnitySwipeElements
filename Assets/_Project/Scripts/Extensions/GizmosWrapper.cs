using UnityEngine;

namespace Extensions
{
    public static class GizmosWrapper
    {
        public static void DrawGizmosRect(Vector3 leftUpCorner, Vector3 rightUpCorner, 
            Vector3 leftDownCorner, Vector3 rightDownCorner)
        {
            Gizmos.DrawLine(leftUpCorner, rightUpCorner);
            Gizmos.DrawLine(rightUpCorner, rightDownCorner);
            Gizmos.DrawLine(rightDownCorner, leftDownCorner);
            Gizmos.DrawLine(leftDownCorner, leftUpCorner);
        }
    }
}