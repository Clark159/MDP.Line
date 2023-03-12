using Autofac;
using MDP.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line.Hosting
{
    public class LineContextFactory : ServiceFactory<LineContext, LineContext, LineContextFactory.Setting>
    {
        // Constructors
        public LineContextFactory()
        {
            // Default
            this.ServiceSingleton = true;
        }


        // Methods
        protected override LineContext CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new LineContext
            (
                componentContext.Resolve<UserRepository>(setting.UserRepository),
                componentContext.Resolve<MessageRepository>(setting.MessageRepository),
                componentContext.Resolve<ReplyTokenRepository>(setting.ReplyTokenRepository),
                componentContext.Resolve<LineProvider>(setting.LineProvider),
                componentContext.Resolve<PublishProvider>(setting.PublishProvider)
            );
        }


        // Class
        public class Setting
        {
            // Properties
            public string UserRepository { get; set; } = String.Empty;

            public string MessageRepository { get; set; } = String.Empty;

            public string ReplyTokenRepository { get; set; } = String.Empty;

            public string LineProvider { get; set; } = String.Empty;

            public string PublishProvider { get; set; } = String.Empty;
        }
    }
}
