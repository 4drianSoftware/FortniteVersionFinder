namespace FortniteVersionFinder.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ActualMain();
        }
        static void ActualMain()
        {
            Console.WriteLine("Enter the path to the FortniteClient-Win64-Shipping.exe file:");
            string path = Console.ReadLine();
            if (!File.Exists(path + "\\FortniteClient-Win64-Shipping.exe"))
            {
                Console.WriteLine("Invalid path! Paths should look like D:\\Fortnite 4.5\\FortniteGame\\Binaries\\Win64.");
                Console.Clear();
                ActualMain();
            }
            else
            {
                Console.WriteLine("Fortnite version is " + Finder.GetVersion(path + "\\FortniteClient-Win64-Shipping.exe"));
                Console.ReadLine();
            }
        }
    }
}