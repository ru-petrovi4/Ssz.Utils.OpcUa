using Ssz.DataAccessGrpcServer_OpcUaClient.ServerListItems;
using Ssz.DataAccessGrpc.ServerBase;
using Ssz.Utils.DataAccess;
using Ssz.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Ssz.DataAccessGrpcServer_OpcUaClient
{
    /// <summary>
    ///     This implementation of a Data Journal List is used to maintain
    ///     a collection of historical data value collections.  Each value
    ///     maintained by a Data Journal List consists of collection of
    ///     data values where each value is associated with a specific time.
    /// </summary>
    public class ProcessElementValuesJournalList : ElementListBase<ProcessElementValuesJournalListItem>
	{
        #region construction and destruction

        /// <summary>
        ///     Constructs a new instance of the <see cref="WorkingDataJournalList" /> class.
        /// </summary>
        public ProcessElementValuesJournalList(ServerContext serverContext, uint listClientAlias, CaseInsensitiveDictionary<string?> listParams)
			: base(serverContext, listClientAlias, listParams)
		{            
        }

        #endregion

        #region public functions

        public override Task<ElementValuesJournalsCollection> ReadElementValuesJournalsAsync(
            DateTime firstTimeStampUtc,
            DateTime secondTimeStampUtc,
            uint numValuesPerAlias,
            Ssz.DataAccessGrpc.ServerBase.TypeId calculation,
            CaseInsensitiveDictionary<string?> params_,
            List<uint> serverAliases)
        {
            var reply = new ElementValuesJournalsCollection();

            foreach (int index in Enumerable.Range(0, serverAliases.Count))
            {
                reply.ElementValuesJournals.Add(new ElementValuesJournal());
            }

            return Task.FromResult(reply);
        }

        #endregion

        #region protected functions

        protected override ProcessElementValuesJournalListItem OnNewElementListItem(uint clientAlias, uint serverAlias, string elementId)
        {
            return new ProcessElementValuesJournalListItem(clientAlias, serverAlias, elementId);
        }

        //protected override Task<List<AddItemToListResult>> OnAddElementListItemsToListAsync(List<ProcessElementValuesJournalListItem> elementListItems)
        //{
        //    if (elementListItems.Count == 0) return;

        //    //if (_dataAccessProvidersCollection.Count == 0)
        //    //{
        //    //    foreach (ProcessElementValuesJournalListItem item in elementListItems)
        //    //    {
        //    //        resultsList.Add(new AddItemToListResult
        //    //        {
        //    //            AliasResult = new AliasResult
        //    //            {
        //    //                ResultCode = DataAccessGrpcResultCodes.E_INVALIDREQUEST,
        //    //                ServerAlias = item.ServerAlias,
        //    //                ClientAlias = item.ClientAlias
        //    //            },
        //    //            IsReadable = true,
        //    //            IsWritable = false
        //    //        });
        //    //    }

        //    //    return;
        //    //}

        //    //foreach (ProcessElementValuesJournalListItem item in elementListItems)
        //    //{
        //    //    foreach (IDataAccessProvider dataAccessProvider in _dataAccessProvidersCollection)
        //    //    {
        //    //        var valueSubscription = new ValuesJournalSubscription(dataAccessProvider, item.ElementId);
        //    //        item.ValuesJournalSubscriptionsCollection.Add(valueSubscription);
        //    //    }

        //    //    resultsList.Add(new AddItemToListResult
        //    //    {
        //    //        AliasResult = new AliasResult
        //    //        {
        //    //            ResultCode = DataAccessGrpcResultCodes.S_OK,
        //    //            ServerAlias = item.ServerAlias,
        //    //            ClientAlias = item.ClientAlias
        //    //        },
        //    //        IsReadable = true,
        //    //        IsWritable = true
        //    //    });
        //    //}
        //}        

        #endregion
    }
}