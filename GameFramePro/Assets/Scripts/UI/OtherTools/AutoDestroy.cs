using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 延时自毁
    /// </summary>
    public class AutoDestroy : MonoBehaviour
    {
        public float destroyTime = 1f;

        void Start()
        {
            Destroy(this.gameObject, destroyTime);
        }
    }
}