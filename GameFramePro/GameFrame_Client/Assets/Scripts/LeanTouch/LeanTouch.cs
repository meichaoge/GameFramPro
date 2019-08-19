using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

namespace Lean.Touch
{
    [CustomEditor(typeof(LeanTouch))]
    public class LeanTouch_Editor : Editor
    {
        private static List<LeanFinger> allFingers = new List<LeanFinger>();

        private static GUIStyle fadingLabel;

        [MenuItem("GameObject/Lean/Touch", false, 1)]
        public static void CreateTouch()
        {
            var gameObject = new GameObject(typeof(LeanTouch).Name);

            Undo.RegisterCreatedObjectUndo(gameObject, "Create Touch");

            gameObject.AddComponent<LeanTouch>();

            Selection.activeGameObject = gameObject;
        }

        // Draw the whole inspector
        public override void OnInspectorGUI()
        {
//            if (LeanTouch.S_Instance> 1)
//            {
//                EditorGUILayout.HelpBox("There is more than one active and enabled LeanTouch...", MessageType.Warning);
//                EditorGUILayout.Separator();
//            }

            var touch = (LeanTouch) target;

            EditorGUILayout.Separator();

            DrawSettings(touch);

            EditorGUILayout.Separator();

            DrawFingers(touch);

            EditorGUILayout.Separator();

            Repaint();
        }

        private void DrawSettings(LeanTouch touch)
        {
            DrawDefault("TapThreshold");
            DrawDefault("SwipeThreshold");
            DrawDefault("ReferenceDpi");
            DrawDefault("GuiLayers");

            EditorGUILayout.Separator();

            DrawDefault("RecordFingers");

            if (touch.RecordFingers == true)
            {
                EditorGUI.indentLevel++;
                DrawDefault("RecordThreshold");
                DrawDefault("RecordLimit");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            DrawDefault("SimulateMultiFingers");

            if (touch.SimulateMultiFingers == true)
            {
                EditorGUI.indentLevel++;
                DrawDefault("PinchTwistKey");
                DrawDefault("MovePivotKey");
                DrawDefault("MultiDragKey");
                DrawDefault("FingerTexture");
                EditorGUI.indentLevel--;
            }
        }

        private void DrawFingers(LeanTouch touch)
        {
            EditorGUILayout.LabelField("Fingers", EditorStyles.boldLabel);

            allFingers.Clear();
            allFingers.AddRange(LeanTouch.Fingers);
            allFingers.AddRange(LeanTouch.InactiveFingers);
            allFingers.Sort((a, b) => a.Index.CompareTo(b.Index));

            for (var i = 0; i < allFingers.Count; i++)
            {
                var finger = allFingers[i];
                var progress = touch.TapThreshold > 0.0f ? finger.Age / touch.TapThreshold : 0.0f;
                var style = GetFadingLabel(finger.Set, progress);

                if (style.normal.textColor.a > 0.0f)
                {
                    var screenPosition = finger.ScreenPosition;

                    EditorGUILayout.LabelField("#" + finger.Index + " x " + finger.TapCount + " (" + Mathf.FloorToInt(screenPosition.x) + ", " + Mathf.FloorToInt(screenPosition.y) + ") - " + finger.Age.ToString("0.0"), style);
                }
            }
        }

        private void DrawDefault(string name)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(name));

            if (EditorGUI.EndChangeCheck() == true)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static GUIStyle GetFadingLabel(bool active, float progress)
        {
            if (fadingLabel == null)
            {
                fadingLabel = new GUIStyle(EditorStyles.label);
            }

            var a = EditorStyles.label.normal.textColor;
            var b = a;
            b.a = active == true ? 0.5f : 0.0f;

            fadingLabel.normal.textColor = Color.Lerp(a, b, progress);

            return fadingLabel;
        }
    }
}
#endif

namespace Lean.Touch
{
    /// <summary>If you add this component to your scene, then it will convert all mouse and touch data into easy to use data.
    /// You can access this data via Lean.Touch.LeanTouch.Instance.Fingers, or hook into the Lean.Touch.LeanTouch.On___ events.
    /// NOTE: If you experience a one frame input delay you should edit your ScriptExecutionOrder to force this script to update before your other scripts.</summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [HelpURL(HelpUrlPrefix + "LeanTouch")]
    public partial class LeanTouch : MonoBehaviour
    {
        public static LeanTouch S_Instance = null;

