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
    public sealed class WarehouseTransferController:Controller
    {
        private readonly IWarehouseOperationsService _service;
        public WarehouseTransferController(IWarehouseOperationsService service)=>_service=service;
        private int CurrentUserId=>int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out int id)?id:0;
        [HttpGet] public async Task<IActionResult> Index(CancellationToken ct=default)
        {var vm=await Page(ct);vm.Movements=await _service.GetMovementsAsync(CurrentUserId,null,"TRF-",ct);return View(vm);}
        [HttpGet] public async Task<IActionResult> Create(CancellationToken ct=default)=>View(await Page(ct));
        private async Task<WarehousePageVm> Page(CancellationToken ct)
        =>new WarehousePageVm{Warehouses=await _service.GetAuthorizedWarehousesAsync(CurrentUserId,ct)};
        [HttpGet] public async Task<IActionResult> Targets(int sourceWarehouseId,int materialId,CancellationToken ct=default)
        {
            try { return Json(await _service.GetTransferTargetsAsync(sourceWarehouseId,materialId,CurrentUserId,ct)); }
            catch(Exception ex) when(ex is ValidationException||ex is UnauthorizedAccessException)
            { return BadRequest(new {message=ex.Message}); }
        }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseTransferInput input,CancellationToken ct=default)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var reference=await _service.TransferAsync(input,CurrentUserId,ct);
                    TempData["SuccessMessage"]="Transfer tamamlandı. Referans: "+reference;
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex) when(ex is ValidationException||ex is UnauthorizedAccessException||ex is InvalidOperationException)
                {ModelState.AddModelError("",ex.Message);}
            }
            var vm=await Page(ct);vm.TransferInput=input;return View(vm);
        }
    }
}
