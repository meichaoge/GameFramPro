//// Decompiled with JetBrains decompiler
//// Type: System.Net.Sockets.UdpClient
//// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
//// MVID: 978BB286-3A1E-4C66-A74B-72341E3A291D
//// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.dll
//
//using System.Security.Permissions;
//using System.Threading.Tasks;
//
//namespace System.Net.Sockets
//{
//  /// <summary>提供用户数据报协议 (UDP) 网络服务。</summary>
//  public class UdpClient2 : IDisposable
//  {
//    private byte[] m_Buffer = new byte[65536];
//    private AddressFamily m_Family = AddressFamily.InterNetwork;
//    private const int MaxUDPSize = 65536;
//    private Socket m_ClientSocket;
//    private bool m_Active;
//    private bool m_CleanedUp;
//    private bool m_IsBroadcast;
//
//    /// <summary>
//    ///   初始化 <see cref="T:System.Net.Sockets.UdpClient" /> 类的新实例。
//    /// </summary>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public UdpClient2()
//      : this(AddressFamily.InterNetwork)
//    {
//    }
//
//    /// <summary>
//    ///   初始化 <see cref="T:System.Net.Sockets.UdpClient" /> 类的新实例。
//    /// </summary>
//    /// <param name="family">
//    ///   其中一个 <see cref="T:System.Net.Sockets.AddressFamily" /> 值，该值指定套接字的寻址方案。
//    /// </param>
//    /// <exception cref="T:System.ArgumentException">
//    ///   <paramref name="family" /> 不是 <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> 或 <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" />。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public UdpClient2(AddressFamily family)
//    {
//      if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
//        throw new ArgumentException(SR.GetString("net_protocol_invalid_family", (object) "UDP"), nameof (family));
//      this.m_Family = family;
//      this.createClientSocket();
//    }
//
//    /// <summary>
//    ///   新实例初始化 <see cref="T:System.Net.Sockets.UdpClient" /> 类，并将其绑定到提供的本地端口号。
//    /// </summary>
//    /// <param name="port">你想要进行通信的本地端口号。</param>
//    /// <exception cref="T:System.ArgumentOutOfRangeException">
//    ///   <paramref name="port" /> 参数是否大于 <see cref="F:System.Net.IPEndPoint.MaxPort" /> 或小于 <see cref="F:System.Net.IPEndPoint.MinPort" />。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public UdpClient2(int port)
//      : this(port, AddressFamily.InterNetwork)
//    {
//    }
//
//    /// <summary>
//    ///   新实例初始化 <see cref="T:System.Net.Sockets.UdpClient" /> 类，并将其绑定到提供的本地端口号。
//    /// </summary>
//    /// <param name="port">用于侦听传入的连接尝试的端口。</param>
//    /// <param name="family">
//    ///   其中一个 <see cref="T:System.Net.Sockets.AddressFamily" /> 值，该值指定套接字的寻址方案。
//    /// </param>
//    /// <exception cref="T:System.ArgumentException">
//    ///   <paramref name="family" /> 不是 <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> 或 <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" />。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentOutOfRangeException">
//    ///   <paramref name="port" /> 大于 <see cref="F:System.Net.IPEndPoint.MaxPort" /> 或小于 <see cref="F:System.Net.IPEndPoint.MinPort" />。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public UdpClient2(int port, AddressFamily family)
//    {
////      if (!ValidationHelper.ValidateTcpPort(port))
////        throw new ArgumentOutOfRangeException(nameof (port));
////      if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
////        throw new ArgumentException(SR.GetString("net_protocol_invalid_family"), nameof (family));
//      this.m_Family = family;
//      IPEndPoint ipEndPoint = this.m_Family != AddressFamily.InterNetwork ? new IPEndPoint(IPAddress.IPv6Any, port) : new IPEndPoint(IPAddress.Any, port);
//      this.createClientSocket();
//      this.Client.Bind((EndPoint) ipEndPoint);
//    }
//
//    /// <summary>
//    ///   初始化 <see cref="T:System.Net.Sockets.UdpClient" /> 类的新实例，并将其绑定到指定的本地终结点。
//    /// </summary>
//    /// <param name="localEP">
//    ///   <see cref="T:System.Net.IPEndPoint" />表示你要向其绑定 UDP 连接的本地终结点。
//    /// </param>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="localEP" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public UdpClient2(IPEndPoint localEP)
//    {
//      if (localEP == null)
//        throw new ArgumentNullException(nameof (localEP));
//      this.m_Family = localEP.AddressFamily;
//      this.createClientSocket();
//      this.Client.Bind((EndPoint) localEP);
//    }
//
//    /// <summary>
//    ///   新实例初始化 <see cref="T:System.Net.Sockets.UdpClient" /> 类，并建立默认远程主机。
//    /// </summary>
//    /// <param name="hostname">您想要连接的远程 DNS 主机的名称。</param>
//    /// <param name="port">您想要连接远程端口号。</param>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="hostname" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentOutOfRangeException">
//    ///   <paramref name="port" /> 不介于 <see cref="F:System.Net.IPEndPoint.MinPort" /> 和 <see cref="F:System.Net.IPEndPoint.MaxPort" />。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public UdpClient2(string hostname, int port)
//    {
//      if (hostname == null)
//        throw new ArgumentNullException(nameof (hostname));
////      if (!ValidationHelper.ValidateTcpPort(port))
////        throw new ArgumentOutOfRangeException(nameof (port));
//      this.Connect(hostname, port);
//    }
//
//    /// <summary>
//    ///   获取或设置基础网络 <see cref="T:System.Net.Sockets.Socket" />。
//    /// </summary>
//    /// <returns>
//    ///   基础网络 <see cref="T:System.Net.Sockets.Socket" />。
//    /// </returns>
//    public Socket Client
//    {
//      get
//      {
//        return this.m_ClientSocket;
//      }
//      set
//      {
//        this.m_ClientSocket = value;
//      }
//    }
//
//    /// <summary>获取或设置一个值，该值指示是否已创建默认远程主机。</summary>
//    /// <returns>
//    ///   <see langword="true" /> 如果连接处于活动状态，则否则为 <see langword="false" />。
//    /// </returns>
//    protected bool Active
//    {
//      get
//      {
//        return this.m_Active;
//      }
//      set
//      {
//        this.m_Active = value;
//      }
//    }
//
//    /// <summary>获取接收从网络中可读取的数据量。</summary>
//    /// <returns>从网络接收的数据的字节数。</returns>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   尝试访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    public int Available
//    {
//      get
//      {
//        return this.m_ClientSocket.Available;
//      }
//    }
//
//    /// <summary>
//    ///   获取或设置一个值，指定的时间 (TTL) 值 Internet 协议 (IP) 发送的数据包通过 <see cref="T:System.Net.Sockets.UdpClient" />。
//    /// </summary>
//    /// <returns>TTL 值中。</returns>
//    public short Ttl
//    {
//      get
//      {
//        return this.m_ClientSocket.Ttl;
//      }
//      set
//      {
//        this.m_ClientSocket.Ttl = value;
//      }
//    }
//
//    /// <summary>
//    ///   获取或设置 <see cref="T:System.Boolean" /> 值，该值指定是否 <see cref="T:System.Net.Sockets.UdpClient" /> 允许 Internet 协议 (IP) 的数据报进行分片。
//    /// </summary>
//    /// <returns>
//    ///   <see langword="true" /> 如果 <see cref="T:System.Net.Sockets.UdpClient" /> 允许数据报分段; 否则为 <see langword="false" />。
//    ///    默认值为 <see langword="true" />。
//    /// </returns>
//    /// <exception cref="T:System.NotSupportedException">
//    ///   此属性可以设置仅对使用的套接字 <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> 标志或 <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> 标志。
//    /// </exception>
//    public bool DontFragment
//    {
//      get
//      {
//        return this.m_ClientSocket.DontFragment;
//      }
//      set
//      {
//        this.m_ClientSocket.DontFragment = value;
//      }
//    }
//
//    /// <summary>
//    ///   获取或设置 <see cref="T:System.Boolean" /> 值，该值指定是否传出多播的数据包将传输到发送应用程序。
//    /// </summary>
//    /// <returns>
//    ///   <see langword="true" /> 如果 <see cref="T:System.Net.Sockets.UdpClient" /> 接收传出多播的数据包数; 否则为 <see langword="false" />。
//    /// </returns>
//    public bool MulticastLoopback
//    {
//      get
//      {
//        return this.m_ClientSocket.MulticastLoopback;
//      }
//      set
//      {
//        this.m_ClientSocket.MulticastLoopback = value;
//      }
//    }
//
//    /// <summary>
//    ///   获取或设置 <see cref="T:System.Boolean" /> 值，该值指定是否 <see cref="T:System.Net.Sockets.UdpClient" /> 可能会发送或接收广播的数据包。
//    /// </summary>
//    /// <returns>
//    ///   <see langword="true" /> 如果 <see cref="T:System.Net.Sockets.UdpClient" /> 允许广播的数据包; 否则为 <see langword="false" />。
//    ///    默认值为 <see langword="false" />。
//    /// </returns>
//    public bool EnableBroadcast
//    {
//      get
//      {
//        return this.m_ClientSocket.EnableBroadcast;
//      }
//      set
//      {
//        this.m_ClientSocket.EnableBroadcast = value;
//      }
//    }
//
//    /// <summary>
//    ///   获取或设置 <see cref="T:System.Boolean" /> 值，指定 <see cref="T:System.Net.Sockets.UdpClient" /> 是否只允许一个客户端使用端口。
//    /// </summary>
//    /// <returns>
//    ///   如果 <see cref="T:System.Net.Sockets.UdpClient" /> 只允许一个客户端使用特定端口，则为 <see langword="true" />；否则为 <see langword="false" />。
//    ///    在 Windows Server 2003、Windows XP Service Pack 2 及更高版本中，默认为 <see langword="true" />；在所有其他版本中，默认为 <see langword="false" />。
//    /// </returns>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   当尝试访问基础套接字时出错。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    public bool ExclusiveAddressUse
//    {
//      get
//      {
//        return this.m_ClientSocket.ExclusiveAddressUse;
//      }
//      set
//      {
//        this.m_ClientSocket.ExclusiveAddressUse = value;
//      }
//    }
//
//    /// <summary>
//    ///   启用或禁用网络地址转换 (NAT) 遍历上 <see cref="T:System.Net.Sockets.UdpClient" /> 实例。
//    /// </summary>
//    /// <param name="allowed">一个布尔值，该值指定是否要启用或禁用 NAT 遍历。</param>
//    public void AllowNatTraversal(bool allowed)
//    {
//      if (allowed)
//        this.m_ClientSocket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
//      else
//        this.m_ClientSocket.SetIPProtectionLevel(IPProtectionLevel.EdgeRestricted);
//    }
//
//    /// <summary>关闭 UDP 连接。</summary>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public void Close()
//    {
//      this.Dispose(true);
//    }
//
//    private void FreeResources()
//    {
//      if (this.m_CleanedUp)
//        return;
//      Socket client = this.Client;
//      if (client != null)
//      {
//        client.InternalShutdown(SocketShutdown.Both);
//        client.Close();
//        this.Client = (Socket) null;
//      }
//      this.m_CleanedUp = true;
//    }
//
//    /// <summary>
//    ///   释放由 <see cref="T:System.Net.Sockets.UdpClient" /> 占用的托管和非托管资源。
//    /// </summary>
//    public void Dispose()
//    {
//      this.Dispose(true);
//    }
//
//    /// <summary>
//    ///   释放由 <see cref="T:System.Net.Sockets.UdpClient" /> 占用的非托管资源，还可以另外再释放托管资源。
//    /// </summary>
//    /// <param name="disposing">
//    ///   若要释放托管资源和非托管资源，则为 <see langword="true" />；若仅释放非托管资源，则为 <see langword="false" />。
//    /// </param>
//    protected virtual void Dispose(bool disposing)
//    {
//      if (!disposing)
//        return;
//      this.FreeResources();
//      GC.SuppressFinalize((object) this);
//    }
//
//    /// <summary>建立默认远程主机使用指定主机名和端口号。</summary>
//    /// <param name="hostname">想向其发送数据的远程主机的 DNS 名称。</param>
//    /// <param name="port">您打算将数据发送到远程主机上的端口号。</param>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentOutOfRangeException">
//    ///   <paramref name="port" /> 不介于 <see cref="F:System.Net.IPEndPoint.MinPort" /> 和 <see cref="F:System.Net.IPEndPoint.MaxPort" />。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public void Connect(string hostname, int port)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (hostname == null)
//        throw new ArgumentNullException(nameof (hostname));
//      if (!ValidationHelper.ValidateTcpPort(port))
//        throw new ArgumentOutOfRangeException(nameof (port));
//      IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
//      Exception exception = (Exception) null;
//      Socket socket1 = (Socket) null;
//      Socket socket2 = (Socket) null;
//      try
//      {
//        if (this.m_ClientSocket == null)
//        {
//          if (Socket.OSSupportsIPv4)
//            socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//          if (Socket.OSSupportsIPv6)
//            socket1 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
//        }
//        foreach (IPAddress address in hostAddresses)
//        {
//          try
//          {
//            if (this.m_ClientSocket == null)
//            {
//              if (address.AddressFamily == AddressFamily.InterNetwork && socket2 != null)
//              {
//                socket2.Connect(address, port);
//                this.m_ClientSocket = socket2;
//                socket1?.Close();
//              }
//              else if (socket1 != null)
//              {
//                socket1.Connect(address, port);
//                this.m_ClientSocket = socket1;
//                socket2?.Close();
//              }
//              this.m_Family = address.AddressFamily;
//              this.m_Active = true;
//              break;
//            }
//            if (address.AddressFamily == this.m_Family)
//            {
//              this.Connect(new IPEndPoint(address, port));
//              this.m_Active = true;
//              break;
//            }
//          }
//          catch (Exception ex)
//          {
//            if (NclUtilities.IsFatal(ex))
//              throw;
//            else
//              exception = ex;
//          }
//        }
//      }
//      catch (Exception ex)
//      {
//        if (NclUtilities.IsFatal(ex))
//          throw;
//        else
//          exception = ex;
//      }
//      finally
//      {
//        if (!this.m_Active)
//        {
//          socket1?.Close();
//          socket2?.Close();
//          if (exception != null)
//            throw exception;
//          throw new SocketException(SocketError.NotConnected);
//        }
//      }
//    }
//
//    /// <summary>建立默认远程主机使用指定的 IP 地址和端口号。</summary>
//    /// <param name="addr">
//    ///   <see cref="T:System.Net.IPAddress" /> 您打算将数据发送到的远程主机。
//    /// </param>
//    /// <param name="port">端口号想向其发送数据。</param>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="addr" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentOutOfRangeException">
//    ///   <paramref name="port" /> 不介于 <see cref="F:System.Net.IPEndPoint.MinPort" /> 和 <see cref="F:System.Net.IPEndPoint.MaxPort" />。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public void Connect(IPAddress addr, int port)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (addr == null)
//        throw new ArgumentNullException(nameof (addr));
//      if (!ValidationHelper.ValidateTcpPort(port))
//        throw new ArgumentOutOfRangeException(nameof (port));
//      this.Connect(new IPEndPoint(addr, port));
//    }
//
//    /// <summary>建立默认远程主机使用指定的网络终结点。</summary>
//    /// <param name="endPoint">
//    ///   <see cref="T:System.Net.IPEndPoint" /> ，指定想要将数据发送的网络终结点。
//    /// </param>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="endPoint" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    public void Connect(IPEndPoint endPoint)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (endPoint == null)
//        throw new ArgumentNullException(nameof (endPoint));
//      this.CheckForBroadcast(endPoint.Address);
//      this.Client.Connect((EndPoint) endPoint);
//      this.m_Active = true;
//    }
//
//    private void CheckForBroadcast(IPAddress ipAddress)
//    {
//      if (this.Client == null || this.m_IsBroadcast || !ipAddress.IsBroadcast)
//        return;
//      this.m_IsBroadcast = true;
//      this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
//    }
//
//    /// <summary>将 UDP 数据报发送到位于指定远程终结点的主机。</summary>
//    /// <param name="dgram">
//    ///   <see cref="T:System.Byte" /> 类型的数组，它指定你打算发送的 UDP 数据报，表示为字节数组。
//    /// </param>
//    /// <param name="bytes">数据报中的字节数。</param>
//    /// <param name="endPoint">
//    ///   一个 <see cref="T:System.Net.IPEndPoint" />，表示要将数据报发送到的主机和端口。
//    /// </param>
//    /// <returns>已发送的字节数。</returns>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="dgram" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.InvalidOperationException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已建立默认远程主机。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (dgram == null)
//        throw new ArgumentNullException(nameof (dgram));
//      if (this.m_Active && endPoint != null)
//        throw new InvalidOperationException(SR.GetString("net_udpconnected"));
//      if (endPoint == null)
//        return this.Client.Send(dgram, 0, bytes, SocketFlags.None);
//      this.CheckForBroadcast(endPoint.Address);
//      return this.Client.SendTo(dgram, 0, bytes, SocketFlags.None, (EndPoint) endPoint);
//    }
//
//    /// <summary>将 UDP 数据报发送到指定远程主机上的指定端口。</summary>
//    /// <param name="dgram">
//    ///   <see cref="T:System.Byte" /> 类型的数组，它指定你打算发送的 UDP 数据报，表示为字节数组。
//    /// </param>
//    /// <param name="bytes">数据报中的字节数。</param>
//    /// <param name="hostname">要向其发送数据报的远程主机的名称。</param>
//    /// <param name="port">要与之通信的远程端口号。</param>
//    /// <returns>已发送的字节数。</returns>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="dgram" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.InvalidOperationException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已建立默认远程主机。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public int Send(byte[] dgram, int bytes, string hostname, int port)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (dgram == null)
//        throw new ArgumentNullException(nameof (dgram));
//      if (this.m_Active && (hostname != null || port != 0))
//        throw new InvalidOperationException(SR.GetString("net_udpconnected"));
//      if (hostname == null || port == 0)
//        return this.Client.Send(dgram, 0, bytes, SocketFlags.None);
//      IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
//      int index = 0;
//      while (index < hostAddresses.Length && hostAddresses[index].AddressFamily != this.m_Family)
//        ++index;
//      if (hostAddresses.Length == 0 || index == hostAddresses.Length)
//        throw new ArgumentException(SR.GetString("net_invalidAddressList"), nameof (hostname));
//      this.CheckForBroadcast(hostAddresses[index]);
//      IPEndPoint ipEndPoint = new IPEndPoint(hostAddresses[index], port);
//      return this.Client.SendTo(dgram, 0, bytes, SocketFlags.None, (EndPoint) ipEndPoint);
//    }
//
//    /// <summary>将 UDP 数据报发送到远程主机。</summary>
//    /// <param name="dgram">
//    ///   <see cref="T:System.Byte" /> 类型的数组，它指定你打算发送的 UDP 数据报，表示为字节数组。
//    /// </param>
//    /// <param name="bytes">数据报中的字节数。</param>
//    /// <returns>已发送的字节数。</returns>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="dgram" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.InvalidOperationException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已建立默认远程主机。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public int Send(byte[] dgram, int bytes)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (dgram == null)
//        throw new ArgumentNullException(nameof (dgram));
//      if (!this.m_Active)
//        throw new InvalidOperationException(SR.GetString("net_notconnected"));
//      return this.Client.Send(dgram, 0, bytes, SocketFlags.None);
//    }
//
//    /// <summary>
//    ///   以异步方式将数据报发送到目标。
//    ///    通过指定目标 <see cref="T:System.Net.EndPoint" />。
//    /// </summary>
//    /// <param name="datagram">
//    ///   一个 <see cref="T:System.Byte" /> 数组，其中包含要发送的数据。
//    /// </param>
//    /// <param name="bytes">要发送的字节数。</param>
//    /// <param name="endPoint">
//    ///   <see cref="T:System.Net.EndPoint" /> 表示数据的目标。
//    /// </param>
//    /// <param name="requestCallback">
//    ///   一个 <see cref="T:System.AsyncCallback" /> 委托，它引用操作完成时要调用的方法。
//    /// </param>
//    /// <param name="state">
//    ///   用户定义的对象，其中包含有关发送操作的信息。
//    ///    当操作完成时，此对象会被传递给 <paramref name="requestCallback" /> 委托。
//    /// </param>
//    /// <returns>
//    ///   <see cref="T:System.IAsyncResult" /> 对象，可引用异步发送。
//    /// </returns>
//    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//    public IAsyncResult BeginSend(
//      byte[] datagram,
//      int bytes,
//      IPEndPoint endPoint,
//      AsyncCallback requestCallback,
//      object state)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (datagram == null)
//        throw new ArgumentNullException(nameof (datagram));
//      if (bytes > datagram.Length || bytes < 0)
//        throw new ArgumentOutOfRangeException(nameof (bytes));
//      if (this.m_Active && endPoint != null)
//        throw new InvalidOperationException(SR.GetString("net_udpconnected"));
//      if (endPoint == null)
//        return this.Client.BeginSend(datagram, 0, bytes, SocketFlags.None, requestCallback, state);
//      this.CheckForBroadcast(endPoint.Address);
//      return this.Client.BeginSendTo(datagram, 0, bytes, SocketFlags.None, (EndPoint) endPoint, requestCallback, state);
//    }
//
//    /// <summary>
//    ///   以异步方式将数据报发送到目标。
//    ///    目标指定的主机名称和端口号。
//    /// </summary>
//    /// <param name="datagram">
//    ///   一个 <see cref="T:System.Byte" /> 数组，其中包含要发送的数据。
//    /// </param>
//    /// <param name="bytes">要发送的字节数。</param>
//    /// <param name="hostname">目标主机中。</param>
//    /// <param name="port">目标端口号。</param>
//    /// <param name="requestCallback">
//    ///   一个 <see cref="T:System.AsyncCallback" /> 委托，它引用操作完成时要调用的方法。
//    /// </param>
//    /// <param name="state">
//    ///   用户定义的对象，其中包含有关发送操作的信息。
//    ///    当操作完成时，此对象会被传递给 <paramref name="requestCallback" /> 委托。
//    /// </param>
//    /// <returns>
//    ///   <see cref="T:System.IAsyncResult" /> 对象，可引用异步发送。
//    /// </returns>
//    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//    public IAsyncResult BeginSend(
//      byte[] datagram,
//      int bytes,
//      string hostname,
//      int port,
//      AsyncCallback requestCallback,
//      object state)
//    {
//      if (this.m_Active && (hostname != null || port != 0))
//        throw new InvalidOperationException(SR.GetString("net_udpconnected"));
//      IPEndPoint endPoint = (IPEndPoint) null;
//      if (hostname != null && port != 0)
//      {
//        IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
//        int index = 0;
//        while (index < hostAddresses.Length && hostAddresses[index].AddressFamily != this.m_Family)
//          ++index;
//        if (hostAddresses.Length == 0 || index == hostAddresses.Length)
//          throw new ArgumentException(SR.GetString("net_invalidAddressList"), nameof (hostname));
//        this.CheckForBroadcast(hostAddresses[index]);
//        endPoint = new IPEndPoint(hostAddresses[index], port);
//      }
//      return this.BeginSend(datagram, bytes, endPoint, requestCallback, state);
//    }
//
//    /// <summary>
//    ///   以异步方式将数据报发送到远程主机。
//    ///    通过调用以前指定目标 <see cref="Overload:System.Net.Sockets.UdpClient.Connect" />。
//    /// </summary>
//    /// <param name="datagram">
//    ///   一个 <see cref="T:System.Byte" /> 数组，其中包含要发送的数据。
//    /// </param>
//    /// <param name="bytes">要发送的字节数。</param>
//    /// <param name="requestCallback">
//    ///   一个 <see cref="T:System.AsyncCallback" /> 委托，它引用操作完成时要调用的方法。
//    /// </param>
//    /// <param name="state">
//    ///   用户定义的对象，其中包含有关发送操作的信息。
//    ///    当操作完成时，此对象会被传递给 <paramref name="requestCallback" /> 委托。
//    /// </param>
//    /// <returns>
//    ///   <see cref="T:System.IAsyncResult" /> 对象，可引用异步发送。
//    /// </returns>
//    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//    public IAsyncResult BeginSend(
//      byte[] datagram,
//      int bytes,
//      AsyncCallback requestCallback,
//      object state)
//    {
//      return this.BeginSend(datagram, bytes, (IPEndPoint) null, requestCallback, state);
//    }
//
//    /// <summary>结束挂起的异步发送。</summary>
//    /// <param name="asyncResult">
//    ///   调用 <see cref="Overload:System.Net.Sockets.UdpClient.BeginSend" /> 后返回的 <see cref="T:System.IAsyncResult" /> 对象。
//    /// </param>
//    /// <returns>
//    ///   如果成功，字节数已发送到 <see cref="T:System.Net.Sockets.UdpClient" />。
//    /// </returns>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="asyncResult" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentException">
//    ///   <paramref name="asyncResult" /> 通过调用未返回 <see cref="M:System.Net.Sockets.Socket.BeginSend(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.AsyncCallback,System.Object)" /> 方法。
//    /// </exception>
//    /// <exception cref="T:System.InvalidOperationException">
//    ///   <see cref="M:System.Net.Sockets.Socket.EndSend(System.IAsyncResult)" /> 之前已调用为异步读取。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   当尝试访问基础套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    public int EndSend(IAsyncResult asyncResult)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (this.m_Active)
//        return this.Client.EndSend(asyncResult);
//      return this.Client.EndSendTo(asyncResult);
//    }
//
//    /// <summary>返回由一台远程主机发送的 UDP 数据报。</summary>
//    /// <param name="remoteEP">
//    ///   <see cref="T:System.Net.IPEndPoint" /> ，表示从其发送数据的远程主机。
//    /// </param>
//    /// <returns>
//    ///   类型的数组 <see cref="T:System.Byte" /> ，其中包含数据报数据。
//    /// </returns>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" />  已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public byte[] Receive(ref IPEndPoint remoteEP)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      EndPoint remoteEP1 = this.m_Family != AddressFamily.InterNetwork ? (EndPoint) IPEndPoint.IPv6Any : (EndPoint) IPEndPoint.Any;
//      int from = this.Client.ReceiveFrom(this.m_Buffer, 65536, SocketFlags.None, ref remoteEP1);
//      remoteEP = (IPEndPoint) remoteEP1;
//      if (from >= 65536)
//        return this.m_Buffer;
//      byte[] numArray = new byte[from];
//      Buffer.BlockCopy((Array) this.m_Buffer, 0, (Array) numArray, 0, from);
//      return numArray;
//    }
//
//    /// <summary>以异步方式从远程主机接收数据报。</summary>
//    /// <param name="requestCallback">
//    ///   一个 <see cref="T:System.AsyncCallback" /> 委托，它引用操作完成时要调用的方法。
//    /// </param>
//    /// <param name="state">
//    ///   用户定义的对象，其中包含有关接收操作的信息。
//    ///    当操作完成时，此对象会被传递给 <paramref name="requestCallback" /> 委托。
//    /// </param>
//    /// <returns>
//    ///   <see cref="T:System.IAsyncResult" /> 对象，可引用异步接收。
//    /// </returns>
//    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//    public IAsyncResult BeginReceive(AsyncCallback requestCallback, object state)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      EndPoint remoteEP = this.m_Family != AddressFamily.InterNetwork ? (EndPoint) IPEndPoint.IPv6Any : (EndPoint) IPEndPoint.Any;
//      return this.Client.BeginReceiveFrom(this.m_Buffer, 0, 65536, SocketFlags.None, ref remoteEP, requestCallback, state);
//    }
//
//    /// <summary>结束挂起的异步接收。</summary>
//    /// <param name="asyncResult">
//    ///   调用 <see cref="M:System.Net.Sockets.UdpClient.BeginReceive(System.AsyncCallback,System.Object)" /> 后返回的 <see cref="T:System.IAsyncResult" /> 对象。
//    /// </param>
//    /// <param name="remoteEP">指定的远程终结点。</param>
//    /// <returns>
//    ///   如果成功，则接收的字节数。
//    ///    如果不成功，此方法将返回 0。
//    /// </returns>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="asyncResult" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentException">
//    ///   <paramref name="asyncResult" /> 通过调用未返回 <see cref="M:System.Net.Sockets.UdpClient.BeginReceive(System.AsyncCallback,System.Object)" /> 方法。
//    /// </exception>
//    /// <exception cref="T:System.InvalidOperationException">
//    ///   <see cref="M:System.Net.Sockets.UdpClient.EndReceive(System.IAsyncResult,System.Net.IPEndPoint@)" /> 之前已调用为异步读取。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   当尝试访问基础时出错 <see cref="T:System.Net.Sockets.Socket" />。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      EndPoint endPoint = this.m_Family != AddressFamily.InterNetwork ? (EndPoint) IPEndPoint.IPv6Any : (EndPoint) IPEndPoint.Any;
//      int from = this.Client.EndReceiveFrom(asyncResult, ref endPoint);
//      remoteEP = (IPEndPoint) endPoint;
//      if (from >= 65536)
//        return this.m_Buffer;
//      byte[] numArray = new byte[from];
//      Buffer.BlockCopy((Array) this.m_Buffer, 0, (Array) numArray, 0, from);
//      return numArray;
//    }
//
//    /// <summary>
//    ///   添加 <see cref="T:System.Net.Sockets.UdpClient" /> 到多播组。
//    /// </summary>
//    /// <param name="multicastAddr">
//    ///   多路广播 <see cref="T:System.Net.IPAddress" /> 您想要加入的组。
//    /// </param>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentException">
//    ///   IP 地址不是与兼容 <see cref="T:System.Net.Sockets.AddressFamily" /> 值，该值定义的套接字的寻址方案。
//    /// </exception>
//    public void JoinMulticastGroup(IPAddress multicastAddr)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (multicastAddr == null)
//        throw new ArgumentNullException(nameof (multicastAddr));
//      if (multicastAddr.AddressFamily != this.m_Family)
//        throw new ArgumentException(SR.GetString("net_protocol_invalid_multicast_family", (object) "UDP"), nameof (multicastAddr));
//      if (this.m_Family == AddressFamily.InterNetwork)
//        this.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, (object) new MulticastOption(multicastAddr));
//      else
//        this.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, (object) new IPv6MulticastOption(multicastAddr));
//    }
//
//    /// <summary>
//    ///   添加 <see cref="T:System.Net.Sockets.UdpClient" /> 到多播组。
//    /// </summary>
//    /// <param name="multicastAddr">
//    ///   多路广播 <see cref="T:System.Net.IPAddress" /> 您想要加入的组。
//    /// </param>
//    /// <param name="localAddress">
//    ///   本地 <see cref="T:System.Net.IPAddress" />。
//    /// </param>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public void JoinMulticastGroup(IPAddress multicastAddr, IPAddress localAddress)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (this.m_Family != AddressFamily.InterNetwork)
//        throw new SocketException(SocketError.OperationNotSupported);
//      this.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, (object) new MulticastOption(multicastAddr, localAddress));
//    }
//
//    /// <summary>
//    ///   添加 <see cref="T:System.Net.Sockets.UdpClient" /> 到多播组。
//    /// </summary>
//    /// <param name="ifindex">与对其进行加入多播的组本地 IP 地址关联的接口索引。</param>
//    /// <param name="multicastAddr">
//    ///   多路广播 <see cref="T:System.Net.IPAddress" /> 您想要加入的组。
//    /// </param>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    public void JoinMulticastGroup(int ifindex, IPAddress multicastAddr)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (multicastAddr == null)
//        throw new ArgumentNullException(nameof (multicastAddr));
//      if (ifindex < 0)
//        throw new ArgumentException(SR.GetString("net_value_cannot_be_negative"), nameof (ifindex));
//      if (this.m_Family != AddressFamily.InterNetworkV6)
//        throw new SocketException(SocketError.OperationNotSupported);
//      this.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, (object) new IPv6MulticastOption(multicastAddr, (long) ifindex));
//    }
//
//    /// <summary>
//    ///   添加 <see cref="T:System.Net.Sockets.UdpClient" /> 到具有指定的时间 (TTL) 的多播组。
//    /// </summary>
//    /// <param name="multicastAddr">
//    ///   <see cref="T:System.Net.IPAddress" /> 要加入的多播组。
//    /// </param>
//    /// <param name="timeToLive">生存时间 (TTL)，以的路由器跃点数为单位。</param>
//    /// <exception cref="T:System.ArgumentOutOfRangeException">
//    ///   所提供的 TTL 不是介于 0 和 255 之间
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="multicastAddr" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentException">
//    ///   IP 地址不是与兼容 <see cref="T:System.Net.Sockets.AddressFamily" /> 值，该值定义的套接字的寻址方案。
//    /// </exception>
//    public void JoinMulticastGroup(IPAddress multicastAddr, int timeToLive)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (multicastAddr == null)
//        throw new ArgumentNullException(nameof (multicastAddr));
//      if (!ValidationHelper.ValidateRange(timeToLive, 0, (int) byte.MaxValue))
//        throw new ArgumentOutOfRangeException(nameof (timeToLive));
//      this.JoinMulticastGroup(multicastAddr);
//      this.Client.SetSocketOption(this.m_Family == AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, timeToLive);
//    }
//
//    /// <summary>离开多播的组。</summary>
//    /// <param name="multicastAddr">
//    ///   <see cref="T:System.Net.IPAddress" /> 多路广播的组将保留。
//    /// </param>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentException">
//    ///   IP 地址不是与兼容 <see cref="T:System.Net.Sockets.AddressFamily" /> 值，该值定义的套接字的寻址方案。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="multicastAddr" /> 为 <see langword="null" />。
//    /// </exception>
//    public void DropMulticastGroup(IPAddress multicastAddr)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (multicastAddr == null)
//        throw new ArgumentNullException(nameof (multicastAddr));
//      if (multicastAddr.AddressFamily != this.m_Family)
//        throw new ArgumentException(SR.GetString("net_protocol_invalid_multicast_family", (object) "UDP"), nameof (multicastAddr));
//      if (this.m_Family == AddressFamily.InterNetwork)
//        this.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, (object) new MulticastOption(multicastAddr));
//      else
//        this.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, (object) new IPv6MulticastOption(multicastAddr));
//    }
//
//    /// <summary>离开多播的组。</summary>
//    /// <param name="multicastAddr">
//    ///   <see cref="T:System.Net.IPAddress" /> 多路广播的组将保留。
//    /// </param>
//    /// <param name="ifindex">要退出的多播组对本地地址。</param>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentException">
//    ///   IP 地址不是与兼容 <see cref="T:System.Net.Sockets.AddressFamily" /> 值，该值定义的套接字的寻址方案。
//    /// </exception>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="multicastAddr" /> 为 <see langword="null" />。
//    /// </exception>
//    public void DropMulticastGroup(IPAddress multicastAddr, int ifindex)
//    {
//      if (this.m_CleanedUp)
//        throw new ObjectDisposedException(this.GetType().FullName);
//      if (multicastAddr == null)
//        throw new ArgumentNullException(nameof (multicastAddr));
//      if (ifindex < 0)
//        throw new ArgumentException(SR.GetString("net_value_cannot_be_negative"), nameof (ifindex));
//      if (this.m_Family != AddressFamily.InterNetworkV6)
//        throw new SocketException(SocketError.OperationNotSupported);
//      this.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, (object) new IPv6MulticastOption(multicastAddr, (long) ifindex));
//    }
//
//    /// <summary>以异步方式将 UDP 数据报发送到远程主机。</summary>
//    /// <param name="datagram">
//    ///   <see cref="T:System.Byte" /> 类型的数组，它指定你打算发送的 UDP 数据报，表示为字节数组。
//    /// </param>
//    /// <param name="bytes">数据报中的字节数。</param>
//    /// <returns>
//    ///   返回 <see cref="T:System.Threading.Tasks.Task`1" />。
//    /// </returns>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="dgram" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.InvalidOperationException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已建立默认远程主机。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//    public Task<int> SendAsync(byte[] datagram, int bytes)
//    {
//      return Task<int>.Factory.FromAsync<byte[], int>(new Func<byte[], int, AsyncCallback, object, IAsyncResult>(this.BeginSend), new Func<IAsyncResult, int>(this.EndSend), datagram, bytes, (object) null);
//    }
//
//    /// <summary>以异步方式将 UDP 数据报发送到远程主机。</summary>
//    /// <param name="datagram">
//    ///   <see cref="T:System.Byte" /> 类型的数组，它指定你打算发送的 UDP 数据报，表示为字节数组。
//    /// </param>
//    /// <param name="bytes">数据报中的字节数。</param>
//    /// <param name="endPoint">
//    ///   一个 <see cref="T:System.Net.IPEndPoint" />，表示要将数据报发送到的主机和端口。
//    /// </param>
//    /// <returns>
//    ///   返回 <see cref="T:System.Threading.Tasks.Task`1" />。
//    /// </returns>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="dgram" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.InvalidOperationException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已建立默认远程主机。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//    public Task<int> SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint)
//    {
//      return Task<int>.Factory.FromAsync<byte[], int, IPEndPoint>(new Func<byte[], int, IPEndPoint, AsyncCallback, object, IAsyncResult>(this.BeginSend), new Func<IAsyncResult, int>(this.EndSend), datagram, bytes, endPoint, (object) null);
//    }
//
//    /// <summary>以异步方式将 UDP 数据报发送到远程主机。</summary>
//    /// <param name="datagram">
//    ///   <see cref="T:System.Byte" /> 类型的数组，它指定你打算发送的 UDP 数据报，表示为字节数组。
//    /// </param>
//    /// <param name="bytes">数据报中的字节数。</param>
//    /// <param name="hostname">要向其发送数据报的远程主机的名称。</param>
//    /// <param name="port">要与之通信的远程端口号。</param>
//    /// <returns>
//    ///   返回 <see cref="T:System.Threading.Tasks.Task`1" />。
//    /// </returns>
//    /// <exception cref="T:System.ArgumentNullException">
//    ///   <paramref name="dgram" /> 为 <see langword="null" />。
//    /// </exception>
//    /// <exception cref="T:System.InvalidOperationException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已建立默认远程主机。
//    /// </exception>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   <see cref="T:System.Net.Sockets.UdpClient" /> 已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//    public Task<int> SendAsync(byte[] datagram, int bytes, string hostname, int port)
//    {
//      return Task<int>.Factory.FromAsync((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => this.BeginSend(datagram, bytes, hostname, port, callback, state)), new Func<IAsyncResult, int>(this.EndSend), (object) null);
//    }
//
//    /// <summary>返回的 UDP 数据报以异步方式由一台远程主机发送的。</summary>
//    /// <returns>
//    ///   返回 <see cref="T:System.Threading.Tasks.Task`1" />。
//    /// 
//    ///   表示异步操作的任务对象。
//    /// </returns>
//    /// <exception cref="T:System.ObjectDisposedException">
//    ///   基础 <see cref="T:System.Net.Sockets.Socket" />  已关闭。
//    /// </exception>
//    /// <exception cref="T:System.Net.Sockets.SocketException">
//    ///   访问套接字时出错。
//    ///    有关详细信息，请参阅备注部分。
//    /// </exception>
//    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
//    public Task<UdpReceiveResult> ReceiveAsync()
//    {
//      return Task<UdpReceiveResult>.Factory.FromAsync((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => this.BeginReceive(callback, state)), (Func<IAsyncResult, UdpReceiveResult>) (ar =>
//      {
//        IPEndPoint remoteEP = (IPEndPoint) null;
//        return new UdpReceiveResult(this.EndReceive(ar, ref remoteEP), remoteEP);
//      }), (object) null);
//    }
//
//    private void createClientSocket()
//    {
//      this.Client = new Socket(this.m_Family, SocketType.Dgram, ProtocolType.Udp);
//    }
//  }
//}
