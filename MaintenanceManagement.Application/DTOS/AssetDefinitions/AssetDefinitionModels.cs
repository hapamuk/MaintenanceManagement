using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceManagement.Application.DTOs.AssetDefinitions
{
    public sealed class AssetCategoryDefinitionInput
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Kategori adı zorunludur."), StringLength(150)]
        public string Name { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public List<int> UnitIds { get; set; } = new();
    }
    public sealed class BrandDefinitionInput
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Marka adı zorunludur."), StringLength(150)]
        public string Name { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
    public sealed class AssetModelDefinitionInput
    {
        public int Id { get; set; }
        [Range(1,int.MaxValue, ErrorMessage = "Marka seçiniz.")]
        public int BrandId { get; set; }
        [Required(ErrorMessage = "Model adı zorunludur."), StringLength(150)]
        public string Name { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
    public sealed class DefinitionOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public int? BrandId { get; set; }
    }
    public sealed class DefinitionPageVm
    {
        public string PageTitle { get; set; } = "";
        public string Type { get; set; } = "";
        public AssetCategoryDefinitionInput Category { get; set; } = new();
        public BrandDefinitionInput Brand { get; set; } = new();
        public AssetModelDefinitionInput Model { get; set; } = new();
        public IReadOnlyList<DefinitionOption> Categories { get; set; } = Array.Empty<DefinitionOption>();
        public IReadOnlyList<DefinitionOption> Brands { get; set; } = Array.Empty<DefinitionOption>();
        public IReadOnlyList<DefinitionOption> Models { get; set; } = Array.Empty<DefinitionOption>();
        public IReadOnlyList<DefinitionOption> Units { get; set; } = Array.Empty<DefinitionOption>();
    }
}
