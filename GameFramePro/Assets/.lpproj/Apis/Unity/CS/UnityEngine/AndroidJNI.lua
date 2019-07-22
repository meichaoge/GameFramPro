---@class CS.UnityEngine.AndroidJNI : CS.System.Object
CS.UnityEngine.AndroidJNI = {}

---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.AttachCurrentThread()
end

---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.DetachCurrentThread()
end

---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.GetVersion()
end

---@param name : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.FindClass(name)
end

---@param refMethod : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.FromReflectedMethod(refMethod)
end

---@param refField : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.FromReflectedField(refField)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param isStatic : CS.System.Boolean
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToReflectedMethod(clazz, methodID, isStatic)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param isStatic : CS.System.Boolean
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToReflectedField(clazz, fieldID, isStatic)
end

---@param clazz : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetSuperclass(clazz)
end

---@param clazz1 : CS.System.IntPtr
---@param clazz2 : CS.System.IntPtr
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJNI.IsAssignableFrom(clazz1, clazz2)
end

---@param obj : CS.System.IntPtr
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.Throw(obj)
end

---@param clazz : CS.System.IntPtr
---@param message : CS.System.String
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.ThrowNew(clazz, message)
end

---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ExceptionOccurred()
end

function CS.UnityEngine.AndroidJNI.ExceptionDescribe()
end

function CS.UnityEngine.AndroidJNI.ExceptionClear()
end

---@param message : CS.System.String
function CS.UnityEngine.AndroidJNI.FatalError(message)
end

---@param capacity : CS.System.Int32
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.PushLocalFrame(capacity)
end

---@param ptr : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.PopLocalFrame(ptr)
end

---@param obj : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewGlobalRef(obj)
end

---@param obj : CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.DeleteGlobalRef(obj)
end

---@param obj : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewLocalRef(obj)
end

---@param obj : CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.DeleteLocalRef(obj)
end

---@param obj1 : CS.System.IntPtr
---@param obj2 : CS.System.IntPtr
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJNI.IsSameObject(obj1, obj2)
end

---@param capacity : CS.System.Int32
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.EnsureLocalCapacity(capacity)
end

---@param clazz : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.AllocObject(clazz)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewObject(clazz, methodID, args)
end

---@param obj : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetObjectClass(obj)
end

---@param obj : CS.System.IntPtr
---@param clazz : CS.System.IntPtr
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJNI.IsInstanceOf(obj, clazz)
end

---@param clazz : CS.System.IntPtr
---@param name : CS.System.String
---@param sig : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetMethodID(clazz, name, sig)
end

---@param clazz : CS.System.IntPtr
---@param name : CS.System.String
---@param sig : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetFieldID(clazz, name, sig)
end

---@param clazz : CS.System.IntPtr
---@param name : CS.System.String
---@param sig : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetStaticMethodID(clazz, name, sig)
end

---@param clazz : CS.System.IntPtr
---@param name : CS.System.String
---@param sig : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetStaticFieldID(clazz, name, sig)
end

---@param bytes : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewStringUTF(bytes)
end

---@param str : CS.System.IntPtr
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.GetStringUTFLength(str)
end

