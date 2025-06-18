using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace PdfBookletConverter.Services;

/// <summary>
/// Handles the business logic for converting a standard PDF into a booklet format.
/// </summary>
public class BookletService
{
    /// <summary>
    /// Creates a booklet-style PDF from a source PDF.
    /// </summary>
    /// <param name="inputPath">The full path to the source PDF file.</param>
    /// <param name="outputPath">The full path for the generated booklet PDF file.</param>
    public void CreateBooklet(string inputPath, string outputPath)
    {
        using var pdfReader = new PdfReader(inputPath);
        using var sourceDoc = new PdfDocument(pdfReader);
        using var pdfWriter = new PdfWriter(outputPath);
        using var destDoc = new PdfDocument(pdfWriter);

        int originalPageCount = sourceDoc.GetNumberOfPages();

        // A booklet must have a page count that is a multiple of 4.
        // We pad with blank pages if necessary.
        int paddedPageCount = originalPageCount % 4 == 0
            ? originalPageCount
            : originalPageCount + (4 - (originalPageCount % 4));

        var pageOrder = GetBookletPageOrder(paddedPageCount);

        // The booklet pages will be A4 landscape.
        destDoc.SetDefaultPageSize(PageSize.A4.Rotate());
        var bookletPageSize = destDoc.GetDefaultPageSize();
        float halfWidth = bookletPageSize.GetWidth() / 2;

        for (int i = 0; i < pageOrder.Count; i += 2)
        {
            int leftPageNum = pageOrder[i];
            int rightPageNum = pageOrder[i + 1];

            var newPage = destDoc.AddNewPage();
            var canvas = new PdfCanvas(newPage);

            // Add left page content
            if (leftPageNum <= originalPageCount)
            {
                var pageToCopy = sourceDoc.GetPage(leftPageNum).CopyAsFormXObject(destDoc);
                var targetRect = new Rectangle(0, 0, halfWidth, bookletPageSize.GetHeight());
                canvas.AddXObjectFittedIntoRectangle(pageToCopy, targetRect);
            }

            // Add right page content
            if (rightPageNum <= originalPageCount)
            {
                var pageToCopy = sourceDoc.GetPage(rightPageNum).CopyAsFormXObject(destDoc);
                var targetRect = new Rectangle(halfWidth, 0, halfWidth, bookletPageSize.GetHeight());
                canvas.AddXObjectFittedIntoRectangle(pageToCopy, targetRect);
            }
        }
    }

    /// <summary>
    /// Generates the correct page sequence for booklet printing.
    /// Example for 8 pages: [8, 1, 2, 7, 6, 3, 4, 5]
    /// </summary>
    /// <param name="totalPaddedPages">The total number of pages, padded to a multiple of 4.</param>
    /// <returns>A list of integers representing the page order.</returns>
    private List<int> GetBookletPageOrder(int totalPaddedPages)
    {
        var pageOrder = new List<int>();
        for (int i = 0; i < totalPaddedPages / 2; i += 2)
        {
            pageOrder.Add(totalPaddedPages - i);
            pageOrder.Add(i + 1);
            pageOrder.Add(i + 2);
            pageOrder.Add(totalPaddedPages - i - 1);
        }
        return pageOrder;
    }
}