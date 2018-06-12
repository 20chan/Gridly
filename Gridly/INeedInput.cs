using System.Collections.Generic;

namespace Gridly
{
    public interface INeedInput
    {
        List<IConnectable> ConnectedInputs { get; }
        void ConnectFrom(IConnectable from);
        void DisconnectFrom(IConnectable from);
    }
}
