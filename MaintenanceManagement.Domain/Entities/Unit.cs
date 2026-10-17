using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class Unit : BaseEntity
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Birim Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Birim Kodu")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Organizasyon Türü")]
        public int OrganizationTypeId { get; set; }

        [Display(Name = "Üst Birim")]
        public int? ParentUnitId { get; set; }

        public bool IsActive { get; set; } = true;

        public OrganizationType OrganizationType { get; set; } = null!;
        public Unit? ParentUnit { get; set; }

        public ICollection<Unit> ChildUnits { get; set; } =
            new HashSet<Unit>();

        public ICollection<User> Users { get; set; } =
            new HashSet<User>();

        public ICollection<Role> Roles { get; set; } =
            new HashSet<Role>();

        public ICollection<Team> Teams { get; set; } =
            new HashSet<Team>();

        public ICollection<UnitWorkType> UnitWorkTypes { get; set; } =
            new HashSet<UnitWorkType>();

        public ICollection<Asset> Assets { get; set; } =
            new HashSet<Asset>();

        public ICollection<Material> Materials { get; set; } =
            new HashSet<Material>();

        public ICollection<Warehouse> Warehouses { get; set; } =
            new HashSet<Warehouse>();

        public ICollection<WorkRequest> RequestedWorkRequests
        { get; set; } = new HashSet<WorkRequest>();

        public ICollection<WorkRequest> ResponsibleWorkRequests
        { get; set; } = new HashSet<WorkRequest>();

        public ICollection<WorkOrder> WorkOrders { get; set; } =
            new HashSet<WorkOrder>();

        public ICollection<PlannedMaintenance> PlannedMaintenances
        { get; set; } = new HashSet<PlannedMaintenance>();

        public ICollection<AssetMovement> OutgoingAssetMovements
        { get; set; } = new HashSet<AssetMovement>();

        public ICollection<AssetMovement> IncomingAssetMovements
        { get; set; } = new HashSet<AssetMovement>();
    }
}
