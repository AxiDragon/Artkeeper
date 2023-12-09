using System;
using System.Threading;

namespace Artkeeper.StaticClasses
{
    public static class Update
    {
        private const int sleepTime = 100;
        private static Thread updateThread;
        public static Action OnUpdate = delegate { };

        private static bool initialized = false;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            updateThread = new Thread(UpdateLoop);
            updateThread.IsBackground = true;
            updateThread.Start();
        }

        public static void UpdateLoop()
        {
            while (true)
            {
                OnUpdate?.Invoke();

                Thread.Sleep(sleepTime);
            }
        }
    }
}
