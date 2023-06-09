using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ssz.DataAccessGrpcServer_OpcUaClient;
using Ssz.DataAccessGrpc.ServerBase;
using Ssz.Utils;
using Ssz.Utils.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ssz.DataAccessGrpcServer_OpcUaClient
{
    public partial class MainBackgroundService : BackgroundService
    {
        #region construction and destruction

        public MainBackgroundService(ILogger<MainBackgroundService> logger, IConfiguration configuration, IServiceProvider serviceProvider, ServerWorkerBase serverWorker)
        {
            Logger = logger;
            Configuration = configuration;
            ServiceProvider = serviceProvider;
            _serverWorker = serverWorker;

            _serverWorker.ShutdownRequested += This_ShutdownRequested;

            _utilityDataAccessProvider = ActivatorUtilities.CreateInstance<GrpcDataAccessProvider>(serviceProvider);
            _processDataAccessProvider = ActivatorUtilities.CreateInstance<GrpcDataAccessProvider>(ServiceProvider);
        }        

        #endregion

        #region public functions

        public ILogger<MainBackgroundService> Logger { get; }

        public IConfiguration Configuration { get; }

        public IServiceProvider ServiceProvider { get; }        

        #endregion

        #region protected functions

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug("ExecuteAsync begin.");

            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(_serverWorker.ThreadSafeDispatcher));

            //_utilityDataAccessProvider.EventMessagesCallback += UtilityDataAccessProviderOnEventMessagesCallback;
            _utilityDataAccessProvider.Initialize(null,
                true,
                true,
                Program.Options.GetCentralServerAddress(),
                DataAccessConstants.ControlEngine_ClientApplicationName,
                Environment.MachineName,
                @"", // Utility context
                new CaseInsensitiveDictionary<string?>(),
                _serverWorker.ThreadSafeDispatcher);            

            DsDevice device;
            if (Program.Options.ProcessDirectory != @"")
            {
                device = ActivatorUtilities.CreateInstance<DsDevice>(ServiceProvider,
                    _processDataAccessProvider,
                    new DirectoryInfo(Program.Options.ProcessDirectory),
                    _serverWorker.ThreadSafeDispatcher);
            }
            else
            {
                device = ActivatorUtilities.CreateInstance<DsDevice>(ServiceProvider,
                    _processDataAccessProvider,                    
                    _serverWorker.ThreadSafeDispatcher);
            }

            //new DsController(
            //    ActivatorUtilities.CreateInstance<ILogger<DsController>>(ServiceProvider),
            //    );
            ((ServerWorker)_serverWorker).Device = device;

            _processDataAccessProvider.Initialize(device.ElementIdsMap,
                true,
                true,
                Program.Options.GetCentralServerAddress(),
                DataAccessConstants.ControlEngine_ClientApplicationName,
                Environment.MachineName,
                Program.Options.CentralServerSystemName,
                new CaseInsensitiveDictionary<string?>(),
                _serverWorker.ThreadSafeDispatcher);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested || _shutdownRequested) break;
                await Task.Delay(3).ConfigureAwait(false);
                if (cancellationToken.IsCancellationRequested || _shutdownRequested) break;

                DateTime nowUtc = DateTime.UtcNow;
                
                if (nowUtc - _processDataAccessProvider.InitializedDateTimeUtc > DataAccessConstants.UnrecoverableTimeout &&
                    nowUtc - _processDataAccessProvider.LastSuccessfulConnectionDateTimeUtc > DataAccessConstants.UnrecoverableTimeout)
                    break;

                //if (modelTimeValueSubscription.ValueStatusTimestamp.ValueStatusCode == ValueStatusCode.Good)
                //{
                //    int modelTimeSeconds = modelTimeValueSubscription.ValueStatusTimestamp.Value.ValueAsInt32(false);
                //    if (modelTimeSeconds > 0)
                //        device.DoWork((UInt64)modelTimeSeconds * 1000, nowUtc, cancellationToken);
                //}               

                await _serverWorker.DoWorkAsync(nowUtc, cancellationToken).ConfigureAwait(false);
            }

            _processDataAccessProvider.Close();

            ((ServerWorker)_serverWorker).Device = null;
            device.Dispose();            

            await _serverWorker.ShutdownAsync().ConfigureAwait(false);
            
            await _utilityDataAccessProvider.DisposeAsync().ConfigureAwait(false);

            Program.SafeShutdown();
        }

        #endregion

        #region private functions

        private void This_ShutdownRequested(object? sender, EventArgs args)
        {
            _shutdownRequested = true;
        }

        #endregion        

        #region private fields

        private readonly ServerWorkerBase _serverWorker;

        private readonly GrpcDataAccessProvider _utilityDataAccessProvider;

        /// <summary>
        ///    No one can use this provider, except DsController, because DsController can call GrpcDataAccessProvider.ReInitialize(),
        ///    and all items are lost.
        ///    DsController can add items only in DoWork function, when GrpcDataAccessProvider is Initialized.
        /// </summary>
        private readonly GrpcDataAccessProvider _processDataAccessProvider;

        private bool _shutdownRequested;

        #endregion
    }
}
