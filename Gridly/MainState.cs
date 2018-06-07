using static Gridly.EditorState;

namespace Gridly
{
    internal enum EditorState
    {
        IDEAL,
        NEURON_CONNECTING,
        NEURON_DRAGGING,
        NEURON_DISCONNECTING,
    }

    internal static class MainStateExtensions
    {
        public static bool IsNeuralEditor(this EditorState state)
            => IDEAL <= state && state <= NEURON_DISCONNECTING;
    }
}
