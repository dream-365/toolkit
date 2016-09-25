namespace DirUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
            string disk = args[0];

            string symbol = args[1];

            string exportPath = args[2];

            var vdisk = VirtualDisk.From(disk, symbol);

            vdisk.ExportTo(exportPath);
        }
    }
}
