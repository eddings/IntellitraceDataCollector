using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelliTraceDataCollector
{
    internal interface IIntelliTraceDataCollector
    {
        string CollectionPlanNameWithPath { get; }

        string  OutputPath { get;  }
        ICommandStatus StartCollection();
        ICommandStatus StopCollection();
        void GetCollectionStatus();
    }
}
