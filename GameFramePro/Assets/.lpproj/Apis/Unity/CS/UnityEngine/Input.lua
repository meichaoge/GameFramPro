---@class CS.UnityEngine.Input : CS.System.Object
CS.UnityEngine.Input = {}

---@property readwrite CS.UnityEngine.Input.simulateMouseWithTouches : CS.System.Boolean
CS.UnityEngine.Input.simulateMouseWithTouches = nil

---@property readonly CS.UnityEngine.Input.anyKey : CS.System.Boolean
CS.UnityEngine.Input.anyKey = nil

---@property readonly CS.UnityEngine.Input.anyKeyDown : CS.System.Boolean
CS.UnityEngine.Input.anyKeyDown = nil

---@property readonly CS.UnityEngine.Input.inputString : CS.System.String
CS.UnityEngine.Input.inputString = nil

---@property readonly CS.UnityEngine.Input.mousePosition : CS.UnityEngine.Vector3
CS.UnityEngine.Input.mousePosition = nil

---@property readonly CS.UnityEngine.Input.mouseScrollDelta : CS.UnityEngine.Vector2
CS.UnityEngine.Input.mouseScrollDelta = nil

---@property readwrite CS.UnityEngine.Input.imeCompositionMode : CS.UnityEngine.IMECompositionMode
CS.UnityEngine.Input.imeCompositionMode = nil

---@property readonly CS.UnityEngine.Input.compositionString : CS.System.String
CS.UnityEngine.Input.compositionString = nil

---@property readonly CS.UnityEngine.Input.imeIsSelected : CS.System.Boolean
CS.UnityEngine.Input.imeIsSelected = nil

---@property readwrite CS.UnityEngine.Input.compositionCursorPos : CS.UnityEngine.Vector2
CS.UnityEngine.Input.compositionCursorPos = nil

---@property readwrite CS.UnityEngine.Input.eatKeyPressOnTextFieldFocus : CS.System.Boolean
CS.UnityEngine.Input.eatKeyPressOnTextFieldFocus = nil

---@property readonly CS.UnityEngine.Input.mousePresent : CS.System.Boolean
CS.UnityEngine.Input.mousePresent = nil

---@property readonly CS.UnityEngine.Input.touchCount : CS.System.Int32
CS.UnityEngine.Input.touchCount = nil

---@property readonly CS.UnityEngine.Input.touchPressureSupported : CS.System.Boolean
CS.UnityEngine.Input.touchPressureSupported = nil

---@property readonly CS.UnityEngine.Input.stylusTouchSupported : CS.System.Boolean
CS.UnityEngine.Input.stylusTouchSupported = nil

---@property readonly CS.UnityEngine.Input.touchSupported : CS.System.Boolean
CS.UnityEngine.Input.touchSupported = nil

---@property readwrite CS.UnityEngine.Input.multiTouchEnabled : CS.System.Boolean
CS.UnityEngine.Input.multiTouchEnabled = nil

---@property readonly CS.UnityEngine.Input.isGyroAvailable : CS.System.Boolean
CS.UnityEngine.Input.isGyroAvailable = nil

---@property readonly CS.UnityEngine.Input.deviceOrientation : CS.UnityEngine.DeviceOrientation
CS.UnityEngine.Input.deviceOrientation = nil

---@property readonly CS.UnityEngine.Input.acceleration : CS.UnityEngine.Vector3
CS.UnityEngine.Input.acceleration = nil

---@property readwrite CS.UnityEngine.Input.compensateSensors : CS.System.Boolean
CS.UnityEngine.Input.compensateSensors = nil

---@property readonly CS.UnityEngine.Input.accelerationEventCount : CS.System.Int32
CS.UnityEngine.Input.accelerationEventCount = nil

---@property readwrite CS.UnityEngine.Input.backButtonLeavesApp : CS.System.Boolean
CS.UnityEngine.Input.backButtonLeavesApp = nil

---@property readonly CS.UnityEngine.Input.location : CS.UnityEngine.LocationService
CS.UnityEngine.Input.location = nil

---@property readonly CS.UnityEngine.Input.compass : CS.UnityEngine.Compass
CS.UnityEngine.Input.compass = nil

---@property readonly CS.UnityEngine.Input.gyro : CS.UnityEngine.Gyroscope
CS.UnityEngine.Input.gyro = nil

---@property readonly CS.UnityEngine.Input.touches : CS.UnityEngine.Touch[]
CS.UnityEngine.Input.touches = nil

---@property readonly CS.UnityEngine.Input.accelerationEvents : CS.UnityEngine.AccelerationEvent[]
CS.UnityEngine.Input.accelerationEvents = nil

---@return CS.UnityEngine.Input
function CS.UnityEngine.Input()
end

---@param axisName : CS.System.String
---@return CS.System.Single
function CS.UnityEngine.Input.GetAxis(axisName)
end

---@param axisName : CS.System.String
---@return CS.System.Single
function CS.UnityEngine.Input.GetAxisRaw(axisName)
end

---@param buttonName : CS.System.String
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetButton(buttonName)
end

---@param buttonName : CS.System.String
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetButtonDown(buttonName)
end

---@param buttonName : CS.System.String
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetButtonUp(buttonName)
end

---@param button : CS.System.Int32
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetMouseButton(button)
end

---@param button : CS.System.Int32
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetMouseButtonDown(button)
end

---@param button : CS.System.Int32
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetMouseButtonUp(button)
end

function CS.UnityEngine.Input.ResetInputAxes()
end

---@param joystickName : CS.System.String
---@return CS.System.Boolean
function CS.UnityEngine.Input.IsJoystickPreconfigured(joystickName)
end

---@return CS.System.String[]
function CS.UnityEngine.Input.GetJoystickNames()
end

---@param index : CS.System.Int32
---@return CS.UnityEngine.Touch
function CS.UnityEngine.Input.GetTouch(index)
end

---@param index : CS.System.Int32
---@return CS.UnityEngine.AccelerationEvent
function CS.UnityEngine.Input.GetAccelerationEvent(index)
end

---@param key : CS.UnityEngine.KeyCode
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetKey(key)
end

---@param name : CS.System.String
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetKey(name)
end

---@param key : CS.UnityEngine.KeyCode
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetKeyUp(key)
end

---@param name : CS.System.String
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetKeyUp(name)
end

---@param key : CS.UnityEngine.KeyCode
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetKeyDown(key)
end

---@param name : CS.System.String
---@return CS.System.Boolean
function CS.UnityEngine.Input.GetKeyDown(name)
end