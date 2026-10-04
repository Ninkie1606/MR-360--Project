using System;
using UnityEngine;
using UnityEngine.Video;

namespace VR360
{
    [RequireComponent(typeof(VideoPlayer))]
    public class Video360Player : MonoBehaviour
    {
        public enum ProjectionMode
        {
            SkyboxPanoramic,
            MeshRenderer
        }

        [Header("Video Bron")]
        [Tooltip("Sleep hier je 360 MP4 videoclip in.")]
        [SerializeField] private VideoClip videoClip;
        [Tooltip("Of gebruik een URL / pad (leeg laten als je VideoClip gebruikt).")]
        [SerializeField] private string videoUrl = "";

        [Header("Weergave Instellingen")]
        [SerializeField] private ProjectionMode projectionMode = ProjectionMode.SkyboxPanoramic;
        [Tooltip("Doel MeshRenderer (alleen nodig bij MeshRenderer modus).")]
        [SerializeField] private MeshRenderer targetMeshRenderer;
        [Tooltip("Resolutie van de interne RenderTexture (standaard 4096x2048 voor hoge kwaliteit 360).")]
        [SerializeField] private Vector2Int textureResolution = new Vector2Int(4096, 2048);

        [Header("Afspeelopties")]
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private bool loopVideo = true;
        [SerializeField] private bool enableKeyboardDebugControls = true;

        [Header("Audio & Camera")]
        [Tooltip("Voegt automatisch het rondkijk-script toe aan de Main Camera als deze ontbreekt.")]
        [SerializeField] private bool autoAddCameraLook = true;
        [Tooltip("Optionele AudioSource om het geluid van de video af te spelen.")]
        [SerializeField] private AudioSource audioSource;

        private VideoPlayer videoPlayer;
        private RenderTexture renderTexture;
        private Material panoramicSkyboxMaterial;

        public VideoPlayer Player => videoPlayer;
        public RenderTexture VideoTexture => renderTexture;

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            SetupRenderTexture();
            SetupProjection();
            SetupVideoPlayer();

            if (autoAddCameraLook)
            {
                EnsureCameraLook();
            }
        }

        private void EnsureCameraLook()
        {
            Camera cam = Camera.main;
            if (cam != null && cam.GetComponent<VRCameraLook>() == null)
            {
                cam.gameObject.AddComponent<VRCameraLook>();
            }
        }

        private void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        private void SetupRenderTexture()
        {
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(textureResolution.x, textureResolution.y, 0, RenderTextureFormat.ARGB32)
                {
                    name = "VR360_RenderTexture",
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear,
                    antiAliasing = 1
                };
                renderTexture.Create();
            }
        }

        private void SetupProjection()
        {
            if (projectionMode == ProjectionMode.SkyboxPanoramic)
            {
                Shader skyboxShader = Shader.Find("Skybox/Panoramic");
                if (skyboxShader == null)
                {
                    Debug.LogWarning("[Video360Player] Shader 'Skybox/Panoramic' niet direct gevonden, zoekt alternatief...");
                    skyboxShader = Shader.Find("Universal Render Pipeline/Unlit");
                }

                if (skyboxShader != null)
                {
                    panoramicSkyboxMaterial = new Material(skyboxShader)
                    {
                        name = "VR360_SkyboxMaterial"
                    };
                    panoramicSkyboxMaterial.mainTexture = renderTexture;
                    panoramicSkyboxMaterial.SetTexture("_MainTex", renderTexture);
                    panoramicSkyboxMaterial.SetFloat("_ImageType", 0); // 360 degrees
                    panoramicSkyboxMaterial.SetFloat("_Layout", 0); // None (Equirectangular 360)

                    RenderSettings.skybox = panoramicSkyboxMaterial;
                    DynamicGI.UpdateEnvironment();
                }
            }
            else if (projectionMode == ProjectionMode.MeshRenderer && targetMeshRenderer != null)
            {
                Material mat = targetMeshRenderer.material;
                if (mat != null)
                {
                    mat.mainTexture = renderTexture;
                    if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", renderTexture);
                }
            }
        }

        private void SetupVideoPlayer()
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = loopVideo;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.skipOnDrop = true; // FORCEER Unity om frames te droppen ipv audio te vertragen!

            if (audioSource != null)
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                videoPlayer.EnableAudioTrack(0, true);
                videoPlayer.SetTargetAudioSource(0, audioSource);
            }
            else
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            }

            if (videoClip != null)
            {
                videoPlayer.source = VideoSource.VideoClip;
                videoPlayer.clip = videoClip;
            }
            else if (!string.IsNullOrEmpty(videoUrl))
            {
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = videoUrl;
            }

            videoPlayer.errorReceived += (source, message) =>
            {
                Debug.LogError($"[Video360Player] Fout bij afspelen video: {message}");
            };
        }

        private void Update()
        {
            if (enableKeyboardDebugControls)
            {
                HandleDebugKeyboard();
            }
        }

        private void HandleDebugKeyboard()
        {
            // Handig tijdens testen in de Unity editor
#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb != null)
            {
                if (kb.spaceKey.wasPressedThisFrame)
                {
                    TogglePlayPause();
                }
                if (kb.rKey.wasPressedThisFrame)
                {
                    Restart();
                }
            }
#else
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TogglePlayPause();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
#endif
        }

        public void Play()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
        }

        public void Pause()
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }

        public void TogglePlayPause()
        {
            if (videoPlayer == null) return;

            if (videoPlayer.isPlaying)
                Pause();
            else
                Play();
        }

        public void Restart()
        {
            if (videoPlayer != null)
            {
                videoPlayer.time = 0;
                videoPlayer.Play();
            }
        }

        public void SetClip(VideoClip newClip)
        {
            videoClip = newClip;
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = newClip;
            videoPlayer.Prepare();
        }

        public void SetUrl(string newUrl)
        {
            videoUrl = newUrl;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = newUrl;
            videoPlayer.Prepare();
        }

        private void OnDestroy()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }

            if (panoramicSkyboxMaterial != null)
            {
                Destroy(panoramicSkyboxMaterial);
            }
        }
    }
}
