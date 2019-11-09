using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace GenericMethodTest
{
    /// <summary>
    /// 接口生成器
    /// </summary>
    internal static class InterfaceGenerator
    {
        private static Random _Random = new Random();

        private static char GetRandomLetter()
        {
            int i = (_Random.Next() % 26) + 97;
            byte[] b = BitConverter.GetBytes(i);
            return BitConverter.ToChar(b, 0);
        }

        private static string GetRandomString(int n)
        {
            char[] chars = new char[n];
            for (int i = 0; i < n; i++)
            {
                chars[i] = GetRandomLetter();
            }
            return new string(chars);
        }

        private static void LoadArg(ILGenerator gen, int index)
        {
            switch (index)
            {
                case 0:
                    gen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    gen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index < 128)
                    {
                        gen.Emit(OpCodes.Ldarg_S, index);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Ldarg, index);
                    }
                    break;
            }
        }

        public static T GetInterface<T>(Delegate GM)
        {
            if (typeof(T).IsInterface)
            {
                Type delegateType = GM.GetType();
                if (delegateType.IsGenericType)
                {
                    if (typeof(MulticastDelegate).IsAssignableFrom(delegateType.GetGenericTypeDefinition()))
                    {
                        Type[] genericTypes = delegateType.GetGenericArguments();
                        if (genericTypes.Length == 1)
                        {
                            Type genericType = genericTypes[0];

#if SAVE
                            string theFilename = "InterfaceGenerator.Attachments.dll";
#endif
                            AssemblyName aname = new AssemblyName();
                            aname.Name = string.Format("InterfaceGenerator.Attachments.{0}", GetRandomString(16));
                            aname.Version = new Version("2.0.0.0");
                            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(aname,
#if SAVE
 AssemblyBuilderAccess.RunAndSave
#else
 AssemblyBuilderAccess.Run
#endif
);
                            ModuleBuilder module = assembly.DefineDynamicModule(GetRandomString(8)
#if SAVE
, theFilename
#endif
);
                            TypeBuilder builder = module.DefineType(GetRandomString(16), TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.Public);
                            builder.AddInterfaceImplementation(typeof(T));

                            // 先定义成员域，用于保存传入的委托。
                            FieldBuilder field = builder.DefineField(GetRandomString(8), delegateType, FieldAttributes.Private);

                            // 定义构造器。
                            ConstructorBuilder ctor = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { delegateType });
                            ILGenerator ctorGen = ctor.GetILGenerator();
                            ctorGen.Emit(OpCodes.Ldarg_0);
                            ctorGen.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[] { }));
                            ctorGen.Emit(OpCodes.Ldarg_0);
                            ctorGen.Emit(OpCodes.Ldarg_1);
                            ctorGen.Emit(OpCodes.Stfld, field);
                            ctorGen.Emit(OpCodes.Ret);

                            // 虽然这么写，但事实上接口只有一个方法。
                            foreach (MethodInfo bmi in typeof(T).GetMethods())
                            {
                                ParameterInfo[] paramInfos = bmi.GetParameters();
                                Type[] argTypes = new Type[paramInfos.Length];
                                int i = 0;
                                foreach (ParameterInfo pi in paramInfos)
                                {
                                    argTypes[i++] = pi.ParameterType;
                                }
                                MethodAttributes attributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.ReuseSlot | MethodAttributes.Public;
                                MethodBuilder method = builder.DefineMethod(bmi.Name, attributes, bmi.ReturnType, argTypes);
                                builder.DefineMethodOverride(method, bmi);
                                MethodInfo dmi = delegateType.GetMethod("Invoke");
                                ILGenerator methodGen = method.GetILGenerator();
                                bool hasReturn = false;
                                if (dmi.ReturnType != typeof(void))
                                {
                                    methodGen.DeclareLocal(dmi.ReturnType);
                                    hasReturn = true;
                                }
                                methodGen.Emit(OpCodes.Ldarg_0);
                                methodGen.Emit(OpCodes.Ldfld, field);

                                i = 0;
                                foreach (ParameterInfo pi in dmi.GetParameters())
                                {
                                    LoadArg(methodGen, i + 1);
                                    if (!pi.ParameterType.IsAssignableFrom(argTypes[i]))
                                    {
                                        if (argTypes[i].IsClass)
                                        {
                                            methodGen.Emit(OpCodes.Castclass, pi.ParameterType);
                                        }
                                        else
                                        {
                                            methodGen.Emit(OpCodes.Unbox, pi.ParameterType);
                                        }
                                    }
                                    i++;
                                }
                                methodGen.Emit(OpCodes.Callvirt, dmi);
                                if (hasReturn)
                                {
                                    methodGen.Emit(OpCodes.Stloc_0);
                                    methodGen.Emit(OpCodes.Ldloc_0);
                                }
                                methodGen.Emit(OpCodes.Ret);
                            }
                            Type target = builder.CreateType();
#if SAVE
                            assembly.Save(theFilename);
#endif
                            ConstructorInfo ci = target.GetConstructor(new Type[] { delegateType });
                            return (T)ci.Invoke(new object[] { GM });
                        }
                    }
                }
            }
            return default(T);
        }
    }
}
