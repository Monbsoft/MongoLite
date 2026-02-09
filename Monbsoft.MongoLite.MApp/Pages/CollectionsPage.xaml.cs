using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.ViewModels;

namespace Monbsoft.MongoLite.MApp.Pages;

public partial class CollectionsPage : ContentPage
{
    private readonly CollectionsViewModel _viewModel;

    public CollectionsPage(CollectionsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CollectionsPage.OnAppearing error: {ex.Message}");
        }
    }

    private async void OnCollectionSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is MongoCollectionInfo collection)
        {
            if (sender is CollectionView cv)
                cv.SelectedItem = null;

            await Shell.Current.GoToAsync($"documents?collectionName={collection.Name}");
        }
    }
}
