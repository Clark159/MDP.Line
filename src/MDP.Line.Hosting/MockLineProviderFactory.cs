using Autofac;
using MDP.Hosting;
using MDP.Line.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line.Hosting
{
    public class MockLineProviderFactory : ServiceFactory<LineProvider, MockLineProvider, MockLineProviderFactory.Setting>
    {
        // Methods
        protected override MockLineProvider CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new MockLineProvider();
        }


        // Class
        public class Setting
        {

        }
    }
}
