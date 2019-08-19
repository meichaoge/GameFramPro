//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Security.Permissions;
//using System.Threading.Tasks;
//
//
//namespace GameFramePro.NetWorkEx
//{
//    public class UdpClient
//    {
//        private byte[] m_Buffer = new byte[65536];
//        private AddressFamily m_Family = AddressFamily.InterNetwork;
//        private const int MaxUDPSize = 65536;
//        private Socket m_ClientSocket;
//        private bool m_Active;
//        private bool m_CleanedUp;
//        private bool m_IsBroadcast;
//
//        public UdpClient()
//            : this(AddressFamily.InterNetwork)
//        {
//        }
//
//        public UdpClient(AddressFamily family)
//        {
//            if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
//                throw new ArgumentException(SR.GetString("net_protocol_invalid_family", (object) "UDP"), nameof(family));
//            this.m_Family = family;
//            this.createClientSocket();
//        }
//
//        public UdpClient(int port)
//            : this(port, AddressFamily.InterNetwork)
//        {
//        }
//
//        public UdpClient(int port, AddressFamily family)
//        {
//            if (!ValidationHelper.ValidateTcpPort(port))
//                throw new ArgumentOutOfRangeException(nameof(port));
//            if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
//                throw new ArgumentException(SR.GetString("net_protocol_invalid_family"), nameof(family));
//            this.m_Family = family;
//            IPEndPoint ipEndPoint = this.m_Family != AddressFamily.InterNetwork ? new IPEndPoint(IPAddress.IPv6Any, port) : new IPEndPoint(IPAddress.Any, port);
//            this.createClientSocket();
//            this.Client.Bind((EndPoint) ipEndPoint);
//        }
//
//        public UdpClient(IPEndPoint localEP)
//        {
//            if (localEP == null)
//                throw new ArgumentNullException(nameof(localEP));
//            this.m_Family = localEP.AddressFamily;
//            this.createClientSocket();
//            this.Client.Bind((EndPoint) localEP);
//        }
//
//        public UdpClient(string hostname, int port)
//        {
//            if (hostname == null)
//                throw new ArgumentNullException(nameof(hostname));
//            if (!ValidationHelper.ValidateTcpPort(port))
//                throw new ArgumentOutOfRangeException(nameof(port));
//            this.Connect(hostname, port);
//        }
//
//        public Socket Client
//        {
//            get { return this.m_ClientSocket; }
//            set { this.m_ClientSocket = value; }
//        }
//
//        protected bool Active
//        {
//            get { return this.m_Active; }
//            set { this.m_Active = value; }
//        }
//
//        public int Available
//        {
//            get { return this.m_ClientSocket.Available; }
//        }
//
//        public short Ttl
//        {
//            get { return this.m_ClientSocket.Ttl; }
//            set { this.m_ClientSocket.Ttl = value; }
//        }
//
//        public bool DontFragment
//        {
//            get { return this.m_ClientSocket.DontFragment; }
//            set { this.m_ClientSocket.DontFragment = value; }
//        }
//
//        public bool MulticastLoopback
//        {
//            get { return this.m_ClientSocket.MulticastLoopback; }
//            set { this.m_ClientSocket.MulticastLoopback = value; }
//        }
//
//        public bool EnableBroadcast
//        {
//            get { return this.m_ClientSocket.EnableBroadcast; }
//            set { this.m_ClientSocket.EnableBroadcast = value; }
//        }
//
//        public bool ExclusiveAddressUse
//        {
//            get { return this.m_ClientSocket.ExclusiveAddressUse; }
//            set { this.m_ClientSocket.ExclusiveAddressUse = value; }
//        }
//
//        public void AllowNatTraversal(bool allowed)
//        {
//            if (allowed)
//                this.m_ClientSocket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
//            else
//                this.m_ClientSocket.SetIPProtectionLevel(IPProtectionLevel.EdgeRestricted);
//        }
//
//        public void Close()
//        {
//            this.Dispose(true);
//        }
//
//        private void FreeResources()
//        {
//            if (this.m_CleanedUp)
//                return;
//            Socket client = this.Client;
//            if (client != null)
//            {
//                client.InternalShutdown(SocketShutdown.Both);
//                client.Close();
//                this.Client = (Socket) null;
//            }
//
//            this.m_CleanedUp = true;
//        }
//
//        public void Dispose()
//        {
//            this.Dispose(true);
//        }
//
//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposing)
//                return;
//            this.FreeResources();
//            GC.SuppressFinalize((object) this);
//        }
//
//        public void Connect(string hostname, int port)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (hostname == null)
//                throw new ArgumentNullException(nameof(hostname));
//            if (!ValidationHelper.ValidateTcpPort(port))
//                throw new ArgumentOutOfRangeException(nameof(port));
//            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
//            Exception exception = (Exception) null;
//            Socket socket1 = (Socket) null;
//            Socket socket2 = (Socket) null;
//            try
//            {
//                if (this.m_ClientSocket == null)
//                {
//                    if (Socket.OSSupportsIPv4)
//                        socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//                    if (Socket.OSSupportsIPv6)
//                        socket1 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
//                }
//
//                foreach (IPAddress address in hostAddresses)
//                {
//                    try
//                    {
//                        if (this.m_ClientSocket == null)
//                        {
//                            if (address.AddressFamily == AddressFamily.InterNetwork && socket2 != null)
//                            {
//                                socket2.Connect(address, port);
//                                this.m_ClientSocket = socket2;
//                                socket1?.Close();
//                            }
//                            else if (socket1 != null)
//                            {
//                                socket1.Connect(address, port);
//                                this.m_ClientSocket = socket1;
//                                socket2?.Close();
//                            }
//
//                            this.m_Family = address.AddressFamily;
//                            this.m_Active = true;
//                            break;
//                        }
//
//                        if (address.AddressFamily == this.m_Family)
//                        {
//                            this.Connect(new IPEndPoint(address, port));
//                            this.m_Active = true;
//                            break;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        if (NclUtilities.IsFatal(ex))
//                            throw;
//                        else
//                            exception = ex;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                if (NclUtilities.IsFatal(ex))
//                    throw;
//                else
//                    exception = ex;
//            }
//            finally
//            {
//                if (!this.m_Active)
//                {
//                    socket1?.Close();
//                    socket2?.Close();
//                    if (exception != null)
//                        throw exception;
//                    throw new SocketException(SocketError.NotConnected);
//                }
//            }
//        }
//
//        public void Connect(IPAddress addr, int port)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (addr == null)
//                throw new ArgumentNullException(nameof(addr));
//            if (!ValidationHelper.ValidateTcpPort(port))
//                throw new ArgumentOutOfRangeException(nameof(port));
//            this.Connect(new IPEndPoint(addr, port));
//        }
//
//        public void Connect(IPEndPoint endPoint)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (endPoint == null)
//                throw new ArgumentNullException(nameof(endPoint));
//            this.CheckForBroadcast(endPoint.Address);
//            this.Client.Connect((EndPoint) endPoint);
//            this.m_Active = true;
//        }
//
//        private void CheckForBroadcast(IPAddress ipAddress)
//        {
//            if (this.Client == null || this.m_IsBroadcast || !ipAddress.IsBroadcast)
//                return;
//            this.m_IsBroadcast = true;
//            this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
//        }
//
//        public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (dgram == null)
//                throw new ArgumentNullException(nameof(dgram));
//            if (this.m_Active && endPoint != null)
//                throw new InvalidOperationException(SR.GetString("net_udpconnected"));
//            if (endPoint == null)
//                return this.Client.Send(dgram, 0, bytes, SocketFlags.None);
//            this.CheckForBroadcast(endPoint.Address);
//            return this.Client.SendTo(dgram, 0, bytes, SocketFlags.None, (EndPoint) endPoint);
//        }
//
//        public int Send(byte[] dgram, int bytes, string hostname, int port)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (dgram == null)
//                throw new ArgumentNullException(nameof(dgram));
//            if (this.m_Active && (hostname != null || port != 0))
//                throw new InvalidOperationException(SR.GetString("net_udpconnected"));
//            if (hostname == null || port == 0)
//                return this.Client.Send(dgram, 0, bytes, SocketFlags.None);
//            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
//            int index = 0;
//            while (index < hostAddresses.Length && hostAddresses[index].AddressFamily != this.m_Family)
//                ++index;
//            if (hostAddresses.Length == 0 || index == hostAddresses.Length)
//                throw new ArgumentException(SR.GetString("net_invalidAddressList"), nameof(hostname));
//            this.CheckForBroadcast(hostAddresses[index]);
//            IPEndPoint ipEndPoint = new IPEndPoint(hostAddresses[index], port);
//            return this.Client.SendTo(dgram, 0, bytes, SocketFlags.None, (EndPoint) ipEndPoint);
//        }
//
//        public int Send(byte[] dgram, int bytes)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (dgram == null)
//                throw new ArgumentNullException(nameof(dgram));
//            if (!this.m_Active)
//                throw new InvalidOperationException(SR.GetString("net_notconnected"));
//            return this.Client.Send(dgram, 0, bytes, SocketFlags.None);
//        }
//
//        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//        public IAsyncResult BeginSend(
//            byte[] datagram,
//            int bytes,
//            IPEndPoint endPoint,
//            AsyncCallback requestCallback,
//            object state)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (datagram == null)
//                throw new ArgumentNullException(nameof(datagram));
//            if (bytes > datagram.Length || bytes < 0)
//                throw new ArgumentOutOfRangeException(nameof(bytes));
//            if (this.m_Active && endPoint != null)
//                throw new InvalidOperationException(SR.GetString("net_udpconnected"));
//            if (endPoint == null)
//                return this.Client.BeginSend(datagram, 0, bytes, SocketFlags.None, requestCallback, state);
//            this.CheckForBroadcast(endPoint.Address);
//            return this.Client.BeginSendTo(datagram, 0, bytes, SocketFlags.None, (EndPoint) endPoint, requestCallback, state);
//        }
//
//        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//        public IAsyncResult BeginSend(
//            byte[] datagram,
//            int bytes,
//            string hostname,
//            int port,
//            AsyncCallback requestCallback,
//            object state)
//        {
//            if (this.m_Active && (hostname != null || port != 0))
//                throw new InvalidOperationException(SR.GetString("net_udpconnected"));
//            IPEndPoint endPoint = (IPEndPoint) null;
//            if (hostname != null && port != 0)
//            {
//                IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
//                int index = 0;
//                while (index < hostAddresses.Length && hostAddresses[index].AddressFamily != this.m_Family)
//                    ++index;
//                if (hostAddresses.Length == 0 || index == hostAddresses.Length)
//                    throw new ArgumentException(SR.GetString("net_invalidAddressList"), nameof(hostname));
//                this.CheckForBroadcast(hostAddresses[index]);
//                endPoint = new IPEndPoint(hostAddresses[index], port);
//            }
//
//            return this.BeginSend(datagram, bytes, endPoint, requestCallback, state);
//        }
//
//        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//        public IAsyncResult BeginSend(
//            byte[] datagram,
//            int bytes,
//            AsyncCallback requestCallback,
//            object state)
//        {
//            return this.BeginSend(datagram, bytes, (IPEndPoint) null, requestCallback, state);
//        }
//
//        public int EndSend(IAsyncResult asyncResult)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (this.m_Active)
//                return this.Client.EndSend(asyncResult);
//            return this.Client.EndSendTo(asyncResult);
//        }
//
//        public byte[] Receive(ref IPEndPoint remoteEP)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            EndPoint remoteEP1 = this.m_Family != AddressFamily.InterNetwork ? (EndPoint) IPEndPoint.IPv6Any : (EndPoint) IPEndPoint.Any;
//            int from = this.Client.ReceiveFrom(this.m_Buffer, 65536, SocketFlags.None, ref remoteEP1);
//            remoteEP = (IPEndPoint) remoteEP1;
//            if (from >= 65536)
//                return this.m_Buffer;
//            byte[] numArray = new byte[from];
//            Buffer.BlockCopy((Array) this.m_Buffer, 0, (Array) numArray, 0, from);
//            return numArray;
//        }
//
//        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//        public IAsyncResult BeginReceive(AsyncCallback requestCallback, object state)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            EndPoint remoteEP = this.m_Family != AddressFamily.InterNetwork ? (EndPoint) IPEndPoint.IPv6Any : (EndPoint) IPEndPoint.Any;
//            return this.Client.BeginReceiveFrom(this.m_Buffer, 0, 65536, SocketFlags.None, ref remoteEP, requestCallback, state);
//        }
//
//        public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            EndPoint endPoint = this.m_Family != AddressFamily.InterNetwork ? (EndPoint) IPEndPoint.IPv6Any : (EndPoint) IPEndPoint.Any;
//            int from = this.Client.EndReceiveFrom(asyncResult, ref endPoint);
//            remoteEP = (IPEndPoint) endPoint;
//            if (from >= 65536)
//                return this.m_Buffer;
//            byte[] numArray = new byte[from];
//            Buffer.BlockCopy((Array) this.m_Buffer, 0, (Array) numArray, 0, from);
//            return numArray;
//        }
//
//        public void JoinMulticastGroup(IPAddress multicastAddr)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (multicastAddr == null)
//                throw new ArgumentNullException(nameof(multicastAddr));
//            if (multicastAddr.AddressFamily != this.m_Family)
//                throw new ArgumentException(SR.GetString("net_protocol_invalid_multicast_family", (object) "UDP"), nameof(multicastAddr));
//            if (this.m_Family == AddressFamily.InterNetwork)
//                this.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, (object) new MulticastOption(multicastAddr));
//            else
//                this.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, (object) new IPv6MulticastOption(multicastAddr));
//        }
//
//        public void JoinMulticastGroup(IPAddress multicastAddr, IPAddress localAddress)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (this.m_Family != AddressFamily.InterNetwork)
//                throw new SocketException(SocketError.OperationNotSupported);
//            this.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, (object) new MulticastOption(multicastAddr, localAddress));
//        }
//
//        public void JoinMulticastGroup(int ifindex, IPAddress multicastAddr)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (multicastAddr == null)
//                throw new ArgumentNullException(nameof(multicastAddr));
//            if (ifindex < 0)
//                throw new ArgumentException(SR.GetString("net_value_cannot_be_negative"), nameof(ifindex));
//            if (this.m_Family != AddressFamily.InterNetworkV6)
//                throw new SocketException(SocketError.OperationNotSupported);
//            this.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, (object) new IPv6MulticastOption(multicastAddr, (long) ifindex));
//        }
//
//        public void JoinMulticastGroup(IPAddress multicastAddr, int timeToLive)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (multicastAddr == null)
//                throw new ArgumentNullException(nameof(multicastAddr));
//            if (!ValidationHelper.ValidateRange(timeToLive, 0, (int) byte.MaxValue))
//                throw new ArgumentOutOfRangeException(nameof(timeToLive));
//            this.JoinMulticastGroup(multicastAddr);
//            this.Client.SetSocketOption(this.m_Family == AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, timeToLive);
//        }
//
//        public void DropMulticastGroup(IPAddress multicastAddr)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (multicastAddr == null)
//                throw new ArgumentNullException(nameof(multicastAddr));
//            if (multicastAddr.AddressFamily != this.m_Family)
//                throw new ArgumentException(SR.GetString("net_protocol_invalid_multicast_family", (object) "UDP"), nameof(multicastAddr));
//            if (this.m_Family == AddressFamily.InterNetwork)
//                this.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, (object) new MulticastOption(multicastAddr));
//            else
//                this.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, (object) new IPv6MulticastOption(multicastAddr));
//        }
//
//        public void DropMulticastGroup(IPAddress multicastAddr, int ifindex)
//        {
//            if (this.m_CleanedUp)
//                throw new ObjectDisposedException(this.GetType().FullName);
//            if (multicastAddr == null)
//                throw new ArgumentNullException(nameof(multicastAddr));
//            if (ifindex < 0)
//                throw new ArgumentException(SR.GetString("net_value_cannot_be_negative"), nameof(ifindex));
//            if (this.m_Family != AddressFamily.InterNetworkV6)
//                throw new SocketException(SocketError.OperationNotSupported);
//            this.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, (object) new IPv6MulticastOption(multicastAddr, (long) ifindex));
//        }
//
//        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//        public Task<int> SendAsync(byte[] datagram, int bytes)
//        {
//            return Task<int>.Factory.FromAsync<byte[], int>(new Func<byte[], int, AsyncCallback, object, IAsyncResult>(this.BeginSend), new Func<IAsyncResult, int>(this.EndSend), datagram, bytes, (object) null);
//        }
//
//        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//        public Task<int> SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint)
//        {
//            return Task<int>.Factory.FromAsync<byte[], int, IPEndPoint>(new Func<byte[], int, IPEndPoint, AsyncCallback, object, IAsyncResult>(this.BeginSend), new Func<IAsyncResult, int>(this.EndSend), datagram, bytes, endPoint, (object) null);
//        }
//
//        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//        public Task<int> SendAsync(byte[] datagram, int bytes, string hostname, int port)
//        {
//            return Task<int>.Factory.FromAsync((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => this.BeginSend(datagram, bytes, hostname, port, callback, state)), new Func<IAsyncResult, int>(this.EndSend), (object) null);
//        }
//
//        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//        public Task<UdpReceiveResult> ReceiveAsync()
//        {
//            return Task<UdpReceiveResult>.Factory.FromAsync((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => this.BeginReceive(callback, state)), (Func<IAsyncResult, UdpReceiveResult>) (ar =>
//            {
//                IPEndPoint remoteEP = (IPEndPoint) null;
//                return new UdpReceiveResult(this.EndReceive(ar, ref remoteEP), remoteEP);
//            }), (object) null);
//        }
//
//        private void createClientSocket()
//        {
//            this.Client = new Socket(this.m_Family, SocketType.Dgram, ProtocolType.Udp);
//        }
//    }
//}
