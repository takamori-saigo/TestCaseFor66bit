using EquipmentWarehouse.Application.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http.Json;

namespace EquipmentWarehouse.Pages;

public partial class WarehouseItems : IAsyncDisposable
{
    private List<WareHouseItemDTO> allItems = new();
    private List<WareHouseItemDTO> filteredItems = new();
    private List<RefItem> allTypes = new();
    private List<RefItem> allSuppliers = new();
    private List<RefItem> allCountries = new();
    private List<RefItem> availableTypes = new();
    private List<RefItem> availableSuppliers = new();

    private string searchQuery = "";
    private string filterSupplierId = "";
    private string filterTypeId = "";
    private string priceFrom = "";
    private string priceTo = "";

    private bool showModal;
    private bool showDeleteConfirm;
    private bool showAddCountry;
    private Guid? editingItemId;
    private string formModel = "";
    private decimal formPrice;
    private string formPhotoUrl = "";
    private Guid formTypeId;
    private Guid formSupplierId;
    private Guid formCountryId;
    private string newCountryName = "";
    private string deletingItemName = "";
    private Guid deletingItemId;

    private Timer? searchDebounce;

    bool IsFormValid =>
        !string.IsNullOrWhiteSpace(formModel) &&
        formTypeId != Guid.Empty &&
        formSupplierId != Guid.Empty &&
        formCountryId != Guid.Empty &&
        formPrice >= 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    async Task LoadData()
    {
        try
        {
            allItems = await Service.GetAllAsync();
            allTypes = await Http.GetFromJsonAsync<List<RefItem>>(Navigation.BaseUri + "api/references/types") ?? new();
            allSuppliers = await Http.GetFromJsonAsync<List<RefItem>>(Navigation.BaseUri + "api/references/suppliers") ?? new();
            allCountries = await Http.GetFromJsonAsync<List<RefItem>>(Navigation.BaseUri + "api/references/countries") ?? new();
        }
        catch
        {
            allItems = new();
            allTypes = new();
            allSuppliers = new();
            allCountries = new();
        }
        availableTypes = new List<RefItem>(allTypes);
        availableSuppliers = new List<RefItem>(allSuppliers);
        ApplyFilters();
    }

    void ApplyFilters()
    {
        var query = searchQuery?.Trim().ToLower() ?? "";
        var supId = string.IsNullOrEmpty(filterSupplierId) ? (Guid?)null : Guid.Parse(filterSupplierId);
        var typId = string.IsNullOrEmpty(filterTypeId) ? (Guid?)null : Guid.Parse(filterTypeId);
        decimal? fromP = decimal.TryParse(priceFrom?.Replace('.', ','), out var f) ? f : null;
        decimal? toP = decimal.TryParse(priceTo?.Replace('.', ','), out var t) ? t : null;

        filteredItems = allItems.Where(item =>
        {
            if (supId.HasValue && item.Supplier.Id != supId.Value) return false;
            if (typId.HasValue && item.Type.Id != typId.Value) return false;
            if (fromP.HasValue && item.Price < fromP.Value) return false;
            if (toP.HasValue && item.Price > toP.Value) return false;
            if (!string.IsNullOrEmpty(query))
            {
                if (!item.Type.Name.ToLower().Contains(query) &&
                    !item.Supplier.Name.ToLower().Contains(query) &&
                    !item.Model.ToLower().Contains(query))
                    return false;
            }
            return true;
        }).ToList();

        var typeIds = supId.HasValue
            ? allItems.Where(i => i.Supplier.Id == supId.Value).Select(i => i.Type.Id).Distinct().ToHashSet()
            : allItems.Select(i => i.Type.Id).Distinct().ToHashSet();
        var supplierIds = typId.HasValue
            ? allItems.Where(i => i.Type.Id == typId.Value).Select(i => i.Supplier.Id).Distinct().ToHashSet()
            : allItems.Select(i => i.Supplier.Id).Distinct().ToHashSet();

        availableTypes = allTypes.Where(t => typeIds.Contains(t.Id)).ToList();
        availableSuppliers = allSuppliers.Where(s => supplierIds.Contains(s.Id)).ToList();
    }

