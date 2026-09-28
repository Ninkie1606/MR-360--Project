#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace VR360.Editor
{
    public static class SetupVR360Scene
    {
        [MenuItem("Tools/VR 360/Setup 360 Video Player in Scene", false, 10)]
        public static void CreateVR360Setup()
        {
            // 1. Zoek of maak het 360 Video Manager object
            GameObject managerObj = GameObject.Find("VR_360_Player");
            if (managerObj == null)
            {
                managerObj = new GameObject("VR_360_Player");
                Undo.RegisterCreatedObjectUndo(managerObj, "Create VR 360 Player");
            }

            VideoPlayer videoPlayer = managerObj.GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                videoPlayer = Undo.AddComponent<VideoPlayer>(managerObj);
            }

            AudioSource audioSource = managerObj.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = Undo.AddComponent<AudioSource>(managerObj);
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // 2D stereo voor 360 video tenzij ruimtelijke audio gewenst is
            }

            Video360Player player360 = managerObj.GetComponent<Video360Player>();
            if (player360 == null)
            {
                player360 = Undo.AddComponent<Video360Player>(managerObj);
            }

            // Koppel audioSource veld via SerializedObject
            SerializedObject serializedPlayer = new SerializedObject(player360);
            SerializedProperty audioProp = serializedPlayer.FindProperty("audioSource");
            if (audioProp != null)
            {
                audioProp.objectReferenceValue = audioSource;
                serializedPlayer.ApplyModifiedProperties();
            }

            // 2. Configureer de Camera
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                camObj.AddComponent<AudioListener>();
                Undo.RegisterCreatedObjectUndo(camObj, "Create Main Camera");
            }

            // Voeg VRCameraLook toe voor handige Editor testing
            if (mainCam.GetComponent<VRCameraLook>() == null)
            {
                Undo.AddComponent<VRCameraLook>(mainCam.gameObject);
            }

            // Zet camera clear flags op Skybox
            mainCam.clearFlags = CameraClearFlags.Skybox;

            Selection.activeGameObject = managerObj;

            EditorUtility.DisplayDialog(
                "VR 360 Setup Voltooid!",
                "Het 'VR_360_Player' object is toegevoegd aan je scene.\n\n" +
                "Volgende stappen:\n" +
                "1. Selecteer 'VR_360_Player' in de Hierarchy.\n" +
                "2. Sleep je 360 video bestand (.mp4) in het vak 'Video Clip'.\n" +
                "3. Druk op Play!\n\n" +
                "Tip: In de Editor kun je met de rechtermuisknop 360° rondkijken.",
                "Top!"
            );
        }
    }
}
#endif
