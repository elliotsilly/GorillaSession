using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;

namespace GorillaSession
{
    [BepInPlugin("com.elliot.gorillasession", "GorillaSession", "1.0.1")]
    public class Main : BaseUnityPlugin
    {
        private float sessionStartTime;
        private string savePath;
        private GameObject textObject;
        private TextMeshPro text;
        private PlaytimeManager manager;

        private ConfigEntry<string> textColorHexConfig;

        void Awake()
        {
            textColorHexConfig = Config.Bind("Display", "Text Color", "#FFFFFFFF", "Color of GorillaSession text (e.g. #FFFFFFFF)");

            string dataFolder = Path.Combine(BepInEx.Paths.BepInExRootPath, "GorillaSessionData");
            Directory.CreateDirectory(dataFolder);

            savePath = Path.Combine(dataFolder, "playtime_data.json");

            manager = new PlaytimeManager();
            manager.LoadPlaytime(savePath);

            sessionStartTime = Time.time;

            StartCoroutine(SetupText());
        }

        IEnumerator SetupText()
        {
            yield return new WaitUntil(() => GorillaTagger.Instance?.offlineVRRig != null);

            textObject = new GameObject("GorillaSession");
            text = textObject.AddComponent<TextMeshPro>();
            text.font = GorillaTagger.Instance.offlineVRRig.playerText1.font;
            text.fontSize = 5f;

            Color parsedColor = ParseHexColor(textColorHexConfig.Value);
            text.color = parsedColor;

            textObject.transform.position = new Vector3(-63.7395f, 12.2092f, - 81.8738f);
            textObject.transform.localScale = new Vector3(0.0364f, 0.0418f, 0.1f);
            textObject.transform.rotation = Quaternion.Euler(0.8469f, 52.4483f, 0f);

            StartCoroutine(UpdateDisplay());
        }

        IEnumerator UpdateDisplay()
        {
            while (true)
            {
                float sessionSeconds = Time.time - sessionStartTime;
                float totalSeconds = manager.Data.totalSeconds + sessionSeconds;

                TimeSpan sessionTime = TimeSpan.FromSeconds(sessionSeconds);
                TimeSpan totalTime = TimeSpan.FromSeconds(totalSeconds);

                text.text = $"<b>GorillaSession</b>\nby: elliot :3\n\n" +
                            $"Current Session: {sessionTime:hh\\:mm\\:ss}\n" +
                            $"Total Time: {totalTime:hh\\:mm\\:ss}";

                yield return new WaitForSeconds(1f);
            }
        }

        void OnApplicationQuit()
        {
            manager.Data.totalSeconds += Time.time - sessionStartTime;
            manager.SavePlaytime(savePath);
        }

        private Color ParseHexColor(string hex)
        {

            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length == 6)
            {

                hex += "FF";
            }

            if (hex.Length != 8)
            {
                return Color.white;
            }

            try
            {
                byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                byte a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

                return new Color32(r, g, b, a);
            }
            catch (Exception e)
            {
                //will change to white if the color isnt loaded (i think)
                return Color.white;
            }
        }
    }
}
