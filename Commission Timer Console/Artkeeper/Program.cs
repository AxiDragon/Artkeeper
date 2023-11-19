internal class Program
{
    private static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Clear();

        new WindowTimer();
        WindowChangeDetector.StartActiveWindowChangeDetection();
    }
}
