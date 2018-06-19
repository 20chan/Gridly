namespace Gridly
{
    public class Tester
    {
        public TestCase TestCase;

        public bool[][] Inputs => TestCase.Inputs;
        public bool[][] Outputs => TestCase.Outputs;
        public int ElapsedTick;
        public int CorcondanceCount;
        public bool Succeed;

        public Tester(TestCase tc)
        {
            TestCase = tc;
            Reset();
        }

        public void Reset()
        {
            ElapsedTick = 0;
            CorcondanceCount = 0;
            Succeed = false;
        }
    }
}