        public const string HelpUrlPrefix = "http://carloswilkes.github.io/Documentation/LeanTouch#";
        public const string PlusHelpUrlPrefix = "http://carloswilkes.github.io/Documentation/LeanTouchPlus#";


        /// <summary>This gets fired when a finger begins touching the screen (LeanFinger = The current finger)</summary>
        public static System.Action<LeanFinger> OnFingerDown;

        /// <summary>This gets fired every frame a finger is touching the screen (LeanFinger = The current finger)</summary>
        public static System.Action<LeanFinger> OnFingerSet;

        /// <summary>This gets fired when a finger stops touching the screen (LeanFinger = The current finger)</summary>
        public static System.Action<LeanFinger> OnFingerUp;

        /// <summary>This gets fired when a finger taps the screen (this is when a finger begins and stops touching the screen within the 'TapThreshold' time) (LeanFinger = The current finger)</summary>
        public static System.Action<LeanFinger> OnFingerTap;

        /// <summary>This gets fired when a finger swipes the screen (this is when a finger begins and stops touching the screen within the 'TapThreshold' time, and also moves more than the 'SwipeThreshold' distance) (LeanFinger = The current finger)</summary>
        public static System.Action<LeanFinger> OnFingerSwipe;

        /// <summary>This gets fired every frame at least one finger is touching the screen (List = Fingers).</summary>
        public static System.Action<List<LeanFinger>> OnGesture;

        /// <summary>This gets fired after a finger has stopped touching the screen for more than TapThreshold seconds, and is removed from both the active and inactive finger lists.</summary>
        public static System.Action<LeanFinger> OnFingerExpired;


        /// <summary>This allows you to set how many seconds are required between a finger down/up for a tap to be registered.</summary>
        public float TapThreshold = DefaultTapThreshold;

        public const float DefaultTapThreshold = 0.2f;

        public static float CurrentTapThreshold
        {
            get { return S_Instance ? S_Instance.TapThreshold : DefaultTapThreshold; }
        }

        /// <summary>This allows you to set how many pixels of movement (relative to the ReferenceDpi) are required within the TapThreshold for a swipe to be triggered.</summary>
        public float SwipeThreshold = DefaultSwipeThreshold;

        public const float DefaultSwipeThreshold = 100.0f;

        public static float CurrentSwipeThreshold
        {
            get { return S_Instance != null ? S_Instance.SwipeThreshold : DefaultSwipeThreshold; }
        }

        /// <summary>This allows you to set the default DPI you want the input scaling to be based on.</summary>
        public int ReferenceDpi = DefaultReferenceDpi;

        public const int DefaultReferenceDpi = 200;

        public static int CurrentReferenceDpi
        {
            get { return S_Instance != null ? S_Instance.ReferenceDpi : DefaultReferenceDpi; }
        }

        /// <summary>This allows you to set which layers your GUI is on, so it can be ignored by each finger.</summary>
        public LayerMask GuiLayers = Physics.DefaultRaycastLayers;

        public static LayerMask CurrentGuiLayers
        {
            get { return S_Instance != null ? S_Instance.GuiLayers : (LayerMask) Physics.DefaultRaycastLayers; }
        }


        /// <summary>Should each fnger record snapshots of their screen positions?</summary>
        public bool RecordFingers = true;

        /// <summary>This allows you to set the amount of pixels a finger must move for another snapshot to be stored.</summary>
        public float RecordThreshold = 5.0f;

        /// <summary>This allows you to set the maximum amount of seconds that can be recorded, 0 = unlimited.</summary>
        public float RecordLimit = 10.0f;

        /// <summary>This allows you to simulate multi touch inputs on devices that don't support them (e.g. desktop).</summary>
        public bool SimulateMultiFingers = true;

        /// <summary>This allows you to set which key is required to simulate multi key twisting.</summary>
        public KeyCode PinchTwistKey = KeyCode.LeftControl;

        /// <summary>This allows you to set which key is required to change the pivot point of the pinch twist gesture.</summary>
        public KeyCode MovePivotKey = KeyCode.LeftAlt;

        /// <summary>This allows you to set which key is required to simulate multi key dragging.</summary>
        public KeyCode MultiDragKey = KeyCode.LeftAlt;


