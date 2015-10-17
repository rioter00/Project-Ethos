using System.IO;
using System.Net;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;

namespace Ethos.Server.Instance
{
    public class Application : ApplicationBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Application));
        private InstanceServerContext _application;

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new Peer(initRequest, _application);
        }

        protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state)
        {
            return new ServerPeer(initResponse, out _application);
        }

        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state)
        {
            Log.Error($"Failed to connect to Master-Server, {errorMessage}");
        }

        protected override void Setup()
        {
            ExitGames.Logging.LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);

            var config = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (config.Exists)
                XmlConfigurator.ConfigureAndWatch(config);
            else
                BasicConfigurator.Configure();

            ConnectToServerTcp(new IPEndPoint(IPAddress.Loopback, 4530), "Ethos.Master", null);
            Log.Info("Application Started!");
        }

        protected override void TearDown()
        {
            _application?.Transport.Disconnect();
            Log.Info("Application Stopped!");
        }
    }
}