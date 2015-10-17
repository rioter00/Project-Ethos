using System.IO;
using Ethos.Base.Infrastructure.Serialization;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Photon.SocketServer;

namespace Ethos.Server.Master
{
    public class Application : ApplicationBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Application));
        private MasterServerContext _application;

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new Peer(initRequest, _application);
        }

        protected override void Setup()
        {
            ExitGames.Logging.LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);

            var config = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (config.Exists)
                XmlConfigurator.ConfigureAndWatch(config);
            else
                BasicConfigurator.Configure();

            _application = new MasterServerContext(new BinarySerializer());
            _application.Setup();

            Log.Info("Application Started!");
        }

        protected override void TearDown()
        {
            _application.Dispose();
            Log.Info("Application Stopped!");
        }
    }
}