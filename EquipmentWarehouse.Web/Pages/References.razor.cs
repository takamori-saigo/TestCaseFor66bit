using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace EquipmentWarehouse.Pages;

public partial class References
{
    private string activeTab = "types";
    private List<ReferenceItem> types = new();
    private List<ReferenceItem> suppliers = new();

    private bool showModal;
    private bool showDeleteConfirm;
    private bool isTypeTab;
    private Guid? editingId;
    private string formName = "";
    private string? formDescription;
    private Guid deletingId;
    private string deletingName = "";
    private bool deletingIsType;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    async Task LoadData()
    {
        try
        {
            types = await Http.GetFromJsonAsync<List<ReferenceItem>>(Navigation.BaseUri + "api/references/types") ?? new();
            suppliers = await Http.GetFromJsonAsync<List<ReferenceItem>>(Navigation.BaseUri + "api/references/suppliers") ?? new();
        }
        catch
        {
            types = new();
            suppliers = new();
        }
    }

    void ShowTypesTab() => activeTab = "types";
    void ShowSuppliersTab() => activeTab = "suppliers";

    void StartCreate(bool isType)
    {
        isTypeTab = isType;
        editingId = null;
        formName = "";
        formDescription = null;
        showModal = true;
    }

    void StartEdit(bool isType, ReferenceItem item)
    {
        isTypeTab = isType;
        editingId = item.Id;
        formName = item.Name;
        formDescription = item.Description;
        showModal = true;
    }

    async Task SaveEntity()
    {
        var url = isTypeTab ? $"{Navigation.BaseUri}api/references/types" : $"{Navigation.BaseUri}api/references/suppliers";
        var body = new SaveReferenceBody { Id = editingId, Name = formName, Description = formDescription };
        await Http.PostAsJsonAsync(url, body);
        showModal = false;
        await LoadData();
    }

    void CloseModal() => showModal = false;

    void ConfirmDelete(bool isType, ReferenceItem item)
    {
        deletingIsType = isType;
        deletingId = item.Id;
        deletingName = item.Name;
        showDeleteConfirm = true;
    }

    void CancelDelete() => showDeleteConfirm = false;

    async Task ExecuteDelete()
    {
        var url = deletingIsType
            ? $"{Navigation.BaseUri}api/references/types/{deletingId}"
            : $"{Navigation.BaseUri}api/references/suppliers/{deletingId}";
        await Http.DeleteAsync(url);
        showDeleteConfirm = false;
        await LoadData();
    }

    string FormatDate(DateTime dt) => dt.ToString("dd.MM.yyyy HH:mm");

    class ReferenceItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    class SaveReferenceBody
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
