using System;
using System.Collections;
using System.Reflection;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppSystem.Diagnostics;
using RumbleModdingAPI;
using BuildInfo = ArcadeRumble.BuildInfo;
using UnityEngine.UI;
using static MelonLoader.MelonLogger;
using Il2CppRUMBLE.Input;
using Il2CppRUMBLE.UI;
using UnityEngine.Windows;
using Il2CppRUMBLE.Pools;
using static RumbleModdingAPI.Calls.GameObjects.DDOL.GameInstance.UI.LegacyRecordingCameraUI.Panel;
using UnityEngine.VFX;
using Il2CppRUMBLE.Managers;
using static Il2CppRootMotion.FinalIK.RagdollUtility;
using RumbleModUI;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using MelonLoader.Utils;
using Il2CppPhoton.Voice;
using System.Text;
using static Il2CppRootMotion.FinalIK.GenericPoser;
using static ArcadeRumble.AudioManager;
using Il2CppRUMBLE.Combat.ShiftStones;
using Il2CppRUMBLE.Environment;
using Il2CppTMPro;
using static RumbleModdingAPI.Calls.GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard.TextObjects;
using UnityEngine.InputSystem;
using Il2CppRootMotion.FinalIK;
using static Il2CppRootMotion.FinalIK.HitReactionVRIK;
using static RumbleModdingAPI.Calls.GameObjects.DDOL.GameInstance.Initializable;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem.XR;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Interactions.InteractionBase;
using System.Net.Http.Headers;
using Il2CppPlayFab.ClientModels;
using Il2CppSystem.Runtime.Remoting.Messaging;
using static RumbleModdingAPI.Calls.ControllerMap;

[assembly: MelonInfo(typeof(ArcadeRumble.Class1), BuildInfo.Name, BuildInfo.Version, BuildInfo.Author, BuildInfo.DownloadLink)]
[assembly: MelonGame(null, null)]

namespace ArcadeRumble
{    public static class BuildInfo
    {
        public const string Name = "ArcadeRumble";
        public const string Description = "Turns RUMBLE into an arcade game";
        public const string Author = "MadLike";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class Class1 : MelonMod
    {

        public static GameObject? cameraHolder = null;
        public static Camera? newCamera = null;

        //public static GameObject? staticCameraHolder = null;
        public static Camera? staticCamera = null;

        private static float deadzone = 0.3f;
        private static float fallSoundCooldown = 0;

        public static PlayerCamera? playerCameraComponent = null;
        private string currentScene = "Loader";

        public static bool assetLoaded = false;
        public static bool modEnabled = false;

        public static bool livOnlyMode = false;



        public static bool modEnabledAtAllTimes = true;//SET TO FALSE ON RELEASE --------------------------------------------------------------------






        public static bool usedSnapTurnBefore = false;

        private Dictionary<GameObject, ClipData> activeVFXClips = new Dictionary<GameObject, ClipData>();

        //technical bools to fix bugs
        public static bool fistBumpPrevention = false;

        public static AudioManager.ClipData? gymAmbience = null;
        public static AudioManager.ClipData? matchAmbience = null;


        private static Dictionary<PlayerBoxInteractionTrigger, float> playerInteractionTimestamps = new Dictionary<PlayerBoxInteractionTrigger, float>();
        private static List<GameObject> activeStructures = new List<GameObject>();
        private Dictionary<GameObject, ClipData> explodedStructures = new Dictionary<GameObject, ClipData>();
        public List<AudioManager.ClipData> activeSounds = new List<AudioManager.ClipData>();
        Dictionary<GameObject, bool> lastFrameActiveStates = new Dictionary<GameObject, bool>();

        private Dictionary<Il2CppRUMBLE.Players.Player, GameObject> playerTexts = new Dictionary<Il2CppRUMBLE.Players.Player, GameObject>();


        public static GameObject? localHealthBar;
        public static GameObject? enemyHealthBar;

        private static int localScreenshakeFramesLeft = 0;
        private static int enemyScreenshakeFramesLeft = 0;

        public static bool matchFound = false;
        public static bool friendQueue = false;

        private static int lastPlayerHealth = 20;
        private static int lastEnemyHealth = 20;

        public static Material? greenMaterial;
        public static Material? yellowMaterial;
        public static Material? redMaterial;
        public static Material? blueMaterial;
        public static Material? whiteMaterial;
        public static Material? versusBlueMaterial;
        public static Material? versusRedMaterial;

        public static Transform? enemyPlayer = null;

        public static bool wasSetToSnapTurn = false;
        public static bool joysticksPressed = false;

        public static Vector2 actualTurnInput = new Vector2(0, 0);


        private static List<GameObject> localShiftstones = new List<GameObject>();
        private static List<GameObject> enemyShiftstones = new List<GameObject>();

        private static InputActionMap map = new InputActionMap("Rest Xr Map");

        private static InputAction leftJoystickClick = InputActionSetupExtensions.AddAction(map, "Left Joystick Click", (InputActionType)0, (string)null, (string)null, (string)null, (string)null, (string)null);

        private static InputAction rightJoystickClick = InputActionSetupExtensions.AddAction(map, "Right Joystick Click", (InputActionType)0, (string)null, (string)null, (string)null, (string)null, (string)null);


        Mod Mod = new Mod();
        private ModSetting<bool>? modEnabledUI;
        private ModSetting<bool>? enabledInGym;
        private ModSetting<bool>? hideOwnHealthBar;
        private ModSetting<bool>? hideOwnHealthBarMatch;
        private ModSetting<bool>? hideOwnAndEnemyHealthBar;
        private ModSetting<bool>? enable8BitSounds;
        private ModSetting<bool>? parkHealthBarMode;
        private ModSetting<bool>? showEnemyHealthBarsPark;
        private ModSetting<bool>? handsEnabled;
        private ModSetting<float>? matchTheme8bitVolume;

        private ModSetting<bool>? staticCameraMode;

        private ModSetting<float>? staticCameraPositionX;
        private ModSetting<float>? staticCameraPositionY;
        private ModSetting<float>? staticCameraPositionZ;

        private ModSetting<float>? staticCameraRotationX;
        private ModSetting<float>? staticCameraRotationY;
        private ModSetting<float>? staticCameraRotationZ;


        private ModSetting<float>? leftHandX;
        private ModSetting<float>? leftHandY;
        private ModSetting<float>? leftHandZ;

        private ModSetting<float>? rightHandX;
        private ModSetting<float>? rightHandY;
        private ModSetting<float>? rightHandZ;

        //private ModSetting<float>? staticCameraRotationX1;
        //private ModSetting<float>? staticCameraRotationY1;
        //private ModSetting<float>? staticCameraRotationZ1;

        //private ModSetting<float>? staticCameraRotationX2;
        //private ModSetting<float>? staticCameraRotationY2;
        //private ModSetting<float>? staticCameraRotationZ2;

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            UI.instance.UI_Initialized += OnUIInit;
        }


        public void OnUIInit()
        {
            Mod.ModName = BuildInfo.Name;
            Mod.ModVersion = BuildInfo.Version;
            Mod.SetFolder("ArcadeRumble");
            Mod.AddDescription("Description", "", BuildInfo.Description, new Tags { IsSummary = true });

            modEnabledUI = Mod.AddToList("Is Mod Enabled", true, 0, "Requires exiting into the Gym to take effect", new Tags());
            enabledInGym = Mod.AddToList("Is Mod Enabled In Gym", false, 0, "Enables the mod in the gym (may be very difficult to get around). Requires reentering the Gym to take effect", new Tags());
            hideOwnHealthBar = Mod.AddToList("Hide Own Health Bar In Gym/Park", false, 0, "Does NOT require exiting into the Gym to take effect", new Tags());
            hideOwnHealthBarMatch = Mod.AddToList("Hide Own Health Bar In Match", false, 0, "Does NOT require exiting into the Gym to take effect", new Tags());
            enable8BitSounds = Mod.AddToList("Enable 8-bit audio", true, 0, "Does NOT require exiting into the Gym to take effect", new Tags());
            hideOwnAndEnemyHealthBar = Mod.AddToList("Hide Own and Enemy Health Bars In Matches", false, 0, "Why..? Does NOT require exiting into the Gym to take effect", new Tags());
            parkHealthBarMode = Mod.AddToList("Park Health Bar Mode", false, 0, "False (red) = health bar displayed below character, True (green) = health bar displayed at the top left corner. Does NOT require exiting into the Gym to take effect", new Tags());
            showEnemyHealthBarsPark = Mod.AddToList("Show Enemy Health Bars In Parks", true, 0, "Does NOT require exiting into the Gym to take effect", new Tags());
            handsEnabled = Mod.AddToList("Enable Cosmetic Hands", false, 0, "Does NOT require exiting into the Gym to take effect", new Tags());

            matchTheme8bitVolume = Mod.AddToList("8-bit Match Theme Volume", 0.3f, "Accepts values from 0 to 1. Requires exiting into the Gym to take effect", new Tags());

            staticCameraMode = Mod.AddToList("Static LIV Camera", false, 0, "False: camera follows the head as per usual. True: camera is stationary and its position and rotation are determined by the settings.", new Tags());

            staticCameraPositionX = Mod.AddToList("Static LIV Camera Position Offset X", 0f, "Accepts values from -4 to 4", new Tags());
            staticCameraPositionY = Mod.AddToList("Static LIV Camera Position Offset Y", 1.75f, "Accepts values from -4 to 4", new Tags());
            staticCameraPositionZ = Mod.AddToList("Static LIV Camera Position Offset Z", -0.5f, "Accepts values from -4 to 4", new Tags());

            staticCameraRotationX = Mod.AddToList("Static LIV Camera Rotation Offset X", 30f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());
            staticCameraRotationY = Mod.AddToList("Static LIV Camera Rotation Offset Y", 0f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());
            staticCameraRotationZ = Mod.AddToList("Static LIV Camera Rotation Offset Z", 0f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());


            leftHandX = Mod.AddToList("Static LIV Camera Rotation Offset X1", -20f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());
            leftHandY = Mod.AddToList("Static LIV Camera Rotation Offset Y1", 25f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());
            leftHandZ = Mod.AddToList("Static LIV Camera Rotation Offset Z1", 90f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());

            rightHandX = Mod.AddToList("Static LIV Camera Rotation Offset X2", 160f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());
            rightHandY = Mod.AddToList("Static LIV Camera Rotation Offset Y2", -25f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());
            rightHandZ = Mod.AddToList("Static LIV Camera Rotation Offset Z2", -90f, "Accepts values from -360 to 360. Rotation in degrees", new Tags());

            Mod.GetFromFile();

            UI.instance.AddMod(Mod);
        }

        private IEnumerator YellowGhostPoseCloningCoroutine()
        {
            while (!GameObject.Find("BootLoaderPlayer/Visuals/Left"))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            GameObject newParent = newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject();

            GameObject armMesh = GameObject.Find("BootLoaderPlayer/Visuals/Left").gameObject;

            //GameObject armMeshLeft = GameObject.Find("BootLoaderPlayer/Visuals/Left").gameObject;
            //GameObject armMeshRight = GameObject.Find("BootLoaderPlayer/Visuals/Right").gameObject;
            GameObject invertedSphere = GameObject.Find("________________SCENE_________________/Text/Iverted sphere").gameObject;
            

            GameObject armMeshClone = GameObject.Instantiate(armMesh).gameObject;
            //GameObject armMeshClone1 = GameObject.Instantiate(armMeshLeft).gameObject;
            //GameObject armMeshClone2 = GameObject.Instantiate(armMeshRight).gameObject;
            GameObject invertedSphereClone = GameObject.Instantiate(invertedSphere).gameObject;

            armMeshClone.SetActive(false);
            armMeshClone.name = "YellowGhostPoseObject1";
            armMeshClone.transform.parent = newParent.transform;

            //armMeshClone1.SetActive(true);
            //armMeshClone1.name = "leftHand";
            //armMeshClone1.transform.parent = newParent.transform;

            //armMeshClone2.SetActive(true);
            //armMeshClone2.name = "rightHand";
            //armMeshClone2.transform.parent = newParent.transform;

            invertedSphereClone.transform.localScale = new UnityEngine.Vector3(15, 15, 15);
            invertedSphereClone.SetActive(false);
            invertedSphereClone.name = "InvertedSphereCloneArcadeRumble";
            invertedSphereClone.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
            invertedSphereClone.transform.parent = newParent.transform;
        }

        static string CleanUsername(string input)
        {
            string pattern = @"<#.*?>";

            return Regex.Replace(input, pattern, string.Empty);
        }
        public override void OnUpdate()
        {


            //if (!assetLoaded) return;

            //Transform newParent1 = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            //var playerManager2 = Calls.Managers.GetPlayerManager();

            //if (playerManager2 != null && playerManager2.AllPlayers != null)
            //{
            //    for (int index = 0; index < playerManager2.AllPlayers.Count; index++)
            //    {
            //        var player = playerManager2.AllPlayers[index];
            //        if (player != null && player.Controller != null && player.Controller.name == "Player Controller(Clone)" && player.Data.GeneralData.PublicUsername == "MadLike")
            //        {
            //            GameObject controller = player.Controller.gameObject;
            //            Transform rightControllerOriginal = controller.transform.Find("Visuals").GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(6);//0 90 70 rotation -0.04f -0.04f 0.03f position

            //            newParent1.transform.Find("phone").transform.position = rightControllerOriginal.position + rightControllerOriginal.transform.TransformDirection(new Vector3(0.03f, 0.14f, 0f));//new Vector3(Mathf.Clamp((float)((ModSetting)leftHandX).SavedValue, -360, 360), Mathf.Clamp((float)((ModSetting)leftHandY).SavedValue, -360, 360), Mathf.Clamp((float)((ModSetting)leftHandZ).SavedValue, -360, 360)));//new Vector3(0.07f, 0.09f, 0.03f));
            //            newParent1.transform.Find("phone").transform.rotation = rightControllerOriginal.rotation * Quaternion.Euler(0, -90, -90);//Quaternion.Euler(Mathf.Clamp((float)((ModSetting)rightHandX).SavedValue, -360, 360), Mathf.Clamp((float)((ModSetting)rightHandY).SavedValue, -360, 360), Mathf.Clamp((float)((ModSetting)rightHandZ).SavedValue, -360, 360));//Quaternion.Euler(0, -90, -80);
            //        }
            //    }
            //}

            if (!modEnabled) return;

            if (cameraHolder == null || cameraHolder.transform == null) return;

            if (!(bool)((ModSetting)enable8BitSounds).SavedValue)
            {
                var poolsParent1 = Calls.Pools.Structures.GetPoolCube().transform.parent;
                PoolManager poolManager = poolsParent1.GetComponent<PoolManager>();
                var poolSettingsArray = poolManager.resourcesToPool;

                var soundPoolSetting = poolSettingsArray[54];

                soundPoolSetting.ResetPoolOnSceneLoad = true;

                

                poolsParent1.GetChild(54).gameObject.SetActive(true);


            }

            Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

 
            GameObject headset = cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            GameObject leftController = cameraHolder.transform.GetChild(1).GetChild(1).gameObject;
            GameObject rightController = cameraHolder.transform.GetChild(1).GetChild(2).gameObject;

            

            if (headset == null || leftController == null || rightController == null) return;

            if (currentScene == "Map0" && Calls.GameObjects.Map0.Logic.MatchSlabOne.MatchSlab.SlabBuddyMatchVariant.MatchForm.GetGameObject().active || currentScene == "Map1" && Calls.GameObjects.Map1.Logic.MatchSlabOne.MatchSlab.SlabBuddyMatchVariant.MatchForm.GetGameObject().active)
            {
                modEnabled = false;

                Transform targetTransform = Calls.Managers.GetPlayerManager().localPlayer.Controller.transform.GetChild(1).GetChild(0).GetChild(0);
                GameObject parentController = Calls.Managers.GetPlayerManager().localPlayer.Controller.gameObject;

                targetTransform.GetComponent<PlayerCamera>().camera = targetTransform.GetComponent<Camera>();
                targetTransform.GetComponent<Camera>().enabled = true;

                cameraHolder.transform.Find("LIV").gameObject.SetActive(false);

                parentController.transform.Find("LIV").gameObject.SetActive(true);

                newParent.Find("MenuFormDDOL").gameObject.GetComponent<SettingsForm>().SetTurnMode(InputConfiguration.TurnMode.Snap, true);

                ChangeHeadVisibility(false);

                headset.GetComponent<Camera>().enabled = false;

                var poolsParent1 = Calls.Pools.Structures.GetPoolCube().transform.parent;
                PoolManager poolManager = poolsParent1.GetComponent<PoolManager>();
                var poolSettingsArray = poolManager.resourcesToPool;

                var soundPoolSetting = poolSettingsArray[54];


                poolsParent1.GetChild(54).gameObject.SetActive(true);

                int poolChildren1 = poolsParent1.transform.GetChild(54).childCount;

                for (int i = poolChildren1 - 1; i >= 0; i--)
                {
                    GameObject.Destroy(poolsParent1.GetChild(54).GetChild(i).gameObject);
                }

                newParent.Find("ArcadeCamera(Clone)").gameObject.SetActive(false);
                newParent.Find("HealthBarCamera").gameObject.SetActive(false);
                newParent.Find("VersusCamera").gameObject.SetActive(false);


                if (matchAmbience != null)
                {
                    AudioManager.StopPlayback(matchAmbience);
                }

                return;
            }

            if (!livOnlyMode)
            {
                headset.GetComponent<Camera>().enabled = true;
            }
            

            Vector3 leftControllerRelativePosition = headset.transform.InverseTransformPoint(leftController.transform.position);
            Vector3 rightControllerRelativePosition = headset.transform.InverseTransformPoint(rightController.transform.position);

            Quaternion fixedHeadsetRotation = Quaternion.Euler(0, headset.transform.rotation.eulerAngles.y, headset.transform.rotation.eulerAngles.z);

            Quaternion leftControllerRelativeRotation = Quaternion.Inverse(headset.transform.rotation) * leftController.transform.rotation;
            Quaternion rightControllerRelativeRotation = Quaternion.Inverse(headset.transform.rotation) * rightController.transform.rotation;

            var playerManager = Calls.Managers.GetPlayerManager();
            var localPlayerController = playerManager.localPlayer.Controller;
            

            GameObject localPlayerHeadset = localPlayerController.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

            if (localPlayerHeadset == null) return;

            //// Apply the X-axis rotation of the camera holder headset to the local player's headset
            //Vector3 cameraHolderEulerAngles = headset.transform.rotation.eulerAngles; // Get camera holder's rotation in Euler angles
            //Vector3 localPlayerEulerAngles = localPlayerHeadset.transform.rotation.eulerAngles; // Get local player's current rotation in Euler angles
            //localPlayerEulerAngles.x = cameraHolderEulerAngles.x; // Set the X-axis rotation
            //localPlayerHeadset.transform.rotation = Quaternion.Euler(localPlayerEulerAngles); // Apply the new rotation

            GameObject localPlayerLeftController = localPlayerController.transform.GetChild(1).GetChild(1).gameObject;
            GameObject localPlayerRightController = localPlayerController.transform.GetChild(1).GetChild(2).gameObject;

            newParent.transform.Find("leftHand").position = leftController.transform.position + leftController.transform.TransformDirection(new Vector3(0, 0.025f, 0.05f));
            newParent.transform.Find("rightHand").position = rightController.transform.position + rightController.transform.TransformDirection(new Vector3(0, 0.025f, 0.05f)); ;


            newParent.transform.Find("leftHand").rotation = leftController.transform.rotation * Quaternion.Euler(-70f, 100f, 0f);
            newParent.transform.Find("rightHand").rotation = rightController.transform.rotation * Quaternion.Euler(70f, 80f, 0);

            if ((bool)((ModSetting)handsEnabled).SavedValue)
            {
                newParent.transform.Find("leftHand").GetChild(0).GetComponent<Renderer>().enabled = true;
                newParent.transform.Find("rightHand").GetChild(0).GetComponent<Renderer>().enabled = true;
            }
            else
            {
                newParent.transform.Find("leftHand").GetChild(0).GetComponent<Renderer>().enabled = false;
                newParent.transform.Find("rightHand").GetChild(0).GetComponent<Renderer>().enabled = false;
            }



            Transform localVRObject = localPlayerController.transform.GetChild(1);
            Transform arcadeCamera = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform.Find("ArcadeCamera(Clone)");

            if (localVRObject == null || arcadeCamera == null)
                return;

            var playerMovement = localPlayerController.gameObject.GetComponent<PlayerMovement>();
            Vector2 walkInput = playerMovement.currentRawWalkInput;
            Vector2 turnInput = actualTurnInput;//raw turn input is always (1, 0)


            bool isWalking = Mathf.Abs(walkInput.x) > deadzone || Mathf.Abs(walkInput.y) > deadzone;
            bool isTurning = Mathf.Abs(turnInput.x) > deadzone || Mathf.Abs(turnInput.y) > deadzone;


            Vector3 cameraForward = arcadeCamera.forward;
            Vector3 cameraRight = arcadeCamera.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            if (!livOnlyMode)
            {
                playerMovement.minMaxVignetteIntensity = new Vector2(0, 0);


                if (isTurning)
                {
                    float turnAngle = Mathf.Atan2(turnInput.x, turnInput.y) * Mathf.Rad2Deg;


                    Quaternion VRRotation = Quaternion.Euler(0, turnAngle + arcadeCamera.eulerAngles.y, 0);
                    Quaternion targetRotation = Quaternion.Euler(cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).rotation.eulerAngles.x, turnAngle + arcadeCamera.eulerAngles.y, cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).rotation.eulerAngles.z);

                    float currentYRotation = localPlayerHeadset.transform.rotation.eulerAngles.y;
                    float targetYRotation = targetRotation.eulerAngles.y;

                    float angleDifference = Mathf.DeltaAngle(currentYRotation, targetYRotation);

                    float requiredTurnSpeed = angleDifference / Time.deltaTime;

                    playerMovement.minMaxSmoothTurnSpeed = new Vector2(requiredTurnSpeed, requiredTurnSpeed);


                }

                if (!isWalking && !isTurning)
                {
                    Quaternion resetRotation = Quaternion.Euler(cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).rotation.eulerAngles.x, localPlayerHeadset.transform.rotation.eulerAngles.y, cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).rotation.eulerAngles.z);

