using Microsoft.Xna.Framework;

namespace Gridly
{
    public interface IConnectable
    {
        uint ID { get; }
        Vector2 Position { get; }

        void Activate(IConnectable from);
    }
}
