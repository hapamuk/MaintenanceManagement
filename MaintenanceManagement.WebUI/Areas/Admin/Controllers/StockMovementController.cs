using MaintenanceManagement.Application.DTOs.WarehouseOperations;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")][Authorize]
    public sealed class StockMovementController : Controller
    {
        private readonly IWarehouseOperationsService _service;
        public StockMovementController(IWarehouseOperationsService service)=>_service=service;
        private int CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out int id)?id:0;
        private async Task<WarehousePageVm> Page(CancellationToken ct)
            => new WarehousePageVm{Warehouses=await _service.GetAuthorizedWarehousesAsync(CurrentUserId,ct)};
        [HttpGet] public async Task<IActionResult> Index(CancellationToken ct=default)
        { var vm=await Page(ct);vm.Movements=await _service.GetMovementsAsync(CurrentUserId,null,null,ct);return View(vm); }
        [HttpGet] public async Task<IActionResult> CreateIn(CancellationToken ct=default)=>View(await Page(ct));
        [HttpGet] public async Task<IActionResult> CreateOut(CancellationToken ct=default)=>View(await Page(ct));
        [HttpGet] public async Task<IActionResult> MaterialsByWarehouse(int warehouseId,bool onlyStocked,CancellationToken ct=default)
        {
            try { return Json(await _service.GetMaterialsForWarehouseAsync(warehouseId,CurrentUserId,onlyStocked,ct)); }
            catch(Exception ex) when(ex is ValidationException||ex is UnauthorizedAccessException)
            { return BadRequest(new {message=ex.Message}); }
        }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIn(StockOperationInput input,CancellationToken ct=default)
            =>await Save(input,true,ct);
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOut(StockOperationInput input,CancellationToken ct=default)
            =>await Save(input,false,ct);
        private async Task<IActionResult> Save(StockOperationInput input,bool isIn,CancellationToken ct)
        {
            if(!ModelState.IsValid){var vm=await Page(ct);vm.StockInput=input;return View(isIn?"CreateIn":"CreateOut",vm);}
            try
            {
                var reference=isIn?await _service.StockInAsync(input,CurrentUserId,ct):await _service.StockOutAsync(input,CurrentUserId,ct);
                TempData["SuccessMessage"]="İşlem tamamlandı. Referans: "+reference;
                return RedirectToAction(nameof(History));
            }
            catch(Exception ex) when(ex is ValidationException || ex is UnauthorizedAccessException || ex is InvalidOperationException)
            {ModelState.AddModelError("",ex.Message);var vm=await Page(ct);vm.StockInput=input;return View(isIn?"CreateIn":"CreateOut",vm);}
        }
        [HttpGet] public async Task<IActionResult> History(int? warehouseId,string? search,CancellationToken ct=default)
        {var vm=await Page(ct);vm.WarehouseId=warehouseId;vm.Search=search;vm.Movements=await _service.GetMovementsAsync(CurrentUserId,warehouseId,search,ct);return View(vm);}
        [HttpGet] public async Task<IActionResult> WorkOrderUsages(string? search,CancellationToken ct=default)
        {var vm=await Page(ct);vm.Search=search;vm.WorkOrders=await _service.GetWorkOrderUsesAsync(CurrentUserId,search,ct);return View(vm);}
    }
}
