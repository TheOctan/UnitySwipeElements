using System;
using UnityEngine;
using UnityEngine.UI;

namespace OctanGames.UI.Background
{
    public class BackgroundHorizon : MonoBehaviour
    {
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _horizon;

        public Vector3 GetHorizonPosition()
        {
            float horizonY = _horizon.rect.height + _background.offsetMin.y;
            return new Vector3(0, horizonY, 0);
        }
    }
}