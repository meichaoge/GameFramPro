using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace Lean.Touch
{
    public partial class LeanTouch
    {
        /// <summary>This list contains all currently active fingers (including simulated ones)</summary>
        public static List<LeanFinger> Fingers = new List<LeanFinger>(10);

        /// <summary>This list contains all currently inactive fingers (this allows for pooling and tapping)</summary>
        public static List<LeanFinger> InactiveFingers = new List<LeanFinger>(10);

        
        
        // This stores the highest mouse button index
        private static int highestMouseButton = 7;

        // The current pivot (0,0 = bottom left, 1,1 = top right)
        private static Vector2 pivot = new Vector2(0.5f, 0.5f);

        // Used to find if the GUI is in use
        private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

        // Used to return non GUI fingers
        private static List<LeanFinger> filteredFingers = new List<LeanFinger>(10);
        
        

        // Used by RaycastGui
        private static PointerEventData tempPointerEventData;

        // Used by RaycastGui
        private static EventSystem tempEventSystem;


        /// <summary>If you multiply this value with any other pixel delta (e.g. ScreenDelta), then it will become device resolution independant.</summary>
        public static float ScalingFactor
        {
            get
            {
                // Get the current screen DPI
                var dpi = Screen.dpi;

                // If it's 0 or less, it's invalid, so return the default scale of 1.0
                if (dpi <= 0)
                    return 1.0f;

                // DPI seems valid, so scale it against the reference DPI
                return  CurrentReferenceDpi / dpi;
            }
        }

        // Returns true if any mouse button is pressed
        public static bool AnyMouseButtonSet
        {
            get
            {
                for (var i = 0; i < highestMouseButton; i++)
                {
                    if (Input.GetMouseButton(i) == true)
                        return true;
                }

                return false;
            }
        }

        /// <summary>This will return true if the mouse or any finger is currently using the GUI.</summary>
        public static bool GuiInUse
        {
            get
            {
                // Legacy GUI in use?
                if (GUIUtility.hotControl > 0)
                    return true;

                // New GUI in use?
                for (var i = Fingers.Count - 1; i >= 0; i--)
                {
                    if (Fingers[i].StartedOverGui == true)
                        return true;
                }

                return false;
            }
        }

        // If currentCamera is null, this will return the camera attached to gameObject, or return Camera.main
        public static Camera GetCamera(Camera currentCamera, GameObject gameObject = null)
        {
            if (currentCamera == null)
            {
                if (gameObject != null)
                    currentCamera = gameObject.GetComponent<Camera>();

                if (currentCamera == null)
                    currentCamera = Camera.main;
            }

            return currentCamera;
        }

        // Return the framerate independant damping factor (-1 = instant)
        public static float GetDampenFactor(float dampening, float deltaTime)
        {
            if (dampening < 0.0f)
                return 1.0f;

            if (Application.isPlaying == false)
                return 1.0f;

            return 1.0f - Mathf.Exp(-dampening * deltaTime);
        }

        /// <summary>This will return true if the specified screen point is over any GUI elements.</summary>
        //2019/8/3 扩展了PointOverGui() ， 使用默认的事件系统判定
        public static bool PointOverGui()
        {
# if (UNITY_ANDROID|| UNITY_IOS )&&! UNITY_EDITOR
                  if (Input.touchCount > 0)
                  {
                      for (int index = 0; index < Input.touchCount ; index++)
                      {
                          if (EventSystem.current.IsPointerOverGameObject(index))
                              return true;
                      }
                  }
         
                  return false;
#else
            return EventSystem.current.IsPointerOverGameObject();
#endif
        }

        public static bool PointOverGui(Vector2 screenPosition)
        {
            return RaycastGui(screenPosition).Count > 0;
        }

        /// <summary>This will return all the RaycastResults under the specified screen point using the current layerMask.
        /// NOTE: The first result (0) will be the top UI element that was first hit.</summary>
        public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
        {
            return RaycastGui(screenPosition, CurrentGuiLayers);
        }

        /// <summary>This will return all the RaycastResults under the specified screen point using the specified layerMask.
        /// NOTE: The first result (0) will be the top UI element that was first hit.</summary>
        public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
        {
            tempRaycastResults.Clear();

            var currentEventSystem = EventSystem.current;

            if (currentEventSystem != null)
            {
                // Create point event data for this event system?
                if (currentEventSystem != tempEventSystem)
                {
                    tempEventSystem = currentEventSystem;

                    if (tempPointerEventData == null)
                    {
                        tempPointerEventData = new PointerEventData(tempEventSystem);
                    }
                    else
                    {
                        tempPointerEventData.Reset();
                    }
                }

                // Raycast event system at the specified point
                tempPointerEventData.position = screenPosition;

                currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

                // Loop through all results and remove any that don't match the layer mask
                if (tempRaycastResults.Count > 0)
                {
                    for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
                    {
                        var raycastResult = tempRaycastResults[i];
                        var raycastLayer = 1 << raycastResult.gameObject.layer;

                        if ((raycastLayer & layerMask) == 0)
                        {
                            tempRaycastResults.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to RaycastGui because your scene doesn't have an event system! To add one, go to: GameObject/UI/EventSystem");
            }

            return tempRaycastResults;
        }

        /// <summary>This allows you to filter all the fingers based on the specified requirements.
        /// NOTE: If ignoreGuiFingers is set, Fingers will be filtered to remove any with StartedOverGui.
        /// NOTE: If requiredFingerCount is greather than 0, this method will return null if the finger count doesn't match.
        /// NOTE: If requiredSelectable is set, and its SelectingFinger isn't null, it will return just that finger.</summary>
        public static List<LeanFinger> GetFingers(bool ignoreIfStartedOverGui, bool ignoreIfOverGui, int requiredFingerCount = 0)
        {
            filteredFingers.Clear();

            for (var i = 0; i < Fingers.Count; i++)
            {
                var finger = Fingers[i];

                // Ignore?
                if (ignoreIfStartedOverGui && finger.StartedOverGui)
                    continue;

                if (ignoreIfOverGui && finger.IsOverGui)
                    continue;

                // Add
                filteredFingers.Add(finger);
            }

            if (requiredFingerCount > 0)
            {
                if (filteredFingers.Count != requiredFingerCount)
                {
                    filteredFingers.Clear();
                    return filteredFingers;
                }
            }

            return filteredFingers;
        }

    }
}