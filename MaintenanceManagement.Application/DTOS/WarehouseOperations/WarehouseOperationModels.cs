using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceManagement.Application.DTOs.WarehouseOperations
{
    public sealed class StockOperationInput
    {
        [Range(1,int.MaxValue)] public int WarehouseId {get;set;}
        [Range(1,int.MaxValue)] public int MaterialId {get;set;}
        [Range(typeof(decimal),"0.0001","999999999",ParseLimitsInInvariantCulture=true,
            ErrorMessage="Miktar sıfırdan büyük olmalıdır.")]
        public decimal Quantity {get;set;}
        [StringLength(300)] public string? Description {get;set;}
    }
    public sealed class WarehouseTransferInput
    {
        [Range(1,int.MaxValue)] public int SourceWarehouseId {get;set;}
        [Range(1,int.MaxValue)] public int TargetWarehouseId {get;set;}
        [Range(1,int.MaxValue)] public int MaterialId {get;set;}
        [Range(typeof(decimal),"0.0001","999999999",ParseLimitsInInvariantCulture=true)]
        public decimal Quantity {get;set;}
        [StringLength(300)] public string? Description {get;set;}
    }
    // Çoklu transfer sepeti: tek kayıt isteğinde birden fazla malzeme.
    public sealed class WarehouseTransferCartInput
    {
        [Range(1, int.MaxValue)] public int SourceWarehouseId { get; set; }
        [Range(1, int.MaxValue)] public int TargetWarehouseId { get; set; }
        [StringLength(300)] public string? Description { get; set; }
        [Required, MinLength(1, ErrorMessage = "Sepete en az bir malzeme ekleyiniz.")]
        public List<WarehouseTransferCartItemInput> Items { get; set; } = new();
    }
    public sealed class WarehouseTransferCartItemInput
    {
        [Range(1, int.MaxValue)] public int MaterialId { get; set; }
        [Range(typeof(decimal), "0.0001", "999999999", ParseLimitsInInvariantCulture = true)]
        public decimal Quantity { get; set; }
    }
    public sealed class TransferMaterialOption
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public decimal AvailableQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
    public sealed class WorkOrderMaterialUsageInput
    {
        [Range(1,int.MaxValue)] public int WorkOrderId { get; set; }
        [Range(1,int.MaxValue)] public int WarehouseId { get; set; }
        [Range(1,int.MaxValue)] public int MaterialId { get; set; }
        [Range(typeof(decimal),"0.0001","999999999",ParseLimitsInInvariantCulture=true)]
        public decimal Quantity { get; set; }
        [StringLength(300)] public string? Description { get; set; }
    }
    public sealed class OptionRow
    {
        public int Id {get;set;}
        public string Text {get;set;}=string.Empty;
        public int? UnitId {get;set;}
        public string OrganizationPath {get;set;}=string.Empty;
        public int Depth {get;set;}
    }
    public sealed class MovementRow
    {
        public int Id {get;set;}
        public DateTime Date {get;set;}
        public string Warehouse {get;set;}=string.Empty;
        public string SourceWarehouse {get;set;}=string.Empty;
        public string TargetWarehouse {get;set;}=string.Empty;
        public string Operation {get;set;}=string.Empty;
        public string Material {get;set;}=string.Empty;
        public string Unit {get;set;}=string.Empty;
        public string Reference {get;set;}=string.Empty;
        public string Description {get;set;}=string.Empty;
        public string Direction {get;set;}=string.Empty;
        public decimal Quantity {get;set;}
        public string Personnel {get;set;}=string.Empty;
    }
    public sealed class StockRow
    {
        public int WarehouseId {get;set;}
        public string Warehouse {get;set;}=string.Empty;
        public string Material {get;set;}=string.Empty;
        public string Unit {get;set;}=string.Empty;
        public decimal Quantity {get;set;}
        public decimal Minimum {get;set;}
        public bool IsCritical => Quantity<=Minimum;
    }
    public sealed class WorkOrderUseRow
    {
        public int WorkOrderMaterialId {get;set;}
        public int WorkOrderId {get;set;}
        public string WorkOrderNumber {get;set;}=string.Empty;
        public string WorkOrderTitle {get;set;}=string.Empty;
        public string Warehouse {get;set;}=string.Empty;
        public string Material {get;set;}=string.Empty;
        public string Unit {get;set;}=string.Empty;
        public decimal Quantity {get;set;}
        public string MovementReference {get;set;}=string.Empty;
        public DateTime Date {get;set;}
    }
    public sealed class WarehousePageVm
    {
        public int? WarehouseId {get;set;}
        public int? MaterialId {get;set;}
        public string? Search {get;set;}
        public StockOperationInput StockInput {get;set;}=new();
        public WarehouseTransferInput TransferInput {get;set;}=new();
        public IReadOnlyList<OptionRow> Warehouses {get;set;}=Array.Empty<OptionRow>();
        public IReadOnlyList<OptionRow> Materials {get;set;}=Array.Empty<OptionRow>();
        public IReadOnlyList<MovementRow> Movements {get;set;}=Array.Empty<MovementRow>();
        public IReadOnlyList<StockRow> Stocks {get;set;}=Array.Empty<StockRow>();
        public IReadOnlyList<WorkOrderUseRow> WorkOrders {get;set;}=Array.Empty<WorkOrderUseRow>();
    }
}
