#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Goblin.Gameplay.Logic.Common.Defines;


namespace Goblin.SkillPipelineEditor
{
    public static class Prefs
    {
        public static readonly string CONFIG_PATH =
            $"{Application.dataPath}/../ProjectSettings/Goblin.SkillPipelineEditor.txt";

        [Serializable]
        class SerializedData
        {
            public float SnapInterval = GAME_DEFINE.SP_DATA_FRAME;
            [NonSerialized]
            public int FrameRate = GAME_DEFINE.SP_DATA_FRAME;
            [NonSerialized]
            public float timeScale = 1f;
            public int AutoSaveSeconds;
            public string SavePath = "Assets/";
            public bool ScrollWheelZooms = true;

            public bool MagnetSnapping = true;
            public float TrackListLeftMargin = 180f;
        }

        private static SerializedData _data;

        private static SerializedData data
        {
            get
            {
                if (_data == null)
                {
                    if (File.Exists(CONFIG_PATH))
                    {
                        var json = File.ReadAllText(CONFIG_PATH);
                        _data = JsonUtility.FromJson<SerializedData>(json);
                    }

                    if (_data == null)
                    {
                        _data = new SerializedData();
                    }
                }

                return _data;
            }
        }

        public static bool scrollWheelZooms
        {
            get => data.ScrollWheelZooms;
            set
            {
                if (data.ScrollWheelZooms != value)
                {
                    data.ScrollWheelZooms = value;
                    Save();
                }
            }
        }


        public static int autoSaveSeconds
        {
            get => data.AutoSaveSeconds;
            set
            {
                if (data.AutoSaveSeconds != value)
                {
                    data.AutoSaveSeconds = value;
                    Save();
                }
            }
        }

        public static string savePath
        {
            get => data.SavePath;
            set
            {
                if (data.SavePath != value)
                {
                    data.SavePath = value;
                    Save();
                }
            }
        }


        public static bool magnetSnapping
        {
            get => data.MagnetSnapping;
            set
            {
                if (data.MagnetSnapping != value)
                {
                    data.MagnetSnapping = value;
                    Save();
                }
            }
        }

        public static float trackListLeftMargin
        {
            get => data.TrackListLeftMargin;
            set
            {
                if (Math.Abs(data.TrackListLeftMargin - value) > 0.001f)
                {
                    data.TrackListLeftMargin = value;
                    Save();
                }
            }
        }

        public static int frameRate
        {
            get => data.FrameRate;
        }

        public static float timeScale
        {
            get => data.timeScale;
            set => data.timeScale = value;
        }

        public static float snapInterval
        {
            get => Mathf.Max(data.SnapInterval, 0.001f);
            set
            {
                if (Math.Abs(data.SnapInterval - value) > 0.001f)
                {
                    data.SnapInterval = Mathf.Max(value, 0.001f);
                    Save();
                }
            }
        }

        static void Save()
        {
            System.IO.File.WriteAllText(CONFIG_PATH, JsonUtility.ToJson(data));
        }
    }
}

#endif