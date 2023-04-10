using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ssz.DataAccessGrpcServer_OpcUaClient
{
    /// <summary>
    ///     Struct
    /// </summary>
    public struct DsParamInfo
    {
        #region public functions

        public static readonly DsParamInfo[] EmptyParamsArray = new DsParamInfo[0];

        /// <summary>
        ///     Must be in Upper-Case
        /// </summary>
        public string Name;

        public string Desc;          

        public bool IsArray;

        #endregion
    }
}
