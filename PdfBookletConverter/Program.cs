using iText.Kernel.Exceptions;
using PdfBookletConverter.Services;

namespace PdfBookletConverter;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("--- PDF Booklet Converter ---");

        try
        {
            Console.Write("Enter the full path of the input PDF file: ");
            string? inputPath = Console.ReadLine();

            inputPath = inputPath?.Replace("\"", "").Trim();

            // Basic validation
            if (string.IsNullOrWhiteSpace(inputPath) )
            {
                throw new ArgumentException("Input and output paths cannot be empty.");
            }

            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("The specified input file does not exist.", inputPath);
            }

            string inputDirectory = Path.GetDirectoryName(inputPath)!;

            // Generate the output path in the same directory as the input file
            string outputPath = Path.Combine(inputDirectory ?? string.Empty, "booklet_output.pdf");

            var bookletService = new BookletService();

            Console.WriteLine("\nProcessing... Please wait.");
            bookletService.CreateBooklet(inputPath, outputPath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSuccess! Booklet created at: {outputPath}");
            Console.ResetColor();
        }
        catch(PdfException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nAn error occurred: {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nAn error occurred: {ex.Message}");
            Console.ResetColor();
        }
    }
}