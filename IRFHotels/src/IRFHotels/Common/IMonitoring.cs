using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IRFHotels.Common
{

    [ServiceContract]
    public interface IMonitoring
    {
        [OperationContract]
        void ImAlive(int id, int freeRoomCount);
    }
}
