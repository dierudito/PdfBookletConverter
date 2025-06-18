using PdfBookletConverter.Services;

namespace PdfBookletConverter;

public class Program
{
    // The main entry point for the application.
    [STAThread]
    public static void Main(string[] args)
    {
        // If args are provided, run in silent mode (e.g., from context menu).
        // Otherwise, run in interactive mode (e.g., direct execution).
        if (args.Length > 0)
        {
            RunSilentMode(args[0]);
        }
        else
        {
            RunInteractiveMode();
        }
    }

    /// <summary>
    /// Runs the application in silent mode, processing a file path from arguments.
    /// Errors are displayed in a message box.
    /// </summary>
    /// <param name="inputPath">The full path of the input PDF file.</param>
    private static void RunSilentMode(string inputPath)
    {
        try
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
            {
                throw new FileNotFoundException("The specified input file does not exist or the path is invalid.", inputPath);
            }

            string? inputDirectory = Path.GetDirectoryName(inputPath);
            string inputFileName = Path.GetFileNameWithoutExtension(inputPath);
            string outputPath = Path.Combine(inputDirectory ?? string.Empty, $"{inputFileName}_booklet.pdf");

            var bookletService = new BookletService();
            bookletService.CreateBooklet(inputPath, outputPath);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while creating the booklet:\n\n{ex.Message}";
            WindowsApi.MessageBox(IntPtr.Zero, errorMessage, "PDF Booklet Converter Error", WindowsApi.MB_OK | WindowsApi.MB_ICONERROR);
        }
    }

    /// <summary>
    /// Runs the application in interactive mode, with a console window for user input/output.
    /// </summary>
    private static void RunInteractiveMode()
    {
        // Allocate a console window as this is a WinExe application now.
        if (WindowsApi.AllocConsole())
        {
            try
            {
                // Re-opening standard streams is necessary after AllocConsole.
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                Console.SetIn(new StreamReader(Console.OpenStandardInput()));

                Console.WriteLine("--- PDF Booklet Converter (Interactive Mode) ---");

                Console.Write("Enter the full path of the input PDF file: ");
                string? inputPath = Console.ReadLine();
                inputPath = inputPath?.Replace("\"", "").Trim();

                if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
                {
                    throw new FileNotFoundException("The specified input file does not exist or path is invalid.", inputPath ?? "null");
                }

                string? inputDirectory = Path.GetDirectoryName(inputPath);
                string inputFileName = Path.GetFileNameWithoutExtension(inputPath);
                string outputPath = Path.Combine(inputDirectory ?? string.Empty, $"{inputFileName}_booklet.pdf");

                var bookletService = new BookletService();

                Console.WriteLine("\nProcessing... Please wait.");
                bookletService.CreateBooklet(inputPath, outputPath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSuccess! Booklet created at: {outputPath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                WindowsApi.FreeConsole();
            }
        }
        else
        {
            // If console allocation fails, show a message box.
            WindowsApi.MessageBox(IntPtr.Zero, "Failed to allocate console for interactive mode.", "Startup Error", WindowsApi.MB_OK | WindowsApi.MB_ICONERROR);
        }
    }
}