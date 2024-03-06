using OctanGames.Gameplay.Levels;
using UnityEditor;
using UnityEngine;

namespace OctanGames.Editor
{
    [CustomEditor(typeof(LevelConfig))]
    public class LevelEditor : UnityEditor.Editor
    {
        private LevelConfig _levelConfig;
        private bool _showBoard;

        public void OnEnable()
        {
            _levelConfig = (LevelConfig)target;
            if (_levelConfig.IsMapNull() && _levelConfig.Rows * _levelConfig.Columns != 0)
            {
                _levelConfig.CreateMap();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            _levelConfig.Rows = EditorGUILayout.IntField("Rows", _levelConfig.Rows);
            _levelConfig.Columns = EditorGUILayout.IntField("Columns", _levelConfig.Columns);
            if (EditorGUI.EndChangeCheck())
            {
                if (_levelConfig.Rows * _levelConfig.Columns != 0)
                {
                    _levelConfig.ResizeMap();
                }
            }

            CreateDoubleArray(_levelConfig.Rows, _levelConfig.Columns, ref _showBoard, "Level", false);
            EditorUtility.SetDirty(_levelConfig);
            base.OnInspectorGUI();
        }

        private void CreateDoubleArray(
            int rows,
            int columns, ref bool showBoard,
            string boardName = "Board",
            bool reverseArray = true)
        {
            showBoard = EditorGUILayout.Foldout(showBoard, boardName);
            if (!showBoard || rows * columns == 0)
            {
                return;
            }

            EditorGUI.indentLevel = 0;

            GUIStyle tableStyle = GetTableStyle();
            GUIStyle headerColumnStyle = GetHeaderColumnStyle();
            GUIStyle columnStyle = GetColumnStyle();
            GUIStyle rowStyle = GetRowStyle();
            GUIStyle rowHeaderStyle = GetRowHeaderStyle(columnStyle);
            GUIStyle columnHeaderStyle = GetColumnHeaderStyle();
            GUIStyle columnLabelStyle = GetColumnLabelStyle(rowHeaderStyle);
            GUIStyle cornerLabelStyle = GetCornerLabelStyle();
            GUIStyle rowLabelStyle = GetRowLabelStyle();

            EditorGUILayout.BeginHorizontal(tableStyle);
            for (int x = -1; x < (reverseArray ? rows : columns); x++)
            {
                EditorGUILayout.BeginVertical((x == -1) ? headerColumnStyle : columnStyle);
                for (int y = -1; y < (reverseArray ? columns : rows); y++)
                {
                    if (x == -1 && y == -1)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField(" ", cornerLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (x == -1 && y != -1)
                    {
                        EditorGUILayout.BeginVertical(columnHeaderStyle);
                        EditorGUILayout.LabelField(y.ToString(), rowLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (y == -1)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField(x.ToString(), columnLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }

                    Color originalBackgroundColor = GUI.backgroundColor;
                    if (x >= 0 && y >= 0)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                        int i, j;
                        if (reverseArray)
                        {
                            i = x;
                            j = y;
                        }
                        else
                        {
                            i = y;
                            j = x;
                        }

                        GUI.backgroundColor = _levelConfig._Colors[_levelConfig[i, j]];
                        if (GUILayout.Button(""))
                        {
                            _levelConfig[i, j]++;
                            if (_levelConfig[i, j] >= _levelConfig._Colors.Length)
                            {
                                _levelConfig[i, j] = 0;
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    GUI.backgroundColor = originalBackgroundColor;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private static GUIStyle GetTableStyle()
        {
            return new GUIStyle("box")
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin =
                {
                    left = 32
                }
            };
        }
        private static GUIStyle GetHeaderColumnStyle()
        {
            return new GUIStyle
            {
                fixedWidth = 35
            };
        }
        private static GUIStyle GetColumnStyle()
        {
            return new GUIStyle
            {
                fixedWidth = 25
            };
        }
        private static GUIStyle GetRowStyle()
        {
            return new GUIStyle
            {
                fixedHeight = 25
            };
        }
        private static GUIStyle GetRowHeaderStyle(GUIStyle columnStyle)
        {
            return new GUIStyle
            {
                fixedWidth = columnStyle.fixedWidth - 1
            };
        }
        private static GUIStyle GetColumnLabelStyle(GUIStyle rowHeaderStyle)
        {
            return new GUIStyle
            {
                fixedWidth = rowHeaderStyle.fixedWidth - 6,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
        }
        private static GUIStyle GetColumnHeaderStyle()
        {
            return new GUIStyle
            {
                fixedWidth = 30,
                fixedHeight = 25.5f
            };
        }
        private static GUIStyle GetCornerLabelStyle()
        {
            return new GUIStyle
            {
                fixedWidth = 42,
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.BoldAndItalic,
                fontSize = 14,
                padding =
                {
                    top = -5
                }
            };
        }
        private static GUIStyle GetRowLabelStyle()
        {
            return new GUIStyle
            {
                fixedWidth = 25,
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold
            };
        }
    }
}