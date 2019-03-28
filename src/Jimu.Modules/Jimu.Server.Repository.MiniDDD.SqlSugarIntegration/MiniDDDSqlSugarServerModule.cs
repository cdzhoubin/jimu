﻿using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.Extensions.Configuration;
using MiniDDD.UnitOfWork;
using MiniDDD.UnitOfWork.SqlSugar;

namespace Jimu.Server.Repository.MiniDDD.SqlSugarIntegration
{
    public class MiniDDDSqlSugarServerModule : ServerModuleBase
    {
        readonly MiniDDDSqlSugarOptions _options;
        IContainer _container = null;
        public MiniDDDSqlSugarServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(MiniDDDSqlSugarOptions).Name).Get<MiniDDDSqlSugarOptions>();
        }


        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            if (_options != null)
            {
                DbContextOptions dbContextOptions = new DbContextOptions
                {
                    ConnectionString = _options.ConnectionString,
                    DbType = _options.DbType
                };
                Action<string> logAction = null;
                if (_options.OpenLogTrace)
                {
                    logAction = (log) =>
                    {
                        if (_container != null && _container.IsRegistered<Jimu.Logger.ILogger>())
                        {
                            Jimu.Logger.ILogger logger = _container.Resolve<Jimu.Logger.ILogger>();
                            logger.Info($"【SqlSugar】 - {log}");
                        }
                    };
                }

                serviceContainerBuilder.RegisterType<UnitOfWork>()
                    .WithParameter("options", dbContextOptions)
                    .WithParameter("logAction", logAction).InstancePerLifetimeScope();
            }
            base.DoServiceRegister(serviceContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            _container = container;
            base.DoServiceInit(container);
        }
    }
}