                    playerMovement.minMaxSmoothTurnSpeed = new Vector2(0, 0);

                }
            }

            






            //if (localPlayerLeftController != null)
            //{
            //    localPlayerLeftController.transform.position = localPlayerHeadset.transform.TransformPoint(leftControllerRelativePosition) + new Vector3(0, 0.025f, 0);
            //    localPlayerLeftController.transform.rotation = localPlayerHeadset.transform.rotation * leftControllerRelativeRotation * Quaternion.Euler(-45, 0, -210);
            //}

            //if (localPlayerRightController != null)
            //{
            //    localPlayerRightController.transform.position = localPlayerHeadset.transform.TransformPoint(rightControllerRelativePosition) + new Vector3(0, 0.025f, 0);
            //    localPlayerRightController.transform.rotation = localPlayerHeadset.transform.rotation * rightControllerRelativeRotation * Quaternion.Euler(-45, 0, 210);
            //}






            // Define alpha variables for Lerp
            //float positionAlpha = 0.8f; // Default value for position smoothing
            //float rotationAlpha = 0.8f; // Default value for rotation smoothing

            //// Smoothly update the position and rotation of the left controller
            //if (localPlayerLeftController != null)
            //{
            //    Vector3 targetLeftPosition = localPlayerHeadset.transform.TransformPoint(leftControllerRelativePosition) + new Vector3(0, 0, 0);
            //    Quaternion targetLeftRotation = localPlayerHeadset.transform.rotation * leftControllerRelativeRotation * Quaternion.Euler(0, 0, -30);

            //    localPlayerLeftController.transform.position = Vector3.Lerp(
            //        localPlayerLeftController.transform.position,
            //        targetLeftPosition,
            //        positionAlpha
            //    );

            //    localPlayerLeftController.transform.rotation = Quaternion.Slerp(
            //        localPlayerLeftController.transform.rotation,
            //        targetLeftRotation,
            //        rotationAlpha
            //    );
            //}

            //// Smoothly update the position and rotation of the right controller
            //if (localPlayerRightController != null)
            //{
            //    Vector3 targetRightPosition = localPlayerHeadset.transform.TransformPoint(rightControllerRelativePosition) + new Vector3(0, 0, 0);
            //    Quaternion targetRightRotation = localPlayerHeadset.transform.rotation * rightControllerRelativeRotation * Quaternion.Euler(0, 0, 30);

            //    localPlayerRightController.transform.position = Vector3.Lerp(
            //        localPlayerRightController.transform.position,
            //        targetRightPosition,
            //        positionAlpha
            //    );

            //    localPlayerRightController.transform.rotation = Quaternion.Slerp(
            //        localPlayerRightController.transform.rotation,
            //        targetRightRotation,
            //        rotationAlpha
            //    );
            //}








            //Vector3 positionOffset = new Vector3(0, 0, -4);//gym default
            //Quaternion rotationOffset = Quaternion.Euler(5, 0, 0);
            //float zoomScale = 2f;

            //if (currentScene == "Park")
            //{
            //    positionOffset = new Vector3(0, 5, -7);
            //    rotationOffset = Quaternion.Euler(35, 0, 0);
            //    zoomScale = 3f;
            //}
            //else if (currentScene.Contains("Map"))
            //{
            //    positionOffset = new Vector3(0, 0, -7);
            //    rotationOffset = Quaternion.Euler(5, 0, 0);
            //    zoomScale = 3f;
            //}

            //var camera = newParent.transform.Find("ArcadeCamera(Clone)");

            //camera.GetComponent<Camera>().enabled = true;
            //camera.transform.position = localPlayerHeadset.transform.position + positionOffset;
            //camera.transform.rotation = rotationOffset;
            //camera.GetComponent<Camera>().orthographicSize = zoomScale;

            if ((bool)((ModSetting)staticCameraMode).SavedValue)
            {
                var livPosition = new Vector3(100, -15, 0) + new Vector3(Mathf.Clamp((float)((ModSetting)staticCameraPositionX).SavedValue, -4, 4), Mathf.Clamp((float)((ModSetting)staticCameraPositionY).SavedValue, -4, 4), Mathf.Clamp((float)((ModSetting)staticCameraPositionZ).SavedValue, -4, 4));
                var livRotation = Quaternion.Euler(Mathf.Clamp((float)((ModSetting)staticCameraRotationX).SavedValue, -360, 360), Mathf.Clamp((float)((ModSetting)staticCameraRotationY).SavedValue, -360, 360), Mathf.Clamp((float)((ModSetting)staticCameraRotationZ).SavedValue, -360, 360));

                var LIVObject = cameraHolder.transform.Find("LIV");

                if (livOnlyMode)
                {
                    LIVObject = Calls.Managers.GetPlayerManager().localPlayer.Controller.transform.Find("LIV");
                }

                var LIVComponent = LIVObject.GetComponent<Il2CppLIV.SDK.Unity.LIV>();

                try
                {
                    LIVComponent.render.SetPose(livPosition, livRotation, 60, false);//fov is 60
                }
                catch
                {
                }

            }


            var camera = newParent.transform.Find("ArcadeCamera(Clone)");
            var camera1 = newParent.transform.Find("HealthBarCamera");

            camera.GetComponent<Camera>().enabled = true;
            camera1.GetComponent<Camera>().enabled = true;

            var playerManager1 = Calls.Managers.GetPlayerManager();

            var enemyPlayerName = "";
            var currentEnemyHealth = 20;
            dynamic? enemyEquippedShiftstones = null;
            var currentPlayerHealth = playerManager1.localPlayer.Data.HealthPoints;

            var keysToRemove = new List<Il2CppRUMBLE.Players.Player>();
            foreach (var entry in playerTexts)
            {
                var player = entry.Key;
                var textObject = entry.Value;

                if (player == null || currentScene != "Park" || (bool)((ModSetting)hideOwnHealthBar).SavedValue && player.Controller == playerManager1.localPlayer.Controller.gameObject || (bool)((ModSetting)parkHealthBarMode).SavedValue && player.Controller == playerManager1.localPlayer.Controller.gameObject || !(bool)((ModSetting)showEnemyHealthBarsPark).SavedValue && player.Controller != playerManager1.localPlayer.Controller.gameObject)
                {

                    if (textObject != null)
                    {
                        GameObject.Destroy(textObject);
                    }

     
                    keysToRemove.Add(player);
                }
            }

            foreach (var key in keysToRemove)
            {
                playerTexts.Remove(key);
            }



            if (playerManager1 != null && playerManager1.AllPlayers != null)
            {
                for (int index = 0; index < playerManager1.AllPlayers.Count; index++)
                {
                    var player = playerManager1.AllPlayers[index];
                    if (player != null && player.Controller != null && player.Controller.name == "Player Controller(Clone)")
                    {
                        GameObject controller = player.Controller.gameObject;

                        

                        if (currentScene == "Park")
                        {
                            if (!(bool)((ModSetting)hideOwnHealthBar).SavedValue && player.Controller == playerManager1.localPlayer.Controller.gameObject || !(bool)((ModSetting)parkHealthBarMode).SavedValue && player.Controller == playerManager1.localPlayer.Controller.gameObject || (bool)((ModSetting)showEnemyHealthBarsPark).SavedValue && player.Controller != playerManager1.localPlayer.Controller.gameObject)
                            {

                                if (!playerTexts.ContainsKey(player))
                                {
                                    var usernameObject = GameObject.Instantiate(newParent.Find("TextPrefab").gameObject);
                                    usernameObject.transform.parent = newParent;


                                    var usernameTextObject = usernameObject.GetComponent<TextMeshPro>();
                                    usernameTextObject.text = CleanUsername(player.Data.GeneralData.PublicUsername);
                                    usernameTextObject.fontSize = 2f;
                                    usernameTextObject.color = new Color(1, 1, 1, 1);
                                    usernameTextObject.outlineColor = new Color(0, 0, 0, 1f);
                                    usernameTextObject.enableWordWrapping = false;
                                    usernameTextObject.outlineWidth = 0.6f;

                                    CreateHealthBar(false, usernameObject.transform);

                                    playerTexts[player] = usernameObject;
                                }
                                else
                                {

                                    GameObject usernameObject = playerTexts[player];

                                    GameObject VRObject = player.Controller.transform.Find("VR").GetChild(0).GetChild(0).gameObject;
                                    GameObject FootColliderObject = player.Controller.transform.Find("VR").GetChild(3).gameObject;

                                    var gameplayCamera = newParent.Find("ArcadeCamera(Clone)").gameObject;
                                    var healthBarCamera = newParent.Find("HealthBarCamera").gameObject;

                                    if (VRObject != null)
                                    {
                                        if (player.Controller != playerManager1.localPlayer.Controller.gameObject)
                                        {
                                            VRObject.transform.GetChild(2).GetComponent<AudioSource>().dopplerLevel = 0;
                                            VRObject.transform.GetChild(2).GetComponent<AudioSource>().maxDistance = 1000;

                                            if (player.Controller.transform.Find("LIV"))
                                            {
                                                player.Controller.transform.Find("LIV").gameObject.SetActive(false);
                                            }
                                        }

                                        //usernameObject.GetComponent<TextMeshPro>().SetOutlineColor(new Color(0, 0, 0, 1f));
                                        //usernameObject.GetComponent<TextMeshPro>().SetOutlineThickness(0.3f);

                                        Vector3 playerWorldPosition = VRObject.transform.position;

                                        Vector3 localPositionInGameplayCam = gameplayCamera.transform.InverseTransformPoint(playerWorldPosition);

                                        Vector3 positionInHealthBarCam = healthBarCamera.transform.TransformPoint(localPositionInGameplayCam);

                                        positionInHealthBarCam.z = 1.1f + positionInHealthBarCam.z / 10;


                                        Vector3 playerWorldPosition1 = FootColliderObject.transform.position;

                                        Vector3 localPositionInGameplayCam1 = gameplayCamera.transform.InverseTransformPoint(playerWorldPosition1);

                                        Vector3 positionInHealthBarCam1 = healthBarCamera.transform.TransformPoint(localPositionInGameplayCam1);

                                        positionInHealthBarCam1.z = 1.1f + positionInHealthBarCam1.z / 10;

                                        usernameObject.transform.position = positionInHealthBarCam + new Vector3(0.1f, -(positionInHealthBarCam.y - positionInHealthBarCam1.y), 0);

                                        
                                        usernameObject.transform.position += new Vector3(0f, -0.05f, 0);
                                        usernameObject.transform.GetChild(0).position = positionInHealthBarCam + new Vector3(0, -0.2f - (positionInHealthBarCam.y - positionInHealthBarCam1.y), 0);

                                        for (int i = 0; i < 10; i++)
                                        {

                                            usernameObject.transform.GetChild(0).GetChild(i).position = new Vector3(usernameObject.transform.GetChild(0).GetChild(i).position.x, usernameObject.transform.GetChild(0).GetChild(i).position.y, usernameObject.transform.GetChild(0).position.z + -0.1f);

                                            if ((i + 1) * 2 > player.Data.HealthPoints && (i + 1) * 2 - 1 > player.Data.HealthPoints)
                                            {
                                                usernameObject.transform.GetChild(0).GetChild(i).GetComponent<Renderer>().enabled = false;
                                            }
                                            else if ((i + 1) * 2 > player.Data.HealthPoints && (i + 1) * 2 - 1 == player.Data.HealthPoints)
                                            {
                                                usernameObject.transform.GetChild(0).GetChild(i).GetComponent<Renderer>().material = redMaterial;
                                                usernameObject.transform.GetChild(0).GetChild(i).GetComponent<Renderer>().enabled = true;
                                            }
                                            else if ((i + 1) * 2 <= player.Data.HealthPoints)
                                            {
                                                usernameObject.transform.GetChild(0).GetChild(i).GetComponent<Renderer>().material = whiteMaterial;
                                                usernameObject.transform.GetChild(0).GetChild(i).GetComponent<Renderer>().enabled = true;
                                            }

                                        }

                                        if (player.Data.HealthPoints == 20)
                                        {
                                            usernameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = blueMaterial;
                                        }
                                        else if (player.Data.HealthPoints > 15)
                                        {
                                            usernameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = yellowMaterial;
                                        }
                                        else if (player.Data.HealthPoints > 5)
                                        {
                                            usernameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = yellowMaterial;
                                        }
                                        else
                                        {
                                            usernameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = redMaterial;
                                        }

                                    }
                                }


                            }

                            
                        }



                        if (controller == playerManager1.localPlayer.Controller.gameObject)
                        {
                            continue;
                        }

                        Transform? healthObject = player.Controller.transform.Find("Health");
                        Transform nametag = player.Controller.transform.Find("NameTag");

                        if (nametag != null)
                        {
                            nametag.gameObject.SetActive(false);
                        }

                        if (healthObject != null)
                        {
                            healthObject.GetChild(0).gameObject.SetActive(false);
                        }

                        enemyPlayerName = CleanUsername(player.Data.GeneralData.PublicUsername);
                        currentEnemyHealth = player.Data.HealthPoints;

                        enemyPlayer = controller.transform.GetChild(1).GetChild(0).GetChild(0);
                        enemyEquippedShiftstones = player.Data.EquipedShiftStones;
                    }
                }
            }


            if (currentScene.Contains("Map") && enemyPlayer != null)
            {
                Vector3 midpoint = (localPlayerHeadset.transform.position + enemyPlayer.position) / 2;

                Vector3 lookAtVector = (enemyPlayer.position - localPlayerHeadset.transform.position).normalized;

                Vector3 rightVector = Vector3.Cross(Vector3.up, lookAtVector).normalized;

                Vector3 localHeadsetPositionFlat = new Vector3(localPlayerHeadset.transform.position.x, localPlayerHeadset.transform.position.y, 0);
                Vector3 enemyPlayerPositionFlat = new Vector3(enemyPlayer.position.x, enemyPlayer.position.y, 0);

                float distance = Vector3.Distance(localPlayerHeadset.transform.position, enemyPlayer.position);

                Vector3 cameraPosition = midpoint + rightVector * (4 * (distance / 3));

                camera.transform.position = cameraPosition + new Vector3(0, 1f - (distance / 20), 0);

                Quaternion lookAtRotation = Quaternion.LookRotation(midpoint - camera.transform.position);//camera.transform.position - midpoint
                Vector3 lookAtEulerAngles = lookAtRotation.eulerAngles;
                lookAtEulerAngles.x = 10;
                camera.transform.rotation = Quaternion.Euler(lookAtEulerAngles);

                Vector3 positionOffset = new Vector3(midpoint.x, midpoint.y + 1.5f - (distance / 20), midpoint.z - (distance / 8f) - 4f);
                Quaternion rotationOffset = Quaternion.Euler(5, 0, 0);
                float zoomScale = Mathf.Max(2f, 1 + (distance / 2.6f));

                camera.GetComponent<Camera>().orthographicSize = zoomScale;
            }
            else
            {
                Vector3 positionOffset = new Vector3(0, 0, -4);
                Quaternion rotationOffset = Quaternion.Euler(5, 0, 0);
                float zoomScale = 2f;

                if (currentScene == "Park")
                {
                    positionOffset = new Vector3(0, 5, -7);
                    rotationOffset = Quaternion.Euler(35, 0, 0);
                    zoomScale = 3f;
                }
                else if (currentScene.Contains("Map"))
                {
                    positionOffset = new Vector3(0, 0, -7);
                    rotationOffset = Quaternion.Euler(5, 0, 0);
                    zoomScale = 3f;
                }


                camera.transform.position = localPlayerHeadset.transform.position + positionOffset;
                camera.transform.rotation = rotationOffset;
                camera.GetComponent<Camera>().orthographicSize = zoomScale;

                
            }



            if (localHealthBar == null || enemyHealthBar == null)
            {
                return;
            }

            if (currentPlayerHealth < lastPlayerHealth)
            {
                AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\PlayerHit.mp3", 0.5f, false);
                localScreenshakeFramesLeft = 30;
            }

            if (currentEnemyHealth < lastEnemyHealth)
            {
                AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\PlayerHit.mp3", 0.5f, false);
                enemyScreenshakeFramesLeft = 30;
            }



            if (currentPlayerHealth > 19)
            {
                localHealthBar.GetComponent<MeshRenderer>().material = blueMaterial;
            }
            else if (currentPlayerHealth >= 15)
            {
                localHealthBar.GetComponent<MeshRenderer>().material = greenMaterial;
            }
            else if (currentPlayerHealth > 5)
            {
                localHealthBar.GetComponent<MeshRenderer>().material = yellowMaterial;
            }
            else
            {
                localHealthBar.GetComponent<MeshRenderer>().material = redMaterial;
            }

            if (currentEnemyHealth > 19)
            {
                enemyHealthBar.GetComponent<MeshRenderer>().material = blueMaterial;
            }
            else if (currentEnemyHealth >= 15)
            {
                enemyHealthBar.GetComponent<MeshRenderer>().material = greenMaterial;
            }
            else if (currentEnemyHealth > 5)
            {
                enemyHealthBar.GetComponent<MeshRenderer>().material = yellowMaterial;
            }
            else
            {
                enemyHealthBar.GetComponent<MeshRenderer>().material = redMaterial;
            }




            lastPlayerHealth = currentPlayerHealth;
            lastEnemyHealth = currentEnemyHealth;

            Vector3 screenshakeOffsetLocal = new Vector3(0, 0, 0);
            Vector3 screenshakeOffsetEnemy = new Vector3(0, 0, 0);

            if (localScreenshakeFramesLeft > 0)
            {
                localScreenshakeFramesLeft--;

                screenshakeOffsetLocal = new Vector3(UnityEngine.Random.Range(-1f, 1f) / 70f, UnityEngine.Random.Range(-1f, 1f) / 70f, 0);
            }


            if (enemyScreenshakeFramesLeft > 0)
            {
                enemyScreenshakeFramesLeft--;
                screenshakeOffsetEnemy = new Vector3(UnityEngine.Random.Range(-1f, 1f) / 70f, UnityEngine.Random.Range(-1f, 1f) / 70f, 0);
            }

            if (currentScene == "Gym" || currentScene == "Park")
            {
                screenshakeOffsetEnemy = new Vector3(100, 0, 0);
            }

            if ((bool)((ModSetting)hideOwnAndEnemyHealthBar).SavedValue && currentScene.Contains("Map"))
            {
                screenshakeOffsetLocal = new Vector3(100, 0, 0);
                screenshakeOffsetEnemy = new Vector3(100, 0, 0);
            }

            if ((bool)((ModSetting)hideOwnHealthBar).SavedValue && currentScene == "Gym" || (bool)((ModSetting)hideOwnHealthBar).SavedValue && currentScene == "Park" || !(bool)((ModSetting)parkHealthBarMode).SavedValue && currentScene == "Park" || (bool)((ModSetting)hideOwnHealthBarMatch).SavedValue && currentScene.Contains("Map"))
            {
                screenshakeOffsetLocal = new Vector3(100, 0, 0);
            }

            //add screenshake if you need to move the healthbars away, if you need the enemy health bar away you can also add enemy screenshake

            localHealthBar.transform.position = camera1.transform.position + new Vector3(-0.6f * 2, 0.725f * 2, 1) + screenshakeOffsetLocal;
            localHealthBar.transform.rotation = Quaternion.Euler(0, 0, 0);

            for (int i = 0; i < 20; i++)
            {
                var cube = localHealthBar.transform.GetChild(i);

                float xOffset = (i * 1.0f) + 0.3f;
                switch (i)
                {
                    case 2:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 6:
                    case 7:
                    case 8:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 9:
                    case 10:
                    case 11:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 12:
                    case 13:
                    case 14:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 15:
                    case 16:
                    case 17:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 18:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 19:
                        xOffset = (i * 0.85f) + 0.3f;
                        break;
                }



                cube.transform.position = localHealthBar.transform.position + new Vector3(-8.85f + xOffset, 0.02f, -0.5f) / 10 + screenshakeOffsetLocal; // Centered offset
                cube.transform.rotation = localHealthBar.transform.rotation;//8.85 0.02 -1 //-11.1f - xOffset, -15.75f, 50f

                if (i < currentPlayerHealth)
                {
                    cube.GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    cube.GetComponent<Renderer>().enabled = false;
                }
            }

            enemyHealthBar.transform.position = camera1.transform.position + new Vector3(0.6f * 2, 0.725f * 2, 1) + screenshakeOffsetEnemy;
            enemyHealthBar.transform.rotation = Quaternion.Euler(0, 0, 0);

            for (int i = 0; i < 20; i++)
            {
                var cube = enemyHealthBar.transform.GetChild(i);

                float xOffset = (i * 1.0f) + 0.3f;
                switch (i)
                {
                    case 2:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 6:
                    case 7:
                    case 8:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 9:
                    case 10:
                    case 11:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 12:
                    case 13:
                    case 14:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 15:
                    case 16:
                    case 17:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 18:
                        xOffset = (i * 0.95f) + 0.3f;
                        break;
                    case 19:
                        xOffset = (i * 0.85f) + 0.3f;
                        break;
                }

                cube.transform.position = enemyHealthBar.transform.position + new Vector3(8.85f - xOffset, 0.02f, -0.5f) / 10 + screenshakeOffsetEnemy; // Centered offset
                cube.transform.rotation = enemyHealthBar.transform.rotation;

                if (i < currentEnemyHealth)
                {
                    cube.GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    cube.GetComponent<Renderer>().enabled = false;
                }
            }


            var localPlayerNameObject = newParent.transform.Find("LocalPlayerName");
            var enemyPlayerNameObject = newParent.transform.Find("EnemyPlayerName");

            localPlayerNameObject.GetComponent<TextMeshPro>().text = CleanUsername(playerManager.localPlayer.Data.GeneralData.PublicUsername);

            localPlayerNameObject.transform.position = camera1.transform.position + new Vector3(-0.85f * 2, 0.83f * 2, 1) + screenshakeOffsetLocal;
            
            enemyPlayerNameObject.GetComponent<TextMeshPro>().text = enemyPlayerName;

            enemyPlayerNameObject.transform.position = camera1.transform.position + new Vector3(0.85f * 2, 0.83f * 2, 1) + screenshakeOffsetEnemy;

            float offset = 0.015f;

            for (int i = 0; i < localPlayerNameObject.childCount; i++)
            {
                Transform localChild = localPlayerNameObject.GetChild(i);
                Transform enemyChild = enemyPlayerNameObject.GetChild(i);

                Vector3 localParentPosition = localPlayerNameObject.position;
                Vector3 enemyParentPosition = enemyPlayerNameObject.position;

                switch (i)
                {
                    case 0:
                        localChild.position = localParentPosition + new Vector3(-offset, offset, 0.1f);
                        enemyChild.position = enemyParentPosition + new Vector3(-offset, offset, 0.1f);
                        break; // Top-left
                    case 1:
                        localChild.position = localParentPosition + new Vector3(0, offset, 0.1f);
                        enemyChild.position = enemyParentPosition + new Vector3(0, offset, 0.1f);
                        break; // Top
                    case 2:
                        localChild.position = localParentPosition + new Vector3(offset, offset, 0.1f);
                        enemyChild.position = enemyParentPosition + new Vector3(offset, offset, 0.1f);
                        break; // Top-right
                    case 3:
                        localChild.position = localParentPosition + new Vector3(-offset, 0, 0.1f);
                        enemyChild.position = enemyParentPosition + new Vector3(-offset, 0, 0.1f);
                        break; // Left
                    case 4:
                        localChild.position = localParentPosition + new Vector3(offset, 0, 0.1f);
                        enemyChild.position = enemyParentPosition + new Vector3(offset, 0, 0.1f);
                        break; // Right
                    case 5:
                        localChild.position = localParentPosition + new Vector3(-offset, -offset, 0.1f);
                        enemyChild.position = enemyParentPosition + new Vector3(-offset, -offset, 0.1f);
                        break; // Bottom-left
                    case 6:
                        localChild.position = localParentPosition + new Vector3(0, -offset, 0.1f);
                        enemyChild.position = enemyParentPosition + new Vector3(0, -offset, 0.1f);
                        break; // Bottom
                    case 7:
                        localChild.position = localParentPosition + new Vector3(offset, -offset, 0.1f);
                        enemyChild.position = enemyParentPosition + new Vector3(offset, -offset, 0.1f);
                        break; // Bottom-right
                }

                localChild.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Left;
                enemyChild.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Right;

                localChild.GetComponent<TextMeshPro>().text = localPlayerNameObject.GetComponent<TextMeshPro>().text;
                enemyChild.GetComponent<TextMeshPro>().text = enemyPlayerNameObject.GetComponent<TextMeshPro>().text;

            }



            for (int i = 0; i < 8; i++)
            {
                if (playerManager.localPlayer.Data.EquipedShiftStones[0] == i)
                {
                    localShiftstones[i].transform.position = new Vector3(297.95f, 1.2f, 1) + screenshakeOffsetLocal;
                    localShiftstones[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (playerManager.localPlayer.Data.EquipedShiftStones[1] != i)
                {
                    localShiftstones[i].transform.position = new Vector3(280, 0, 0);
                }

                if (playerManager.localPlayer.Data.EquipedShiftStones[1] == i)
                {
                    if (playerManager.localPlayer.Data.EquipedShiftStones[0] == -1)//left hand doesnt have a shiftstone, move the stone to the left
                    {
                        localShiftstones[i].transform.position = new Vector3(297.95f, 1.2f, 1) + screenshakeOffsetLocal;
                        localShiftstones[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        localShiftstones[i].transform.position = new Vector3(298.175f, 1.2f, 1) + screenshakeOffsetLocal;
                        localShiftstones[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    
                }
                else if(playerManager.localPlayer.Data.EquipedShiftStones[0] != i)
                {
                    localShiftstones[i].transform.position = new Vector3(280, 0, 0);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if (enemyEquippedShiftstones != null && enemyEquippedShiftstones[0] == i)
                {
                    enemyShiftstones[i].transform.position = new Vector3(302.05f, 1.2f, 1) + screenshakeOffsetEnemy;
                    enemyShiftstones[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (enemyEquippedShiftstones == null || enemyEquippedShiftstones[1] != i)
                {
                    enemyShiftstones[i].transform.position = new Vector3(280, 0, 0);
                }

                if (enemyEquippedShiftstones != null && enemyEquippedShiftstones[1] == i)
                {
                    if (enemyEquippedShiftstones[0] == -1)//left hand doesnt have a shiftstone, move the stone to the left
                    {
                        enemyShiftstones[i].transform.position = new Vector3(302.05f, 1.2f, 1) + screenshakeOffsetEnemy;
                        enemyShiftstones[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        enemyShiftstones[i].transform.position = new Vector3(301.825f, 1.2f, 1) + screenshakeOffsetEnemy;
                        enemyShiftstones[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                    }

                }
                else if (enemyEquippedShiftstones == null || enemyEquippedShiftstones[0] != i)
                {
                    enemyShiftstones[i].transform.position = new Vector3(280, 0, 0);
                }
            }




            var poolsParent = Calls.Pools.Structures.GetPoolCube().transform.parent;

            int poolChildren = poolsParent.transform.childCount;


            for (int i = 0; i < poolChildren; ++i)
            {
                Transform poolsChild = poolsParent.transform.GetChild(i);
                int vfxChildren = poolsChild.childCount;

                int playedSounds = 0;

                for (int j = 0; j < vfxChildren; ++j)
                {
                    Transform vfxChild = poolsChild.GetChild(j);
                    GameObject vfxObject = vfxChild.gameObject;

                    bool isActive = vfxObject.activeSelf;

                    if (!lastFrameActiveStates.ContainsKey(vfxObject))
                    {
                        lastFrameActiveStates[vfxObject] = false;
                    }

                    bool wasActiveLastFrame = lastFrameActiveStates[vfxObject];
                    bool isCurrentlyTracked = activeVFXClips.ContainsKey(vfxObject);

                    string vfxName = vfxObject.name;

                    if (isActive && !wasActiveLastFrame && (bool)((ModSetting)enable8BitSounds).SavedValue)
                    {
                        if (playedSounds < 1)
                        {
                            var clipData = AudioManager.PlayAudioForVFX(vfxName);
                            if (clipData != null)
                            {
                                activeSounds.Add(clipData);
                                activeVFXClips[vfxObject] = clipData;
                                playedSounds++;
                            }
                        }
                        else
                        {
                            activeVFXClips[vfxObject] = new AudioManager.ClipData
                            {
                                WaveOut = null,
                                VolumeProvider = null
                            };
                        }
                    }

                    if (!isActive && wasActiveLastFrame || !(bool)((ModSetting)enable8BitSounds).SavedValue)
                    {
                        if (isCurrentlyTracked)
                        {
                            activeSounds.Remove(activeVFXClips[vfxObject]);
                            AudioManager.StopPlayback(activeVFXClips[vfxObject]);
                            activeVFXClips.Remove(vfxObject);
                        }
                    }

                    lastFrameActiveStates[vfxObject] = isActive;
                }


                if (rightJoystickClick.ReadValue<float>() == 1f && leftJoystickClick.ReadValue<float>() == 1f && !joysticksPressed)
                {
                    joysticksPressed = true;

                    Transform vrObject = cameraHolder.transform.GetChild(1);

                    Vector3 targetHeadsetPosition = new Vector3(100, -14.8f, -1.1f);
                    float targetHeadsetRotationY = 0f;

                    Vector3 headsetPosition = headset.transform.position;
                    Quaternion headsetRotation = headset.transform.rotation;

                    Vector3 vrPosition = vrObject.position;
                    Quaternion vrRotation = vrObject.rotation;

                    float adjustedX = vrPosition.x + (targetHeadsetPosition.x - headsetPosition.x);
                    float adjustedZ = vrPosition.z + (targetHeadsetPosition.z - headsetPosition.z);

                    vrPosition.x = adjustedX;
                    vrPosition.z = adjustedZ;
                    vrPosition.y = targetHeadsetPosition.y;

                    vrObject.position = vrPosition;

                    float headsetDeltaRotationY = Mathf.DeltaAngle(headsetRotation.eulerAngles.y, targetHeadsetRotationY);
                    Quaternion rotationAdjustment = Quaternion.Euler(0, headsetDeltaRotationY, 0);
                    vrObject.rotation = rotationAdjustment * vrRotation;
                }
                if (rightJoystickClick.ReadValue<float>() == 0f && leftJoystickClick.ReadValue<float>() == 0f && joysticksPressed)
                {
                    joysticksPressed = false;

                    Transform vrObject = cameraHolder.transform.GetChild(1);

                    Vector3 targetHeadsetPosition = new Vector3(100, -14.8f, -1.1f);
                    float targetHeadsetRotationY = 0f;


                    Vector3 headsetPosition = headset.transform.position;
                    Quaternion headsetRotation = headset.transform.rotation;

                    Vector3 vrPosition = vrObject.position;
                    Quaternion vrRotation = vrObject.rotation;

                    float adjustedX = vrPosition.x + (targetHeadsetPosition.x - headsetPosition.x);
                    float adjustedZ = vrPosition.z + (targetHeadsetPosition.z - headsetPosition.z);

                    vrPosition.x = adjustedX;
                    vrPosition.z = adjustedZ;
                    vrPosition.y = targetHeadsetPosition.y;

                    vrObject.position = vrPosition;

                    float headsetDeltaRotationY = Mathf.DeltaAngle(headsetRotation.eulerAngles.y, targetHeadsetRotationY);
                    Quaternion rotationAdjustment = Quaternion.Euler(0, headsetDeltaRotationY, 0);
                    vrObject.rotation = rotationAdjustment * vrRotation;
                }



            }



            for (int i = activeStructures.Count - 1; i >= 0; i--)
            {
                GameObject structure = activeStructures[i];
                if (!structure.active)
                {
                    activeStructures.RemoveAt(i);

                    if (explodedStructures.ContainsKey(structure))
                    {
                        AudioManager.StopPlayback(explodedStructures[structure]);
                        activeSounds.Remove(explodedStructures[structure]);
                        explodedStructures.Remove(structure);
                    }

                    continue;
                }

                int childCount = structure.transform.childCount;


                for (int g = 0; g < childCount; ++g)
                {
                    Transform pooledAudioSource = structure.transform.GetChild(g);
                    if (pooledAudioSource != null && pooledAudioSource.name == "PooledAudioSource" && pooledAudioSource.gameObject.active && (bool)((ModSetting)enable8BitSounds).SavedValue)
                    {
                        pooledAudioSource.gameObject.SetActive(false);
                        var audioSource = pooledAudioSource.GetComponent<UnityEngine.AudioSource>();
                        if (audioSource != null)
                        {
                            audioSource.enabled = false;
                        }

                        var explodeStatusEffect = structure.transform.Find("ExplodeStatus_VFX");

                        if (explodeStatusEffect != null && !explodedStructures.ContainsKey(structure))
                        {
                            var clipData = AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\ExplodeStatus.mp3", 0.5f, true);
                            if (clipData != null)
                            {
                                activeSounds.Add(clipData);
                                explodedStructures[structure] = clipData;
                            }
                            
                            
                        }
                    }
                    else if (pooledAudioSource != null && pooledAudioSource.name == "PooledAudioSource" && !pooledAudioSource.gameObject.active && !(bool)((ModSetting)enable8BitSounds).SavedValue)
                    {
                        pooledAudioSource.gameObject.SetActive(true);
                        var audioSource = pooledAudioSource.GetComponent<UnityEngine.AudioSource>();
                        if (audioSource != null)
                        {
                            audioSource.enabled = true;
                        }

                        if (explodedStructures.ContainsKey(structure))
                        {
                            AudioManager.StopPlayback(explodedStructures[structure]);
                            activeSounds.Remove(explodedStructures[structure]);
                            explodedStructures.Remove(structure);
                        }
                    }
                }

            }

        }

        private IEnumerator CameraCloning()
        {

            yield return new WaitForSeconds(2f);
            

            if (!(bool)((ModSetting)modEnabledUI).SavedValue || currentScene == "Gym" && !(bool)((ModSetting)enabledInGym).SavedValue)
            {

                Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

                newParent.Find("ArcadeCamera(Clone)").gameObject.SetActive(false);
                newParent.Find("HealthBarCamera").gameObject.SetActive(false);
                newParent.Find("VersusCamera").gameObject.SetActive(false);

                var poolsParent = Calls.Pools.Structures.GetPoolCube().transform.parent;
                PoolManager poolManager = poolsParent.GetComponent<PoolManager>();
                var poolSettingsArray = poolManager.resourcesToPool;

                var soundPoolSetting = poolSettingsArray[54];

                poolsParent.GetChild(54).gameObject.SetActive(true);

                int poolChildren = poolsParent.transform.GetChild(54).childCount;

                for (int i = poolChildren - 1; i >= 0; i--)
                {
                    GameObject.Destroy(poolsParent.GetChild(54).GetChild(i).gameObject);
                }

                if (usedSnapTurnBefore)
                {

                    newParent.Find("MenuFormDDOL").gameObject.GetComponent<SettingsForm>().SetTurnMode(InputConfiguration.TurnMode.Snap, true);
                    
                    if (currentScene == "Gym")
                    {
                        Calls.GameObjects.Gym.Logic.SlabbuddyMenuVariant.MenuForm.Base.ControlsSlab.GetGameObject().transform.GetChild(0).GetChild(2).GetChild(8).GetComponent<InteractionSlider>().SetStep(0, true, false);
                    }
                }

                yield break;
            }

            if (modEnabledAtAllTimes || !currentScene.Contains("Map") || (friendQueue && currentScene.Contains("Map")) || (currentScene.Contains("Map") && Calls.Mods.doesOpponentHaveMod(Mod.ModName, Mod.ModVersion, false)))
            {
                Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

                if (newParent.Find("MenuFormDDOL").GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(8).GetComponent<InteractionSlider>().snappedStep == 0)
                {
                    usedSnapTurnBefore = true;
                }

                modEnabled = true;

                

                newParent.Find("MenuFormDDOL").gameObject.GetComponent<SettingsForm>().SetTurnMode(InputConfiguration.TurnMode.Smooth, false);//minMaxSmoothTurnSpeed 50, 250
            }

            if (!modEnabled)
            {
                var poolsParent = Calls.Pools.Structures.GetPoolCube().transform.parent;
                PoolManager poolManager = poolsParent.GetComponent<PoolManager>();
                var poolSettingsArray = poolManager.resourcesToPool;

                var soundPoolSetting = poolSettingsArray[54];

                poolsParent.GetChild(54).gameObject.SetActive(true);

                int poolChildren = poolsParent.transform.GetChild(54).childCount;

                for (int i = poolChildren - 1; i >= 0; i--)
                {
                    GameObject.Destroy(poolsParent.GetChild(54).GetChild(i).gameObject);
                }

                Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

                newParent.Find("ArcadeCamera(Clone)").gameObject.SetActive(false);
                newParent.Find("HealthBarCamera").gameObject.SetActive(false);
                newParent.Find("VersusCamera").gameObject.SetActive(false);

                if (usedSnapTurnBefore)
                {
                    newParent.Find("MenuFormDDOL").gameObject.GetComponent<SettingsForm>().SetTurnMode(InputConfiguration.TurnMode.Snap, true);
                }

                yield break;
            }

            Transform newParent1 = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            newParent1.Find("ArcadeCamera(Clone)").gameObject.SetActive(true);
            newParent1.Find("HealthBarCamera").gameObject.SetActive(true);
            newParent1.Find("VersusCamera").gameObject.SetActive(true);


            if (currentScene == "Gym")
            {
                Calls.GameObjects.Gym.Logic.Ambience.GetGameObject().SetActive(false);

                gymAmbience = AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\GymAmbience.mp3", 0.2f, true);
            }

            if (currentScene == "Park")
            {
                Calls.GameObjects.Park.Logic.Ambience.GetGameObject().SetActive(false);

                gymAmbience = AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\GymAmbience.mp3", 0.2f, true);
            }

            if (cameraHolder == null)
            {
                

                var playerManager = Calls.Managers.GetPlayerManager();
                var parentController = playerManager.localPlayer.Controller;

                cameraHolder = UnityEngine.Object.Instantiate(parentController.gameObject);
                cameraHolder.GetComponent<PlayerController>().assignedPlayer = null;
                cameraHolder.name = "ArcadeRumble_CameraHolder";


                newCamera = cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Camera>();

                cameraHolder.transform.GetChild(0).gameObject.SetActive(false);
                cameraHolder.transform.GetChild(2).gameObject.SetActive(false);
                cameraHolder.transform.GetChild(3).gameObject.SetActive(false);
                cameraHolder.transform.GetChild(4).gameObject.SetActive(false);
                cameraHolder.transform.GetChild(5).gameObject.SetActive(false);
                cameraHolder.transform.GetChild(6).gameObject.SetActive(false);
                cameraHolder.transform.GetChild(7).gameObject.SetActive(false);//9 is liv
                cameraHolder.transform.GetChild(8).gameObject.SetActive(false);

                if (livOnlyMode)
                {
                    cameraHolder.transform.GetChild(9).gameObject.SetActive(false);
                }

                GameObject leftController = cameraHolder.transform.GetChild(1).GetChild(1).gameObject;
                GameObject rightController = cameraHolder.transform.GetChild(1).GetChild(2).gameObject;
                GameObject headset = cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
                Transform vrObject = cameraHolder.transform.GetChild(1);

                headset.GetComponent<TrackedPoseDriver>().enabled = true;

                if (!livOnlyMode)
                {
                    headset.GetComponent<Camera>().enabled = true;
                }
               


                leftController.GetComponent<TrackedPoseDriver>().enabled = true;

                rightController.GetComponent<TrackedPoseDriver>().enabled = true;

                PlayerMovement playerMovement = parentController.gameObject.GetComponent<PlayerMovement>();

                ChangeHeadVisibility(true);

                if (!livOnlyMode)
                {
                    RedirectPlayerCamera(parentController.gameObject);
                }
               

                GameObject.Find("Health/Local/Player health bar").SetActive(false);

                if (currentScene != "Gym" && currentScene != "Park")
                {


                    MelonCoroutines.Start(VersusScreenActivate());
                }

                int maxFrames = 10;

                Vector3 targetHeadsetPosition = new Vector3(100, -14.8f, -1.1f);
                float targetHeadsetRotationY = 0f;

                for (int i = 0; i < maxFrames; i++)
                {

                    Vector3 headsetPosition = headset.transform.position;
                    Quaternion headsetRotation = headset.transform.rotation;

                    Vector3 vrPosition = vrObject.position;
                    Quaternion vrRotation = vrObject.rotation;

                    float adjustedX = vrPosition.x + (targetHeadsetPosition.x - headsetPosition.x);
                    float adjustedZ = vrPosition.z + (targetHeadsetPosition.z - headsetPosition.z);

                    vrPosition.x = adjustedX;
                    vrPosition.z = adjustedZ;
                    vrPosition.y = targetHeadsetPosition.y;

                    vrObject.position = vrPosition;

                    float headsetDeltaRotationY = Mathf.DeltaAngle(headsetRotation.eulerAngles.y, targetHeadsetRotationY);
                    Quaternion rotationAdjustment = Quaternion.Euler(0, headsetDeltaRotationY, 0);
                    vrObject.rotation = rotationAdjustment * vrRotation;

                    yield return null;
                }

                

                var poolsParent = Calls.Pools.Structures.GetPoolCube().transform.parent;
                PoolManager poolManager = poolsParent.GetComponent<PoolManager>();
                var poolSettingsArray = poolManager.resourcesToPool;

                var soundPoolSetting = poolSettingsArray[54];

                soundPoolSetting.ResetPoolOnSceneLoad = true;

                poolsParent.GetChild(54).gameObject.SetActive(false);
            }
        }

        public static float SineOut(float t)
        {
            return Mathf.Sin(t * Mathf.PI * 0.5f);
        }

        private IEnumerator VersusScreenMoveLine(Vector3 startVector, Vector3 endVector, GameObject copyPositionObject, float angle, float randomRange1, float randomRange2)
        {
            float i = 1;
            float frameCount = UnityEngine.Random.Range(45, 55);
            Vector3 offsetPosition = new Vector3(0, 0, 0);

            Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.3f, 0.01f, 1f);//final size is 
            cube.transform.rotation = Quaternion.Euler(0, 0, angle);
            cube.GetComponent<MeshRenderer>().material = whiteMaterial;
            cube.transform.parent = newParent;

            while (copyPositionObject != null)
            {
                if (i >= frameCount)
                {
                    i = 1;

                    frameCount = UnityEngine.Random.Range(40, 50);

                    for (int g = 0; g < UnityEngine.Random.Range(0, 5); g++)
                    {
                        yield return null;
                    }

                    if (copyPositionObject == null)
                    {
                        break;
                    }
                }

                if (i == 1)
                {
                    offsetPosition = new Vector3(UnityEngine.Random.Range(randomRange1, randomRange2) / 1f, 0, -0.05f);
                }


                var startPos = copyPositionObject.transform.position + startVector + offsetPosition;
                var endPos = copyPositionObject.transform.position + endVector + offsetPosition;

                float t = i / frameCount;

                cube.transform.position = Vector3.Lerp(startPos, endPos, t);

                i++;

                yield return null;
            }

            GameObject.Destroy(cube);
        }

        private void VersusScreenSpawnLine(Vector3 startVector, Vector3 endVector, GameObject copyPositionObject, float angle, float randomRange1, float randomRange2)
        {
            MelonCoroutines.Start(VersusScreenMoveLine(startVector, endVector, copyPositionObject, angle, randomRange1, randomRange2)); //cant fire a coroutine from another coroutine, so a middle-man function is required (why are unity and visual studio so horrible)
        }

        private IEnumerator VersusScreenMovePlayer(Vector3 startVector, Vector3 endVector, GameObject copyPositionObject, float angleY, GameObject original, float xOffset, float yOffset)
        {

            var clone = GameObject.Instantiate(original);
            clone.GetComponent<PlayerController>().assignedPlayer = null;
            clone.GetComponent<PlayerResetSystem>().enabled = false;
            if (clone.transform.Find("LIV"))
            {
                clone.transform.Find("LIV").gameObject.SetActive(false);
            }

            GameObject cloneVisuals = clone.transform.Find("Visuals").gameObject;
            GameObject playerVisuals = original.transform.Find("Visuals").gameObject;


            Transform VR = clone.transform.Find("VR").transform;
            Transform leftController1 = VR.GetChild(1).transform;
            Transform rightController1 = VR.GetChild(2).transform;
            Transform pillBody = clone.transform.Find("Physics").GetChild(0).transform;
            Transform headset1 = VR.GetChild(0).GetChild(0).transform;

            Transform VROriginal = original.transform.Find("VR").transform;
            Transform leftControllerOriginal = VROriginal.GetChild(1).transform;
            Transform rightControllerOriginal = VROriginal.GetChild(2).transform;
            Transform pillBodyOriginal = original.transform.Find("Physics").GetChild(0).transform;
            Transform headsetOriginal = VROriginal.GetChild(0).GetChild(0).transform;

            cloneVisuals.GetComponent<VRIK>().enabled = true;

            leftController1.gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
            rightController1.gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
            headset1.gameObject.GetComponent<TrackedPoseDriver>().enabled = false;




            Rigidbody[] rigidbodies = clone.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = true;
            }

            Renderer cloneRenderer = clone.GetComponent<Renderer>();
            if (cloneRenderer != null)
            {
                cloneRenderer.enabled = true;
            }

            Renderer[] cloneRenderers = clone.GetComponentsInChildren<Renderer>();
            foreach (var rend in cloneRenderers)
            {
                rend.enabled = true;
            }

            Collider cloneCollider = clone.GetComponent<Collider>();
            MeshCollider meshCloneCollider = clone.GetComponent<MeshCollider>();

            Collider[] cloneColliders = clone.GetComponentsInChildren<Collider>();
            MeshCollider[] meshCloneColliders = clone.GetComponentsInChildren<MeshCollider>();

            foreach (var rend in cloneColliders)
            {
                rend.enabled = false;
            }

            foreach (var rend in meshCloneColliders)
            {
                rend.enabled = false;
            }

            if (cloneCollider != null)
            {
                cloneCollider.enabled = false;
            }
            if (meshCloneCollider != null)
            {
                meshCloneCollider.enabled = false;
            }


            clone.transform.GetChild(2).GetChild(0).GetComponent<Renderer>().enabled = false;
            clone.transform.GetChild(3).GetChild(0).GetComponent<Renderer>().enabled = false;

            cloneVisuals.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetComponent<Renderer>().enabled = false;
            cloneVisuals.transform.GetChild(1).GetChild(0).GetChild(3).GetChild(1).GetComponent<Renderer>().enabled = false;


            GameObject headset = headsetOriginal.gameObject;
            GameObject leftController = leftControllerOriginal.gameObject;
            GameObject rightController = rightControllerOriginal.gameObject;

           

            GameObject localPlayerHeadset = headset1.gameObject;

            GameObject localPlayerLeftController = leftController1.gameObject;
            GameObject localPlayerRightController = rightController1.gameObject;



            Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            

            

            while (copyPositionObject != null)
            {
                cloneVisuals.transform.position = copyPositionObject.transform.position + new Vector3(xOffset, yOffset, -3f);
                cloneVisuals.transform.rotation = Quaternion.Euler(0, angleY, 0);

                VR.position = copyPositionObject.transform.position + new Vector3(xOffset, yOffset, -3f);
                VR.rotation = Quaternion.Euler(0, angleY, 0);


                if (headset == null || leftController == null || rightController == null) continue;

                if (localPlayerHeadset == null) continue;

                //reusing code from onupdate because its the exact same situation (old onupdate code because i switched to using smooth turn instead)

                float positionAlpha = 0.8f;
                float rotationAlpha = 0.8f;

                Vector3 headsetRelativePosition = VROriginal.transform.InverseTransformPoint(headset.transform.position);
                Quaternion headsetRelativeRotation = Quaternion.Inverse(VROriginal.transform.rotation) * headset.transform.rotation;

                Vector3 headsetRelativeEulerRotation = headsetRelativeRotation.eulerAngles;
                headsetRelativeEulerRotation.y = 0;
                headsetRelativeRotation = Quaternion.Euler(headsetRelativeEulerRotation);

                if (headset1 != null)
                {
                    Vector3 targetLeftPosition = VR.transform.TransformPoint(headsetRelativePosition) + new Vector3(0, 0, 0);
                    Quaternion targetLeftRotation = VR.transform.rotation * headsetRelativeRotation;

                    headset1.transform.position = Vector3.Lerp(
                        headset1.transform.position,
                        targetLeftPosition,
                        positionAlpha
                    );

                    headset1.transform.rotation = Quaternion.Slerp(
                        headset1.transform.rotation,
                        targetLeftRotation,
                        rotationAlpha
                    );
                }

                Vector3 leftControllerRelativePosition = headset.transform.InverseTransformPoint(leftController.transform.position);
                Vector3 rightControllerRelativePosition = headset.transform.InverseTransformPoint(rightController.transform.position);



                Quaternion leftControllerRelativeRotation = Quaternion.Inverse(headset.transform.rotation) * leftController.transform.rotation;
                Quaternion rightControllerRelativeRotation = Quaternion.Inverse(headset.transform.rotation) * rightController.transform.rotation;

                if (localPlayerLeftController != null)
                {
                    Vector3 targetLeftPosition = localPlayerHeadset.transform.TransformPoint(leftControllerRelativePosition) + new Vector3(0, 0, 0);
                    Quaternion targetLeftRotation = localPlayerHeadset.transform.rotation * leftControllerRelativeRotation * Quaternion.Euler(0, 0, -0);

                    localPlayerLeftController.transform.position = Vector3.Lerp(
                        localPlayerLeftController.transform.position,
                        targetLeftPosition,
                        positionAlpha
                    );

                    localPlayerLeftController.transform.rotation = Quaternion.Slerp(
                        localPlayerLeftController.transform.rotation,
                        targetLeftRotation,
                        rotationAlpha
                    );
                }


                if (localPlayerRightController != null)
                {
                    Vector3 targetRightPosition = localPlayerHeadset.transform.TransformPoint(rightControllerRelativePosition) + new Vector3(0, 0, 0);
                    Quaternion targetRightRotation = localPlayerHeadset.transform.rotation * rightControllerRelativeRotation * Quaternion.Euler(0, 0, 0);

                    localPlayerRightController.transform.position = Vector3.Lerp(
                        localPlayerRightController.transform.position,
                        targetRightPosition,
                        positionAlpha
                    );

                    localPlayerRightController.transform.rotation = Quaternion.Slerp(
                        localPlayerRightController.transform.rotation,
                        targetRightRotation,
                        rotationAlpha
                    );
                }

                yield return null;
            }

            //dont destroy because it breaks health
        }

        private void VersusScreenSpawnPlayer(Vector3 startVector, Vector3 endVector, GameObject copyPositionObject, float angleY, GameObject controller, float xOffset, float yOffset)
        {
            MelonCoroutines.Start(VersusScreenMovePlayer(startVector, endVector, copyPositionObject, angleY, controller, xOffset, yOffset)); //cant fire a coroutine from another coroutine, so a middle-man function is required (why are unity and visual studio so horrible)
        }

        private IEnumerator VersusScreenActivate()
        {;

            AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\VersusTheme.mp3", 0.5f, false);

            Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            newParent.Find("VersusScreen").GetComponent<Renderer>().enabled = true;

            var angle = 75f;
            var angleRad = (90 - angle) * Mathf.Deg2Rad;

            var sine = Mathf.Sin(angleRad);
            var cosine = Mathf.Cos(angleRad);

            var hypotenuse = 5f;
            var hypotenuse1 = 3.5f;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(5f, 0.025f, 1f);//final size is 
            cube.transform.position = new Vector3(600.005f, 1f, 2f);
            cube.transform.rotation = Quaternion.Euler(0, 0, angle);
            cube.GetComponent<MeshRenderer>().material = whiteMaterial;
            cube.transform.parent = newParent;


            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.transform.localScale = new Vector3(5f, 0.025f, 1f);//final size is 
            cube1.transform.position = new Vector3(600.03f, 1f, 2f);
            cube1.transform.rotation = Quaternion.Euler(0, 0, angle);
            cube1.GetComponent<MeshRenderer>().material = whiteMaterial;
            cube1.transform.parent = newParent;


            GameObject leftSide = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftSide.transform.localScale = new Vector3(5f, 5f, 1f);//final size is 
            leftSide.transform.position = new Vector3(597.525f, 1.4f, 3f);
            leftSide.transform.rotation = Quaternion.Euler(0, 0, angle);
            leftSide.GetComponent<MeshRenderer>().material = versusBlueMaterial;
            leftSide.transform.parent = newParent;

            GameObject rightSide = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightSide.transform.localScale = new Vector3(5f, 5f, 1f);//final size is 
            rightSide.transform.position = new Vector3(602.5f, 0.6f, 3f);
            rightSide.transform.rotation = Quaternion.Euler(0, 0, angle);
            rightSide.GetComponent<MeshRenderer>().material = versusRedMaterial;
            rightSide.transform.parent = newParent;

            float frameCount = 30;

            var startPos1 = cube.transform.position + new Vector3(-hypotenuse * sine, -hypotenuse * cosine, 0);
            var startPos2 = cube1.transform.position + new Vector3(hypotenuse * sine, hypotenuse * cosine, 0);
            var startPos3 = leftSide.transform.position + new Vector3(-hypotenuse * sine, -hypotenuse * cosine, 0);
            var startPos4 = rightSide.transform.position + new Vector3(hypotenuse * sine, hypotenuse * cosine, 0);

            var endPos1 = cube.transform.position;
            var endPos2 = cube1.transform.position;
            var endPos3 = leftSide.transform.position;
            var endPos4 = rightSide.transform.position;





            var enemyPlayerName = CleanUsername(Calls.Managers.GetPlayerManager().localPlayer.Data.GeneralData.PublicUsername);
            var enemyPlayerBP = Calls.Managers.GetPlayerManager().localPlayer.Data.GeneralData.BattlePoints;
            GameObject enemyPlayer1 = Calls.Managers.GetPlayerManager().localPlayer.Controller.gameObject;

            var playerManager1 = Calls.Managers.GetPlayerManager();

            if (playerManager1 != null && playerManager1.AllPlayers != null)
            {
                for (int index = 0; index < playerManager1.AllPlayers.Count; index++)
                {
                    var player = playerManager1.AllPlayers[index];
                    if (player != null && player.Controller != null && player.Controller.name == "Player Controller(Clone)")
                    {
                        GameObject controller = player.Controller.gameObject;
                        enemyPlayerName = CleanUsername(player.Data.GeneralData.PublicUsername);

                        if (controller == playerManager1.localPlayer.Controller.gameObject)
                        {
                            continue;
                        }

                        enemyPlayerBP = player.Data.GeneralData.BattlePoints;
                        enemyPlayer1 = controller;
                        break;
                    }
                }
            }



            VersusScreenSpawnPlayer(new Vector3(-hypotenuse * sine, -hypotenuse * cosine, 0), new Vector3(hypotenuse * sine, hypotenuse * cosine, 0), leftSide, 130, Calls.Managers.GetPlayerManager().localPlayer.Controller.gameObject, 1.55f, -1.8f);
            VersusScreenSpawnPlayer(new Vector3(hypotenuse * sine, hypotenuse * cosine, 0), new Vector3(-hypotenuse * sine, -hypotenuse * cosine, 0), rightSide, 230, enemyPlayer1, -1f, -1f);

            for (float i = 1; i <= frameCount; i++)
            {
                
                float t = SineOut(i / frameCount);

                cube.transform.position = Vector3.Lerp(startPos1, endPos1, t);
                cube1.transform.position = Vector3.Lerp(startPos2, endPos2, t);
                leftSide.transform.position = Vector3.Lerp(startPos3, endPos3, t);
                rightSide.transform.position = Vector3.Lerp(startPos4, endPos4, t);

                if (i % 2 == 0)
                {
                    VersusScreenSpawnLine(new Vector3(-hypotenuse1 * sine, -hypotenuse1 * cosine, 0), new Vector3(hypotenuse1 * sine, hypotenuse1 * cosine, 0), leftSide, angle, -1.5f, 2.25f);
                    VersusScreenSpawnLine(new Vector3(hypotenuse1 * sine, hypotenuse1 * cosine, 0), new Vector3(-hypotenuse1 * sine, -hypotenuse1 * cosine, 0), rightSide, angle, -2.25f, 1.5f);
                }

                yield return null;
            }

            var VersusV = GameObject.Instantiate(newParent.Find("TextPrefab").gameObject);
            VersusV.transform.parent = newParent;


            var VersusVTextObject = VersusV.GetComponent<TextMeshPro>();
            VersusVTextObject.text = "V";
            VersusVTextObject.fontSize = 0f;
            VersusVTextObject.color = new Color(1, 1, 1, 1);
            VersusVTextObject.outlineColor = new Color(0, 0, 0, 1f);
            VersusVTextObject.enableWordWrapping = false;
            VersusVTextObject.outlineWidth = 0f;

            var VersusS = Game.Instantiate(VersusV);
            VersusS.transform.parent = newParent;

            var VersusSTextObject = VersusS.GetComponent<TextMeshPro>();
            VersusSTextObject.text = "S";

            VersusV.transform.position = new Vector3(600.175f, 0.25f, 0.5f) + new Vector3(0.1f, 0.9f, 0);
            VersusS.transform.position = new Vector3(600.425f, -0.1f, 0.5f) + new Vector3(0.1f, 0.9f, 0);

            var localPlayerNameObject = Game.Instantiate(VersusV);
            localPlayerNameObject.transform.parent = newParent;

            var localPlayerNameTextObject = localPlayerNameObject.GetComponent<TextMeshPro>();
            localPlayerNameTextObject.text = CleanUsername(Calls.Managers.GetPlayerManager().localPlayer.Data.GeneralData.PublicUsername);
            localPlayerNameTextObject.alignment = TextAlignmentOptions.Left;

            var enemyPlayerNameObject = Game.Instantiate(VersusV);
            enemyPlayerNameObject.transform.parent = newParent;

            var enemyPlayerNameTextObject = enemyPlayerNameObject.GetComponent<TextMeshPro>();
            enemyPlayerNameTextObject.text = enemyPlayerName;
            enemyPlayerNameTextObject.alignment = TextAlignmentOptions.Right;

            localPlayerNameObject.transform.position = new Vector3(598.7f, 1f, 0.5f) + new Vector3(0.1f, 0.9f, 0);
            enemyPlayerNameObject.transform.position = new Vector3(601.3f, 1f, 0.5f) + new Vector3(0.1f, 0.9f, 0);



            var localPlayerBPObject = Game.Instantiate(VersusV);
            localPlayerBPObject.transform.parent = newParent;

            var localPlayerBPTextObject = localPlayerBPObject.GetComponent<TextMeshPro>();
            localPlayerBPTextObject.text = Calls.Managers.GetPlayerManager().localPlayer.Data.GeneralData.BattlePoints.ToString() + " BP";
            localPlayerBPTextObject.alignment = TextAlignmentOptions.Left;

            var enemyPlayerBPObject = Game.Instantiate(VersusV);
            enemyPlayerBPObject.transform.parent = newParent;

            var enemyPlayerBPTextObject = enemyPlayerBPObject.GetComponent<TextMeshPro>();
            enemyPlayerBPTextObject.text = enemyPlayerBP.ToString() + " BP";
            enemyPlayerBPTextObject.alignment = TextAlignmentOptions.Right;

            localPlayerBPObject.transform.position = new Vector3(598.7f, 0.705f, 0.5f) + new Vector3(0.1f, 0.9f, 0);
            enemyPlayerBPObject.transform.position = new Vector3(601.3f, 0.705f, 0.5f) + new Vector3(0.1f, 0.9f, 0);

            int frameCount1 = 5;

            

            for (float i = 1; i <= frameCount1; i++)
            {

                float t = SineOut(i / frameCount1);

                VersusVTextObject.fontSize = Mathf.Lerp(0, 5, t);
                VersusSTextObject.fontSize = Mathf.Lerp(0, 5, t);
                localPlayerNameTextObject.fontSize = Mathf.Lerp(0, 3, t);
                enemyPlayerNameTextObject.fontSize = Mathf.Lerp(0, 3, t);
                localPlayerBPTextObject.fontSize = Mathf.Lerp(0, 2, t);
                enemyPlayerBPTextObject.fontSize = Mathf.Lerp(0, 2, t);

                VersusV.transform.position -= new Vector3(0.02f, -0.02f, 0);
                VersusS.transform.position -= new Vector3(0.02f, -0.02f, 0);
                localPlayerNameObject.transform.position -= new Vector3(0.02f, -0.02f, 0);
                enemyPlayerNameObject.transform.position -= new Vector3(0.02f, -0.02f, 0);
                localPlayerBPObject.transform.position -= new Vector3(0.02f, -0.02f, 0);
                enemyPlayerBPObject.transform.position -= new Vector3(0.02f, -0.02f, 0);


                yield return null;
            }

            yield return new WaitForSeconds(3f);

            var startPos5 = cube.transform.position;
            var startPos6 = cube1.transform.position;
            var startPos7 = leftSide.transform.position;
            var startPos8 = rightSide.transform.position;

            var endPos5 = cube.transform.position + new Vector3(hypotenuse * sine, hypotenuse * cosine, 0);
            var endPos6 = cube1.transform.position + new Vector3(-hypotenuse * sine, -hypotenuse * cosine, 0);
            var endPos7 = leftSide.transform.position + new Vector3(hypotenuse * sine, hypotenuse * cosine, 0);
            var endPos8 = rightSide.transform.position + new Vector3(-hypotenuse * sine, -hypotenuse * cosine, 0);

            for (float i = 1; i <= frameCount1; i++)
            {

                float t = SineOut(i / frameCount1);

                VersusVTextObject.fontSize = Mathf.Lerp(5, 0, t);
                VersusSTextObject.fontSize = Mathf.Lerp(5, 0, t);
                localPlayerNameTextObject.fontSize = Mathf.Lerp(3, 0, t);
                enemyPlayerNameTextObject.fontSize = Mathf.Lerp(3, 0, t);
                localPlayerBPTextObject.fontSize = Mathf.Lerp(2, 0, t);
                enemyPlayerBPTextObject.fontSize = Mathf.Lerp(2, 0, t);

                VersusV.transform.position += new Vector3(0.02f, -0.02f, 0);
                VersusS.transform.position += new Vector3(0.02f, -0.02f, 0);
                localPlayerNameObject.transform.position += new Vector3(0.02f, -0.02f, 0);
                enemyPlayerNameObject.transform.position += new Vector3(0.02f, -0.02f, 0);
                localPlayerBPObject.transform.position += new Vector3(0.02f, -0.02f, 0);
                enemyPlayerBPObject.transform.position += new Vector3(0.02f, -0.02f, 0);


                yield return null;
            }

            for (float i = 1; i <= frameCount; i++)
            {

                float t = SineOut(i / frameCount);

                cube.transform.position = Vector3.Lerp(startPos5, endPos5, t);
                cube1.transform.position = Vector3.Lerp(startPos6, endPos6, t);
                leftSide.transform.position = Vector3.Lerp(startPos7, endPos7, t);
                rightSide.transform.position = Vector3.Lerp(startPos8, endPos8, t);

                yield return null;
            }

            GameObject.Destroy(cube);
            GameObject.Destroy(cube1);
            GameObject.Destroy(leftSide);
            GameObject.Destroy(rightSide);

            GameObject.Destroy(VersusV);
            GameObject.Destroy(VersusS);

            GameObject.Destroy(localPlayerNameObject);
            GameObject.Destroy(enemyPlayerNameObject);

            GameObject.Destroy(localPlayerBPObject);
            GameObject.Destroy(enemyPlayerBPObject);

            newParent.Find("VersusScreen").GetComponent<Renderer>().enabled = false;

            yield return new WaitForSeconds(5f);

            matchAmbience = AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\ArenaTheme.mp3", Mathf.Clamp01((float)((ModSetting)matchTheme8bitVolume).SavedValue), true);
        }

        private static void RedirectPlayerCamera(GameObject parentGO)
        {
            try
            {
                Transform targetTransform = parentGO.transform.GetChild(1).GetChild(0).GetChild(0);

                targetTransform.GetComponent<PlayerCamera>().camera = newCamera; //assing the playercamera to the new one so that it is actually the one being used by the game instead of the original controller's one
                targetTransform.GetComponent<Camera>().enabled = false;

                Transform leftController = parentGO.transform.GetChild(1).GetChild(1);
                Transform rightController = parentGO.transform.GetChild(1).GetChild(2);

                parentGO.transform.Find("LIV").gameObject.SetActive(false);

            }
            catch (Exception ex)
            {  
            }
        }

        private static void CreateMaterial(out Material material, Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();

            material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
            {
                mainTexture = texture
            };

            Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            cube.transform.position = new Vector3(200, 0, 0);
            cube.GetComponent<MeshRenderer>().material = material;
            cube.transform.parent = newParent;
        }

        public static void CreateHealthBar(bool isLocal, Transform? assignedParent = null)
        {
            int divider = 10;
            int divider1 = 10;
            int divider2 = 10;
            int amountOfCubes = 20;

            if (assignedParent != null)
            {
                divider = 30;
                divider1 = 33;
                divider2 = 33;
                amountOfCubes = 10;
            }

            Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            GameObject healthBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            healthBar.transform.localScale = new Vector3(18.35f, 2f, 1f) / divider;
            healthBar.transform.position = new Vector3(2.8f + 800, -1, -1.05f);
            healthBar.name = isLocal ? "LocalHealthBar" : "EnemyHealthBar";


            MeshRenderer healthBarRenderer = healthBar.GetComponent<MeshRenderer>();
            healthBarRenderer.material = greenMaterial;


            for (int i = 0; i < amountOfCubes; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(0.7f, 1.2f, 1f) / divider1;

                float xOffset = (i * 1.0f) + 0.3f; // spacing + 0.3 offset
                cube.transform.position = new Vector3(2.8f + 800, -1, -3.5f) + new Vector3(-9.75f + xOffset + 800, 0f, -0.01f) / divider2;

                if (assignedParent != null)
                {
                    cube.transform.localScale = new Vector3(1f, 1.2f, 1f) / divider1;

                    xOffset = (i * 2.0f) + 0.4f;
                    cube.transform.position = new Vector3(2.8f + 800, -1, -3.5f) + new Vector3(-9.75f + xOffset, 0f, -0.01f) / divider2;
                }

                Texture2D whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();

                Material whiteMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                {
                    mainTexture = whiteTexture
                };

                MeshRenderer cubeRenderer = cube.GetComponent<MeshRenderer>();
                cubeRenderer.material = whiteMaterial;

                cube.transform.parent = healthBar.transform;
            }

            healthBar.transform.parent = newParent;

            var playerGameObject = GameObject.Instantiate(newParent.Find("TextPrefab").gameObject);
            playerGameObject.transform.position = new Vector3(800, 0, 0);
            playerGameObject.transform.parent = newParent;
            

            var playerComponent = playerGameObject.GetComponent<TextMeshPro>();
            playerComponent.text = "name";
            playerComponent.fontSize = 3f;
            playerComponent.color = new Color(1, 1, 1, 1);
            playerComponent.outlineColor = new Color(0, 0, 0, 1f);
            playerComponent.enableWordWrapping = false;
            playerComponent.outlineWidth = 0.3f;

            for (int i = 0; i < 8; i++)
            {
                var playerGameObject1 = GameObject.Instantiate(newParent.Find("TextPrefab").gameObject);
                playerGameObject1.transform.position = new Vector3(800, 0, 0);
                playerGameObject1.transform.parent = playerGameObject.transform;


                var playerComponent1 = playerGameObject1.GetComponent<TextMeshPro>();
                playerComponent1.text = Convert.ToString(i);
                playerComponent1.fontSize = 3f;
                playerComponent1.color = new Color(0, 0, 0, 1);
                playerComponent1.outlineColor = new Color(0, 0, 0, 1f);
                playerComponent1.enableWordWrapping = false;
                playerComponent1.outlineWidth = 0f;
            }

            if (isLocal)
            {
                playerGameObject.name = "LocalPlayerName";
                playerComponent.alignment = TextAlignmentOptions.Left;
                localHealthBar = healthBar;
            }
            else
            {
                if (assignedParent == null)
                {
                    playerGameObject.name = "EnemyPlayerName";
                    playerComponent.alignment = TextAlignmentOptions.Right;
                    enemyHealthBar = healthBar;
                }
                else
                {
                    GameObject.Destroy(playerGameObject);
                    healthBar.name = "AttachedHealthBar";
                    healthBar.transform.parent = assignedParent;
                }
            }
        }

        public static void CreateShiftstoneImage(Material shiftstoneMaterialPrefab, string shiftstoneName, bool isLocal)
        {
            Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            cube.transform.position = new Vector3(295f, 0, 0);

            cube.GetComponent<Renderer>().material = GameObject.Instantiate(shiftstoneMaterialPrefab);
            cube.name = shiftstoneName;            

            cube.transform.parent = newParent;

            if (isLocal)
            {
                localShiftstones.Add(cube);
            }
            else
            {
                enemyShiftstones.Add(cube);
            }
        }

        public static void ChangeHeadVisibility(bool state)
        {
            var controller = Calls.Managers.GetPlayerManager().localPlayer.Controller.gameObject;
            Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            if (state)
            {
                controller.GetComponent<PlayerController>().controllerType = Il2CppRUMBLE.Players.ControllerType.Remote;

                newParent.Find("DressingRoomDDOL").GetComponent<Il2CppRUMBLE.CharacterCreation.Interactable.DressingRoom>().UpdatePlayerVisuals();

                controller.GetComponent<PlayerController>().controllerType = Il2CppRUMBLE.Players.ControllerType.Local;
            }
            else
            {
                newParent.Find("DressingRoomDDOL").GetComponent<Il2CppRUMBLE.CharacterCreation.Interactable.DressingRoom>().UpdatePlayerVisuals();
            }
        }

        public void LoadScreenAsset()
        {
            using (System.IO.Stream bundleStream = MelonAssembly.Assembly.GetManifestResourceStream("ArcadeRumble.AssetBundles.cameratest"))
            {
                byte[] bundleBytes = new byte[bundleStream.Length];
                bundleStream.Read(bundleBytes, 0, bundleBytes.Length);
                UnityEngine.Il2CppAssetBundle bundle = UnityEngine.Il2CppAssetBundleManager.LoadFromMemory(bundleBytes);

                if (bundle == null)
                {
                    return;
                }

                Transform newParent = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

                var textPrefab = GameObject.Instantiate(Calls.GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard.PlayerTags.HighscoreTag0.Nr.GetGameObject());
                textPrefab.name = "TextPrefab";
                textPrefab.transform.position = new Vector3(200, 0, 0);
                textPrefab.transform.parent = newParent;

                CreateMaterial(out greenMaterial, new Color(0f, 0.65f, 0f, 1f));
                CreateMaterial(out yellowMaterial, new Color(0.65f, 0.65f, 0, 1f));
                CreateMaterial(out redMaterial, new Color(0.65f, 0, 0, 1f));
                CreateMaterial(out blueMaterial, new Color(0.3f, 0.35f, 0.45f, 1f));
                CreateMaterial(out whiteMaterial, new Color(1f, 1f, 1f, 1f));
                CreateMaterial(out versusBlueMaterial, new Color(0f, 0f, 1f, 1f));
                CreateMaterial(out versusRedMaterial, new Color(1f, 0f, 0f, 1f));

                CreateHealthBar(true);
                CreateHealthBar(false);


                RenderTexture renderTexture = new RenderTexture(215, 170, 32, RenderTextureFormat.ARGB32)//165 120
                {
                    dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
                    antiAliasing = 1,
                    useMipMap = false,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point,
                    graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm,
                    depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt
                };
                renderTexture.Create();

                RenderTexture renderTextureHealth = new RenderTexture(516, 408, 32, RenderTextureFormat.ARGB32)//165 120
                {
                    dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
                    antiAliasing = 1,
                    useMipMap = false,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point,
                    graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm,
                    depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt
                };
                renderTextureHealth.Create();

                RenderTexture renderTextureVersus = new RenderTexture(430, 340, 32, RenderTextureFormat.ARGB32)//165 120
                {
                    dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
                    antiAliasing = 1,
                    useMipMap = false,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point,
                    graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm,
                    depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt
                };
                renderTextureVersus.Create();


                var cameraPrefab = bundle.LoadAsset<GameObject>("ArcadeCamera");
                if (cameraPrefab == null)
                {
                    MelonLogger.Error("Failed to load ArcadeCamera.");
                    return;
                }

                var cameraPrefab1 = bundle.LoadAsset<GameObject>("HeathBarCamera");
                if (cameraPrefab1 == null)
                {
                    MelonLogger.Error("Failed to load HeathBarCamera.");
                    return;
                }

                var flowPrefab = bundle.LoadAsset<Material>("flow");
                if (flowPrefab == null)
                {
                    MelonLogger.Error("Failed to load shifstone texture.");
                    return;
                }

                var volatilePrefab = bundle.LoadAsset<Material>("volatile");
                if (volatilePrefab == null)
                {
                    MelonLogger.Error("Failed to load shifstone texture.");
                    return;
                }

                var chargePrefab = bundle.LoadAsset<Material>("charge");
                if (chargePrefab == null)
                {
                    MelonLogger.Error("Failed to load shifstone texture.");
                    return;
                }

                var guardPrefab = bundle.LoadAsset<Material>("guard");
                if (guardPrefab == null)
                {
                    MelonLogger.Error("Failed to load shifstone texture.");
                    return;
                }

                var surgePrefab = bundle.LoadAsset<Material>("surge");
                if (surgePrefab == null)
                {
                    MelonLogger.Error("Failed to load shifstone texture.");
                    return;
                }

                var adamantPrefab = bundle.LoadAsset<Material>("adamant");
                if (adamantPrefab == null)
                {
                    MelonLogger.Error("Failed to load shifstone texture.");
                    return;
                }

                var stubbornPrefab = bundle.LoadAsset<Material>("stubborn");
                if (stubbornPrefab == null)
                {
                    MelonLogger.Error("Failed to load shifstone texture.");
                    return;
                }

                var vigorPrefab = bundle.LoadAsset<Material>("vigor");
                if (vigorPrefab == null)
                {
                    MelonLogger.Error("Failed to load shifstone texture.");
                    return;
                }

                //following the ingame index list, 0 is adamant, volatile is 7, -1 is none
                CreateShiftstoneImage(adamantPrefab, "AdamantStoneImage", true);
                CreateShiftstoneImage(chargePrefab, "ChargeStoneImage", true);
                CreateShiftstoneImage(flowPrefab, "FlowStoneImage", true);
                CreateShiftstoneImage(guardPrefab, "GuardStoneImage", true);
                CreateShiftstoneImage(stubbornPrefab, "StubbornStoneImage", true);
                CreateShiftstoneImage(surgePrefab, "SurgeStoneImage", true);
                CreateShiftstoneImage(vigorPrefab, "VigorStoneImage", true);
                CreateShiftstoneImage(volatilePrefab, "VolatileStoneImage", true);




                CreateShiftstoneImage(adamantPrefab, "AdamantStoneImage", false);
                CreateShiftstoneImage(chargePrefab, "ChargeStoneImage", false);
                CreateShiftstoneImage(flowPrefab, "FlowStoneImage", false);
                CreateShiftstoneImage(guardPrefab, "GuardStoneImage", false);
                CreateShiftstoneImage(stubbornPrefab, "StubbornStoneImage", false);
                CreateShiftstoneImage(surgePrefab, "SurgeStoneImage", false);
                CreateShiftstoneImage(vigorPrefab, "VigorStoneImage", false);
                CreateShiftstoneImage(volatilePrefab, "VolatileStoneImage", false);

                var camera = GameObject.Instantiate(cameraPrefab);
                camera.transform.position = new Vector3(0, 2, -5);
                camera.gameObject.GetComponent<Camera>().targetTexture = renderTexture;
                camera.gameObject.GetComponent<Camera>().orthographic = true;
                camera.gameObject.GetComponent<Camera>().orthographicSize = 3f;
                camera.gameObject.GetComponent<Camera>().nearClipPlane = 0;


                camera.transform.parent = newParent;

                var camera1 = GameObject.Instantiate(cameraPrefab1);
                camera1.name = "HealthBarCamera";
                camera1.transform.position = new Vector3(300, 0, 0);
                camera1.gameObject.GetComponent<Camera>().targetTexture = renderTextureHealth;
                camera1.gameObject.GetComponent<Camera>().orthographic = true;
                camera1.gameObject.GetComponent<Camera>().orthographicSize = 2f;
                camera1.gameObject.GetComponent<Camera>().nearClipPlane = 0;
                camera1.gameObject.GetComponent<Camera>().farClipPlane = 10;


                camera1.transform.parent = newParent;


                var camera2 = GameObject.Instantiate(cameraPrefab1);
                camera2.name = "VersusCamera";
                camera2.transform.position = new Vector3(600, 1, 0);
                camera2.gameObject.GetComponent<Camera>().targetTexture = renderTextureVersus;
                camera2.gameObject.GetComponent<Camera>().orthographic = true;
                camera2.gameObject.GetComponent<Camera>().orthographicSize = 1.5f;
                camera2.gameObject.GetComponent<Camera>().nearClipPlane = -10;
                camera2.gameObject.GetComponent<Camera>().farClipPlane = 30;


                camera2.transform.parent = newParent;

                var machinePrefab = bundle.LoadAsset<GameObject>("machine1 Variant");
                if (machinePrefab == null)
                {
                    return;
                }

                var machine = GameObject.Instantiate(machinePrefab);
    
                machine.transform.position = new Vector3(100, -15, 0);
                machine.transform.rotation = Quaternion.Euler(0, 180, 0);
                machine.transform.localScale = new Vector3(20, 20, 20);


                machine.transform.parent = newParent;

                machine.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.UseProxyVolume;

                Vector3 offset = new Vector3(0, 0, 0);
                Vector3 offset1 = new Vector3(0, 1.51f, -0.09f);

                float machineYRotation = machine.transform.eulerAngles.y;

                newParent.transform.Find("InvertedSphereCloneArcadeRumble").transform.position = new Vector3(100, -15f, 0);
                newParent.transform.Find("InvertedSphereCloneArcadeRumble").gameObject.SetActive(true);

                var screenGlassPrefab = bundle.LoadAsset<GameObject>("screen (1) Variant");
                if (screenGlassPrefab == null)
                {
                    MelonLogger.Error("Failed to load MachineFinal.");
                    return;
                }

                var screenMaterialPrefab = bundle.LoadAsset<Material>("HealthBarScreenMaterial");
                if (screenMaterialPrefab == null)
                {
                    MelonLogger.Error("Failed to load MachineFinal.");
                    return;
                }

                var screenGlass = GameObject.Instantiate(screenGlassPrefab);

                screenGlass.transform.position = machine.transform.position + offset1;
                screenGlass.transform.localScale = new Vector3(20, 20, 20);
                screenGlass.transform.rotation = Quaternion.Euler(330, machineYRotation, 0);

                Renderer renderer = screenGlass.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = GameObject.Instantiate(screenMaterialPrefab);
                    Material mat = renderer.material;
                    mat.mainTexture = renderTexture;
                    mat.color = new Color(1.1f, 1.3f, 2f, 1.2f);
                }

                screenGlass.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

                screenGlass.transform.parent = newParent;


                var screenGlass1 = GameObject.Instantiate(screenGlassPrefab);
                screenGlass1.name = "HealthBarScreen";

                screenGlass1.transform.position = machine.transform.position + offset1 + new Vector3(0, 0.005f, -0.001f);
                screenGlass1.transform.localScale = new Vector3(20, 20, 20);
                screenGlass1.transform.rotation = Quaternion.Euler(330, machineYRotation, 0);

                Renderer renderer1 = screenGlass1.GetComponent<Renderer>();
                if (renderer1 != null)
                {
                    Material mat = renderer1.material;
                    mat.mainTexture = renderTextureHealth;
                    mat.color = new Color(1.1f, 1.3f, 2f, 1.2f);
                }

                screenGlass1.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

                screenGlass1.transform.parent = newParent;


                var screenGlass2 = GameObject.Instantiate(screenGlassPrefab);
                screenGlass2.name = "VersusScreen";

                screenGlass2.transform.position = machine.transform.position + offset1 + new Vector3(0, 0.01f, -0.01f);
                screenGlass2.transform.localScale = new Vector3(20, 20, 20);
                screenGlass2.transform.rotation = Quaternion.Euler(330, machineYRotation, 0);

                Renderer renderer2 = screenGlass2.GetComponent<Renderer>();
                if (renderer2 != null)
                {
                    Material mat = renderer2.material;
                    mat.mainTexture = renderTextureVersus;
                    mat.color = new Color(1.1f, 1.3f, 2f, 1.2f);
                }

                renderer2.enabled = false;

                screenGlass2.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

                screenGlass2.transform.parent = newParent;

                var redButtonPrefab = bundle.LoadAsset<GameObject>("button_red 1 Variant");
                if (redButtonPrefab == null)
                {
                    MelonLogger.Error("Failed to load redButton.");
                    return;
                }

                var redButton1 = GameObject.Instantiate(redButtonPrefab);
                redButton1.name = "redButton1";

                redButton1.transform.position = machine.transform.position + new Vector3(0.07f, 1.2f, -0.44f);

                redButton1.transform.localScale = new Vector3(20, 20, 20);
                redButton1.transform.rotation = Quaternion.Euler(100, machineYRotation, 0);

                redButton1.transform.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;


                redButton1.transform.parent = newParent;



                

                var redButton = GameObject.Instantiate(redButtonPrefab);
                redButton.name = "redButton";

                redButton.transform.position = machine.transform.position + new Vector3(0.25f, 1.21f, -0.375f);
                redButton.transform.localScale = new Vector3(20, 20, 20);
                redButton.transform.rotation = Quaternion.Euler(100, machineYRotation, 0);

                redButton.transform.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

                redButton.transform.parent = newParent;

                var joystickPrefab = bundle.LoadAsset<GameObject>("joystick Variant");
                if (joystickPrefab == null)
                {
                    MelonLogger.Error("Failed to load joystickPrefab.");
                    return;
                }

                var joystick = GameObject.Instantiate(joystickPrefab);
                joystick.name = "joystick";

                joystick.transform.position = machine.transform.position + new Vector3(-0.225f, 1.2f, -0.43f);
                joystick.transform.localScale = new Vector3(20, 20, 20);
                joystick.transform.rotation = Quaternion.Euler(100, machineYRotation, 0);

                joystick.transform.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

                joystick.transform.parent = newParent;


                var leftHandPrefab = bundle.LoadAsset<GameObject>("hand (1)");
                if (leftHandPrefab == null)
                {
                    MelonLogger.Error("Failed to load leftHandPrefab.");
                    return;
                }

                var rightHandPrefab = bundle.LoadAsset<GameObject>("hand2 (1)");
                if (rightHandPrefab == null)
                {
                    MelonLogger.Error("Failed to load rightHandPrefab.");
                    return;
                }

                var leftHand = GameObject.Instantiate(leftHandPrefab);
                leftHand.name = "leftHand";

                leftHand.transform.position = new Vector3(900, 0, 0);
                leftHand.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                leftHand.transform.GetChild(0).GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                leftHand.transform.GetChild(0).GetComponent<Renderer>().material = newParent.transform.Find("YellowGhostPoseObject1").gameObject.GetComponent<Renderer>().material;

                leftHand.transform.parent = newParent;


                var rightHand = GameObject.Instantiate(rightHandPrefab);
                rightHand.name = "rightHand";

                rightHand.transform.position = new Vector3(900, 0, 0);
                rightHand.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                rightHand.transform.GetChild(0).GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                rightHand.transform.GetChild(0).GetComponent<Renderer>().material = newParent.transform.Find("YellowGhostPoseObject1").gameObject.GetComponent<Renderer>().material;

                rightHand.transform.parent = newParent;



                var menuForm = GameObject.Instantiate(Calls.GameObjects.Gym.Logic.SlabbuddyMenuVariant.MenuForm.GetGameObject());
                menuForm.name = "MenuFormDDOL";
                menuForm.transform.position = new Vector3(800, 0, 0);
                menuForm.transform.parent = newParent;


                var dressingRoom = GameObject.Instantiate(Calls.GameObjects.Gym.Scene.GymProduction.GetGameObject().transform.GetChild(5).gameObject);
                dressingRoom.name = "DressingRoomDDOL";
                dressingRoom.transform.position = new Vector3(800, 0, 0);
                dressingRoom.transform.parent = newParent;

                var phonePrefab = bundle.LoadAsset<GameObject>("phoneFinal");
                if (phonePrefab == null)
                {
                    MelonLogger.Error("Failed to load phonePrefab.");
                    return;
                }

                var phone = GameObject.Instantiate(phonePrefab);
                phone.name = "phone";

                phone.transform.position = new Vector3(900, 0, 0);
                phone.transform.localScale = new Vector3(8, 8, 8);

                phone.transform.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

                phone.transform.parent = newParent;

                assetLoaded = true;

            }
        }

        private IEnumerator LoadScreenAssetDelayed()
        {
            yield return new WaitForSeconds(1f);

            LoadScreenAsset();
        }


        private IEnumerator DisableVFX(string currentScene, bool enabled)
        {
            if (currentScene == "Gym")
            {
                while (GameObject.Find("Hand_L_Poseghost") == null)
                {
                    yield return null;
                }
            }
            else
            {
                while (Calls.GameObjects.DDOL.GameInstance.GetGameObject() == null)
                {
                    yield return null;
                }
            }

            yield return new WaitForSeconds(2f);

            var poolsParent = Calls.Pools.Structures.GetPoolCube().transform.parent;
            PoolManager poolManager = poolsParent.GetComponent<PoolManager>();


            var poolSettingsArray = poolManager.resourcesToPool;

            int poolChildren = poolsParent.transform.childCount;
            for (int i = 0; i < poolChildren; ++i)
            {

                if (i == 22 || i == 23 || i == 24 || i == 25 || i == 35)
                {
                    continue;
                }

                var child = poolsParent.transform.GetChild(i).gameObject;


                if (child.name.Contains("VFX"))
                {
                    var visualEffect = child.GetComponent<VisualEffect>();
                    if (visualEffect != null)
                    {
                        visualEffect.enabled = enabled;
                    }

                    var pooledVisualEffect = child.GetComponent<PooledVisualEffect>();
                    if (pooledVisualEffect != null)
                    {
                        pooledVisualEffect.playOnFetchPool = enabled;
                        pooledVisualEffect.enableOnFetchPool = enabled;
                    }

                    if (i < poolSettingsArray.Length)
                    {
                        var poolSetting = poolSettingsArray[i];
                        if (poolSetting != null)
                        {
                            var resource = poolSetting.Resource;

                            if (resource != null)
                            {
                                var resourceVisualEffect = resource.GetComponent<VisualEffect>();
                                if (resourceVisualEffect != null)
                                {
                                    resourceVisualEffect.enabled = enabled;
                                }

                                var resourcePooledVisualEffect = resource.GetComponent<PooledVisualEffect>();
                                if (resourcePooledVisualEffect != null)
                                {
                                    resourcePooledVisualEffect.playOnFetchPool = enabled;
                                    resourcePooledVisualEffect.enableOnFetchPool = true;
                                }
                            }
                        }
                    }
                }

                int vfxChildren = child.transform.childCount;
                for (int g = 0; g < vfxChildren; ++g)
                {
                    var grandChild = child.transform.GetChild(g).gameObject;

                    var grandChildVisualEffect = grandChild.GetComponent<VisualEffect>();
                    if (grandChildVisualEffect != null)
                    {
                        grandChildVisualEffect.enabled = enabled;
                    }

                    var grandChildPooledVisualEffect = grandChild.GetComponent<PooledVisualEffect>();
                    if (grandChildPooledVisualEffect != null)
                    {
                        grandChildPooledVisualEffect.playOnFetchPool = enabled;
                        grandChildPooledVisualEffect.enableOnFetchPool = true;
                    }
                }

                if (child.name.Contains("VFX"))
                {
                    child.SetActive(enabled);
                }
            }


        }

        private IEnumerator DisableAllMeshes()
        {
            yield return new WaitForSeconds(2f);

            string[] excludedSubstrings = {
                "rumblebee",
                "boodals",
                "bananab",
                "guib",
                "wexy",
                "stellar",
                "unity",
                "icey",
            };

            Transform newParent1 = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform;

            var playerManager2 = Calls.Managers.GetPlayerManager();

            if (playerManager2?.AllPlayers != null)
            {
                foreach (var player in playerManager2.AllPlayers)
                {
                    if (player?.Controller != null && player.Controller.name == "Player Controller(Clone)" && player.Controller != playerManager2.localPlayer.Controller)
                    {
                        string username = CleanUsername(player.Data.GeneralData.PublicUsername.ToLower());

                        // Find the first matching substring
                        string matchedSub = excludedSubstrings.FirstOrDefault(sub => username.Contains(sub));

                        if (matchedSub == null)
                        {
                            // Log the specific matched substring
                            MelonLogger.Msg($"Excluded user found: {player.Data.GeneralData.PublicUsername} matched with '{matchedSub}'");

                            Transform controller = player.Controller.transform;

                            // Disable Controller's Child 0
                            GameObject child0 = controller.GetChild(0).gameObject;
                            child0.SetActive(false);

                            // Disable VRIK and PlayerIK components
                            child0.GetComponent<VRIK>().enabled = false;
                            child0.GetComponent<PlayerIK>().enabled = false;

                            // Disable TrackedPoseDriver components
                            Transform child1 = controller.GetChild(1);
                            child1.GetChild(0).GetChild(0).GetComponent<TrackedPoseDriver>().enabled = false;
                            child1.GetChild(1).GetComponent<TrackedPoseDriver>().enabled = false;
                            child1.GetChild(2).GetComponent<TrackedPoseDriver>().enabled = false;

                            // Disable additional GameObjects
                            controller.Find("LIV").gameObject.SetActive(false);
                            controller.Find("Park").gameObject.SetActive(false);
                            controller.Find("FrontFlapPhysicsBone").gameObject.SetActive(false);
                            controller.Find("HairPhysicsBone").gameObject.SetActive(false);
                            controller.Find("Hitboxes").gameObject.SetActive(false);
                            controller.Find("Health").gameObject.SetActive(false);

                            // Disable PlayerPhysics component and Physics GameObject
                            Transform physics = controller.Find("Physics");
                            physics.GetComponent<PlayerPhysics>().enabled = false;
                            physics.gameObject.SetActive(false);
                        }
                    }
                }
            }

        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;

            if (sceneName == "Park")
            {
                MelonCoroutines.Start(DisableAllMeshes());
                MelonCoroutines.Start(DisableVFX(currentScene, false));
            }

            //enemyPlayer = null;
            

            //InputActionSetupExtensions.AddBinding(leftJoystickClick, "<XRController>{LeftHand}/joystickClicked", (string)null, (string)null, (string)null);
            //InputActionSetupExtensions.AddBinding(rightJoystickClick, "<XRController>{RightHand}/joystickClicked", (string)null, (string)null, (string)null);
            //map.Enable();

            //if (sceneName == "Gym")
            //{
            //    modEnabled = false;
            //    friendQueue = false;
            //}

            //if (sceneName == "Gym" && !assetLoaded)
            //{
            //    MelonCoroutines.Start(LoadScreenAssetDelayed());
            //}

            //if (gymAmbience != null)
            //{
            //    AudioManager.StopPlayback(gymAmbience);
            //}

            //if (matchAmbience != null)
            //{
            //    AudioManager.StopPlayback(matchAmbience);
            //}

            //if (sceneName == "Loader")
            //{
            //    MelonCoroutines.Start(YellowGhostPoseCloningCoroutine());
            //}
            //else
            //{
            //    foreach (var clipData in activeSounds)
            //    {
            //        AudioManager.StopPlayback(clipData);
            //    }

            //    activeSounds.Clear();
            //    activeVFXClips.Clear();
            //    explodedStructures.Clear();
            //    activeStructures.Clear();

            //    MelonCoroutines.Start(CameraCloning());
            //}




        }


        //public override void OnFixedUpdate()
        //{
        //    fallSoundCooldown = MathF.Max(fallSoundCooldown - Time.fixedDeltaTime, 0);

        //    if (matchFound)
        //    {

        //        int stepIndex = Calls.GameObjects.Gym.Logic.HeinhouserProducts.MatchConsole.RankRelaxControls.GetGameObject().transform.GetChild(8).gameObject.GetComponent<Il2CppRUMBLE.Interactions.InteractionBase.InteractionSlider>().snappedStep;
        //        if (stepIndex == 5)
        //        {
        //            friendQueue = true;
        //        }
        //        else
        //        {
        //            friendQueue = false;
        //        }
        //        matchFound = false;
        //    }
        //}


        //[HarmonyPatch(typeof(PlayerMovement), "SnapTurnToLookAtPosition", new Type[] { typeof(Vector3) })]
        //public class DisableSnapForward
        //{
        //    private static bool Prefix(PlayerMovement __instance, ref Vector3 lookAtPosition)
        //    {
        //        if (!modEnabled || livOnlyMode)
        //        {
        //            return true;
        //        }


        //        return false;
        //    }
        //}

        //[HarmonyPatch(typeof(PlayerMovement), "SnapTurn", new Type[] { typeof(float) })]
        //public class DisableSnapForward1
        //{
        //    private static bool Prefix(PlayerMovement __instance, ref float angle)
        //    {
        //        if (!modEnabled || livOnlyMode)
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //}

        //[HarmonyPatch(typeof(PlayerMovement), "ProcessTurnInput")]
        //public static class PatchMove2
        //{

        //    static bool Prefix(PlayerMovement __instance)
        //    {
        //        if (!modEnabled || livOnlyMode)
        //        {
        //            __instance.minMaxSmoothTurnSpeed = new Vector2(50, 250);
        //            return true;
        //        }

        //        if (__instance.currentRawTurnInput != new Vector2(1 ,0))
        //        {
        //            actualTurnInput = __instance.currentRawTurnInput;
        //        }
                
        //        __instance.currentRawTurnInput = new Vector2(1, 0);

        //        return true;
        //    }
        //}

        //[HarmonyPatch(typeof(PlayerMovement), "Move", new Type[] { typeof(Vector2) })]
        //public static class PatchMove
        //{

        //    static bool Prefix(PlayerMovement __instance, ref Vector2 input)
        //    {
        //        try
        //        {

        //            if (__instance.parentController == null || __instance.parentController.gameObject == null)
        //                return true;


        //            if (__instance.parentController.ControllerType != Il2CppRUMBLE.Players.ControllerType.Local)
        //                return true;

        //            if (cameraHolder == null || cameraHolder.transform == null || !modEnabled || livOnlyMode)
        //                return true;

        //            Transform localPlayerHeadset = __instance.parentController.transform.GetChild(1).GetChild(0).GetChild(0);
        //            Transform localVRObject = __instance.parentController.transform.GetChild(1);
        //            Transform arcadeCamera = Calls.GameObjects.DDOL.GameInstance.GetGameObject().transform.Find("ArcadeCamera(Clone)");

        //            if (localPlayerHeadset == null || arcadeCamera == null)
        //                return true;

        //            var playerMovement = __instance.parentController.gameObject.GetComponent<PlayerMovement>();
        //            Vector2 walkInput = playerMovement.currentRawWalkInput;
        //            Vector2 turnInput = actualTurnInput;//raw turn input is always (1, 0)


        //            bool isWalking = Mathf.Abs(walkInput.x) > deadzone || Mathf.Abs(walkInput.y) > deadzone;
        //            bool isTurning = Mathf.Abs(turnInput.x) > deadzone || Mathf.Abs(turnInput.y) > deadzone;


        //            Vector3 cameraForward = arcadeCamera.forward;
        //            Vector3 cameraRight = arcadeCamera.right;

        //            cameraForward.y = 0;
        //            cameraRight.y = 0;
        //            cameraForward.Normalize();
        //            cameraRight.Normalize();

        //            playerMovement.minMaxVignetteIntensity = new Vector2(0, 0);


        //            if (isWalking)
        //            {
        //                Vector3 moveDirection = (cameraForward * walkInput.y + cameraRight * walkInput.x).normalized;

        //                float sprintMultiplier = input.magnitude / Mathf.Clamp01(walkInput.magnitude);

        //                if (isTurning)
        //                {
        //                    Vector3 characterRelativeMovement = localPlayerHeadset.InverseTransformDirection(moveDirection) * sprintMultiplier;

        //                    input = new Vector2(characterRelativeMovement.x, characterRelativeMovement.z);
        //                }
        //                else
        //                {
        //                    float moveAngle = Mathf.Atan2(walkInput.x, walkInput.y) * Mathf.Rad2Deg;
        //                    Quaternion VRRotation = Quaternion.Euler(0, moveAngle + arcadeCamera.eulerAngles.y, 0);
        //                    Quaternion targetRotation = Quaternion.Euler(cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).rotation.eulerAngles.x, moveAngle + arcadeCamera.eulerAngles.y, cameraHolder.transform.GetChild(1).GetChild(0).GetChild(0).rotation.eulerAngles.z);


        //                    float currentYRotation = localPlayerHeadset.rotation.eulerAngles.y;
        //                    float targetYRotation = targetRotation.eulerAngles.y;

        //                    float angleDifference = Mathf.DeltaAngle(currentYRotation, targetYRotation);

        //                    float requiredTurnSpeed = angleDifference / Time.deltaTime;

        //                    playerMovement.minMaxSmoothTurnSpeed = new Vector2(requiredTurnSpeed, requiredTurnSpeed);


        //                    float normalizedSpeed = Mathf.Clamp01(walkInput.magnitude);
        //                    input = new Vector2(0, normalizedSpeed * sprintMultiplier); //make them always walk forward since theyre rotated in that direction already. Make sure to multiply by the spring multiplier because rumble adjusts joystick inputs instead of actual velocity
        //                }
        //            }
        //            else
        //            {
        //                input = Vector2.zero;
        //            }



        //        }
        //        catch (Exception ex)
        //        {
        //            MelonLogger.Error($"Exception during Move patch: {ex.Message}");
        //        }

        //        return true;
        //    }
        //}



        //[HarmonyPatch(typeof(Il2CppRUMBLE.Players.Subsystems.PlayerBoxInteractionSystem), "OnPlayerBoxInteraction", new Type[] { typeof(PlayerBoxInteractionTrigger), typeof(PlayerBoxInteractionTrigger) })]
        //public static class FistbumpPatch
        //{
        //    private static void Postfix(PlayerBoxInteractionTrigger first, PlayerBoxInteractionTrigger second)
        //    {
        //        //postfix triggers twice. Simple gate to make it only run once at a time
        //        if (fistBumpPrevention == false)
        //        {
        //            fistBumpPrevention = true;
        //        }
        //        else
        //        {
        //            fistBumpPrevention = false;
        //            return;
        //        }

        //        if (modEnabled == false)
        //        {
        //            return;
        //        }


        //        if (playerInteractionTimestamps.ContainsKey(first))
        //        {
        //            float previousTimestamp = playerInteractionTimestamps[first];
        //            float currentTimestamp = first.previousBoxTimestamp;

        //            if (currentTimestamp - previousTimestamp <= 0.5f)
        //            {
        //                return;
        //            }

        //            playerInteractionTimestamps[first] = currentTimestamp;
        //        }
        //        else if (first.previousBoxTimestamp > 0.1f)
        //        {
  
        //            playerInteractionTimestamps.Add(first, first.previousBoxTimestamp);
        //        }
        //        else
        //        {
        //            return;
        //        }

        //        if (playerInteractionTimestamps.ContainsKey(second))
        //        {
        //            float previousTimestamp = playerInteractionTimestamps[second];
        //            float currentTimestamp = second.previousBoxTimestamp;

        //            if (currentTimestamp - previousTimestamp <= 0.5f)
        //            {
        //                return;
        //            }

        //            playerInteractionTimestamps[second] = currentTimestamp;
        //        }
        //        else if (second.previousBoxTimestamp > 0.1f)
        //        {
        //            playerInteractionTimestamps.Add(second, second.previousBoxTimestamp);
        //        }
        //        else
        //        {
        //            return;
        //        }

        //        int randomIndex = UnityEngine.Random.Range(1, 4);
        //        string soundFilePath = $@"\ArcadeRumble\FistBump{randomIndex}.mp3";
        //        AudioManager.PlaySoundIfFileExists(soundFilePath, 1, false);


        //    }
        //}

        //[HarmonyPatch(typeof(PlayerMovement), "OnBecameGrounded")]
        //private static class PlayerMovement_OnBecameGrounded_Patch
        //{
        //    private static void Postfix(PlayerMovement __instance)
        //    {

        //        if (!modEnabled || fallSoundCooldown > 0)
        //        {
        //            return;
        //        }

        //        fallSoundCooldown = 0.2f;

        //        AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Land.mp3", 0.35f, false);
        //    }
        //}

        //[HarmonyPatch(typeof(Il2CppRUMBLE.MoveSystem.FlickModifier), "Execute", new Type[] { typeof(Il2CppRUMBLE.MoveSystem.IProcessor), typeof(Il2CppRUMBLE.MoveSystem.StackConfiguration) })]
        //public static class FlickStartEvent
        //{
        //    private static void Postfix(Il2CppRUMBLE.MoveSystem.IProcessor processor, Il2CppRUMBLE.MoveSystem.StackConfiguration config)
        //    {
        //        if (!modEnabled)
        //        {
        //            return;
        //        }

        //        AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Flick.mp3", 0.5f, false);
        //    }
        //}

        //[HarmonyPatch(typeof(Il2CppRUMBLE.MoveSystem.HoldModifier), "Execute", new Type[] { typeof(Il2CppRUMBLE.MoveSystem.IProcessor), typeof(Il2CppRUMBLE.MoveSystem.StackConfiguration) })]
        //public static class HoldStartEvent
        //{
        //    private static void Postfix(Il2CppRUMBLE.MoveSystem.HoldModifier __instance, Il2CppRUMBLE.MoveSystem.IProcessor processor, Il2CppRUMBLE.MoveSystem.StackConfiguration config)// //Il2CppRUMBLE.MoveSystem.Stack __instance,
        //    {
        //        if (!modEnabled)
        //        {
        //            return;
        //        }

        //        AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Hold.mp3");
        //    }
        //}

        //[HarmonyPatch(typeof(Il2CppRUMBLE.MoveSystem.Stack), "Execute")]
        //private static class Stack_Execute_Patch
        //{
        //    private static void Postfix(Il2CppRUMBLE.MoveSystem.Stack __instance, Il2CppRUMBLE.MoveSystem.IProcessor processor)
        //    {


        //        if (!modEnabled)
        //        {
        //            return;
        //        }

        //        switch (__instance.cachedName)
        //        {
        //            case "RockSlide":
        //                AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Dash.mp3", 0.5f, false);
        //                break;
        //            case "Jump":
        //                AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Jump.mp3", 0.5f, false);
        //                break;
        //        }



        //    }
        //}


        //[HarmonyPatch(typeof(Il2CppRUMBLE.Players.Subsystems.PlayerAudioPresence), "OnFootStepAudio", new Type[] { typeof(Il2CppRUMBLE.Players.FootStepAudioInvoker.FootStepData) })]
        //public static class FootstepAudioPlay
        //{
        //    private static void Postfix(Il2CppRUMBLE.Players.Subsystems.PlayerAudioPresence __instance,  Il2CppRUMBLE.Players.FootStepAudioInvoker.FootStepData data)
        //    {

        //        if (!modEnabled)
        //        {
        //            return;
        //        }

        //        Ray ray = new Ray(data.FootPosition, Vector3.down);
        //        float rayDistance = 1f;

        //        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        //        if (hits.Length == 0)
        //        {
        //            return;
        //        }

        //        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        //        foreach (var hit in hits)
        //        {
        //            switch (hit.collider.tag)
        //            {
        //                case "Untagged":
        //                case "Audio_Dirt":
        //                    int randomIndex = UnityEngine.Random.Range(1, 3);
        //                    string soundFilePath = $@"\ArcadeRumble\DirtStep{randomIndex}.mp3";
        //                    AudioManager.PlaySoundIfFileExists(soundFilePath, 0.25f, false);
        //                    return;

        //                case "Audio_Wood":
        //                    int randomIndex1 = UnityEngine.Random.Range(1, 3);
        //                    string soundFilePath1 = $@"\ArcadeRumble\WoodStep{randomIndex1}.mp3";
        //                    AudioManager.PlaySoundIfFileExists(soundFilePath1, 0.25f, false);
        //                    return;

        //                case "Audio_Metal":
        //                    int randomIndex2 = UnityEngine.Random.Range(1, 4);
        //                    string soundFilePath2 = $@"\ArcadeRumble\MetalStep{randomIndex2}.mp3";
        //                    AudioManager.PlaySoundIfFileExists(soundFilePath2, 0.25f, false);
        //                    return;
        //            }
        //        }

        //    }
        //}


        //[HarmonyPatch(typeof(Il2CppRUMBLE.Combat.ShiftStones.PlayerShiftstoneSystem), "AttachShiftStone", new Type[] { typeof(ShiftStone), typeof(int), typeof(bool), typeof(bool) })]
        //public static class ShiftstonePatch
        //{
        //    private static void Postfix(GameObject __instance, ShiftStone stone, int slotIndex, bool saveInSettings, bool syncWithOtherPlayers)
        //    {
        //        if (!modEnabled)
        //        {
        //            return;
        //        }

        //        int randomIndex = UnityEngine.Random.Range(1, 3);
        //        string soundFilePath = $@"\ArcadeRumble\ShiftstoneSwitch{randomIndex}.mp3";
        //        AudioManager.PlaySoundIfFileExists(soundFilePath, 0.25f, false);

        //    }
        //}


        //[HarmonyPatch(typeof(Il2CppRUMBLE.Combat.ShiftStones.PlayerShiftstoneSystem), "ActivateUseShiftstoneEffects", new Type[] { typeof(Il2CppRUMBLE.Input.InputManager.Hand) })]
        //public static class ShiftstonePatch4
        //{
        //    private static void Postfix(PlayerShiftstoneSystem __instance, Il2CppRUMBLE.Input.InputManager.Hand hand)
        //    {

        //        if (!modEnabled)
        //        {
        //            return;
        //        }

        //        AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\ShiftstoneUse.mp3", 0.5f, false);
        //    }
        //}

        //[HarmonyPatch(typeof(Il2CppRUMBLE.MoveSystem.Structure), "OnFetchFromPool")]
        //public static class StructureRespawn
        //{
        //    private static void Postfix(ref Il2CppRUMBLE.MoveSystem.Structure __instance)
        //    {

        //        string name = __instance.processableComponent.gameObject.name;
        //        MeshRenderer structureMeshRenderer;
        //        try
        //        {
        //            if (__instance.processableComponent.gameObject.GetComponent<Tetherball>() != null)
        //            {
        //                name = "BoulderBall";
        //                structureMeshRenderer = __instance.processableComponent.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        //            }
        //            else if (__instance.processableComponent.gameObject.GetComponent<MeshRenderer>() != null)
        //            {
        //                structureMeshRenderer = __instance.processableComponent.gameObject.GetComponent<MeshRenderer>();
        //            }
        //            else
        //            {
        //                structureMeshRenderer = __instance.processableComponent.gameObject.transform.GetComponentInChildren<MeshRenderer>();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            MelonLogger.Error(e);
        //            return;
        //        }

        //        var modInstance = MelonLoader.MelonMod.RegisteredMelons.FirstOrDefault(m => m is Class1) as Class1;

        //        Transform poolParent = Calls.Pools.Structures.GetPoolCube().transform.parent;



        //        switch (name)
        //        {
        //            case "LargeRock":
        //            case "SmallRock":
        //            case "BoulderBall":
        //                activeStructures.Add(structureMeshRenderer.gameObject);
        //                break;

        //            case "Pillar":
        //            case "Disc":
        //            case "Wall":
        //            case "RockCube":
        //            case "Ball":
        //                activeStructures.Add(__instance.processableComponent.gameObject);
        //                break;
        //        }
        //    }
        //}


        //[HarmonyPatch(typeof(Il2CppRUMBLE.Environment.Matchmaking.MatchmakeConsole), "MatchmakeStatusUpdated", new Type[] { typeof(MatchmakingHandler.MatchmakeStatus), typeof(bool) })]
        //public static class Patch1
        //{
        //    private static void Prefix(GameObject __instance, MatchmakingHandler.MatchmakeStatus status, bool instantLeverStep)
        //    {

        //        if (status == MatchmakingHandler.MatchmakeStatus.Success)
        //        {
        //            matchFound = true;
        //        }
        //    }
        //}





    }



    public static class AudioManager
    {
        public class ClipData
        {
            public WaveOutEvent WaveOut { get; set; }
            public ISampleProvider VolumeProvider { get; set; }
        }

        private static IEnumerator PlaySound(ClipData clipData, AudioFileReader reader, bool loop)
        {
            if (clipData == null || clipData.WaveOut == null || reader == null)
            {
                yield break;
            }

            

            do
            {
                clipData.WaveOut.Play();

                while (clipData.WaveOut != null && clipData.WaveOut.PlaybackState == PlaybackState.Playing)
                {
                    yield return null;
                }

                if (loop && clipData.WaveOut != null)
                {
                    clipData.WaveOut.Stop();
                    reader.Position = 0;
                }

            } while (loop && clipData.WaveOut != null);

            reader.Dispose();

            if (clipData.WaveOut != null)
            {
                if (clipData.WaveOut.PlaybackState == PlaybackState.Playing)
                {
                    clipData.WaveOut.Stop();
                }

                clipData.WaveOut.Dispose();
                clipData.WaveOut = null;
            }

            clipData = null;
        }

        public static AudioManager.ClipData PlayAudioForVFX(string vfxName)
        {
            switch (vfxName)
            {
                case "Straight_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Straight.mp3", 0.5f, false);

                case "Uppercut_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Uppercut.mp3", 0.5f, false);

                case "Kick_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Kick.mp3", 0.5f, false);

                case "Stomp_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Stomp.mp3", 0.5f, false);

                case "RockCube":
                case "Pillar":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\SpawnPillar.mp3", 0.5f, false);

                case "Disc":
                case "Ball":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\SpawnDisc.mp3", 0.5f, false);

                case "Wall":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\SpawnWall.mp3", 0.5f, false);

                case "Parry_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Parry.mp3", 0.5f, false);

                case "ExplodeFinale_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\DestroyExploded.mp3", 0.5f, false);

                case "ExplodeActivation_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Explode.mp3", 0.5f, false);

                case "DustBreak_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\CubeBreak.mp3", 0.5f, false);

                case "DustBreakDISC_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\DiscBreak.mp3", 0.5f, false);

                case "DustImpact_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\TerrainCollision.mp3", 0.5f, false);

                case "DustGroundedFriction_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\DustGroundedFriction.mp3", 0.5f, true);

                case "DustPlayerKnockbackVFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\PlayerKnockback.mp3", 0.5f, true);

                case "Ricochet_VFX":
                    return AudioManager.PlaySoundIfFileExists(@"\ArcadeRumble\Ricochet.mp3", 0.5f, false);



                default:
                    return null;
            }
        }

        public static ClipData PlaySoundIfFileExists(string soundFilePath, float volume = 1.0f, bool loop = false)
        {
            string fullPath = MelonEnvironment.UserDataDirectory + soundFilePath;

            var reader = new AudioFileReader(fullPath);

            var volumeProvider = new VolumeSampleProvider(reader)
            {
                Volume = Mathf.Clamp01(volume)
            };

            var waveOut = new WaveOutEvent();
            waveOut.Init(volumeProvider);

            var clipData = new ClipData
            {
                WaveOut = waveOut,
                VolumeProvider = volumeProvider
            };

            MelonCoroutines.Start(PlaySound(clipData, reader, loop));
            return clipData;

        }

        public static void ChangeVolume(ClipData clipData, float volume)
        {
            if (clipData == null || clipData.VolumeProvider == null)
            {
                return;
            }

            if (clipData.VolumeProvider is VolumeSampleProvider volumeProvider)
            {
                volumeProvider.Volume = Mathf.Clamp01(volume);
            }
        }

        public static void StopPlayback(ClipData clipData)
        {
            if (clipData == null || clipData.WaveOut == null)
            {
                return;
            }

            clipData.WaveOut.Stop();
            clipData.WaveOut.Dispose();

            clipData.WaveOut = null;
        }
    }
}