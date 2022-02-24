using elenora.BusinessModels;
using elenora.Features.Inventory;
using elenora.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elenora.Features.ProductPricing
{
    public class ProductPricingService : IProductPricingService
    {
        private readonly DataContext context;
        private readonly IInventoryService inventoryService;

        private const int MIN_MARGIN = 2000;
        private const int MIN_INVENTORY = 10;
        private const int SALES_DURATION = 7;
        private const int DISCOUNT_PERCENTAGE = 30;

        public ProductPricingService(IInventoryService inventoryService, DataContext context)
        {
            this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ProductPrice GetProductPrice(Bracelet product)
        {
            var discount = product.ProductDiscounts.Where(d => d.StartDate < Helper.Now && Helper.Now < d.EndDate).FirstOrDefault();
            if (discount == null)
            {
                return new ProductPrice
                {
                    Price = product.BasePrice
                };
            }
            else
            {
                return new ProductPrice
                {
                    Price = Math.Round(product.BasePrice * (100 - discount.Percentage) / 100),
                    OriginalPrice = product.BasePrice
                };
            }
        }

        public MarginsInformation GetMarginsInformation(Bracelet bracelet)
        {
            var result = new MarginsInformation();
            decimal minBeadsCost = 0;
            decimal maxBeadsCost = 0;
            var description = new StringBuilder();
            foreach (var componentItem in bracelet.ProductComponents)
            {
                var component = componentItem.Component;
                if (!component.ComponentSuppliers.Any())
                {
                    result.MissingInformation = true;
                    description.AppendLine($"{componentItem.Count,2} x {component.Name}: ???");
                }
                else if (component.ComponentSuppliers.Count == 1)
                {
                    description.AppendLine($"{componentItem.Count,2} x {component.Name}: {component.ComponentSuppliers.First().LastPrice * componentItem.Count} Ft");
                    minBeadsCost += component.ComponentSuppliers.First().LastPrice * componentItem.Count;
                    maxBeadsCost += component.ComponentSuppliers.First().LastPrice * componentItem.Count;
                }
                else
                {
                    var suppliers = component.ComponentSuppliers.OrderBy(s => s.LastPrice).ToList();
                    description.AppendLine($"{componentItem.Count,2} x {component.Name}: {suppliers.First().LastPrice * componentItem.Count} Ft - {suppliers.Last().LastPrice * componentItem.Count} Ft");
                    minBeadsCost += suppliers.First().LastPrice * componentItem.Count;
                    maxBeadsCost += suppliers.Last().LastPrice * componentItem.Count;
                }
            }
            var price = GetProductPrice(bracelet);
            result.MinTotal = price.Price - maxBeadsCost;
            result.MaxTotal = price.Price - minBeadsCost;
            result.Description = description.ToString();
            return result;
        }
        public List<int> GetDiscountedProductIds()
        {
            var currentDiscounts = context.ProductDiscounts
                .Where(d => d.StartDate < Helper.Now && Helper.Now < d.EndDate).ToList();
            if (!currentDiscounts.Any())
            {
                var possibleProducts = context.Products
                    .Include(p => p.ProductStatistics)
                    .Include(p => p.ProductDiscounts)
                    .Include(p => p.ProductComponents).ThenInclude(pc => pc.Component).ThenInclude(c => c.ComponentSuppliers)
                    .Where(p =>
                        !p.ProductDiscounts.Any(pd => pd.EndDate > Helper.Now.AddDays(-60)) &&
                        p.State == ProductStateEnum.Active &&
                        (p.ProductType == ProductTypeEnum.Bracelet || p.ProductType == ProductTypeEnum.BraceletPair) &&
                        !p.ProductComponents.Any(pc => pc.Component.ImagesQuality == 0))
                    .ToList()
                    .Select(p => (p, GetMarginsInformation(p)))
                    .Where(t => !t.Item2.MissingInformation &&
                                t.Item2.MinTotal - t.p.BasePrice * 0.3m > MIN_MARGIN &&
                                inventoryService.GetBraceletStock(t.p) >= MIN_INVENTORY)
                    .ToList();
                var highestMarginProduct = possibleProducts.OrderBy(t => t.Item2.MinTotal).Last().p;
                possibleProducts.RemoveAll(p => p.p.Id == highestMarginProduct.Id);
                possibleProducts.RemoveAll(p => GetProductStringRepresentation(p.p) == GetProductStringRepresentation(highestMarginProduct));
                
                var mostPurchased = possibleProducts.Select(p => p.p).OrderBy(p => p.ProductStatistics.Sum(s => s.Purchases)).Last();
                possibleProducts.RemoveAll(p => p.p.Id == mostPurchased.Id);
                possibleProducts.RemoveAll(p => GetProductStringRepresentation(p.p) == GetProductStringRepresentation(mostPurchased));
                
                var bestAddToCartRatio = possibleProducts.Select(p => p.p).OrderBy(p => (double)p.ProductStatistics.Sum(s => s.AddToCarts) / (p.ProductStatistics.Sum(s => s.Views) + 1)).Last();
                possibleProducts.RemoveAll(p => p.p.Id == bestAddToCartRatio.Id);
                possibleProducts.RemoveAll(p => GetProductStringRepresentation(p.p) == GetProductStringRepresentation(bestAddToCartRatio));

                var randomProduct = possibleProducts.Select(p => p.p).ElementAt(new Random().Next(possibleProducts.Count));

                currentDiscounts.Add(CreateDiscountForProduct(highestMarginProduct.Id));
                currentDiscounts.Add(CreateDiscountForProduct(mostPurchased.Id));
                currentDiscounts.Add(CreateDiscountForProduct(bestAddToCartRatio.Id));
                currentDiscounts.Add(CreateDiscountForProduct(randomProduct.Id));
            }
            return currentDiscounts.Select(d => d.ProductId).ToList();
        }

        private ProductDiscount CreateDiscountForProduct(int productId)
        {
            var discount = new ProductDiscount
            {
                ProductId = productId,
                StartDate = Helper.Now.Date,
                EndDate = Helper.Now.Date.AddDays(SALES_DURATION),
                Percentage = DISCOUNT_PERCENTAGE
            };
            context.ProductDiscounts.Add(discount);
            context.SaveChanges();
            return discount;
        }

        private string GetProductStringRepresentation(Bracelet bracelet)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var component in bracelet.ProductComponents.OrderBy(pc => pc.ComponentId))
            {
                sb.Append($"{component.ComponentId}-{component.Count},");
            }
            return sb.ToString();
        }

        public List<OrderCostNode> GetOrderCostInformation(int orderId)
        {
            var order = context.Orders
                .Include(o => o.OrderItems).ThenInclude(i => (i as BraceletOrderItem).Product).ThenInclude(p => p.ProductComponents)
                .First(o => o.Id == orderId);

            var result = new List<OrderCostNode>
            {
                new OrderCostNode
                {
                    Name = "Bevétel",
                    Description = "",
                    MinCost = order.Total,
                    MaxCost = order.Total,
                    AddToSum = true,
                    Error = null
                },
                GetProductCosts(order),
                GetShippingCosts(order),
                GetPackageCosts(order)
            };
            return result;
        }

        private OrderCostNode GetProductCosts(Order order)
        {
            var mainNode = new OrderCostNode { Name = "Karkötők", AddToSum = true };
            foreach (var orderItem in order.OrderItems)
            {
                var productNode = new OrderCostNode();
                var components = new Dictionary<int, int>();

                if (orderItem is BraceletOrderItem)
                {
                    productNode.Name = (orderItem as BraceletOrderItem).Product.Name;
                    productNode.Description = $"{orderItem.Quantity} db.";

                    foreach (var item in (orderItem as BraceletOrderItem).Product.ProductComponents)
                    {
                        if (!components.ContainsKey(item.ComponentId))
                        {
                            components.Add(item.ComponentId, 0);
                        }
                        components[item.ComponentId] += item.Count * orderItem.Quantity;
                    }
                }

                foreach (var item in components)
                {
                    var component = context.Components
                        .Include(c => c.ComponentSuppliers)
                        .First(c => c.Id == item.Key);

                    var minCost = component.ComponentSuppliers.OrderBy(c => c.LastPrice).FirstOrDefault();
                    var maxCost = component.ComponentSuppliers.OrderBy(c => c.LastPrice).LastOrDefault();
                    var componentNode = new OrderCostNode
                    {
                        Name = $"{component.Name} (Id: {component.Id})",
                        Description = $"{item.Value} db.",
                        MinCost = -(minCost?.LastPrice ?? 0) * item.Value,
                        MaxCost = -(maxCost?.LastPrice ?? 0) * item.Value,
                        Error = minCost == null ? "Nincs ár megadva!" : null
                    };
                    if (minCost != null)
                    {
                        if (maxCost.LastPrice != minCost.LastPrice)
                        {
                            componentNode.Description += $" {minCost.LastPrice} - {maxCost.LastPrice} Ft/db";
                        }
                        else
                        {
                            componentNode.Description += $" {minCost.LastPrice} Ft/db";
                        }
                    }
                    productNode.Children.Add(componentNode);
                    productNode.MinCost += componentNode.MinCost;
                    productNode.MaxCost += componentNode.MaxCost;
                    if (componentNode.Error != null) productNode.Error = ">";
                }

                mainNode.Children.Add(productNode);
                mainNode.MinCost += productNode.MinCost;
                mainNode.MaxCost += productNode.MaxCost;
                if (productNode.Error != null) mainNode.Error = ">";
            }
            return mainNode;
        }

        private OrderCostNode GetShippingCosts(Order order)
        {
            var mainNode = new OrderCostNode { Name = "Szállítás", AddToSum = true };
            if (order.ShippingMethod == ShippingMethodEnum.GLS)
            {
                mainNode.Children.Add(new OrderCostNode
                {
                    Name = "GLS Szállítás",
                    Description = "1324 Ft + ÁFA",
                    MinCost = -Math.Round(1324 * 1.27m),
                    MaxCost = -Math.Round(1324 * 1.27m),
                    Error = null
                });
                if (order.PaymentMethod == PaymentMethodEnum.PayAtDelivery)
                {
                    mainNode.Children.Add(new OrderCostNode
                    {
                        Name = "Utánvét kezelés",
                        Description = order.Total < 1000 ? "418 Ft + ÁFA" : "556 Ft + ÁFA",
                        MinCost = -Math.Round(order.Total < 1000 ? 418 * 1.27m : 556 * 1.27m),
                        MaxCost = -Math.Round(order.Total < 1000 ? 418 * 1.27m : 556 * 1.27m),
                        Error = null
                    });
                }
                else
                {
                    mainNode.Error = ">";
                }
            }
            else
            {
                mainNode.Error = ">";
            }
            mainNode.MinCost = mainNode.Children.Sum(c => c.MinCost);
            mainNode.MaxCost = mainNode.Children.Sum(c => c.MaxCost);
            return mainNode;
        }

        private OrderCostNode GetPackageCosts(Order order)
        {
            var mainNode = new OrderCostNode { Name = "Csomagolás", AddToSum = true };

            var braceletsCount = order.OrderItems.Sum(oi => oi.Quantity);
            mainNode.Children.Add(new OrderCostNode
            {
                Name = "Zsákocskák",
                Description = $"{braceletsCount} db.",
                MinCost = -braceletsCount * 142,
                MaxCost = -braceletsCount * 142,
                Error = null
            });
            mainNode.Children.Add(new OrderCostNode
            {
                Name = "Boríték",
                Description = "",
                MinCost = -14,
                MaxCost = -14,
                Error = null
            });
            mainNode.Children.Add(new OrderCostNode
            {
                Name = "Instagram kártya",
                Description = "",
                MinCost = -90,
                MaxCost = -90,
                Error = null
            });
            mainNode.Children.Add(new OrderCostNode
            {
                Name = "Kupon kártya",
                Description = "",
                MinCost = -57,
                MaxCost = -57,
                Error = null
            });

            mainNode.MinCost = mainNode.Children.Sum(c => c.MinCost);
            mainNode.MaxCost = mainNode.Children.Sum(c => c.MaxCost);
            return mainNode;
        }
    }
}