---@param str : CS.System.IntPtr
---@return CS.System.String
function CS.UnityEngine.AndroidJNI.GetStringUTFChars(str)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.String
function CS.UnityEngine.AndroidJNI.CallStringMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.CallObjectMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.CallIntMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJNI.CallBooleanMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Int16
function CS.UnityEngine.AndroidJNI.CallShortMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Byte
function CS.UnityEngine.AndroidJNI.CallByteMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.SByte
function CS.UnityEngine.AndroidJNI.CallSByteMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Char
function CS.UnityEngine.AndroidJNI.CallCharMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Single
function CS.UnityEngine.AndroidJNI.CallFloatMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Double
function CS.UnityEngine.AndroidJNI.CallDoubleMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Int64
function CS.UnityEngine.AndroidJNI.CallLongMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
function CS.UnityEngine.AndroidJNI.CallVoidMethod(obj, methodID, args)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.String
function CS.UnityEngine.AndroidJNI.GetStringField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetObjectField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJNI.GetBooleanField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Byte
function CS.UnityEngine.AndroidJNI.GetByteField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.SByte
function CS.UnityEngine.AndroidJNI.GetSByteField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Char
function CS.UnityEngine.AndroidJNI.GetCharField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Int16
function CS.UnityEngine.AndroidJNI.GetShortField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.GetIntField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Int64
function CS.UnityEngine.AndroidJNI.GetLongField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Single
function CS.UnityEngine.AndroidJNI.GetFloatField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Double
function CS.UnityEngine.AndroidJNI.GetDoubleField(obj, fieldID)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.String
function CS.UnityEngine.AndroidJNI.SetStringField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.SetObjectField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Boolean
function CS.UnityEngine.AndroidJNI.SetBooleanField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Byte
function CS.UnityEngine.AndroidJNI.SetByteField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.SByte
function CS.UnityEngine.AndroidJNI.SetSByteField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Char
function CS.UnityEngine.AndroidJNI.SetCharField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Int16
function CS.UnityEngine.AndroidJNI.SetShortField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Int32
function CS.UnityEngine.AndroidJNI.SetIntField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Int64
function CS.UnityEngine.AndroidJNI.SetLongField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Single
function CS.UnityEngine.AndroidJNI.SetFloatField(obj, fieldID, val)
end

