using elenora.BusinessModels;
using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.Inventory
{
    public interface IInventoryService
    {
        public List<ComponentInventory> GetComponentsInventory();
        public void DecreaseInventory(int orderId);
        public bool IsSoldOut(Bracelet product);
        public bool IsBeadSoldOutInBraceletDesigner(Component component);
        public int GetBraceletStock(Bracelet product);
        public List<ProductsInventoryReportItem> GetInventoryReport();
    }
}