        /// <summary>This allows you to set which texture will be used to show the simulated fingers.</summary>
        public Texture2D FingerTexture;



        protected void Awake()
        {
            S_Instance = this;
        }


        protected virtual void Update()
        {
            // Prepare old finger data for new information
            BeginFingers();

            // Poll current touch + mouse data and convert it to fingers
            PollFingers();

            // Process any no longer used fingers
            EndFingers();

            // Update events based on new finger data
            UpdateEvents();
        }

        protected virtual void OnGUI()
        {
            // Show simulated multi fingers?
            if (FingerTexture != null && Input.touchCount == 0 && Fingers.Count > 1)
            {
                for (var i = Fingers.Count - 1; i >= 0; i--)
                {
                    var finger = Fingers[i];

                    // Don't show fingers that just went up, because real touches will be up the frame they release
                    if (finger.Up == false)
                    {
                        var screenPosition = finger.ScreenPosition;
                        var screenRect = new Rect(0, 0, FingerTexture.width, FingerTexture.height);

                        screenRect.center = new Vector2(screenPosition.x, Screen.height - screenPosition.y);

                        GUI.DrawTexture(screenRect, FingerTexture);
                    }
                }
            }
        }

        // Update all Fingers and InactiveFingers so they're ready for the new frame
        private void BeginFingers()
        {
            // Age inactive fingers
            for (var i = InactiveFingers.Count - 1; i >= 0; i--)
            {
                var inactiveFinger = InactiveFingers[i];
                inactiveFinger.Age += Time.unscaledDeltaTime;

                // Just expired?
                if (inactiveFinger.Expired == false && inactiveFinger.Age > TapThreshold)
                {
                    inactiveFinger.Expired = true;
                    OnFingerExpired?.Invoke(inactiveFinger);
                }
            }

            // Reset finger data
            for (var i = Fingers.Count - 1; i >= 0; i--)
            {
                var finger = Fingers[i];
                // Was this set to up last time? If so, it's now inactive
                if (finger.Up )
                {
                    // Make finger inactive
                    Fingers.RemoveAt(i);
                    InactiveFingers.Add(finger);

                    // Reset age so we can time how long it's been inactive
                    finger.Age = 0.0f;

                    // Pool old snapshots
                    finger.ClearSnapshots();
                }
                else
                {
                    finger.LastSet = finger.Set;
                    finger.LastPressure = finger.Pressure;
                    finger.LastScreenPosition = finger.ScreenPosition;

                    finger.Set = false;
                    finger.Tap = false;
                    finger.Swipe = false;
                }
            }
        }
        // Read new hardware finger data
        private void PollFingers()
        {
            // Update real fingers
            if (Input.touchCount > 0)
            {
                for (var i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);

                    // Only poll fingers that are active?
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                    {
                        var pressure = 1.0f;
#if UNITY_5_4_OR_NEWER
                        pressure = touch.pressure;
#endif
                        AddFinger(touch.fingerId, touch.position, pressure);
                    }
                }
            }
            // If there are no real touches, simulate some from the mouse?
            else if (AnyMouseButtonSet )
            {
                var screen = new Rect(0, 0, Screen.width, Screen.height);
                var mousePosition = (Vector2) Input.mousePosition;

                // Is the mouse within the screen?
                if (screen.Contains(mousePosition) == true)
                {
                    AddFinger(-1, mousePosition, 1.0f);

                    // Simulate pinch & twist?
                    if (SimulateMultiFingers )
                    {
                        //var finger0 = FindFinger(0);

                        if (Input.GetKey(MovePivotKey) )
                        {
                            pivot.x = mousePosition.x / Screen.width;
                            pivot.y = mousePosition.y / Screen.height;
                        }

                        if (Input.GetKey(PinchTwistKey))
                        {
                            var center = new Vector2(Screen.width * pivot.x, Screen.height * pivot.y);

                            AddFinger(-2, center - (mousePosition - center), 1.0f);
                            //AddFinger(-2, finger0.StartScreenPosition - finger0.SwipeScreenDelta, 1.0f);
                        }
                        // Simulate multi drag?
                        else if (Input.GetKey(MultiDragKey) )
                        {
                            AddFinger(-2, mousePosition, 1.0f);
                        }
                    }
                }
            }
        }
        // Update all Fingers based on the new finger data
        private void EndFingers()
        {
            for (var i = Fingers.Count - 1; i >= 0; i--)
            {
                var finger = Fingers[i];

                // Up?
                if (finger.Up )
                {
                    // Tap or Swipe?
                    if (finger.Age <= TapThreshold)
                    {
                        if (finger.SwipeScreenDelta.magnitude * ScalingFactor < SwipeThreshold)
                        {
                            finger.Tap = true;
                            finger.TapCount += 1;
                        }
                        else
                        {
                            finger.TapCount = 0;
                            finger.Swipe = true;
                        }
                    }

                    else
                    {
                        finger.TapCount = 0;
                    }
                }
                // Down?
                else if (finger.Down == false)
                {
                    // Age it
                    finger.Age += Time.unscaledDeltaTime;
                }
            }
        }
        private void UpdateEvents()
        {
            var fingerCount = Fingers.Count;

            if (fingerCount > 0)
            {
                for (var i = 0; i < fingerCount; i++)
                {
                    var finger = Fingers[i];

                    if (finger.Tap == true && OnFingerTap != null) OnFingerTap(finger);
                    if (finger.Swipe == true && OnFingerSwipe != null) OnFingerSwipe(finger);
                    if (finger.Down == true && OnFingerDown != null) OnFingerDown(finger);
                    if (finger.Set == true && OnFingerSet != null) OnFingerSet(finger);
                    if (finger.Up == true && OnFingerUp != null) OnFingerUp(finger);
                }

                if (OnGesture != null)
                {
                    filteredFingers.Clear();
                    filteredFingers.AddRange(Fingers);

                    OnGesture(filteredFingers);
                }
            }
        }

