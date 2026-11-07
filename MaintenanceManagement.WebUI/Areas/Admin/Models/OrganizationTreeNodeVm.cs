namespace MaintenanceManagement.WebUI.Areas.Admin.Models
{
    public sealed class OrganizationTreeNodeVm
    {
        public int Id { get; set; }
        public int? ParentUnitId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class OrganizationTreeSelectorVm
    {
        public IReadOnlyList<OrganizationTreeNodeVm> Organizations { get; set; }
            = new List<OrganizationTreeNodeVm>();

        public IReadOnlyCollection<int> SelectedUnitIds { get; set; }
            = new List<int>();
    }
}
