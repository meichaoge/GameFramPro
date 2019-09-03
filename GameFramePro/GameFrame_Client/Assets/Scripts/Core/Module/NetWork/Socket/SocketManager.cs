using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using System;

namespace GameFramePro.NetWorkEx
{

    /// <summary>   /// 对内接受和缓存接受的数据 对外提供必要的访问接口  /// </summary>
    public class SocketManager : Single<SocketManager>
    {

        #region 提供 SocketHead
        private readonly ConcurrentQueue<SocketHead> mSocketHeadQueue = new ConcurrentQueue<SocketHead>(); //复用的Socket Head
        private static object S_GetSocketHeadLocker = new object();

        public SocketHead GetSocketHead()
        {
            lock (S_GetSocketHeadLocker)
            {
                if (mSocketHeadQueue.Count == 0)
                    return new SocketHead();
                if (mSocketHeadQueue.TryDequeue(out var socketHead))
                {
                    socketHead.Reset();
                    return socketHead;
                }
                return new SocketHead();
            }
        }

        public void RecycleSocketHead(SocketHead socketHead)
        {
            if (socketHead == null)
                throw new ArgumentException($"参数 为 null {socketHead}");
            mSocketHeadQueue.Enqueue(socketHead);
        }
        #endregion

    }

}