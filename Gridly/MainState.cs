using static Gridly.MainState;

namespace Gridly
{
    internal enum MainState
    {
        IDEAL,
        NEURON_CONNECTING,
        NEURON_DRAGGING,
        NEURON_DISCONNECTING,
        INNER_CIRCUIT_IDEAL,
        INNER_CIRCUIT_CONNECTING,
        INNER_CIRCUIT_DRAGGING,
        INNER_CIRCUIT_DISCONNECTING,
    }

    internal static class MainStateExtensions
    {
        public static bool IsNeuralEditor(this MainState state)
            => IDEAL <= state && state <= NEURON_DISCONNECTING;
    }
}
