using UnityEngine;
using XLua;

namespace XLuaTest
{
    [Hotfix]
    public class HotfixCalc
    {
        public int Add(int a, int b)
        {
            return a - b;
        }

        public Vector3 Add(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        public int TestOut(int a, out double b, ref string c)
        {
            b = a + 2;
            c = "wrong version";
            return a + 3;
        }

        public int TestOut(int a, out double b, ref string c, GameObject go)
        {
            return TestOut(a, out b, ref c);
        }

        public T Test1<T>()
        {
            return default(T);
        }

        public T1 Test2<T1, T2, T3>(T1 a, out T2 b, ref T3 c)
        {
            b = default(T2);
            return a;
        }

        public static int Test3<T>(T a)
        {
            return 0;
        }

        public static void Test4<T>(T a)
        {
        }

        public void Test5<T>(int a, params T[] arg)
        {

        }
    }

    public class NoHotfixCalc
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    [Hotfix]
    public class GenericClass<T>
    {
        T a;

        public GenericClass(T a)
        {
            this.a = a;
        }

        public void Func1()
        {
            Debug.Log("a=" + a);
        }

        public T Func2()
        {
            return default(T);
        }
    }

    [Hotfix]
    public class InnerTypeTest
    {
        public void Foo()
        {
            _InnerStruct ret = Bar();
            Debug.Log("x=" + ret.x + ",y= " + ret.y );
        }

        struct _InnerStruct
        {
            public int x;
            public int y;
        }

        _InnerStruct Bar()
        {
            return new _InnerStruct { x = 1, y = 2 };
        }
    }

    public class BaseTestHelper
    {

    }

    public class BaseTestBase<T> : BaseTestHelper
    {
        public virtual void Foo(int p)
        {
            Debug.Log("BaseTestBase<>.Foo, p = " + p);
        }
    }

    [Hotfix]
    [LuaCallCSharp]
    public class BaseTest : BaseTestBase<InnerTypeTest>
    {
        public override void Foo(int p)
        {
            Debug.Log("BaseTest<>.Foo, p = " + p);
        }

        public void Proxy(int p)
        {
            base.Foo(p);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    [Hotfix]
    public struct StructTest
    {
        GameObject go;
        public StructTest(GameObject go)
        {
            this.go = go;
        }

        public GameObject GetGo(int a, object b)
        {
            return go;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string Proxy()
        {
            return base.ToString();
        }
    }

    [Hotfix]
    public struct GenericStruct<T>
    {
        T a;

        public GenericStruct(T a)
        {
            this.a = a;
        }

        public T GetA(int p)
        {
            return a;
        }
    }

 
}

