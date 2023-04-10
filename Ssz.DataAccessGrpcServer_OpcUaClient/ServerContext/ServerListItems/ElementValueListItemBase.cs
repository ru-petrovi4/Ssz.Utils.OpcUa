using Ssz.DataAccessGrpc.ServerBase;
using Ssz.Utils.DataAccess;
using Ssz.Utils;
using System;
using System.Collections.Generic;
using System.Threading;


namespace Ssz.DataAccessGrpcServer_OpcUaClient.ServerListItems
{    
    public class ElementValueListItemBase : ElementListItemBase
    {
        #region construction and destruction

        public ElementValueListItemBase(uint clientAlias, uint serverAlias, string elementId)
            : base(clientAlias, serverAlias, elementId)
        {
        }

        #endregion

        #region public functions

        public bool Changed { get; set; }

        public ValueStatusTimestamp ValueStatusTimestamp { get; private set; }

        /// <summary>
        ///     Updates value unconditionally
        /// </summary>
        /// <param name="valueStatusTimestamp"></param>
        public void UpdateValueStatusTimestamp(ValueStatusTimestamp valueStatusTimestamp)
        {
            if (valueStatusTimestamp.ValueStatusCode == ValueStatusCodes.Unknown)
            {
                if (ValueStatusTimestamp.ValueStatusCode != ValueStatusCodes.Unknown)
                {
                    ValueStatusTimestamp = valueStatusTimestamp;
                    Changed = true;
                }
            }
            else
            {
                if (!valueStatusTimestamp.Compare(ValueStatusTimestamp))
                {
                    ValueStatusTimestamp = valueStatusTimestamp;
                    Changed = true;
                }                
            }            
        }

        public void Touch()
        {
            if (ValueStatusTimestamp.ValueStatusCode == ValueStatusCodes.Unknown) return;
            Changed = true;
        }

        public void Reset()
        {
            ValueStatusTimestamp = new ValueStatusTimestamp();
            Changed = false;
            PendingWriteValueStatusTimestamp = null;
        }

        public ValueStatusTimestamp? PendingWriteValueStatusTimestamp { get; set; }

        #endregion
    }
}