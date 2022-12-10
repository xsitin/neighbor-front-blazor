namespace Board.Pages;

using System;
using System.Threading.Tasks;
using Common.Models;
using Data;
using Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using JsonSerializer = System.Text.Json.JsonSerializer;

public partial class Profile
{
    private int page;
    [Parameter] public string Login { get; set; }

    public AccountViewModel AccountViewModel { get; set; } = new();
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] public ImageHelper ImageHelper { get; set; }
    [Inject] private AccountRepository AccountRepository { get; set; }
    [Inject] private AdsRepository AdsRepository { get; set; }
    private PaginationInfo<Ad> PageInfo { get; set; } = new() { Page = 1, PageSize = 21 };

    [Parameter]
    public int Page
    {
        get => PageInfo.Page;
        set => PageInfo.Page = Math.Clamp(value, 1, int.MaxValue);
    }

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine(JsonSerializer.Serialize(PageInfo));
        NavigationManager.LocationChanged += Update;
        await UpdateData();
    }

    private async void Update(object o, LocationChangedEventArgs args)
    {
        if (args.Location == NavigationManager.Uri)
            await UpdateData();
        else
            NavigationManager.LocationChanged -= Update;
    }

    private async Task UpdateData()
    {
        AccountViewModel = await AccountRepository.GetAccount(Login);
        Console.WriteLine(JsonSerializer.Serialize(AccountViewModel));
        PageInfo = await AdsRepository.GetUserAdsAsync(Login, PageInfo.Page, PageInfo.PageSize);
        StateHasChanged();
    }
}
