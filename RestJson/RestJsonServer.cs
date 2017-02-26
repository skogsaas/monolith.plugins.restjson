using Skogsaas.Legion;
using Skogsaas.Monolith.Logging;
using System.Collections.Generic;

namespace Skogsaas.Monolith.Plugins.RestJson
{
    public class RestJsonServer : IPlugin
    {
        private Skogsaas.Legion.RestJson.Server server;

        private Channel configChannel; 

        public void initialize()
        {
            this.configChannel = Manager.Create(Configuration.Constants.Channel);
            this.configChannel.RegisterType(typeof(IRestJsonConfig));
            this.configChannel.SubscribePublish(typeof(IRestJsonConfig), onConfigPublished);
        }

        private void onConfigPublished(Channel c, IObject obj)
        {
            if(this.server == null)
            {
                IRestJsonConfig config = (IRestJsonConfig)obj;

                List<Channel> channels = new List<Channel>();

                foreach(string name in config.Channels)
                {
                    channels.Add(Manager.Create(name));
                }

                Logger.Info($"Creating Rest server on port <{config.Port}> with the channels <{channels}>");
                this.server = new Skogsaas.Legion.RestJson.Server(config.Port, channels.ToArray());

                Logger.Info($"Starting Rest server");
                this.server.Start();
            }
        }
    }
}
