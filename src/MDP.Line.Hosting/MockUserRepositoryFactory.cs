﻿using Autofac;
using MDP.Hosting;
using MDP.Line.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line.Hosting
{
    public class MockUserRepositoryFactory : ServiceFactory<UserRepository, MockUserRepository, MockUserRepositoryFactory.Setting>
    {
        // Methods
        protected override MockUserRepository CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new MockUserRepository();
        }


        // Class
        public class Setting
        {

        }
    }
}
