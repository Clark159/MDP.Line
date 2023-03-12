using Autofac;
using MDP.Hosting;
using MDP.Line.Accesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line.Hosting
{
    public class ApiLineProviderFactory : ServiceFactory<LineProvider, ApiLineProvider, ApiLineProviderFactory.Setting>
    {
        // Methods
        protected override ApiLineProvider CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new ApiLineProvider
            (
                setting.ChannelAccessToken,
                setting.ChannelSecret
            );
        }


        // Class
        public class Setting
        {
            // Properties
            public string ChannelAccessToken { get; set; } = string.Empty;

            public string ChannelSecret { get; set; } = string.Empty;
        }
    }
}
