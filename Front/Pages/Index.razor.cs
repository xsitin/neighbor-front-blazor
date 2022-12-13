using System;
using System.Text.Json;
using System.Threading.Tasks;
using Board.Data;
using Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Board.Pages;

public partial class Index
{
    [Parameter]
    public int Page
    {
        get => PageInfo.Page;
        set => PageInfo.Page = Math.Clamp(value, 1, int.MaxValue);
    }

    private string SearchTitle { get; set; }
    private string Category { get; set; }
    [Inject] private AdsRepository Repository { get; set; }

    [Inject] public NavigationManager NavigationManager { get; set; }

    private PaginationInfo<Ad> PageInfo { get; set; } = new PaginationInfo<Ad>() { PageSize = 21, Page = 1 };


    protected override async Task OnInitializedAsync()
    {
        PageInfo = await GetUpdatedAds();

        async void AdsUpdater(object sender, LocationChangedEventArgs args)
        {
            if (args.Location == NavigationManager.Uri)
            {
                PageInfo = await GetUpdatedAds();
                StateHasChanged();
            }
            else
                NavigationManager.LocationChanged -= AdsUpdater;
        }

        NavigationManager.LocationChanged += AdsUpdater;
    }

    private async Task<PaginationInfo<Ad>> GetUpdatedAds()
    {
        if (PageInfo == null)
            return await Repository.GetPopularAsync();
        if (!string.IsNullOrEmpty(SearchTitle))
            return await Repository.SearchWithTitle(SearchTitle, PageInfo.Page, PageInfo.PageSize);
        if (!string.IsNullOrEmpty(Category))
            return await Repository.GetWithCategory(Category, PageInfo.Page, PageInfo.PageSize);
        return await Repository.GetPopularAsync(PageInfo.Page, PageInfo.PageSize);
    }

    private async Task CategorySelected(string category)
    {
        Category = category;
        SearchTitle = "";
        PageInfo = await Repository.GetWithCategory(category);
    }


    private async Task SearchAdsWithSelectedTitle()
    {
        if (string.IsNullOrEmpty(SearchTitle?.Trim()))
        {
            SearchTitle = "";
            Category = "";
            PageInfo = await Repository.GetPopularAsync();
        }
        else
            PageInfo = await Repository.SearchWithTitle(SearchTitle);
    }
}
