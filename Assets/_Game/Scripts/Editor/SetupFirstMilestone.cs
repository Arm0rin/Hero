#if UNITY_EDITOR
using Hero.CameraSystem;
using Hero.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hero.EditorTools
{
    public static class SetupFirstMilestone
    {
        const string Root = "Assets/_Game";
        [MenuItem("HERO/Build First Milestone Scenes")]
        public static void Build()
        {
            var config = AssetDatabase.LoadAssetAtPath<PlayerMovementConfig>(Root + "/PlayerMovement.asset");
            if (!config)
            {
                config = ScriptableObject.CreateInstance<PlayerMovementConfig>();
                AssetDatabase.CreateAsset(config, Root + "/PlayerMovement.asset");
            }
            var boot = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Boot — first milestone placeholder");
            EditorSceneManager.SaveScene(boot, Root + "/Scenes/00_Boot.unity");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var light = new GameObject("Sun").AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);
            RenderSettings.ambientLight = new Color(.65f, .7f, .8f);
            Block("Ground", new Vector3(0, -.3f, 0), new Vector3(22, .6f, 18), new Color(.16f, .2f, .3f));
            for (int i = 0; i < 5; i++)
                Block("Test Platform " + i, new Vector3(-4 + i * 2.1f, .35f + i * .45f, 4 + i * 2f),
                    new Vector3(2, .35f, 2), new Color(.3f, .8f, .9f));
            var hero = new GameObject("Placeholder Hero");
            hero.transform.position = new Vector3(0, 1.2f, 0);
            var character = hero.AddComponent<CharacterController>();
            character.height = 1.8f;
            character.radius = .35f;
            character.stepOffset = .3f;
            character.slopeLimit = 45f;
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Visual Only";
            body.transform.SetParent(hero.transform, false);
            body.transform.localPosition = new Vector3(0, .9f, 0);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            body.GetComponent<Renderer>().sharedMaterial = Material(new Color(.96f, .64f, .24f));
            var input = hero.AddComponent<MobileInput>();
            var motor = hero.AddComponent<PlayerMotor>();
            motor.config = config;
            motor.input = input;
            var cam = new GameObject("Third Person Camera");
            cam.tag = "MainCamera";
            cam.transform.position = new Vector3(0, 3, -5);
            cam.AddComponent<Camera>();
            cam.AddComponent<AudioListener>();
            var follow = cam.AddComponent<FollowCamera>();
            follow.target = hero.transform;
            // The capsule itself is excluded by the camera cast through the Ignore Raycast layer.
            hero.layer = 2;
            follow.collisionMask = ~(1 << 2);
            motor.cameraTransform = cam.transform;
            var canvas = new GameObject("Mobile Controls", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var c = canvas.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = .5f;
            var joy = Disc("Joystick Base", canvas.transform, new Vector2(190, 190),
                new Vector2(0, 0), new Vector2(180, 210), new Color(1, 1, 1, .3f));
            var knob = Disc("Knob", joy, new Vector2(90, 90),
                new Vector2(.5f, .5f), Vector2.zero, new Color(1, 1, 1, .8f));
            var joystick = joy.gameObject.AddComponent<VirtualJoystick>();
            joystick.baseRect = joy;
            joystick.knob = knob;
            input.joystick = joystick;
            var jump = Disc("Jump", canvas.transform, new Vector2(170, 170),
                Vector2.one, new Vector2(-145, 215), new Color(.25f, .82f, 1f, .8f));
            var button = jump.gameObject.AddComponent<Button>();
            button.onClick.AddListener(input.PressJump);
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            EditorSceneManager.SaveScene(scene, Root + "/Scenes/90_Dev_Playground.unity");
            EditorBuildSettings.scenes = new[] {
                new EditorBuildSettingsScene(Root + "/Scenes/90_Dev_Playground.unity", true)
            };
            AssetDatabase.SaveAssets();
            Debug.Log("HERO: first milestone playground ready. Open 90_Dev_Playground and press Play.");
        }
        static Material Material(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader);
            mat.color = color;
            return mat;
        }
        static GameObject Block(string name, Vector3 position, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = Material(color);
            return go;
        }
        static RectTransform Disc(string name, Transform parent, Vector2 size, Vector2 anchor, Vector2 position, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = anchor == Vector2.one ? Vector2.one :
                anchor == Vector2.zero ? Vector2.zero : new Vector2(.5f, .5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            go.GetComponent<Image>().color = color;
            return rect;
        }
    }
}
#endif
