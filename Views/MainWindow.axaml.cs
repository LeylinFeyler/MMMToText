using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Tesseract;
using IronOcr;

namespace MMMToText.Views;

public partial class MainWindow : Window
{
    private string exportButtonText = "Видобути та експортувати текст";
    private List<IStorageFile> imageFiles = new List<IStorageFile>();
    public MainWindow()
    {
        InitializeComponent();
        this.CanResize = false;
        exportButton.Content = exportButtonText + " [0 файлів]";
    }

    public async void UploadFiles_OnClick(object sender, RoutedEventArgs args)
    {
        if (VisualRoot is not Window window)
            return;

        var storageProvider = window.StorageProvider;

        if (storageProvider.CanOpen)
        {
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Виберіть файли",
                AllowMultiple = true,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Зображення")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" }
                    },
                    FilePickerFileTypes.All
                }
            });

            exportButton.Content = exportButtonText + $"[{files.Count} файлів]";

            foreach (var file in files)
            {
                // You can use file.Name to get the file name
                Console.WriteLine(file.Name);
                Console.WriteLine(file.Path);
                System.Console.WriteLine();

                // Or open the file stream if you need to read the file contents
                // using (var stream = await file.OpenReadAsync())
                // {
                //     // Read from the stream as needed
                // }

                imageFiles.Add(file);
            }
        }
    }

    public void ExportText_OnClick(object sender, RoutedEventArgs args)
    {
        foreach (var file in imageFiles)
        {
            // Tess_Ocr(file);
            // Iron_Ocr(file);
            Manga_Ocr(file);
        }
    }

    public void Tess_Ocr(IStorageFile file)
    {
        // Here you would implement the logic to extract text from the image file
        // For example, using an OCR library like Tesseract or similar
        Console.WriteLine($"Експорт тексту з файлу: {file.Name}");

        var imagePath = file.Path is Uri uri ? uri.LocalPath : file.Path.ToString();
        try
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imagePath.ToString()))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        Console.WriteLine($"Text (GetText): \r\n{text}");
                        // Console.WriteLine("Text (iterator):");
                        // using (var iter = page.GetIterator())
                        // {
                        //     iter.Begin();

                        //     do
                        //     {
                        //         do
                        //         {
                        //             do
                        //             {
                        //                 do
                        //                 {
                        //                     if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                        //                     {
                        //                         Console.WriteLine("<BLOCK>");
                        //                     }

                        //                     Console.Write(iter.GetText(PageIteratorLevel.Word));
                        //                     Console.Write(" ");

                        //                     if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                        //                     {
                        //                         Console.WriteLine();
                        //                     }
                        //                 } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                        //                 if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                        //                 {
                        //                     Console.WriteLine();
                        //                 }
                        //             } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                        //         } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                        //     } while (iter.Next(PageIteratorLevel.Block));
                        // }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
            Console.WriteLine("Unexpected Error: " + e.Message);
            Console.WriteLine("Details: ");
            Console.WriteLine(e.ToString());
        }
        Console.Write("Press any key to continue . . . ");
    }

    public void Iron_Ocr(IStorageFile file)
    {
        var ocr = new IronTesseract();
        // Set the OCR to use Chinese Simplified
        ocr.Language = OcrLanguage.English;
        using (var input = new OcrInput())
        {
            // Add an image to be processed
            input.LoadImage(file.Path.LocalPath);
            // Optional: Enhance the input by deskewing or denoising the image
            // input.Deskew();
            // input.DeNoise();
            // Process the image and retrieve the result
            var result = ocr.Read(input);
            // Store the recognized text in a string
            string testResult = result.Text;
            System.Console.WriteLine($"Text: {testResult}");
            // Save the recognized text to a file since the console might not display Unicode characters properly
            //result.SaveAsTextFile("chinese.txt");
        }
    }

    public void Manga_Ocr(IStorageFile file)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"Manga-Ocr/ocr_wrapper.py \"{file.Path.LocalPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();
        process.WaitForExit();

        Console.WriteLine($"Output text: \n{output}");

        if (!string.IsNullOrWhiteSpace(errors))
            Console.WriteLine("Python errors: " + errors);

    }
}