        // Add a finger based on index, or return the existing one
        private void AddFinger(int index, Vector2 screenPosition, float pressure)
        {
            var finger = FindFinger(index);

            // No finger found?
            if (finger == null)
            {
                var inactiveIndex = FindInactiveFingerIndex(index);

                // Use inactive finger?
                if (inactiveIndex >= 0)
                {
                    finger = InactiveFingers[inactiveIndex];
                    InactiveFingers.RemoveAt(inactiveIndex);

                    // Inactive for too long?
                    if (finger.Age > TapThreshold)
                    {
                        finger.TapCount = 0;
                    }

                    // Reset values
                    finger.Age = 0.0f;
                    finger.Set = false;
                    finger.LastSet = false;
                    finger.Tap = false;
                    finger.Swipe = false;
                    finger.Expired = false;
                }
                // Create new finger?
                else
                {
                    finger = new LeanFinger();

                    finger.Index = index;
                }

                finger.StartScreenPosition = screenPosition;
                finger.LastScreenPosition = screenPosition;
                finger.LastPressure = pressure;
                finger.StartedOverGui = PointOverGui(screenPosition);

                Fingers.Add(finger);
            }

            finger.Set = true;
            finger.ScreenPosition = screenPosition;
            finger.Pressure = pressure;

            // Record?
            if (RecordFingers )
            {
                // Too many snapshots?
                if (RecordLimit > 0.0f)
                {
                    if (finger.SnapshotDuration > RecordLimit)
                    {
                        var removeCount = LeanSnapshot.GetLowerIndex(finger.Snapshots, finger.Age - RecordLimit);

                        finger.ClearSnapshots(removeCount);
                    }
                }

                // Record snapshot?
                if (RecordThreshold > 0.0f)
                {
                    if (finger.Snapshots.Count == 0 || finger.LastSnapshotScreenDelta.magnitude >= RecordThreshold)
                    {
                        finger.RecordSnapshot();
                    }
                }
                else
                {
                    finger.RecordSnapshot();
                }
            }
        }

        // Find the finger with the specified index, or return null
        private LeanFinger FindFinger(int index)
        {
            for (var i = Fingers.Count - 1; i >= 0; i--)
            {
                var finger = Fingers[i];
                if (finger.Index == index)
                    return finger;
            }
            return null;
        }

        // Find the index of the inactive finger with the specified index, or return -1
        private int FindInactiveFingerIndex(int index)
        {
            for (var i = InactiveFingers.Count - 1; i >= 0; i--)
            {
                if (InactiveFingers[i].Index == index)
                    return i;
            }

            return -1;
        }
    }
}