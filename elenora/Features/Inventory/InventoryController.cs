using elenora.Controllers;
using elenora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;

namespace elenora.Features.Inventory
{
    public class InventoryController : BaseController
    {
        private readonly IInventoryService inventoryService;
        private readonly DataContext context;

        public InventoryController(DataContext context, IInventoryService inventoryService, IConfiguration configuration, ICustomerService customerService, IPromotionService promotionService) : base(configuration, customerService, promotionService)
        {
            this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/inventory-report")]
        public ActionResult Report()
        {
            return Ok(inventoryService.GetComponentsInventory());
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/set-inventory/{componentId}-{quantity}")]
        public ActionResult SetInventory(int componentId, int quantity)
        {
            var component = context.Components.First(c => c.Id == componentId);
            component.Quantity = quantity;
            context.SaveChanges();
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/component-quality/{id}-{score}")]
        public IActionResult SetComponentQuality(int id, int score)
        {
            var component = context.Components.First(c => c.Id == id);
            component.ImagesQuality = score;
            context.SaveChanges();
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/component-remark/{id}-{remark}")]
        public IActionResult SetComponentQuality(int id, string remark)
        {
            var component = context.Components.First(c => c.Id == id);
            component.Remark = remark;
            context.SaveChanges();
            return Ok();
        }

        [Route("/admin-report/product-inventory")]
        public IActionResult InventoryReport(string p)
        {
            if (p == "2D3CD721-F872-43E5-ADC3-9A1F54768744")
            {
                var result = new StringBuilder();
                var items = inventoryService.GetInventoryReport();
                result.AppendLine("idstring,category,active,soldout");
                foreach (var item in items)
                {
                    result.AppendLine($"{item.IdString},{item.Category},{item.Active},{item.SoldOut}");
                }
                return Content(result.ToString());
            }
            return NotFound();
        }
    }
}
