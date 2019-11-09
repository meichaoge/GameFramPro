using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace GenericMethodTest
{
    // 为泛型方法定义的委托
    public delegate void GM<T>(T obj, IList<T> list);

    // 为非泛型方法定义的接口
    public interface ING
    {
        void NGM(object obj, object list);
    }

    public class Test00123 : MonoBehaviour
    {
        void Start()
        {
            List<int> list = new List<int>();
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Reset();
            watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                list.Add(i);
            }
            watch.Stop();
            long l1 = watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();
            GM<int> gm = new GM<int>(Test00123.Add);
            for (int i = 0; i < 1000000; i++)
            {
                gm(i, list);
            }
            watch.Stop();
            long l2 = watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();
            MethodInfo mi = typeof(Test00123).GetMethods().Single(x =>
            {
                string name = x.Name;
                var p = x.GetParameters();
                var g = x.GetGenericArguments();
                return x.Name == "Add" &&
                  p.Length == 2 &&
                  g.Length == 1;
                //    p[0].ParameterType == genericType &&
                //  p[1].ParameterType == typeof(IList<>).MakeGenericType(g);
                //   p[2].ParameterType == typeof(Attribute[]);
            });

            //    .GetMethod("Add");
            MethodInfo gmi = mi.MakeGenericMethod(typeof(int));
            for (int i = 0; i < 1000000; i++)
            {
                gmi.Invoke(null, new object[] { i, list });
            }
            watch.Stop();
            long l3 = watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();
            ING ng1 = GetNGC(typeof(int), typeof(Test00123), "Add");
            for (int i = 0; i < 1000000; i++)
            {
                ng1.NGM(i, list);
            }
            watch.Stop();
            long l4 = watch.ElapsedMilliseconds;
            //watch.Reset();
            //watch.Start();
            //ING ng2 = InterfaceGenerator.GetInterface<ING>(new GM<int>(Test00123.Add));
            //for (int i = 0; i < 1000000; i++)
            //{
            //    ng2.NGM(i, list);
            //}
            //watch.Stop();
            //long l5 = watch.ElapsedMilliseconds;
            //    Debug.login("{0}\n{1} vs {2} vs {3} vs {4} vs {5}", list.Count, l1, l2, l3, l4, l5);
            //Console.ReadLine();
            Debug.LogFormat("{0}\n{1} vs {2} vs {3} vs {4} vs ", list.Count, l1, l2, l3, l4);
        }

        public static void Add<T>(T obj, IList<T> list)
        {
            list.Add(obj);
        }

        public static void Add<T,u>(T obj, IList<T> list,u aadd)
        {
            list.Add(obj);
        }

        static ING GetNGC(Type genericType, Type methodType, string methodName)
        {

            MethodInfo mi = methodType.GetMethods().Single(x =>
            {
                string name = x.Name;
                var p = x.GetParameters();
                var g = x.GetGenericArguments();
                return x.Name == methodName &&
                  p.Length == 2 &&
                  g.Length == 1;
              //    p[0].ParameterType == genericType &&
                //  p[1].ParameterType == typeof(IList<>).MakeGenericType(g);
               //   p[2].ParameterType == typeof(Attribute[]);
            });


         //  MethodInfo mi = methodType.GetMethod(methodName);
            MethodInfo gmi = mi.MakeGenericMethod(genericType);
            Delegate gmd = Delegate.CreateDelegate(typeof(GM<>).MakeGenericType(genericType), gmi);
            return Activator.CreateInstance(typeof(GClass<>).MakeGenericType(genericType), gmd) as ING;
        }
    }

    public class GClass<T> : ING
    {
        private GM<T> m_gmd;

        public GClass(GM<T> gmd)
        {
            m_gmd = gmd;
        }
        #region INGClass 成员

        public void NGM(object obj, object list)
        {
            m_gmd((T)obj, (IList<T>)list);
        }

        #endregion
    }




}