using elenora.BusinessModels;
using elenora.Features.Inventory;
using elenora.Features.ProductPricing;
using elenora.Features.StringBraceletDesigner;
using elenora.Models;
using elenora.Services;
using elenora.ViewModels.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace elenora.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataContext context;
        private readonly IOrderService orderService;
        private readonly ICustomerService customerService;
        private readonly IEmailService emailService;
        private readonly IInventoryService inventoryService;
        private readonly IProductPricingService productPricingService;
        private readonly IProductService productService;

        public AdminController(IProductService productService, IProductPricingService productPricingService, IInventoryService inventoryService, DataContext context, IOrderService orderService, ICustomerService customerService, IEmailService emailService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            this.productPricingService = productPricingService ?? throw new ArgumentNullException(nameof(productPricingService));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [Authorize(Roles = "Admin")]
        [Route("/admin")]
        public IActionResult Index()
        {
            return Redirect("/admin/index.html");
        }

        [Route("/admin/login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin1210" && password == "***")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).Wait();
            }
            return Redirect("/admin");
        }


#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/orders")]
        public IActionResult Orders(int pageNumber, int pageSize, string filter, string sortDirection, string sortColumn)
        {
            var query = context.Orders
                .Where(o => o.Status == OrderStatusEnum.OrderPlaced ||
                             o.Status == OrderStatusEnum.PaymentReceived ||
                             o.Status == OrderStatusEnum.OrderCompleted ||
                             o.Status == OrderStatusEnum.OrderPrepared);
            var subquery = query
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                .Include(o => o.EmailAddress)
                .Include(o => o.OrderHistories)
                .Include(o => o.Coupon)
                .Include(o => o.OrderItems).ThenInclude(i => (i as BraceletOrderItem).Product)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomTextBraceletOrderItem).Product)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomBraceletOrderItem).BeadType)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomBraceletOrderItem).SecondaryBeadType)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomBraceletOrderItem).Components).ThenInclude(c => c.Component)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemComplementaryProducts).ThenInclude(c => c.ComplementaryProduct)
                .Where(o => true);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                subquery = subquery.Where(o =>
                    o.BillingAddress.Name.Contains(filter) ||
                    o.EmailAddress.Address.Contains(filter) ||
                    o.Remark.Contains(filter)
                ).AsQueryable();
            }

            IQueryable<Order> orderedQuery = subquery.OrderBy(o => o.OrderedDate);
            if (sortColumn == "createdDate")
            {
                if (sortDirection == "desc") orderedQuery = subquery.OrderByDescending(o => o.OrderedDate).AsQueryable();
            }
            else if (sortColumn == "name")
            {
                if (sortDirection == "asc") orderedQuery = subquery.OrderBy(o => o.BillingAddress.Name);
                else orderedQuery = subquery.OrderByDescending(o => o.BillingAddress.Name);
            }
            else if (sortColumn == "price")
            {
                if (sortDirection == "asc") orderedQuery = subquery.OrderBy(o => o.Total);
                else orderedQuery = subquery.OrderByDescending(o => o.Total);
            }

            var orders = orderedQuery.Skip(pageNumber * pageSize).Take(pageSize).ToList();
            var ordersList = new List<OrderViewModel>();
            foreach (var order in orders)
            {
                var model = new OrderViewModel
                {
                    Id = order.Id,
                    OrderId = order.OrderId,
                    CreatedDate = order.CreatedDate,
                    OrderedDate = order.OrderedDate.Value,
                    Email = order.EmailAddress?.Address,
                    Name = order.BillingAddress.Name,
                    PaymentRequestEmailsSent = order.PaymentRequestEmailsSent,
                    LastPaymentRequestEmailDate = order.LastPaymentRequestEmailDate,
                    ShippingZipCode = order.ShippingAddress?.ZipCode,
                    ShippingCity = order.ShippingAddress?.City,
                    ShippingAddress = order.ShippingAddress?.AddressLine,
                    ShippingName = order.ShippingAddress?.Name,
                    BillingZipCode = order.BillingAddress?.ZipCode,
                    BillingCity = order.BillingAddress?.City,
                    BillingAddress = order.BillingAddress?.AddressLine,
                    BillingName = order.BillingAddress?.Name,
                    Phone = order.Phone,
                    Remark = order.Remark,
                    AdminRemark = order.AdminRemark,
                    ShippingMethod = order.ShippingMethod,
                    ShippingPrice = order.ShippingPrice,
                    PaymentMethod = order.PaymentMethod,
                    PackageTrackingNumber = order.PackageTrackingNumber,
                    ShippingPointAddressInformation = order.ShippingPointAddressInformation,
                    OrderHistories = order.OrderHistories.OrderBy(o => o.Date).Select(h => new OrderHistoryItem
                    {
                        Date = h.Date,
                        Description = h.Description
                    }).ToArray()
                };
                model.Status = GetOrderStatusText(order.Status);

                var actionLogs = context.ActionLogs.Where(l => l.CustomerId == order.CustomerId && l.Referrer != null).Select(l => l.Referrer).Distinct().ToList();
                model.Referrer = string.Join(", ", actionLogs);

                var cartContent = new StringBuilder();
                var orderItems = new List<ViewModels.Admin.OrderItem>();
                foreach (var orderItem in order.OrderItems)
                {
                    var price = Helper.GetFormattedMoney(orderItem.UnitPrice - orderItem.OrderItemComplementaryProducts.Sum(c => c.ComplementaryProduct.Price));
                    orderItems.Add(new ViewModels.Admin.OrderItem
                    {
                        Title = $"{orderItem.Quantity} * {price} Ft - {GetOrderItemDescription(orderItem)}",
                        ImageUrl = GetOrderItemImageUrl(orderItem)
                    });
                    foreach (var complementaryProduct in orderItem.OrderItemComplementaryProducts)
                    {
                        orderItems.Add(new ViewModels.Admin.OrderItem
                        {
                            Title = $" ---> {Helper.GetFormattedMoney(complementaryProduct.ComplementaryProduct.Price)} Ft - {GetOrderItemDescription(complementaryProduct)}",
                            ImageUrl = GetOrderItemImageUrl(complementaryProduct)
                        });
                    }
                }
                if (order.Coupon != null)
                {
                    orderItems.Add(new ViewModels.Admin.OrderItem
                    {
                        Title = $"Kupon: {order.Coupon.Name} ({order.CouponValue} Ft)"
                    });
                }
                model.OrderItems = orderItems.ToArray();

                if (!string.IsNullOrWhiteSpace(order.Remark))
                {
                    cartContent.Append($"<br />{order.Remark}");
                }
                model.CartContent = cartContent.ToString();
                model.Price = order.Total;

                model.OrdersCount = context.Orders.Where(o =>
                    o.EmailAddressId == order.EmailAddressId &&
                    o.Id != order.Id &&
                    o.Status == OrderStatusEnum.OrderCompleted).Count() + 1;
                model.UserTotal = context.Orders.Where(o =>
                    o.EmailAddressId == order.EmailAddressId &&
                    o.Id != order.Id &&
                    o.Status == OrderStatusEnum.OrderCompleted).Sum(o => o.Total) + order.Total;

                ordersList.Add(model);
            }

            var glsOrders = context.Orders.Where(o => o.ShippingMethod == ShippingMethodEnum.GLS && (o.Status == OrderStatusEnum.PaymentReceived || o.PaymentMethod == PaymentMethodEnum.PayAtDelivery) && (o.Status == OrderStatusEnum.OrderPlaced || o.Status == OrderStatusEnum.OrderPrepared || o.Status == OrderStatusEnum.PaymentReceived));
            return Ok(new
            {
                orders = ordersList,
                ordersCount = query.Count(),
                glsWaitingForCompletion = glsOrders.Count(),
                glsWaitingForCompletionPrepared = glsOrders.Count(o => o.Status == OrderStatusEnum.OrderPrepared),
                waitingForPayment = context.Orders.Where(o => o.PaymentMethod == PaymentMethodEnum.BankTransfer && o.Status == OrderStatusEnum.OrderPlaced && o.CreatedDate > Helper.Now.AddDays(-30)).Count()
            });
        }

        private string GetOrderItemDescription(Models.OrderItem orderItem)
        {
            var size = Helper.GetFormattedBraceletSize((orderItem as IBraceletWithSize)?.BraceletSize, (orderItem as BraceletOrderItem)?.BraceletSize2);

            var braceletOrderItem = orderItem as BraceletOrderItem;
            if (braceletOrderItem != null)
            {
                return $@"<a href=""https://www.elenora.hu/karkoto/{braceletOrderItem.Product.IdString}"" target=""_blank"">{braceletOrderItem.Product.Name}</a> - {size}";
            }

            var customBraceletOrderItem = orderItem as CustomBraceletOrderItem;
            if (customBraceletOrderItem != null)
            {
                var name = customBraceletOrderItem.BeadType.Name;
                if (customBraceletOrderItem.SecondaryBeadType != null)
                {
                    name += $" és {customBraceletOrderItem.SecondaryBeadType.Name} - {customBraceletOrderItem.StyleType}";
                }
                name += $" egyedi karkötő - {size}<br />";
                return name + string.Join(" - ", customBraceletOrderItem.Components.OrderBy(c => c.Position).Select(c => c.Component.Name));
            }

            var customTextBraceletOrderItem = orderItem as CustomTextBraceletOrderItem;
            if (customTextBraceletOrderItem != null)
            {
                return $"{customTextBraceletOrderItem.Name} - {size}";
            }

            var stringBraceletOrderItem = orderItem as StringBraceletOrderItem;
            if (stringBraceletOrderItem != null)
            {
                return $"{stringBraceletOrderItem.Name}";
            }

            var giftBraceletOrderItem = orderItem as GiftBraceletOrderItem;
            if (giftBraceletOrderItem != null)
            {
                return "Ajándék lávakő karkötő";
            }

            return "ERROR!!!";
        }

        private string GetOrderItemDescription(OrderItemComplementaryProduct complementaryProductOrderItem)
        {
            return complementaryProductOrderItem.ComplementaryProduct.Name;
        }

        private string GetOrderItemImageUrl(Models.OrderItem orderItem)
        {
            var baseUrl = "https://www.elenora.hu";
            var braceletOrderItem = orderItem as BraceletOrderItem;
            if (braceletOrderItem != null)
            {
                return baseUrl + (braceletOrderItem.Product.MainImage ?? $"/images/products/{braceletOrderItem.Product.IdString}/{braceletOrderItem.Product.IdString}-512.jpg");
            }

            var customBraceletOrderItem = orderItem as CustomBraceletOrderItem;
            if (customBraceletOrderItem != null)
            {
                return baseUrl + $"/ordered-product-images/{customBraceletOrderItem.Id}-{customBraceletOrderItem.Order.CustomerId}.jpg";
            }

            var customTextBraceletOrderItem = orderItem as CustomTextBraceletOrderItem;
            if (customTextBraceletOrderItem != null)
            {
                return baseUrl + (customTextBraceletOrderItem.Product.MainImage ?? $"/images/products/{customTextBraceletOrderItem.Product.IdString}/{customTextBraceletOrderItem.Product.IdString}-600.jpg");
            }

            var stringBraceletOrderItem = orderItem as StringBraceletOrderItem;
            if (stringBraceletOrderItem != null)
            {
                return baseUrl + $"/ordered-product-images-string/{stringBraceletOrderItem.Id}-{stringBraceletOrderItem.Order.CustomerId}.svg";
            }

            var giftBraceletOrderItem = orderItem as GiftBraceletOrderItem;
            if (giftBraceletOrderItem != null)
            {
                return baseUrl + "/images/products/lavako-asvanykarkoto/lavako-asvanykarkoto-512.jpg";
            }

            return "ERROR!!!";
        }

        private string GetOrderItemImageUrl(OrderItemComplementaryProduct complementaryProductOrderItem)
        {
            var baseUrl = "https://www.elenora.hu";
            return baseUrl + complementaryProductOrderItem.ComplementaryProduct.ImageUrl;
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/prepare-order/{orderId}")]
        public IActionResult MarkOrderPrepared(int orderId)
        {
            var order = context.Orders.First(o => o.Id == orderId);
            if (order.Status == OrderStatusEnum.OrderPlaced || order.Status == OrderStatusEnum.PaymentReceived)
            {
                order.Status = OrderStatusEnum.OrderPrepared;
            }
            context.SaveChanges();
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/complete-order/{orderId}")]
        public IActionResult MarkOrderCompleted(int orderId, string packageTrackingNumber)
        {
            var order = context.Orders
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                .Include(o => o.Coupon)
                .Include(o => o.EmailAddress)
                .Include(o => o.OrderItems).ThenInclude(i => (i as BraceletOrderItem).Product)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomTextBraceletOrderItem).Product)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomBraceletOrderItem).BeadType)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomBraceletOrderItem).SecondaryBeadType)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomBraceletOrderItem).Components).ThenInclude(c => c.Component)
                .Include(c => c.OrderItems).ThenInclude(i => i.OrderItemComplementaryProducts).ThenInclude(cp => cp.ComplementaryProduct)
                .First(o => o.Id == orderId);
            order.PackageTrackingNumber = packageTrackingNumber;
            if (order.Status == OrderStatusEnum.OrderPrepared)
            {
                order.Status = OrderStatusEnum.OrderCompleted;
            }
            context.SaveChanges();
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/delete-order/{orderId}")]
        public IActionResult DeleteOrder(int orderId)
        {
            var order = context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => (oi as CustomBraceletOrderItem).Components)
                .First(o => o.Id == orderId);
            context.Remove(order);
            foreach (var orderItem in order.OrderItems)
            {
                if (orderItem is CustomBraceletOrderItem)
                {
                    context.RemoveRange((orderItem as CustomBraceletOrderItem).Components);
                }
                context.Remove(orderItem);
            }
            context.SaveChanges();
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/pay-order/{orderId}")]
        public IActionResult MarkOrderPayed(int orderId)
        {
            var order = context.Orders
                .Include(o => o.BillingAddress)
                .Include(o => o.EmailAddress)
                .Include(o => o.OrderItems)
                .First(o => o.Id == orderId);
            if (order.Status == OrderStatusEnum.OrderPlaced)
            {
                order.Status = OrderStatusEnum.PaymentReceived;
                orderService.SendOrderPaymentReceivedEmail(order);
                inventoryService.DecreaseInventory(order.Id);
                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem is BraceletOrderItem)
                    {
                        productService.LogProductPurchase((orderItem as BraceletOrderItem).ProductId);
                    }
                }
            }
            context.SaveChanges();
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/request-payment/{orderId}")]
        public IActionResult SendPaymentRequestEmail(int orderId)
        {
            orderService.SendPaymentRequestEmail(orderId);
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/actions")]
        public IActionResult Actions()
        {
            var result = new List<ActionViewModel>();
            var actionGroups = context.ActionLogs.ToList()
                .GroupBy(a => a.CustomerId)
                .Select(g => g.OrderBy(a => a.Date));
            foreach (var actionGroup in actionGroups)
            {
                var actionModel = new ActionViewModel
                {
                    CustomerId = actionGroup.First().CustomerId,
                    ActionsCount = actionGroup.Count(),
                    FirstAction = actionGroup.First().Date,
                    LastAction = actionGroup.Last().Date,
                    Referrer = string.Join(", ", actionGroup.Where(a => a.Referrer != null).Select(a => a.Referrer).Distinct())
                };
                var customer = context.Customers
                    .Include(c => c.EmailAddress)
                    .FirstOrDefault(c => c.Id == actionGroup.First().CustomerId);
                actionModel.Email = customer.EmailAddress?.Address;

                var cart = context.Carts
                    .Include(c => c.Coupon)
                    .Include(c => c.CartItems).ThenInclude(ci => (ci as BraceletCartItem).Product)
                    .Include(c => c.CartItems).ThenInclude(ci => (ci as CustomTextBraceletCartItem).Product)
                    .Include(c => c.CartItems).ThenInclude(ci => ci.CartItemComplementaryProducts).ThenInclude(p => p.ComplementaryProduct)
                    .FirstOrDefault(c => c.CustomerId == customer.Id);
                if (cart != null)
                {
                    //todo
                    //actionModel.CartValue = cart.GetTotal();
                }

                var orders = context.Orders.Where(o => o.CustomerId == customer.Id);
                var activeOrder = orders.SingleOrDefault(o => o.CustomerId == customer.Id && (
                    o.Status == OrderStatusEnum.NewOrder ||
                    o.Status == OrderStatusEnum.ShippingMethodSelected ||
                    o.Status == OrderStatusEnum.PaymentInformationProvided
                ));
                if (activeOrder != null)
                {
                    actionModel.CheckoutStatus = GetOrderStatusText(activeOrder.Status);
                }
                actionModel.OrderedValue = orders.Where(o => o.Status == OrderStatusEnum.OrderPlaced || o.Status == OrderStatusEnum.PaymentReceived).Sum(o => o.Total);

                result.Add(actionModel);
            }
            return Ok(result.OrderByDescending(m => m.LastAction));
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/actions/{customerId}")]
        public IActionResult Actions(int customerId)
        {
            var result = new List<ActionStepViewModel>();
            var actionLogs = context.ActionLogs
                .Include(a => a.Product)
                .Where(a => a.CustomerId == customerId)
                .OrderBy(c => c.Date).ToList();
            var lastDate = actionLogs.First().Date;
            foreach (var action in actionLogs)
            {
                var model = new ActionStepViewModel
                {
                    CustomerId = customerId,
                    DateDifference = GetFormattedTimespan(action.Date.Subtract(lastDate)),
                    Step = GetStepString(action)
                };
                lastDate = action.Date;

                result.Add(model);
            }
            return Ok(result);
        }

        private string GetOrderStatusText(OrderStatusEnum status)
        {
            if (status == OrderStatusEnum.NewOrder) return "Új";
            if (status == OrderStatusEnum.ShippingMethodSelected) return "1. lépés kész";
            if (status == OrderStatusEnum.PaymentInformationProvided) return "2. lépés kész";
            if (status == OrderStatusEnum.OrderPlaced) return "Megrendelve";
            if (status == OrderStatusEnum.PaymentReceived) return "Kifizetve";
            if (status == OrderStatusEnum.ShippingMethodSelected) return "Feladva";
            if (status == OrderStatusEnum.OrderPrepared) return "Összeállítva";
            if (status == OrderStatusEnum.OrderCompleted) return "Teljesítve";
            return null;
        }

        private string GetStepString(ActionLog actionLog)
        {
            var result = actionLog.Action.ToString();
            if (actionLog.Action == ActionEnum.AddToCart)
            {
                result = "Kosárba tette: " + actionLog.Product?.Name;
            }
            else if (actionLog.Action == ActionEnum.DeleteFromCart)
            {
                result = "Törölte kosárból: " + actionLog.Product?.Name;
            }
            else if (actionLog.Action == ActionEnum.ViewProduct)
            {
                result = "Megnézte: " + actionLog.Product?.Name;
            }

            result += " " + actionLog.Remark;
            if (actionLog.Referrer != null)
            {
                result += " Reklám: " + actionLog.Referrer;
            }
            return result;
        }

        private string GetFormattedTimespan(TimeSpan timespan)
        {
            var result = "";
            if (timespan.TotalSeconds == 0) return result;
            result = $"{timespan.Seconds}s";
            if (Math.Floor(timespan.TotalMinutes) > 0)
            {
                result = $"{timespan.Minutes} perc {result}";
            }
            if (Math.Floor(timespan.TotalHours) > 0)
            {
                result = $"{timespan.Hours} óra {result}";
            }
            if (Math.Floor(timespan.TotalDays) > 0)
            {
                result = $"{timespan.Days} nap {result}";
            }
            return result;
        }

        [Authorize(Roles = "Admin")]
        [Route("/admin/scheduler")]
        public IActionResult Scheduler()
        {
            orderService.SendAbandonedCartEmailSequences();
            customerService.DeleteOldInactiveCustomers();
            customerService.SendAskForReviewEmails();
            emailService.SendEmailSequences();
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/save-order-remark")]
        [HttpPost]
        public IActionResult SaveOrderRemark([FromBody] SaveOrderRemarkModel data)
        {
            var order = context.Orders.Where(o => o.Id == data.OrderId).First();
            order.AdminRemark = data.Remark;
            context.SaveChanges();
            return Ok();
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/emails")]
        public IActionResult Emails()
        {
            var result = new EmailsViewModel();
            var emailLogs = context.EmailLogs
                .Where(l => l.SentDate > Helper.Now.AddDays(-30))
                .OrderByDescending(l => l.SentDate);

            var abandonedCart_1_1 = emailLogs.FirstOrDefault(l => l.Category == "abandoned-cart-1-1");
            result.AbandonedCart_1_1_Count = emailLogs.Count(l => l.Category == "abandoned-cart-1-1");
            result.AbandonedCart_1_1_Subject = abandonedCart_1_1?.Subject;
            result.AbandonedCart_1_1_Content = abandonedCart_1_1?.BodyHtml;

            var abandonedCart_1_2 = emailLogs.FirstOrDefault(l => l.Category == "abandoned-cart-1-2");
            result.AbandonedCart_1_2_Count = emailLogs.Count(l => l.Category == "abandoned-cart-1-2");
            result.AbandonedCart_1_2_Subject = abandonedCart_1_2?.Subject;
            result.AbandonedCart_1_2_Content = abandonedCart_1_2?.BodyHtml;

            var popup1 = emailLogs.FirstOrDefault(l => l.Category == "popup-1");
            result.Popup_1_Count = emailLogs.Count(l => l.Category == "popup-1");
            result.Popup_1_Subject = popup1?.Subject;
            result.Popup_1_Content = popup1?.BodyHtml;

            var popup2 = emailLogs.FirstOrDefault(l => l.Category == "popup-2");
            result.Popup_2_Count = emailLogs.Count(l => l.Category == "popup-2");
            result.Popup_2_Subject = popup2?.Subject;
            result.Popup_2_Content = popup2?.BodyHtml;

            var orderPlaced = emailLogs.FirstOrDefault(l => l.Category == "order-placed");
            result.OrderPlaced_Count = emailLogs.Count(l => l.Category == "order-placed");
            result.OrderPlaced_Subject = orderPlaced?.Subject;
            result.OrderPlaced_Content = orderPlaced?.BodyHtml;

            var orderCompleted = emailLogs.FirstOrDefault(l => l.Category == "order-completed");
            result.OrderCompleted_Count = emailLogs.Count(l => l.Category == "order-completed");
            result.OrderCompleted_Subject = orderCompleted?.Subject;
            result.OrderCompleted_Content = orderCompleted?.BodyHtml;

            var paymentRequest_1 = emailLogs.FirstOrDefault(l => l.Category == "payment-request-1");
            result.PaymentRequest_1_Count = emailLogs.Count(l => l.Category == "payment-request-1");
            result.PaymentRequest_1_Subject = paymentRequest_1?.Subject;
            result.PaymentRequest_1_Content = paymentRequest_1?.BodyHtml;

            var paymentRequest_2 = emailLogs.FirstOrDefault(l => l.Category == "payment-request-2");
            result.PaymentRequest_2_Count = emailLogs.Count(l => l.Category == "payment-request-2");
            result.PaymentRequest_2_Subject = paymentRequest_2?.Subject;
            result.PaymentRequest_2_Content = paymentRequest_2?.BodyHtml;

            var askForReview = emailLogs.FirstOrDefault(l => l.Category == "review");
            result.AskForReview_Count = emailLogs.Count(l => l.Category == "review");
            result.AskForReview_Subject = askForReview?.Subject;
            result.AskForReview_Content = askForReview?.BodyHtml;

            return Ok(result);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/email-templates")]
        public IActionResult GetEmailTemplates()
        {
            var result = new List<EmailTemplateViewModel>();
            var templates = context.EmailTemplates.ToList();
            var emailAddress = context.EmailAddresses.First(e => e.Address == "boros.csaba94@gmail.com");
            foreach (var template in templates)
            {
                var model = new EmailTemplateViewModel
                {
                    Category = template.Category,
                    Subject = template.Subject,
                    Content = emailService.GetEmailContentFromTemplate(template, emailAddress),
                    SentEmails = context.EmailHistories.Where(h => h.EmailTemplateId == template.Id).Count(),
                    LastSent = context.EmailHistories.Where(h => h.EmailTemplateId == template.Id).Max(h => (DateTime?)h.SentDate),
                    UnsubscribedEmails = context.EmailHistories.Count(h => h.EmailTemplateId == template.Id && h.Unsubscribed)
                };
                if (template.Active)
                {
                    model.Status = "Active";
                }
                else
                {
                    model.Status = "Inactive";
                }
                result.Add(model);
            }

            return Ok(result);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/searches")]
        public IActionResult Searches()
        {
            var searches = context.ActionLogs.Where(l => l.Action == ActionEnum.Search && l.Remark != null).ToList();
            var searchGroups = searches.GroupBy(s => s.Remark.ToLower());
            var result = new List<SearchViewModel>();
            foreach (var group in searchGroups)
            {
                result.Add(new SearchViewModel
                {
                    Keyword = group.Key,
                    Count = group.Count(),
                    FirstSearch = group.Min(g => g.Date),
                    LastSearch = group.Max(g => g.Date)
                });
            }
            return Ok(result);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/popups")]
        public IActionResult Popups()
        {
            var customerPopups = context.Customers.Include(c => c.CustomerPopupStats).Where(c => c.CustomerPopupStats.PopupDisplayedCount > 0).ToList();
            var orderPopups = context.Orders.Include(o => o.OrderPopupStats).Where(o => o.OrderPopupStats.PopupDisplayedCount > 0).ToList();
            var popups = context.Popups.ToList();
            var result = new List<PopupViewModel>();
            foreach (var popup in popups)
            {
                var currentCustomerPopups = customerPopups.Where(c => c.CustomerPopupStats.PopupId == popup.Id);
                var currentOrderPopups = orderPopups.Where(o => o.OrderPopupStats.PopupId == popup.Id);
                var lastDisplayedCustomer = currentCustomerPopups.Max(p => p.CustomerPopupStats.PopupLastDisplayed) ?? DateTime.MinValue;
                var lastDisplayedOrder = currentOrderPopups.Max(p => p.OrderPopupStats.PopupLastDisplayed) ?? DateTime.MinValue;

                var popupStats = new PopupViewModel
                {
                    Id = popup.Id,
                    Name = popup.Name,
                    LastDisplayed = lastDisplayedCustomer > lastDisplayedOrder ? lastDisplayedCustomer : lastDisplayedOrder,
                    ShowCount = currentCustomerPopups.Sum(c => c.CustomerPopupStats.PopupDisplayedCount) + currentOrderPopups.Sum(o => o.OrderPopupStats.PopupDisplayedCount),
                    ActionCount = currentCustomerPopups.Sum(c => c.CustomerPopupStats.PopupActionExecutedCount) + currentOrderPopups.Sum(o => o.OrderPopupStats.PopupActionExecutedCount),
                    OrdersCount = currentOrderPopups.Count(o => o.OrderPopupStats.PopupActionExecutedCount > 0),
                    OrdersWithCouponCount = currentOrderPopups.Count(o => o.OrderPopupStats.PopupActionExecutedCount > 0 && o.CouponId > 0)
                };
                result.Add(popupStats);
            }
            return Ok(result);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/quizzes")]
        public IActionResult Quizzes()
        {
            var result = new List<QuizResultViewModel>();
            var quizResults = context.QuizResults.ToList();
            var quizName = quizResults.First().QuizName;
            int? startCount = quizResults.Count;

            var group1 = quizResults.GroupBy(q => q.Answer1).OrderByDescending(a => a.Count());
            foreach (var answer1 in group1)
            {
                var answer1Text = $"{answer1.Count()} ({Math.Round((decimal)answer1.Count() / quizResults.Count * 100)}%) - {answer1.Key ?? "---"}";
                var group2 = answer1.ToList().GroupBy(q => q.Answer2).OrderByDescending(a => a.Count());
                foreach (var answer2 in group2)
                {
                    var answer2Text = $"{answer2.Count()} ({Math.Round((decimal)answer2.Count() / answer1.Count() * 100)}%) - {answer2.Key ?? "---"}";
                    var group3 = answer2.ToList().GroupBy(q => q.Answer3).OrderByDescending(a => a.Count());
                    foreach (var answer3 in group3)
                    {
                        var answer3Text = $"{answer3.Count()} ({Math.Round((decimal)answer3.Count() / answer2.Count() * 100)}%) - {answer3.Key ?? "---"}";
                        var group4 = answer3.ToList().GroupBy(q => q.Answer4).OrderByDescending(a => a.Count());
                        foreach (var answer4 in group4)
                        {
                            var answer4Text = $"{answer4.Count()} ({Math.Round((decimal)answer4.Count() / answer3.Count() * 100)}%) - {answer4.Key ?? "---"}";
                            var group5 = answer4.ToList().GroupBy(q => q.Answer5).OrderByDescending(a => a.Count());
                            foreach (var answer5 in group5)
                            {
                                var answer5Text = $"{answer5.Count()} ({Math.Round((decimal)answer5.Count() / answer4.Count() * 100)}%) - {answer5.Key ?? "---"}";
                                var group6 = answer5.ToList().GroupBy(q => q.Answer6).OrderByDescending(a => a.Count());
                                foreach (var answer6 in group6)
                                {
                                    var answer6Text = $"{answer6.Count()} ({Math.Round((decimal)answer6.Count() / answer5.Count() * 100)}%) - {answer6.Key ?? "---"}";

                                    result.Add(new QuizResultViewModel
                                    {
                                        QuizName = quizName,
                                        StartCount = startCount,
                                        Answer1Text = answer1Text,
                                        Answer2Text = answer2Text,
                                        Answer3Text = answer3Text,
                                        Answer4Text = answer4Text,
                                        Answer5Text = answer5Text,
                                        Answer6Text = answer6Text
                                    });
                                    if (answer6Text != null)
                                    {
                                        result.Last().Result = answer6.OrderByDescending(a => a.Answer5Date).First().Result;
                                    }
                                    startCount = null;
                                    answer1Text = null;
                                    answer2Text = null;
                                    answer3Text = null;
                                    answer4Text = null;
                                    answer5Text = null;
                                    answer6Text = null;
                                }
                            }
                        }
                    }
                }
            }
            return Ok(result);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/stats")]
        public IActionResult Stats()
        {
            var result = new StatsViewModel();
            var ordersWithBankTransfer = context.Orders
                .Where(o => (o.Status == OrderStatusEnum.OrderCompleted ||
                             o.Status == OrderStatusEnum.OrderPlaced ||
                             o.Status == OrderStatusEnum.PaymentReceived ||
                             o.Status == OrderStatusEnum.OrderPrepared) &&
                            o.PaymentMethod == PaymentMethodEnum.BankTransfer &&
                            o.CreatedDate > Helper.Now.AddDays(-60)).ToList();
            result.BankTransferOrdersAll = ordersWithBankTransfer.Count(o =>
                o.CreatedDate > Helper.Now.AddDays(-30));
            result.BankTransferOrdersPayed = ordersWithBankTransfer.Count(o =>
                o.CreatedDate > Helper.Now.AddDays(-30) &&
                (o.Status == OrderStatusEnum.PaymentReceived || o.Status == OrderStatusEnum.OrderCompleted || o.Status == OrderStatusEnum.OrderPrepared));
            result.BankTransferOrdersAllPrev = ordersWithBankTransfer.Count(o =>
                o.CreatedDate <= Helper.Now.AddDays(-30));
            result.BankTransferOrdersPayedPrev = ordersWithBankTransfer.Count(o =>
                o.CreatedDate <= Helper.Now.AddDays(-30) &&
                (o.Status == OrderStatusEnum.PaymentReceived || o.Status == OrderStatusEnum.OrderCompleted || o.Status == OrderStatusEnum.OrderPrepared));

            var payedOrders = context.Orders.Where(o =>
                (o.Status == OrderStatusEnum.OrderCompleted ||
                 o.Status == OrderStatusEnum.PaymentReceived ||
                 o.Status == OrderStatusEnum.OrderPrepared) &&
                o.CreatedDate > Helper.Now.AddDays(-30));
            result.OrdersNumber = payedOrders.Count();
            result.OrdersTotal = payedOrders.Sum(o => o.Total);
            var payedOrdersPrev = context.Orders.Where(o =>
                (o.Status == OrderStatusEnum.OrderCompleted ||
                 o.Status == OrderStatusEnum.PaymentReceived ||
                 o.Status == OrderStatusEnum.OrderPrepared) &&
                o.CreatedDate <= Helper.Now.AddDays(-30) &&
                o.CreatedDate > Helper.Now.AddDays(-60));
            result.OrdersNumberPrev = payedOrdersPrev.Count();
            result.OrdersTotalPrev = payedOrdersPrev.Sum(o => o.Total);

            var orders = context.Orders
                .Include(o => o.Customer).ThenInclude(c => c.Cart)
                .Where(o => (o.Customer.Cart.CartItems.Any() ||
                            o.Status == OrderStatusEnum.OrderCompleted ||
                            o.Status == OrderStatusEnum.PaymentReceived ||
                            o.Status == OrderStatusEnum.OrderPlaced ||
                            o.Status == OrderStatusEnum.OrderPrepared) &&
                            o.CreatedDate > Helper.Now.AddDays(-60));
            var currentOrders = orders.Where(o => o.CreatedDate > Helper.Now.AddDays(-30));
            result.OrdersInStep1 = currentOrders.Count(o => o.Status == OrderStatusEnum.NewOrder);
            result.OrdersInStep2 = currentOrders.Count(o => o.Status == OrderStatusEnum.ShippingMethodSelected);
            result.OrdersInStep3 = currentOrders.Count(o => o.Status == OrderStatusEnum.PaymentInformationProvided);
            result.OrdersInStep4 = currentOrders.Count(o => o.Status == OrderStatusEnum.OrderPlaced || o.Status == OrderStatusEnum.OrderPrepared);
            var prevOrders = orders.Where(o => o.CreatedDate <= Helper.Now.AddDays(-30));
            result.OrdersInStep1Prev = prevOrders.Count(o => o.Status == OrderStatusEnum.NewOrder);
            result.OrdersInStep2Prev = prevOrders.Count(o => o.Status == OrderStatusEnum.ShippingMethodSelected);
            result.OrdersInStep3Prev = prevOrders.Count(o => o.Status == OrderStatusEnum.PaymentInformationProvided);
            result.OrdersInStep4Prev = prevOrders.Count(o => o.Status == OrderStatusEnum.OrderPlaced || o.Status == OrderStatusEnum.OrderPrepared);

            var abandonedOrdersWithCard = context.Orders
                .Include(o => o.Payments)
                .Where(o => o.PaymentMethod == PaymentMethodEnum.Barion &&
                            o.Payments.Count() > 0 &&
                            o.CreatedDate > Helper.Now.AddDays(-60));
            result.AbandonedCardPayments = abandonedOrdersWithCard.Count(o =>
                o.CreatedDate > Helper.Now.AddDays(-30) &&
                o.Status != OrderStatusEnum.PaymentReceived &&
                o.Status != OrderStatusEnum.OrderCompleted &&
                o.Status != OrderStatusEnum.OrderPrepared);
            result.AbandonedCardPaymentsPrev = abandonedOrdersWithCard.Count(o =>
                o.CreatedDate <= Helper.Now.AddDays(-30) &&
                o.Status != OrderStatusEnum.PaymentReceived &&
                o.Status != OrderStatusEnum.OrderCompleted &&
                o.Status != OrderStatusEnum.OrderPrepared);
            result.PayedCardOrders = abandonedOrdersWithCard.Count(o =>
                o.CreatedDate > Helper.Now.AddDays(-30) &&
                (o.Status == OrderStatusEnum.PaymentReceived ||
                o.Status == OrderStatusEnum.OrderCompleted ||
                o.Status == OrderStatusEnum.OrderPrepared));
            result.PayedCardOrdersPrev = abandonedOrdersWithCard.Count(o =>
                o.CreatedDate <= Helper.Now.AddDays(-30) &&
                (o.Status == OrderStatusEnum.PaymentReceived ||
                o.Status == OrderStatusEnum.OrderCompleted ||
                o.Status == OrderStatusEnum.OrderPrepared));

            var actionLogs = context.ActionLogs.Where(l =>
                l.Date > Helper.Now.AddDays(-60) &&
                (l.Action == ActionEnum.ViewCustomBracelet || l.Action == ActionEnum.AddToCartCustomBracelet));
            result.CustomBraceletViews = actionLogs.Count(l => l.Date > Helper.Now.AddDays(-30) && l.Action == ActionEnum.ViewCustomBracelet);
            result.CustomBraceletAddToCart = actionLogs.Count(l => l.Date > Helper.Now.AddDays(-30) && l.Action == ActionEnum.AddToCartCustomBracelet);
            result.CustomBraceletViewsPrev = actionLogs.Count(l => l.Date <= Helper.Now.AddDays(-30) && l.Action == ActionEnum.ViewCustomBracelet);
            result.CustomBraceletAddToCartPrev = actionLogs.Count(l => l.Date <= Helper.Now.AddDays(-30) && l.Action == ActionEnum.AddToCartCustomBracelet);
            if (result.CustomBraceletViews == 0) result.CustomBraceletViews++;
            if (result.CustomBraceletViewsPrev == 0) result.CustomBraceletViewsPrev++;

            var ordersByEmails = context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Status == OrderStatusEnum.OrderCompleted)
                .ToList()
                .GroupBy(o => o.EmailAddressId);
            result.CustomerLifetimeValue = ordersByEmails.Sum(g => g.Sum(o => o.Total)) / ordersByEmails.Count();
            result.CustomerLifetimeValueNet = ordersByEmails.Sum(g => g.Sum(o => o.EstimatedBusinessValue)) / ordersByEmails.Count();
            result.RepeatOrdersPercentage = 100 - (decimal)ordersByEmails.Count() / ordersByEmails.Sum(o => o.Count()) * 100;
            var repeatedOrders = ordersByEmails.Where(g => g.Count() > 1);
            var divider = 0;
            decimal totalDays = 0;
            foreach (var group in repeatedOrders)
            {
                var ordered = group.OrderBy(o => o.OrderedDate);
                var lastDate = ordered.First().OrderedDate;
                for (var i = 1; i < ordered.Count(); i++)
                {
                    divider++;
                    totalDays += (int)(ordered.ElementAt(i).OrderedDate - lastDate).Value.TotalDays;
                }
            }
            result.AverageDaysBetweenOrders = totalDays / divider;

            return Ok(result);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/draft-products")]
        public IActionResult DraftProducts()
        {
            var products = context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductDiscounts)
                .Include(p => p.ProductComponents).ThenInclude(c => c.Component).ThenInclude(c => c.ComponentSuppliers)
                .Where(p => p.State == ProductStateEnum.Draft)
                .ToList()
                .Select(p => new DraftProductViewModel
                {
                    Id = p.Id,
                    IdString = p.IdString,
                    Price = p.BasePrice,
                    Category = p.Category.Name,
                    Name = p.Name,
                    Components = string.Join("<br />", p.ProductComponents.OrderByDescending(pc => pc.Count).Select(pc => pc.ShowOnProduct ? $"<b>{pc.Component.Name} x {pc.Count}</b>" : $"{pc.Component.Name} x {pc.Count}")),
                    MarginsMinTotal = productPricingService.GetMarginsInformation(p).MinTotal,
                    MarginsMaxTotal = productPricingService.GetMarginsInformation(p).MaxTotal,
                    MarginsDescription = productPricingService.GetMarginsInformation(p).Description,
                    MarginsMissingInformation = productPricingService.GetMarginsInformation(p).MissingInformation,
                });
            return Ok(products);
        }

#if !DEBUG && !E2E
        [Authorize(Roles = "Admin")]
#endif
        [Route("/admin/activate-product/{idString}")]
        public IActionResult ActivateProduct(string idString)
        {
            var product = context.Products.First(p => p.IdString == idString);
            
            //todo majd kitörölni ha van duplikáció validálásunk
            //var x = context.Products.ToList().GroupBy(p => p.IdString).Where(g => g.Count() > 1).ToList().Select(g => g.ToList().Where(p => p.State == ProductStateEnum.Draft).First()).ToList();
            //context.Products.RemoveRange(x);
            //context.ActionLogs.RemoveRange(context.ActionLogs.ToList().Where(a => x.Any(p => p.Id == a.ProductId)));
            
            product.State = ProductStateEnum.Active;
            context.SaveChanges();
            return Ok();
        }
    }
}
