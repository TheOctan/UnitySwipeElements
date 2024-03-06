using UnityEngine;

namespace OctanGames.UI
{
    public sealed class SafeArea : MonoBehaviour
{
    [SerializeField] private float _safeAreaMargin = 40f;

    [Tooltip("Conform to screen safe area on X-axis (default true, disable to ignore)")]
    [SerializeField] private bool _conformX = true;
    [Tooltip("Conform to screen safe area on Y-axis (default true, disable to ignore)")]
    [SerializeField] private bool _conformY = true;

    [SerializeField] private bool _isSymmetrical = true;

    [Header("Debug only")]
    [SerializeField] private bool _logging;
    [SerializeField] private bool _updateAlways;

    private RectTransform _panel;

    private Rect _lastSafeArea;
    private Vector2Int _lastScreenSize;
    private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

    private void Awake()
    {
        _panel = GetComponent<RectTransform>();

        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    public void ForceUpdateSafeArea()
    {
        ApplySafeArea(Screen.safeArea);
    }

    public static Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    private void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (IsScreenChanged(safeArea))
        {
            UpdateLastScreenValues();
            ApplySafeArea(safeArea);
        }
    }

    private bool IsScreenChanged(Rect safeArea)
    {
        return safeArea != _lastSafeArea
               || Screen.width != _lastScreenSize.x
               || Screen.height != _lastScreenSize.y
               || Screen.orientation != _lastOrientation
               || _updateAlways;
    }

    private void UpdateLastScreenValues()
    {
        // Fix for having auto-rotate off and manually forcing a screen orientation.
        // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
        _lastScreenSize.x = Screen.width;
        _lastScreenSize.y = Screen.height;
        _lastOrientation = Screen.orientation;
        _lastSafeArea = GetSafeArea();
    }

    private void ApplySafeArea(Rect safeArea)
    {
        ApplyMargin(ref safeArea);
        CheckIgnoredAxes(ref safeArea);

        // Check for invalid screen startup state on some Samsung devices (see below)
        if (Screen.width > 0 && Screen.height > 0)
        {
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.size;

            NormalizeAnchor(ref anchorMin);
            NormalizeAnchor(ref anchorMax);

#if UNITY_ANDROID
            if (_isSymmetrical)
            {
                if (anchorMin.x > 0)
                {
                    anchorMax.x = 1 - anchorMin.x;
                }
                else if (anchorMax.x < 1)
                {
                    anchorMin.x = 1 - anchorMax.x;
                }
            }
#endif
            _panel.anchorMin = anchorMin;
            _panel.anchorMax = anchorMax;
        }

        Log(safeArea);
    }

    private static void NormalizeAnchor(ref Vector2 anchor)
    {
        anchor.x = Mathf.Clamp01(anchor.x / Screen.width);
        anchor.y = Mathf.Clamp01(anchor.y / Screen.height);
    }

    private void CheckIgnoredAxes(ref Rect safeArea)
    {
        if (IsIgnoredXAxis())
        {
            safeArea.x = 0;
            safeArea.width = Screen.width;
        }

        if (IsIgnoredYAxis())
        {
            safeArea.y = 0;
            safeArea.height = Screen.height;
        }
    }

    private bool IsIgnoredYAxis()
    {
        return !_conformY;
    }

    private bool IsIgnoredXAxis()
    {
        return !_conformX;
    }

    private void ApplyMargin(ref Rect safeArea)
    {
        safeArea.x -= _safeAreaMargin;
        safeArea.xMax += _safeAreaMargin;
        safeArea.y -= _safeAreaMargin;
        safeArea.yMax += _safeAreaMargin;
    }

    private void Log(Rect safeArea)
    {
        if (_logging)
        {
            Debug.Log($"New safe area applied to {name}: x={safeArea.x}, y={safeArea.y}, w={safeArea.width}, h={safeArea.height} on full extents w={Screen.width}, h={Screen.height}");
        }
    }
}
}