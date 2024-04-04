using System.Windows.Threading;

class Settings
{
    public static int FailedAuthCount = 0;
    public static Thread blockThread;
    public static bool isBlocking = false;
}

