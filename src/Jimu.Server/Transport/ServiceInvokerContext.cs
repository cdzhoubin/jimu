﻿using Jimu.Logger;
using Jimu.Server.ServiceContainer;
using System;
using System.Linq;

namespace Jimu.Server.Transport
{
    /// <summary>
    ///     context of service invoker
    /// </summary>
    public class ServiceInvokerContext
    {
        public ServiceInvokerContext(JimuTransportMsg transportMessage, IServiceEntryContainer serviceEntryContainer,
            IResponse response, ILogger logger, JimuAddress address)
        {
            Response = response;
            TransportMessage = transportMessage;
            try
            {
                RemoteInvokeMessage = transportMessage.GetContent<JimuRemoteCallData>();
            }
            catch (Exception ex)
            {
                logger.Error("failed to convert transportmsg.content to  JimuRemoteCallerData.", ex);
                return;
            }

            ServiceEntry = serviceEntryContainer.GetServiceEntry()
                .FirstOrDefault(x => x.Descriptor.Id == RemoteInvokeMessage.ServiceId);
            if (ServiceEntry == null)
            {
                logger.Error($"not found service: {RemoteInvokeMessage.ServiceId}", new EntryPointNotFoundException($"{RemoteInvokeMessage.ServiceId}"));
            }
            Address = address;
        }

        public JimuServiceEntry ServiceEntry { get; }
        public IResponse Response { get; }

        public JimuTransportMsg TransportMessage { get; }

        public JimuRemoteCallData RemoteInvokeMessage { get; }

        public JimuAddress Address { get; }
    }
}