using elenora.BusinessModels;
using elenora.Models;
using elenora.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elenora.Features.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly DataContext context;
        private readonly IEmailService emailService;
        private const int CSOMO_REJTO = 115;

        public InventoryService(IEmailService emailService, DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public List<ComponentInventory> GetComponentsInventory()
        {
            var components = context.Components
                .Include(c => c.ComponentSuppliers)
                .Include(c => c.ProductComponents)
                .ThenInclude(pc => pc.Product)
                .ToList();
            var orders = context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => (oi as BraceletOrderItem).Product).ThenInclude(p => p.ProductComponents)
                .Include(o => o.OrderItems).ThenInclude(oi => (oi as CustomBraceletOrderItem).Components).ThenInclude(cc => cc.Component)
                .Where(o => (o.Status == OrderStatusEnum.OrderCompleted || o.Status == OrderStatusEnum.OrderPrepared || o.Status == OrderStatusEnum.PaymentReceived ||
                            (o.Status == OrderStatusEnum.OrderPlaced && o.PaymentMethod == PaymentMethodEnum.PayAtDelivery)) &&
                             o.OrderedDate > DateTime.Now.AddDays(-30))
                
                .ToList();

            var result = new List<ComponentInventory>();
            foreach (var component in components)
            {
                var resultComponent = new ComponentInventory
                {
                    ComponentId = component.Id,
                    Name = component.Name,
                    ComponentImage = component.ImageUrl,
                    Quantity = component.Quantity,
                    Sources = string.Join(", ", component.ComponentSuppliers.OrderBy(s => s.LastPrice).Select(s => $@"<a href=""{s.Link}"" target=""_blank"">{s.Supplier}({s.LastPrice} Ft)</a>")),
                    ImagesQuality = component.ImagesQuality,
                    Remark = component.Remark
                };

                resultComponent.ContainingBracelets = component.ProductComponents
                    .Count(pc => pc.Product.State == ProductStateEnum.Active);

                result.Add(resultComponent);
            };

            foreach (var order in orders)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var componentItems = GetComponentItems(orderItem)
                        .GroupBy(c => c.ComponentId);
                    foreach (var componentGroup in componentItems)
                    {
                        result.First(c => c.ComponentId == componentGroup.Key).SoldLast30Days += componentGroup.Sum(c => c.Quantity);
                    }
                }
            }

            foreach (var component in components)
            {
                var componentItem = result.FirstOrDefault(c => c.ComponentId == component.Id);
                component.MinimumQuantity = Math.Max(20, componentItem.SoldLast30Days);
                var minimumInBracelets = component.ProductComponents.Where(pc => pc.Product.State == ProductStateEnum.Active).Select(pc => pc.Count).DefaultIfEmpty(0).Max() * 2;
                component.MinimumQuantity = Math.Max(component.MinimumQuantity, minimumInBracelets);
            }

            return result;
        }

        public void DecreaseInventory(int orderId)
        {
            var order = context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => (oi as BraceletOrderItem).Product).ThenInclude(p => p.ProductComponents)
                .Include(o => o.OrderItems).ThenInclude(oi => (oi as CustomBraceletOrderItem).Components).ThenInclude(c => c.Component)
                .Include(o => o.OrderItems).ThenInclude(oi => (oi as CustomTextBraceletOrderItem).Product).ThenInclude(p => p.ProductComponents)
                .First(o => o.Id == orderId);
            var validStatus = order.Status == OrderStatusEnum.PaymentReceived ||
               (order.Status == OrderStatusEnum.OrderPlaced && order.PaymentMethod == PaymentMethodEnum.PayAtDelivery);
            if (!validStatus) return;

            var usedComponents = order.OrderItems.SelectMany(oi => GetComponentItems(oi))
                .Where(o => o != null)
                .GroupBy(ci => ci.ComponentId);
            var componentsWithLowStock = new List<Component>();
            foreach (var componentGroup in usedComponents)
            {
                var component = context.Components.First(c => c.Id == componentGroup.Key);
                component.Quantity -= componentGroup.Sum(c => c.Quantity);
                if (component.Quantity < component.MinimumQuantity && !componentsWithLowStock.Any(c => c.Id == component.Id))
                {
                    componentsWithLowStock.Add(component);
                }
            }
            if (componentsWithLowStock.Any())
            {
                SendLowStockNotification(componentsWithLowStock);
            }
            context.SaveChanges();
        }

        public bool IsSoldOut(Bracelet product)
        {
            return product.ProductComponents.Any(pc => pc.Component.Quantity < (pc.Count > 0 ? pc.Count : 25) * 2);
        }

        public int GetBraceletStock(Bracelet product)
        {
            var stock = int.MaxValue;
            foreach (var component in product.ProductComponents)
            {
                var stockByComponet = component.Component.Quantity / (component.Count > 0 ? component.Count : 25);
                if (stock > stockByComponet)
                {
                    stock = stockByComponet;
                }
            }
            return stock;
        }

        public bool IsBeadSoldOutInBraceletDesigner(Component component)
        {
            return component.Quantity < 50;
        }

        private List<ComponentItem> GetComponentItems(OrderItem orderItem)
        {
            if (orderItem is CustomBraceletOrderItem customBraceletOrderItem) return GetComponentItems(customBraceletOrderItem);
            if (orderItem is CustomTextBraceletOrderItem customTextBraceletOrderItem) return GetComponentItems(customTextBraceletOrderItem);
            if (orderItem is BraceletOrderItem braceletOrderItem) return GetComponentItems(braceletOrderItem);
            if (orderItem is GiftBraceletOrderItem giftBraceletOrderItem) return GetComponentItems(giftBraceletOrderItem);
            return null;
        } 

        private List<ComponentItem> GetComponentItems(CustomBraceletOrderItem orderItem)
        {
            const int totalBeadsCount = 25;
            var result = new List<ComponentItem>();
            result.Add(new ComponentItem { ComponentId = CSOMO_REJTO, Quantity = orderItem.Quantity });
            var extraComponentsLength = orderItem.Components.Sum(c => c.Component.WidthRatio);
            int numberOfBeads = totalBeadsCount - (int)Math.Ceiling(extraComponentsLength);

            if (orderItem.StyleType == CustomBraceletStyleEnum.Simple)
            {
                result.Add(new ComponentItem { ComponentId = orderItem.BeadTypeId, Quantity = numberOfBeads * orderItem.Quantity });
            }
            else
            {
                result.Add(new ComponentItem { ComponentId = orderItem.BeadTypeId, Quantity = numberOfBeads * orderItem.Quantity / 2 });
                result.Add(new ComponentItem { ComponentId = orderItem.SecondaryBeadTypeId.Value, Quantity = numberOfBeads * orderItem.Quantity / 2 });
            }
            result.AddRange(orderItem.Components.Select(c => new ComponentItem { ComponentId = c.ComponentId, Quantity = orderItem.Quantity }));

            return result;
        }

        private List<ComponentItem> GetComponentItems(BraceletOrderItem orderItem)
        {
            var result = new List<ComponentItem>();
            result.AddRange(orderItem.Product.ProductComponents.Select(pc => new ComponentItem { ComponentId = pc.ComponentId, Quantity = pc.Count * orderItem.Quantity }));
            return result;
        }

        private List<ComponentItem> GetComponentItems(CustomTextBraceletOrderItem orderItem)
        {
            var result = new List<ComponentItem>();
            result.AddRange(orderItem.Product.ProductComponents.Select(pc => new ComponentItem { ComponentId = pc.ComponentId, Quantity = pc.Count * orderItem.Quantity }));
            return result;
        }

        private List<ComponentItem> GetComponentItems(GiftBraceletOrderItem orderItem)
        {
            var result = new List<ComponentItem>();
            var bracelet = context.Products
                .Include(p => p.ProductComponents)
                .First(p => p.Id == 410);
            result.AddRange(bracelet.ProductComponents.Select(pc => new ComponentItem { ComponentId = pc.ComponentId, Quantity = pc.Count * orderItem.Quantity }));
            return result;
        }

        private void SendLowStockNotification(List<Component> components)
        {
            var text = new StringBuilder("A készlet nagyon alacsony lett a rendelés miatt!");
            text.AppendLine("<ul>");
            foreach (var component in components)
            {
                text.AppendLine($"<li><b>{component.Name}</b> készlete <b>{component.Quantity}</b> db. alá csökkent (ajánlott min. {component.MinimumQuantity} db.)</li>");
            }
            text.AppendLine("</ul>");
            emailService.SendEmailToAdmins("Alacsony készlet!", text.ToString(), text.ToString(), "low-inventory");
        }

        public List<ProductsInventoryReportItem> GetInventoryReport()
        {
            var products = context.Products
                .Include(p => p.ProductComponents).ThenInclude(pc => pc.Component)
                .ToList()
                .Select(p => new ProductsInventoryReportItem
                {
                    IdString = p.IdString,
                    Category = GetFullCategoryName(p.CategoryId),
                    Active = p.State == ProductStateEnum.Active,
                    SoldOut = IsSoldOut(p)
                }).ToList();
            return products;
        }

        private string GetFullCategoryName(int categoryId)
        {
            var categories = context.Categories.ToList();
            var category = categories.First(c => c.Id == categoryId);
            var parentId = (int)category.Type;
            if (parentId > 0)
            {
                var parentCategory = categories.First(c => c.Id == parentId);
                return $"{parentCategory.Name} / {category.Name}";
            }
            return category.Name;
        }
    }
}
