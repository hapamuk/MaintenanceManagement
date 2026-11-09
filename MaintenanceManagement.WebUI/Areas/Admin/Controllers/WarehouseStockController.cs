using MaintenanceManagement.Application.DTOs.WarehouseOperations;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")][Authorize]
    public sealed class WarehouseStockController:Controller
    {
        private readonly IWarehouseOperationsService _service;
        public WarehouseStockController(IWarehouseOperationsService service)=>_service=service;
        private int CurrentUserId=>int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out int id)?id:0;
        [HttpGet]public async Task<IActionResult> Index(int? warehouseId,CancellationToken ct=default)
        {return View(new WarehousePageVm {WarehouseId=warehouseId,Warehouses=await _service.GetAuthorizedWarehousesAsync(CurrentUserId,ct),Stocks=await _service.GetStocksAsync(CurrentUserId,warehouseId,ct)});}
        [HttpGet]public async Task<IActionResult> CriticalStocks(int? warehouseId,CancellationToken ct=default)
        {var vm=new WarehousePageVm {WarehouseId=warehouseId,Warehouses=await _service.GetAuthorizedWarehousesAsync(CurrentUserId,ct),Stocks=await _service.GetStocksAsync(CurrentUserId,warehouseId,ct)};return View(vm);}
    }
}
