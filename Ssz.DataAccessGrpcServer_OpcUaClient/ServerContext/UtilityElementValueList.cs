using Ssz.DataAccessGrpcServer_OpcUaClient.ServerListItems;
using Ssz.DataAccessGrpc.ServerBase;
using Ssz.Utils.DataAccess;
using Ssz.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Ssz.DataAccessGrpcServer_OpcUaClient
{    
    public class UtilityElementValueList : ElementValueListBase<UtilityElementValueListItem>
    {
        #region construction and destruction

        public UtilityElementValueList(ServerContext serverContext, uint listClientAlias, CaseInsensitiveDictionary<string?> listParams)
            : base(serverContext, listClientAlias, listParams)
        {
        }

        #endregion

        #region public functions

        public override void DoWork(DateTime nowUtc, CancellationToken token)
        {
            if (Disposed) return;

            if (!ListCallbackIsEnabled) return; // Callback is not Enabled.            

            if (nowUtc >= LastCallbackTime.AddMilliseconds(UpdateRateMs))
            {
                Guid dataGuid = ((ServerWorker)ServerContext.ServerWorker).UtilityDataGuid;                
                if (_dataGuid == dataGuid) return;
                _dataGuid = dataGuid;

                LastCallbackTime = nowUtc;

                ServerContext.ElementValuesCallbackMessage? elementValuesCallbackMessage = GetElementValuesCallbackMessage();

                if (elementValuesCallbackMessage is not null)
                {
                    ServerContext.AddCallbackMessage(elementValuesCallbackMessage);
                }
            }
        }

        #endregion

        #region protected functions

        protected override UtilityElementValueListItem OnNewElementListItem(uint clientAlias, uint serverAlias, string elementId)
        {
            return new UtilityElementValueListItem(clientAlias, serverAlias, elementId);
        }

        protected override Task<List<AddItemToListResult>> OnAddElementListItemsToListAsync(List<UtilityElementValueListItem> elementListItems)
        {
            var results = new List<AddItemToListResult>();

            foreach (UtilityElementValueListItem item in elementListItems)
            {
                ((ServerWorker)ServerContext.ServerWorker).AddUtilityElementValueListItem(item);

                results.Add(new AddItemToListResult
                {
                    AliasResult = new AliasResult
                    {
                        StatusCode = (uint)StatusCode.OK,
                        ServerAlias = item.ServerAlias,
                        ClientAlias = item.ClientAlias
                    },
                    IsReadable = true,
                    IsWritable = true
                });
            }

            return Task.FromResult(results);
        }

        /// <summary>
        ///     Returns failed AliasResults only.
        /// </summary>
        /// <param name="elementListItems"></param>
        /// <returns></returns>
        protected override Task<List<AliasResult>> OnRemoveElementListItemsFromListAsync(List<UtilityElementValueListItem> elementListItems)
        {
            var results = new List<AliasResult>();

            foreach (UtilityElementValueListItem item in elementListItems)
            {
                ((ServerWorker)ServerContext.ServerWorker).RemoveUtilityElementValueListItem(item);
            }

            return Task.FromResult(results);
        }

        #endregion

        #region private fields

        private Guid _dataGuid = Guid.Empty;

        #endregion
    }
}