---@param obj : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Double
function CS.UnityEngine.AndroidJNI.SetDoubleField(obj, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.String
function CS.UnityEngine.AndroidJNI.CallStaticStringMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.CallStaticObjectMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.CallStaticIntMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJNI.CallStaticBooleanMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Int16
function CS.UnityEngine.AndroidJNI.CallStaticShortMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Byte
function CS.UnityEngine.AndroidJNI.CallStaticByteMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.SByte
function CS.UnityEngine.AndroidJNI.CallStaticSByteMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Char
function CS.UnityEngine.AndroidJNI.CallStaticCharMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Single
function CS.UnityEngine.AndroidJNI.CallStaticFloatMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Double
function CS.UnityEngine.AndroidJNI.CallStaticDoubleMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
---@return CS.System.Int64
function CS.UnityEngine.AndroidJNI.CallStaticLongMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param methodID : CS.System.IntPtr
---@param args : CS.UnityEngine.jvalue[]
function CS.UnityEngine.AndroidJNI.CallStaticVoidMethod(clazz, methodID, args)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.String
function CS.UnityEngine.AndroidJNI.GetStaticStringField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetStaticObjectField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJNI.GetStaticBooleanField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Byte
function CS.UnityEngine.AndroidJNI.GetStaticByteField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.SByte
function CS.UnityEngine.AndroidJNI.GetStaticSByteField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Char
function CS.UnityEngine.AndroidJNI.GetStaticCharField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Int16
function CS.UnityEngine.AndroidJNI.GetStaticShortField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.GetStaticIntField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Int64
function CS.UnityEngine.AndroidJNI.GetStaticLongField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Single
function CS.UnityEngine.AndroidJNI.GetStaticFloatField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@return CS.System.Double
function CS.UnityEngine.AndroidJNI.GetStaticDoubleField(clazz, fieldID)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.String
function CS.UnityEngine.AndroidJNI.SetStaticStringField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.SetStaticObjectField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Boolean
function CS.UnityEngine.AndroidJNI.SetStaticBooleanField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Byte
function CS.UnityEngine.AndroidJNI.SetStaticByteField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.SByte
function CS.UnityEngine.AndroidJNI.SetStaticSByteField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Char
function CS.UnityEngine.AndroidJNI.SetStaticCharField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Int16
function CS.UnityEngine.AndroidJNI.SetStaticShortField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Int32
function CS.UnityEngine.AndroidJNI.SetStaticIntField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Int64
function CS.UnityEngine.AndroidJNI.SetStaticLongField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Single
function CS.UnityEngine.AndroidJNI.SetStaticFloatField(clazz, fieldID, val)
end

---@param clazz : CS.System.IntPtr
---@param fieldID : CS.System.IntPtr
---@param val : CS.System.Double
function CS.UnityEngine.AndroidJNI.SetStaticDoubleField(clazz, fieldID, val)
end

---@param array : CS.System.Boolean[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToBooleanArray(array)
end

---@param array : CS.System.Byte[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToByteArray(array)
end

---@param array : CS.System.SByte[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToSByteArray(array)
end

---@param array : CS.System.Char[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToCharArray(array)
end

---@param array : CS.System.Int16[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToShortArray(array)
end

---@param array : CS.System.Int32[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToIntArray(array)
end

---@param array : CS.System.Int64[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToLongArray(array)
end

---@param array : CS.System.Single[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToFloatArray(array)
end

---@param array : CS.System.Double[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToDoubleArray(array)
end

---@param array : CS.System.IntPtr[]
---@param arrayClass : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToObjectArray(array, arrayClass)
end

---@param array : CS.System.IntPtr[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.ToObjectArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Boolean[]
function CS.UnityEngine.AndroidJNI.FromBooleanArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Byte[]
function CS.UnityEngine.AndroidJNI.FromByteArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.SByte[]
function CS.UnityEngine.AndroidJNI.FromSByteArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Char[]
function CS.UnityEngine.AndroidJNI.FromCharArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Int16[]
function CS.UnityEngine.AndroidJNI.FromShortArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Int32[]
function CS.UnityEngine.AndroidJNI.FromIntArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Int64[]
function CS.UnityEngine.AndroidJNI.FromLongArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Single[]
function CS.UnityEngine.AndroidJNI.FromFloatArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Double[]
function CS.UnityEngine.AndroidJNI.FromDoubleArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.IntPtr[]
function CS.UnityEngine.AndroidJNI.FromObjectArray(array)
end

---@param array : CS.System.IntPtr
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.GetArrayLength(array)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewBooleanArray(size)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewByteArray(size)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewSByteArray(size)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewCharArray(size)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewShortArray(size)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewIntArray(size)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewLongArray(size)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewFloatArray(size)
end

---@param size : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewDoubleArray(size)
end

---@param size : CS.System.Int32
---@param clazz : CS.System.IntPtr
---@param obj : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.NewObjectArray(size, clazz, obj)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJNI.GetBooleanArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.Byte
function CS.UnityEngine.AndroidJNI.GetByteArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.SByte
function CS.UnityEngine.AndroidJNI.GetSByteArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.Char
function CS.UnityEngine.AndroidJNI.GetCharArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.Int16
function CS.UnityEngine.AndroidJNI.GetShortArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.Int32
function CS.UnityEngine.AndroidJNI.GetIntArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.Int64
function CS.UnityEngine.AndroidJNI.GetLongArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.Single
function CS.UnityEngine.AndroidJNI.GetFloatArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.Double
function CS.UnityEngine.AndroidJNI.GetDoubleArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.GetObjectArrayElement(array, index)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.Byte
function CS.UnityEngine.AndroidJNI.SetBooleanArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.Boolean
function CS.UnityEngine.AndroidJNI.SetBooleanArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.SByte
function CS.UnityEngine.AndroidJNI.SetByteArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.SByte
function CS.UnityEngine.AndroidJNI.SetSByteArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.Char
function CS.UnityEngine.AndroidJNI.SetCharArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.Int16
function CS.UnityEngine.AndroidJNI.SetShortArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.Int32
function CS.UnityEngine.AndroidJNI.SetIntArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.Int64
function CS.UnityEngine.AndroidJNI.SetLongArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.Single
function CS.UnityEngine.AndroidJNI.SetFloatArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param val : CS.System.Double
function CS.UnityEngine.AndroidJNI.SetDoubleArrayElement(array, index, val)
end

---@param array : CS.System.IntPtr
---@param index : CS.System.Int32
---@param obj : CS.System.IntPtr
function CS.UnityEngine.AndroidJNI.SetObjectArrayElement(array, index, obj)
end