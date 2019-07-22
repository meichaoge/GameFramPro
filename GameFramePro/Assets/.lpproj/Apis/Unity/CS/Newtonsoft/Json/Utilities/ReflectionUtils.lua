---@class CS.Newtonsoft.Json.Utilities.ReflectionUtils : CS.System.Object
CS.Newtonsoft.Json.Utilities.ReflectionUtils = {}

---@param propertyInfo : CS.System.Reflection.PropertyInfo
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsVirtual(propertyInfo)
end

---@param v : CS.System.Object
---@return CS.System.Type
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetObjectType(v)
end

---@param t : CS.System.Type
---@param assemblyFormat : CS.System.Runtime.Serialization.Formatters.FormatterAssemblyStyle
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetTypeName(t, assemblyFormat)
end

---@param t : CS.System.Type
---@param assemblyFormat : CS.System.Runtime.Serialization.Formatters.FormatterAssemblyStyle
---@param binder : CS.System.Runtime.Serialization.SerializationBinder
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetTypeName(t, assemblyFormat, binder)
end

---@param t : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsInstantiatableType(t)
end

---@param t : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.HasDefaultConstructor(t)
end

---@param t : CS.System.Type
---@param nonPublic : CS.System.Boolean
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.HasDefaultConstructor(t, nonPublic)
end

---@param t : CS.System.Type
---@return CS.System.Reflection.ConstructorInfo
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetDefaultConstructor(t)
end

---@param t : CS.System.Type
---@param nonPublic : CS.System.Boolean
---@return CS.System.Reflection.ConstructorInfo
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetDefaultConstructor(t, nonPublic)
end

---@param t : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsNullable(t)
end

---@param t : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsNullableType(t)
end

---@param t : CS.System.Type
---@return CS.System.Type
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.EnsureNotNullableType(t)
end

---@param value : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsUnitializedValue(value)
end

---@param type : CS.System.Type
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.CreateUnitializedValue(type)
end

---@param property : CS.System.Reflection.PropertyInfo
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsPropertyIndexed(property)
end

---@param type : CS.System.Type
---@param genericInterfaceDefinition : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.ImplementsGenericDefinition(type, genericInterfaceDefinition)
end

---@param type : CS.System.Type
---@param genericInterfaceDefinition : CS.System.Type
---@param implementingType : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.ImplementsGenericDefinition(type, genericInterfaceDefinition, implementingType)
end

---@param type : CS.System.Type
---@param fullTypeName : CS.System.String
---@param match : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.AssignableToTypeName(type, fullTypeName, match)
end

---@param type : CS.System.Type
---@param fullTypeName : CS.System.String
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.AssignableToTypeName(type, fullTypeName)
end

---@param type : CS.System.Type
---@param genericClassDefinition : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.InheritsGenericDefinition(type, genericClassDefinition)
end

---@param type : CS.System.Type
---@param genericClassDefinition : CS.System.Type
---@param implementingType : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.InheritsGenericDefinition(type, genericClassDefinition, implementingType)
end

---@param type : CS.System.Type
---@return CS.System.Type
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetCollectionItemType(type)
end

---@param dictionaryType : CS.System.Type
---@param keyType : CS.System.Type
---@param valueType : CS.System.Type
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetDictionaryKeyValueTypes(dictionaryType, keyType, valueType)
end

---@param dictionaryType : CS.System.Type
---@return CS.System.Type
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetDictionaryValueType(dictionaryType)
end

---@param dictionaryType : CS.System.Type
---@return CS.System.Type
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetDictionaryKeyType(dictionaryType)
end

---@param member : CS.System.Reflection.MemberInfo
---@return CS.System.Type
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetMemberUnderlyingType(member)
end

---@param member : CS.System.Reflection.MemberInfo
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsIndexedProperty(member)
end

---@param property : CS.System.Reflection.PropertyInfo
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsIndexedProperty(property)
end

---@param member : CS.System.Reflection.MemberInfo
---@param target : CS.System.Object
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetMemberValue(member, target)
end

---@param member : CS.System.Reflection.MemberInfo
---@param target : CS.System.Object
---@param value : CS.System.Object
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.SetMemberValue(member, target, value)
end

---@param member : CS.System.Reflection.MemberInfo
---@param nonPublic : CS.System.Boolean
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.CanReadMemberValue(member, nonPublic)
end

---@param member : CS.System.Reflection.MemberInfo
---@param nonPublic : CS.System.Boolean
---@param canSetReadOnly : CS.System.Boolean
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.CanSetMemberValue(member, nonPublic, canSetReadOnly)
end

---@param type : CS.System.Type
---@param bindingAttr : CS.System.Reflection.BindingFlags
---@return CS.System.Collections.Generic.List
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetFieldsAndProperties(type, bindingAttr)
end

---@param t : CS.System.Type
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetNameAndAssessmblyName(t)
end

---@param genericTypeDefinition : CS.System.Type
---@param innerTypes : CS.System.Type[]
---@return CS.System.Type
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.MakeGenericType(genericTypeDefinition, innerTypes)
end

---@param genericTypeDefinition : CS.System.Type
---@param innerType : CS.System.Type
---@param args : CS.System.Object[]
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.CreateGeneric(genericTypeDefinition, innerType, args)
end

---@param genericTypeDefinition : CS.System.Type
---@param innerTypes : CS.System.Collections.Generic.IList
---@param args : CS.System.Object[]
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.CreateGeneric(genericTypeDefinition, innerTypes, args)
end

---@param genericTypeDefinition : CS.System.Type
---@param innerTypes : CS.System.Collections.Generic.IList
---@param instanceCreator : CS.System.Func
---@param args : CS.System.Object[]
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.CreateGeneric(genericTypeDefinition, innerTypes, instanceCreator, args)
end

---@param value : CS.System.Object
---@param type : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.IsCompatibleValue(value, type)
end

---@param type : CS.System.Type
---@param args : CS.System.Object[]
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.CreateInstance(type, args)
end

---@param fullyQualifiedTypeName : CS.System.String
---@param typeName : CS.System.String
---@param assemblyName : CS.System.String
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.SplitFullyQualifiedTypeName(fullyQualifiedTypeName, typeName, assemblyName)
end

---@param targetType : CS.System.Type
---@param memberInfo : CS.System.Reflection.MemberInfo
---@return CS.System.Reflection.MemberInfo
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetMemberInfoFromType(targetType, memberInfo)
end

---@param targetType : CS.System.Type
---@param bindingAttr : CS.System.Reflection.BindingFlags
---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetFields(targetType, bindingAttr)
end

---@param targetType : CS.System.Type
---@param bindingAttr : CS.System.Reflection.BindingFlags
---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.GetProperties(targetType, bindingAttr)
end

---@param bindingAttr : CS.System.Reflection.BindingFlags
---@param flag : CS.System.Reflection.BindingFlags
---@return CS.System.Reflection.BindingFlags
function CS.Newtonsoft.Json.Utilities.ReflectionUtils.RemoveFlag(bindingAttr, flag)
end