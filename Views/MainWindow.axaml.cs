using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace MMMToText.Views;

public partial class MainWindow : Window
{
    private string exportButtonText = "Видобути та експортувати текст";
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

                // Or open the file stream if you need to read the file contents
                // using (var stream = await file.OpenReadAsync())
                // {
                //     // Read from the stream as needed
                // }
            }
        }
    }

    public void ExportText_OnClick(object sender, RoutedEventArgs args)
    {
        
    }
}