    async Task OnSearchChanged(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString() ?? "";
        searchDebounce?.Dispose();
        searchDebounce = new Timer(async _ =>
        {
            await InvokeAsync(() => { ApplyFilters(); StateHasChanged(); });
        }, null, 300, Timeout.Infinite);
    }

    async Task OnSupplierFilterChanged(ChangeEventArgs e)
    {
        filterSupplierId = e.Value?.ToString() ?? "";
        ApplyFilters();
    }

    async Task OnTypeFilterChanged(ChangeEventArgs e)
    {
        filterTypeId = e.Value?.ToString() ?? "";
        ApplyFilters();
    }

    async Task OnPriceFromChanged(ChangeEventArgs e)
    {
        priceFrom = e.Value?.ToString() ?? "";
        ApplyFilters();
    }

    async Task OnPriceToChanged(ChangeEventArgs e)
    {
        priceTo = e.Value?.ToString() ?? "";
        ApplyFilters();
    }

    async Task ClearFilters()
    {
        searchQuery = "";
        filterSupplierId = "";
        filterTypeId = "";
        priceFrom = "";
        priceTo = "";
        ApplyFilters();
    }

    async Task StartCreate()
    {
        editingItemId = null;
        formModel = "";
        formPrice = 0;
        formPhotoUrl = "";
        formTypeId = Guid.Empty;
        formSupplierId = Guid.Empty;
        formCountryId = Guid.Empty;
        newCountryName = "";
        showAddCountry = false;
        showModal = true;
    }

    async Task StartEdit(WareHouseItemDTO item)
    {
        editingItemId = item.Id;
        formModel = item.Model;
        formPrice = item.Price;
        formPhotoUrl = item.PhotoUrl;
        formTypeId = item.Type.Id;
        formSupplierId = item.Supplier.Id;
        formCountryId = item.Country.Id;
        newCountryName = "";
        showAddCountry = false;
        showModal = true;
    }

    async Task SaveItem()
    {
        var countryName = allCountries.FirstOrDefault(c => c.Id == formCountryId)?.Name ?? "";
        var dto = new CreateUpdateItemDto
        {
            Id = editingItemId,
            Model = formModel.Trim(),
            Price = formPrice,
            PhotoUrl = formPhotoUrl?.Trim() ?? "",
            TypeId = formTypeId,
            SupplierId = formSupplierId,
            CountryName = countryName
        };
        await Service.SaveAsync(dto);
        showModal = false;
        await LoadData();
    }

    void CloseModal() => showModal = false;

    void StartAddCountry()
    {
        newCountryName = "";
        showAddCountry = true;
    }

    async Task AddCountry()
    {
        if (string.IsNullOrWhiteSpace(newCountryName)) return;
        var existing = allCountries.FirstOrDefault(c =>
            c.Name.Equals(newCountryName.Trim(), StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            formCountryId = existing.Id;
        }
        else
        {
            var newCountry = new RefItem { Id = Guid.NewGuid(), Name = newCountryName.Trim() };
            allCountries.Add(newCountry);
            formCountryId = newCountry.Id;
        }
        showAddCountry = false;
        newCountryName = "";
    }

    void CancelAddCountry()
    {
        showAddCountry = false;
        newCountryName = "";
    }

    async void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") await AddCountry();
    }

    void ConfirmDelete(WareHouseItemDTO item)
    {
        deletingItemId = item.Id;
        deletingItemName = $"{item.Type.Name} {item.Model}";
        showDeleteConfirm = true;
    }

    void CancelDelete() => showDeleteConfirm = false;

    async Task ExecuteDelete()
    {
        await Service.DeleteAsync(deletingItemId);
        showDeleteConfirm = false;
        await LoadData();
    }

    public async ValueTask DisposeAsync()
    {
        searchDebounce?.Dispose();
    }

    class RefItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }
}
