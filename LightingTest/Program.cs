using System;

namespace LightingTest {
    #if WINDOWS || LINUX
    public static class Program {
        [STAThread]
        static void Main() {
            using(var game = new LightingTest()) {
                game.Run();
            }
        }
    }
    #endif
}
