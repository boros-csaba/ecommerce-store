using elenora.Features.Inventory;
using elenora.Features.ProductPricing;
using elenora.Features.StringBraceletDesigner;
using elenora.Models;
using elenora.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text;

namespace elenora.Services
{
    public class OrderService : IOrderService
    {
        private readonly DataContext context;
        private readonly ICartService cartService;
        private readonly IEmailService emailService;
        private readonly IInventoryService inventoryService;
        private readonly IProductPricingService productPricingService;
        private readonly IProductService productService;
        private readonly IPromotionService promotionService;

        public OrderService(IProductService productService, IProductPricingService productPricingService, IInventoryService inventoryService, DataContext context, ICartService cartService, IEmailService emailService, IPromotionService promotionService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            this.productPricingService = productPricingService ?? throw new ArgumentNullException(nameof(productPricingService));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        }

        public Order GetCustomerActiveOrder(int customerId)
        {
            var order = GetOrder(o => o.CustomerId == customerId && (
                    o.Status == OrderStatusEnum.NewOrder ||
                    o.Status == OrderStatusEnum.ShippingMethodSelected ||
                    o.Status == OrderStatusEnum.PaymentInformationProvided
                ));
            if (order != null) return order;
            order = new Order
            {
                CreatedDate = Helper.Now,
                ModifiedDate = Helper.Now,
                CustomerId = customerId,
                Status = OrderStatusEnum.NewOrder,
                ShippingMethod = ShippingMethodEnum.GLS,
                PaymentMethod = PaymentMethodEnum.Barion,
                BillingAddress = new Address()
            };
            context.Orders.Add(order);
            context.SaveChanges();
            return order;
        }

        public Order GetCustomerOrder(string orderId, string customerId)
        {
            return GetOrder(o => o.OrderId == orderId && o.Customer.CookieId == customerId);
        }

        public Order GetOrderById(int orderId)
        {
            return GetOrder(o => o.Id == orderId);
        }

        private Order GetOrder(Expression<Func<Order, bool>> filter)
        {
            var order = context.Orders
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                .Include(o => o.EmailAddress)
                .Include(o => o.Payments)
                .Include(o => o.Customer)
                .Include(o => o.Coupon)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemComplementaryProducts).ThenInclude(cp => cp.ComplementaryProduct)
                .Include(o => o.OrderItems).ThenInclude(i => (i as BraceletOrderItem).Product).ThenInclude(p => p.ProductDiscounts)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomTextBraceletOrderItem).Product)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomBraceletOrderItem).BeadType)
                .Include(o => o.OrderItems).ThenInclude(i => (i as CustomBraceletOrderItem).SecondaryBeadType)
                .SingleOrDefault(filter);
            if (order != null)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var bracelet = (orderItem as BraceletOrderItem)?.Product;
                    if (bracelet != null) bracelet.Price = productPricingService.GetProductPrice(bracelet);
                }
            }
            return order;
        }

        public List<FieldValidationError> CompleteShippingMethodStep(int customerId, bool validate, string emailAddress, ShippingMethodEnum shippingMethod, string shippingPointAddressInformation)
        {
            var result = new List<FieldValidationError>();
            var cart = cartService.GetCustomerCart(customerId);
            var order = GetCustomerActiveOrder(customerId);
            bool emailIsValid = true;

            if (validate)
            {
                if (string.IsNullOrWhiteSpace(emailAddress))
                {
                    result.Add(new FieldValidationError { Field = "email" });
                    emailIsValid = false;
                }
                else if (!emailAddress.Contains("@") || !emailAddress.Contains("."))
                {
                    result.Add(new FieldValidationError { Field = "email" });
                    emailIsValid = false;
                }

                if (shippingMethod == ShippingMethodEnum.GlsCsomagpont && string.IsNullOrWhiteSpace(shippingPointAddressInformation))
                {
                    result.Add(new FieldValidationError { Field = "shippingPointAddressInformation" });
                }
            }

            order.EmailAddress = emailService.GetOrCreateEmailAddress(emailAddress, "cart", false);
            if (!string.IsNullOrWhiteSpace(emailAddress) && emailIsValid)
            {
                var customer = context.Customers.First(c => c.Id == customerId);
                customer.EmailAddress = emailService.GetOrCreateEmailAddress(emailAddress, "cart", false);
            }

            if (shippingMethod == ShippingMethodEnum.GlsCsomagpont)
            {
                order.ShippingPointAddressInformation = shippingPointAddressInformation;
            }
            else
            {
                order.ShippingPointAddressInformation = null;
            }

            order.ShippingMethod = shippingMethod;
            cart.ShippingMethod = shippingMethod;
            if (shippingMethod != ShippingMethodEnum.GLS && cart.PaymentMethod == PaymentMethodEnum.PayAtDelivery)
            {
                order.PaymentMethod = PaymentMethodEnum.Barion;
                cart.PaymentMethod = PaymentMethodEnum.Barion;
            }

            if (validate && !result.Any())
            {
                if (order.Status == OrderStatusEnum.NewOrder)
                {
                    order.Status = OrderStatusEnum.ShippingMethodSelected;
                }
            }
            order.ModifiedDate = Helper.Now;
            context.SaveChanges();

            return result;
        }

        public List<FieldValidationError> CompleteBillingInformationStep(int customerId, bool validate, string billingName, string billingZip, string billingCity, string billingAddress, string phone, string remark, PaymentMethodEnum paymentMethod, bool differentShippingAddress, string shippingName = null, string shippingZip = null, string shippingCity = null, string shippingAddress = null)
        {
            var result = new List<FieldValidationError>();
            var cart = cartService.GetCustomerCart(customerId);
            var order = GetCustomerActiveOrder(customerId);

            if (validate)
            {
                if (string.IsNullOrWhiteSpace(billingName))
                {
                    result.Add(new FieldValidationError { Field = "name" });
                }
                if (string.IsNullOrWhiteSpace(billingZip))
                {
                    result.Add(new FieldValidationError { Field = "zipCode" });
                }
                if (string.IsNullOrWhiteSpace(billingCity))
                {
                    result.Add(new FieldValidationError { Field = "city" });
                }
                if (string.IsNullOrWhiteSpace(billingAddress))
                {
                    result.Add(new FieldValidationError { Field = "address" });
                }
                if (paymentMethod == PaymentMethodEnum.PayAtDelivery && string.IsNullOrWhiteSpace(phone))
                {
                    result.Add(new FieldValidationError { Field = "phone" });
                }
                if (differentShippingAddress)
                {
                    if (string.IsNullOrWhiteSpace(shippingName))
                    {
                        result.Add(new FieldValidationError { Field = "shipping-name" });
                    }
                    if (string.IsNullOrWhiteSpace(shippingZip))
                    {
                        result.Add(new FieldValidationError { Field = "shipping-zipCode" });
                    }
                    if (string.IsNullOrWhiteSpace(shippingCity))
                    {
                        result.Add(new FieldValidationError { Field = "shipping-city" });
                    }
                    if (string.IsNullOrWhiteSpace(shippingAddress))
                    {
                        result.Add(new FieldValidationError { Field = "shipping-address" });
                    }
                }
            }
            cart.PaymentMethod = paymentMethod;
            order.PaymentMethod = paymentMethod;
            order.Phone = phone;
            order.Remark = remark;
            if (order.BillingAddress == null) order.BillingAddress = new Address();
            order.BillingAddress.Name = billingName;
            order.BillingAddress.ZipCode = billingZip;
            order.BillingAddress.City = billingCity;
            order.BillingAddress.AddressLine = billingAddress;
            order.DifferentShippingAddress = differentShippingAddress;
            if (order.DifferentShippingAddress)
            {
                if (order.ShippingAddress == null) order.ShippingAddress = new Address();
                order.ShippingAddress.Name = shippingName;
                order.ShippingAddress.ZipCode = shippingZip;
                order.ShippingAddress.City = shippingCity;
                order.ShippingAddress.AddressLine = shippingAddress;
            }
            else if (order.ShippingAddress != null)
            {
                context.Remove(order.ShippingAddress);
                order.ShippingAddress = null;
            }

            if (validate && !result.Any())
            {
                if (order.Status == OrderStatusEnum.ShippingMethodSelected)
                {
                    order.Status = OrderStatusEnum.PaymentInformationProvided;
                }
            }
            order.ModifiedDate = Helper.Now;
            context.SaveChanges();

            return result;
        }

        public void PlaceOrder(int orderId, bool newsletterConsent)
        {
            var order = GetOrderById(orderId);
            var cart = cartService.GetCustomerCart(order.CustomerId);

            if (newsletterConsent)
            {
                order.EmailAddress.Unsubscribed = false;
            }

            order.OrderItems = ConvertCartItemsToOrderItems(order.Id, cart.CartItems);
            var cartTotal = cartService.GetCartTotal(cart);
            if (promotionService.IsPromotionActive(PromotionEnum.GiftLavaBracelet) && cartTotal >= promotionService.GetCurrentOrNextPromotion(PromotionEnum.GiftLavaBracelet).MinOrderValue)
            {
                order.OrderItems.Add(new GiftBraceletOrderItem { PromotionType = PromotionEnum.GiftLavaBracelet, Quantity = 1, UnitPrice = 0 });
            }
            cart.CartItems.ForEach(ci => ci.CartItemComplementaryProducts.ForEach(cp => context.Remove(cp)));
            cart.CartItems.ForEach(ci => context.Remove(ci));
            order.OrderId = GenerateOrderId(order.Id);
            order.OrderedDate = Helper.Now;
            order.ShippingPrice = cartService.GetShippingPrice(cart);
            order.Total = cartTotal;
            order.Status = OrderStatusEnum.OrderPlaced;

            if (cart.Coupon != null && cartService.GetCouponValue(cart) < 0)
            {
                order.Coupon = cart.Coupon;
                order.CouponId = cart.CouponId;
                order.CouponValue = cartService.GetCouponValue(cart);
                cart.Coupon.UsageCount++;
            }

            var customer = context.Customers
                .Include(c => c.CustomerPopupStats)
                .First(c => c.Id == order.CustomerId);
            if (customer.CustomerPopupStats != null)
            {
                order.OrderPopupStats = new OrderPopupStats
                {
                    PopupId = customer.CustomerPopupStats.PopupId,
                    PopupActionExecutedCount = customer.CustomerPopupStats.PopupActionExecutedCount,
                    PopupDisplayedCount = customer.CustomerPopupStats.PopupDisplayedCount,
                    PopupDisplayRemark = customer.CustomerPopupStats.PopupDisplayRemark,
                    PopupLastDisplayed = customer.CustomerPopupStats.PopupLastDisplayed
                };
                customer.CustomerPopupStats.PopupId = null;
                customer.CustomerPopupStats.PopupActionExecutedCount = 0;
                customer.CustomerPopupStats.PopupDisplayedCount = 0;
                customer.CustomerPopupStats.PopupDisplayRemark = null;
                customer.CustomerPopupStats.PopupLastDisplayed = null;
            }

            order.ModifiedDate = Helper.Now;
            context.Remove(cart);
            context.SaveChanges();
            order = GetOrderById(order.Id);
            SendOrderProcessingEmail(order);
            if (order.PaymentMethod == PaymentMethodEnum.PayAtDelivery)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem is BraceletOrderItem)
                    {
                        productService.LogProductPurchase((orderItem as BraceletOrderItem).ProductId);
                    }
                }
            }
            inventoryService.DecreaseInventory(order.Id);
        }

        private List<OrderItem> ConvertCartItemsToOrderItems(int orderId, List<CartItem> cartItems)
        {
            var result = new List<OrderItem>();
            foreach (var cartItem in cartItems)
            {
                OrderItem orderItem;
                if (cartItem is BraceletCartItem)
                {
                    var item = cartItem as BraceletCartItem;
                    orderItem = new BraceletOrderItem
                    {
                        ProductId = item.ProductId,
                        BraceletSize = item.BraceletSize,
                        BraceletSize2 = item.BraceletSize2,
                    };
                }
                else if (cartItem is CustomTextBraceletCartItem)
                {
                    var item = cartItem as CustomTextBraceletCartItem;
                    orderItem = new CustomTextBraceletOrderItem
                    {
                        ProductId = item.ProductId,
                        BraceletSize = item.BraceletSize,
                        CustomText = item.CustomText
                    };
                }
                else if (cartItem is CustomBraceletCartItem)
                {
                    var item = cartItem as CustomBraceletCartItem;
                    orderItem = new CustomBraceletOrderItem
                    {
                        BraceletSize = item.BraceletSize,
                        BeadTypeId = item.BeadTypeId,
                        SecondaryBeadTypeId = item.SecondaryBeadTypeId,
                        StyleType = item.StyleType,
                        Components = item.Components
                    };
                    foreach (var component in item.Components)
                    {
                        component.CartItem = null;
                        component.CartItemId = null;
                        component.OrderItem = orderItem as CustomBraceletOrderItem;
                    }
                }
                else
                {
                    var item = cartItem as StringBraceletCartItem;
                    orderItem = new StringBraceletOrderItem
                    {
                        BraceletType = item.BraceletType,
                        KnotColor = item.KnotColor,
                        FlapColor1 = item.FlapColor1,
                        FlapColor2 = item.FlapColor2,
                        StringColor1 = item.StringColor1,
                        StringColor2 = item.StringColor2,
                        StringColor3 = item.StringColor3
                    };
                }
                orderItem.OrderItemComplementaryProducts = cartItem.CartItemComplementaryProducts.Select(cp =>
                    new OrderItemComplementaryProduct
                    {
                        ComplementaryProductId = cp.ComplementaryProductId,
                        OrderItem = orderItem
                    }).ToList();
                orderItem.OrderId = orderId;
                orderItem.Quantity = cartItem.Quantity;
                orderItem.UnitPrice = cartItem.ItemPrice + cartItem.CartItemComplementaryProducts.Sum(cp => cp.ComplementaryProduct.Price);
                result.Add(orderItem);
            }
            return result;
        }

        private string GenerateOrderId(int orderId)
        {
            return $"{Helper.Now:yMMdd}-{orderId}";
        }

        public Payment CreateNewPayment(Order order)
        {
            var payment = new Payment
            {
                OrderId = order.Id,
                TransactionId = Guid.NewGuid().ToString(),
                Status = "New",
                CreatedDate = Helper.Now
            };
            context.Payments.Add(payment);
            context.SaveChanges();
            return payment;
        }

        public Payment GetPaymentByTransactionId(string transactionId)
        {
            return context.Payments
                .Include(p => p.Order).ThenInclude(o => o.OrderItems).ThenInclude(oi => (oi as BraceletOrderItem).Product)
                .Include(p => p.Order).ThenInclude(o => o.OrderItems).ThenInclude(i => (i as CustomTextBraceletOrderItem).Product)
                .Include(p => p.Order).ThenInclude(o => o.Customer)
                .First(p => p.TransactionId == transactionId);
        }

        public void UpdatePayment(Payment payment, string externalPaymentId, string externalTransactionId, double? fraudRiskScore, string status)
        {
            payment.ExternalId = externalPaymentId;
            payment.ExternalTransactionId = externalTransactionId;
            if (fraudRiskScore != null)
            {
                payment.FraudRiskScore = (int)fraudRiskScore;
            }
            payment.Status = status;
            var order = GetOrderById(payment.OrderId);
            if (status == "Succeeded")
            {
                order.Status = OrderStatusEnum.PaymentReceived;
                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem is BraceletOrderItem)
                    {
                        productService.LogProductPurchase((orderItem as BraceletOrderItem).ProductId);
                    }
                }
                LogOrderAction(order.Id, OrderHistoryActionEnum.SuccessfulPaymentReceived, "Sikeres Barion fizetés.");
            }
            else
            {
                LogOrderAction(order.Id, OrderHistoryActionEnum.FailedPaymentReceived, $"Sikertlen Barion fizetés: {status}.");
                SendBarionPaymenFailedEmail(order);
            }
            context.SaveChanges();
            inventoryService.DecreaseInventory(order.Id);
        }

        public void SendAbandonedCartEmailSequences()
        {
            var startingDate = Helper.Now.AddMonths(-1);
            var recentOrders = context.Orders
                .Include(o => o.Customer)
                .Include(o => o.EmailAddress)
                .Where(o => o.ModifiedDate > startingDate).ToList();

            foreach (var order in recentOrders)
            {
                var canSendEmailSequence = !string.IsNullOrWhiteSpace(order.EmailAddress?.Address) &&
                     order.Status != OrderStatusEnum.PaymentReceived &&
                     !(order.Status == OrderStatusEnum.OrderPlaced && order.PaymentMethod == PaymentMethodEnum.BankTransfer) &&
                     order.EmailSequenceStatus < 2;
                if (!canSendEmailSequence) continue;

                var cart = cartService.GetCustomerCart(order.CustomerId);
                if (cartService.GetCartTotal(cart) <= 0) continue;

                string subject;
                string plainTextMessage;
                string htmlMessage;
                var category = "abandoned-cart-1-";
                if (order.EmailSequenceStatus == 0)
                {
                    if (order.ModifiedDate > Helper.Now.AddHours(-3)) continue;
                    subject = "Nem felejtettél el valamit?";
                    category += "1";
                    plainTextMessage = GetAbandonedCartFirstEmailContent(order);
                    htmlMessage = GetAbandonedCartFirstEmailContentHtml(order, cart);
                }
                else if (order.EmailSequenceStatus == 1)
                {
                    if (order.LastEmailSequenceSentDate > Helper.Now.AddHours(-48)) continue;
                    subject = "Vedd meg most akciósan!";
                    category += "2";
                    if (cart.CouponId != null) continue;
                    var coupon = GenerateAbandonedCartCoupon(order);
                    plainTextMessage = GetAbandonedCartCouponEmailContent(order, coupon);
                    htmlMessage = GetAbandonedCartCouponEmailContentHtml(order, cart, coupon);
                }
                else
                {
                    continue;
                }
                htmlMessage = Helper.GetEmailTemplate().Replace("[EMAIL-CONTENT]", htmlMessage);

                LogOrderAction(order.Id, OrderHistoryActionEnum.EmailSent, $"Email: {category} - {subject}");
                emailService.SendEmail(order.EmailAddress.Address, order.BillingAddress?.Name, subject, plainTextMessage, htmlMessage, category);

                order.EmailSequenceStatus++;
                order.LastEmailSequenceSentDate = Helper.Now;
            }
            context.SaveChanges();
        }

        private Coupon GenerateAbandonedCartCoupon(Order order)
        {
            var coupon = new Coupon
            {
                Name = $"2 000 Ft kedvezmény",
                Value = 2000,
                MinCartValue = 10000,
                MaxUsageCount = 1,
                Code = $"E2EK0{order.Id}",
                StartDate = Helper.Now
            };
            context.Coupons.Add(coupon);
            return coupon;
        }

        private void SendOrderProcessingEmail(Order order)
        {
            var plainTextMessage = GetOrderProcessingEmailContent(order);
            var htmlMessage = GetOrderProcessingEmailContentHtml(order);
            LogOrderAction(order.Id, OrderHistoryActionEnum.EmailSent, $"Email: order-placed - Értesítés a leadott rendeléseddel kapcsolatban");
            emailService.SendEmail(order.EmailAddress.Address, order.BillingAddress.Name, "Értesítés a leadott rendeléseddel kapcsolatban", plainTextMessage, htmlMessage, "order-placed");
            emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Értesítés a leadott rendeléseddel kapcsolatban", plainTextMessage, htmlMessage, "report");
            emailService.SendEmail("brigitta.boros96@gmail.com", "Boros Brigitta", "Értesítés a leadott rendeléseddel kapcsolatban", plainTextMessage, htmlMessage, "report");
        }

        private string GetOrderProcessingEmailContent(Order order)
        {
            var sb = new StringBuilder();
            var total = Helper.GetFormattedMoney(order.Total) + " Ft";

            sb.AppendLine(GetEmailSalutationRow(false, order));
            sb.AppendLine("Köszönjük a megrendelésedet!");
            if (order.PaymentMethod == PaymentMethodEnum.Barion || order.PaymentMethod == PaymentMethodEnum.PayAtDelivery)
            {
                sb.AppendLine("A megrendelésed beérkezett hozzánk és megkezdtük annak feldolgozását.");
            }
            else
            {
                sb.AppendLine("A megrendelésed beérkezett hozzánk. Amint beérkezik az utalásod, megkezdjük a rendelésed feldolgozását.");
                sb.AppendLine("A rendelésed összegét az alábbi számlára utald:");
                sb.AppendLine("Név: Boros Csaba EV");
                sb.AppendLine("Bankszámlaszám: HU71 1040 4072 8676 7588 6969 1006");
                sb.AppendLine("Számlavezető bank: K&H");
                sb.AppendLine($"Utalandó összeg: {total}");
                sb.AppendLine("A megjegyzésben kérlek tűntesd fel a rendelésed számát.");
                sb.AppendLine("");
            }
            sb.AppendLine($"Rendelés száma: {order.OrderId}");

            return sb.ToString();
        }

        private string GetOrderProcessingEmailContentHtml(Order order)
        {
            var sb = new StringBuilder();
            var total = Helper.GetFormattedMoney(order.Total) + " Ft";

            sb.Append(GetEmailSalutationRow(true, order));
            sb.Append(Helper.GetEmailTitleRow("Köszönjük a megrendelésedet!"));
            if (order.PaymentMethod == PaymentMethodEnum.Barion || order.PaymentMethod == PaymentMethodEnum.PayAtDelivery)
            {
                sb.Append(Helper.GetEmailTextRow(@"
                    A megrendelésed beérkezett hozzánk és megkezdtük annak feldolgozását.<br />
                    A rendelésed részletei a következők:"));
            }
            else if (order.PaymentMethod == PaymentMethodEnum.BankTransfer)
            {
                sb.Append(Helper.GetEmailTextRow(@$"
                    A megrendelésed beérkezett hozzánk. Amint beérkezik az utalásod, megkezdjük a rendelésed feldolgozását.<br />
                    <br />
                    A rendelésed összegét az alábbi számlára utald:
                    <br />
                    Név: Boros Csaba EV <br />
                    Bankszámlaszám: HU71 1040 4072 8676 7588 6969 1006 <br />
                    Számlavezető bank: K&H<br />
                    Utalandó összeg: {total} <br />
                    A megjegyzésben kérlek tűntesd fel a rendelésed számát.<br />
                    <br />
                    Ha mégsem utalásasl fizetnél, akkor választhatsz más fizetési módot:"));

                var button1 = Helper.GetEmailButton("Fizetés bankkártyával", $"https://www.elenora.hu/rendeles/kartyas-fizetes/{order.CustomerId}-{order.Id}?s=orderconfirmationemail");
                var button2 = Helper.GetEmailButton("Fizetés a futárnál", $"https://www.elenora.hu/rendeles/fizetesi-mod/{order.OrderId}-{order.Customer.CookieId}?pm=@((int)PaymentMethodEnum.PayAtDelivery)");
                sb.Append(@$"<table style=""margin-bottom: 5px""><tr><td><table>{button1}</table></td><td><table>{button2}</table></td></tr></table>");

                if (order.Total - order.ShippingPrice < Settings.FREE_SHIPPING_THRESHOLD)
                {
                    sb.Append($"<small>Az utánvéttel való fizetés felára {Helper.GetFormattedMoney(Settings.GLS_PAYMENT_PRICE)} Ft</small>");
                }
                else 
                {
                    if (!Settings.FREE_SHIPPING_INCLUDES_PAYMENT)
                    {
                        sb.Append($"<small>Az utánvéttel való fizetés felára {Helper.GetFormattedMoney(Settings.GLS_PAYMENT_PRICE)} Ft</small>");
                    }
                }

                sb.Append(@"<br />
                    A rendelésed részletei a következők:");
            }
            sb.Append(Helper.GetEmailTextRow("<br />"));
            sb.Append(GetOrderContentInformation(order));
            
            return Helper.GetEmailTemplate().Replace("[EMAIL-CONTENT]", sb.ToString());
        }

        public void SendOrderPaymentReceivedEmail(Order order)
        {
            var plainTextMessage = GetOrderPaymentReceivedEmailContent(order);
            var htmlMessage = GetOrderPaymentReceivedEmailContentHtml(order);
            LogOrderAction(order.Id, OrderHistoryActionEnum.EmailSent, $"Email: order-payed - Értesítés a leadott rendeléseddel kapcsolatban");
            emailService.SendEmail(order.EmailAddress.Address, order.BillingAddress.Name, "Értesítés a leadott rendeléseddel kapcsolatban", plainTextMessage, htmlMessage, "order-payed");
        }

        private string GetOrderPaymentReceivedEmailContent(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetEmailSalutationRow(false, order));
            sb.AppendLine("Az utalásod beérkezett hozzánk és megkezdtük a rendelésed feldolgozását.");
            sb.AppendLine($"Rendelés száma: {order.OrderId}");
            return sb.ToString();
        }

        private string GetOrderPaymentReceivedEmailContentHtml(Order order)
        {
            var sb = new StringBuilder();
            sb.Append(GetEmailSalutationRow(true, order));
            sb.Append(Helper.GetEmailTitleRow("Köszönjük a megrendelésedet!"));
            sb.Append(Helper.GetEmailTextRow(@"
                Az utalásod beérkezett hozzánk és megkezdtük a rendelésed feldolgozását.<br />"));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            return Helper.GetEmailTemplate().Replace("[EMAIL-CONTENT]", sb.ToString());
        }

        public void SendPaymentRequestEmail(int orderId)
        {
            var order = context.Orders
                .Include(o => o.BillingAddress)
                .Include(o => o.EmailAddress)
                .First(o => o.Id == orderId);
            if (order.Status != OrderStatusEnum.OrderPlaced || order.PaymentMethod != PaymentMethodEnum.BankTransfer) return;
            if (order.LastPaymentRequestEmailDate != null && (Helper.Now - order.LastPaymentRequestEmailDate.Value).TotalHours < 1) return;
            var subject = "";
            var plainTextMessage = "";
            var htmlMessage = "";
            if (order.PaymentRequestEmailsSent == 0)
            {
                subject = "Értesítés a leadott rendeléseddel kapcsolatban";
                plainTextMessage = GetFirstPaymentRequestEmailContent(order);
                htmlMessage = GetFirstPaymentRequestEmailContentHtml(order);
            }
            else if (order.PaymentRequestEmailsSent == 1)
            {
                subject = "Rendelés";
                plainTextMessage = GetSecondPaymentRequestEmailContent(order);
                htmlMessage = GetSecondPaymentRequestEmailContentHtml(order);
            }
            else return;

            order.PaymentRequestEmailsSent++;
            var category = $"payment-request-{order.PaymentRequestEmailsSent}";
            LogOrderAction(order.Id, OrderHistoryActionEnum.EmailSent, $"Email: {category} - {subject}");
            emailService.SendEmail(order.EmailAddress.Address, order.BillingAddress.Name, subject, plainTextMessage, htmlMessage, category);
            order.LastPaymentRequestEmailDate = Helper.Now;
            context.SaveChanges();
        }

        private string GetFirstPaymentRequestEmailContent(Order order)
        {
            var total = Helper.GetFormattedMoney(order.Total) + " Ft";
            var sb = new StringBuilder();
            sb.AppendLine(GetEmailSalutationRow(false, order));
            sb.AppendLine("Rendelésedet megkaptuk, fizetési módnak az utalást válaszotttad, viszont nem érkezett még be hozzánk az összeg. A rendelésed 5 napig őrizzük meg, ha addig nem érkezik be az utalás, akkor törlésre kerül a rendelés.");
            sb.AppendLine("A rendelésed összegét az alábbi számlára utald:");
            sb.AppendLine("Név: Boros Csaba EV");
            sb.AppendLine("Bankszámlaszám: HU71 1040 4072 8676 7588 6969 1006");
            sb.AppendLine("Számlavezető bank: K&H");
            sb.AppendLine($"Utalandó összeg: {total}");
            sb.AppendLine("A megjegyzésben kérlek tűntesd fel a rendelésed számát.");
            sb.AppendLine("");
            sb.AppendLine($"Rendelés száma: {order.OrderId}");
            return sb.ToString();
        }

        private string GetFirstPaymentRequestEmailContentHtml(Order order)
        {
            var total = Helper.GetFormattedMoney(order.Total) + " Ft";
            var sb = new StringBuilder();
            sb.Append(GetEmailSalutationRow(true, order));
            sb.Append(Helper.GetEmailTitleRow("Köszönjük a megrendelésedet!"));
            sb.Append(Helper.GetEmailTextRow($@"
                Rendelésedet megkaptuk, fizetési módnak az utalást válaszotttad, viszont nem érkezett még be hozzánk az összeg. A rendelésed 5 napig őrizzük meg, ha addig nem érkezik be az utalás, akkor törlésre kerül a rendelés.<br />
                    <br />
                    A rendelésed összegét az alábbi számlára utald:
                    <br />
                    Név: Boros Csaba EV <br />
                    Bankszámlaszám: HU71 1040 4072 8676 7588 6969 1006 <br />
                    Számlavezető bank: K&H<br />
                    Utalandó összeg: {total} <br />
                    A megjegyzésben kérlek tűntesd fel a rendelésed számát. Rendelés száma: {order.OrderId}<br />"));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            return Helper.GetEmailTemplate().Replace("[EMAIL-CONTENT]", sb.ToString());
        }

        private string GetSecondPaymentRequestEmailContent(Order order)
        {
            var total = Helper.GetFormattedMoney(order.Total) + " Ft";
            var sb = new StringBuilder();
            sb.AppendLine("Szia!");
            sb.AppendLine("Beérkezett hozzánk a rendelésed, de még nem került kifizetésre.");
            sb.AppendLine("Van valami gond esetleg? Tudunk segíteni?");
            sb.AppendLine("Keress minket bizalommal!");
            sb.AppendLine("Az alábbi számlára kellene utalni a rendelés összegét:");
            sb.AppendLine("Név: Boros Csaba EV");
            sb.AppendLine("Bankszámlaszám: HU71 1040 4072 8676 7588 6969 1006");
            sb.AppendLine("Számlavezető bank: K&H");
            sb.AppendLine($"Utalandó összeg: {total}");
            sb.AppendLine("A megjegyzésben kérlek tűntesd fel a rendelésed számát.");
            sb.AppendLine($"Rendelés száma: {order.OrderId}");
            sb.AppendLine("Üdvözlettel,");
            sb.AppendLine("Boros Brigitta");
            sb.AppendLine("ELENORA");
            return sb.ToString();
        }

        private string GetSecondPaymentRequestEmailContentHtml(Order order)
        {
            var total = Helper.GetFormattedMoney(order.Total) + " Ft";
            var sb = new StringBuilder();
            sb.AppendLine("Szia!<br /><br />");
            sb.AppendLine("Beérkezett hozzánk a rendelésed, de még nem került kifizetésre.<br />");
            sb.AppendLine("Van valami gond esetleg? Tudunk segíteni?<br />");
            sb.AppendLine("Keress minket bizalommal!<br /><br />");
            sb.AppendLine("Az alábbi számlára kellene utalni a rendelés összegét:<br />");
            sb.AppendLine("Név: Boros Csaba EV<br />");
            sb.AppendLine("Bankszámlaszám: HU71 1040 4072 8676 7588 6969 1006<br />");
            sb.AppendLine("Számlavezető bank: K&H<br />");
            sb.AppendLine($"Utalandó összeg: {total}<br />");
            sb.AppendLine("A megjegyzésben kérlek tűntesd fel a rendelésed számát.<br />");
            sb.AppendLine($"Rendelés száma: {order.OrderId}<br /><br />");
            sb.AppendLine("Üdvözlettel,<br />");
            sb.AppendLine("Boros Brigitta<br />");
            sb.AppendLine("ELENORA");
            return sb.ToString();
        }

        public void SendOrderCompletedEmail(Order order)
        {
            var plainTextMessage = GetOrderCompletedEmailContent(order);
            var htmlMessage = GetOrderCompletedEmailContentHtml(order);
            LogOrderAction(order.Id, OrderHistoryActionEnum.EmailSent, "Email: order-completed - Értesítés a rendelés feladásáról");
            emailService.SendEmail(order.EmailAddress.Address, order.BillingAddress.Name, "Értesítés a rendelés feladásáról", plainTextMessage, htmlMessage, "order-completed");
            emailService.SendEmail("boros.csaba94@gmail.com","Boros Csaba", "Értesítés a rendelés feladásáról", plainTextMessage, htmlMessage, "order-completed");
            context.SaveChanges();
        }

        private string GetOrderCompletedEmailContent(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetEmailSalutationRow(false, order));
            sb.AppendLine($"A {order.OrderId} számú megrendelésedet a mai napon feladtuk a postán.");
            if (!string.IsNullOrWhiteSpace(order.PackageTrackingNumber))
            {
                sb.AppendLine("A csomagot nyomon követheted ezen a linken: https://posta.hu/ugyfelszolgalat/nyomkovetes");
                sb.AppendLine($"Küldeményazonosító: {order.PackageTrackingNumber}");
            }
            return sb.ToString();
        }

        private string GetOrderCompletedEmailContentHtml(Order order)
        {
            var sb = new StringBuilder();
            sb.Append(GetEmailSalutationRow(true, order));
            sb.Append(Helper.GetEmailTextRow($@"A {order.OrderId} számú megrendelésedet a mai napon feladtuk a postán."));
            sb.Append($@"
                A csomagot nyomon követheted ezen a linken: <a href=""https://posta.hu/ugyfelszolgalat/nyomkovetes"" target=""_blank"">Posta nyomkövetés</a><br />
                Küldeményazonosító: {order.PackageTrackingNumber}");
            sb.Append(Helper.GetEmailTextRow("<br />"));
            sb.Append(GetOrderContentInformation(order));
            return Helper.GetEmailTemplate().Replace("[EMAIL-CONTENT]", sb.ToString());
        }

        private string GetAbandonedCartFirstEmailContent(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetEmailSalutationRow(false, order));
            sb.AppendLine("Nem felejtettél valamit a kosárban?");
            sb.AppendLine("A kosarad tartalma már csak megrendelésre vár. Mi megkönyítjük a vásárlásodat, csak kattints az alábbi linkre:");
            sb.AppendLine("https://www.elenora.hu/kosar/folytatas/" + order.Customer.CookieId + "?utm_medium=email&utm_source=abandoned-cart&utm_campaign=abandoned-cart-1-1");

            return sb.ToString();
        }

        private string GetAbandonedCartFirstEmailContentHtml(Order order, Cart cart)
        {
            var sb = new StringBuilder();
            sb.Append(GetEmailSalutationRow(true, order));
            sb.Append(Helper.GetEmailTitleRow("Nem felejtettél valamit a kosárban?"));
            sb.Append(Helper.GetEmailTextRow("A kosarad tartalma már csak megrendelésre vár. Mi megkönyítjük a vásárlásodat, csak kattints az alábbi gombra:"));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            sb.Append(Helper.GetEmailButton("VÁSÁRLÁS FOLYTATÁSA", "https://www.elenora.hu/kosar/folytatas/" + order.Customer.CookieId + "?utm_medium=email&utm_source=abandoned-cart&utm_campaign=abandoned-cart-1-1"));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            sb.Append(Helper.GetEmailTextRow("A kosarad tartalma: "));
            sb.Append(GetEmailCartContent(order, cart));
            return sb.ToString();
        }

        private string GetAbandonedCartCouponEmailContent(Order order, Coupon coupon)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetEmailSalutationRow(false, order));
            sb.AppendLine("Megőriztük a kosarad tartalmát!");
            sb.AppendLine("Ha 24 órán belül megrendeled, akkor megajándékozunk egy 2000Ft értékű kuponnal, melyet 10000Ft feletti vásárlás esetén tudsz beváltani.");
            sb.AppendLine("Jól hangzik, ugye?");
            sb.AppendLine($"A kupon beváltásához használd a {coupon.Code} kódot vagy kattints az alábbi linkre:");
            sb.AppendLine("https://www.elenora.hu/kosar/folytatas/" + order.Customer.CookieId + "/" + coupon.Code + "?utm_medium=email&utm_source=abandoned-cart&utm_campaign=abandoned-cart-1-2");

            return sb.ToString();
        }

        private string GetAbandonedCartCouponEmailContentHtml(Order order, Cart cart, Coupon coupon)
        {
            var sb = new StringBuilder();
            sb.Append(GetEmailSalutationRow(true, order));
            sb.Append(Helper.GetEmailTitleRow("Megőriztük a kosarad tartalmát!"));
            sb.Append(Helper.GetEmailTextRow("Ha 24 órán belül megrendeled, akkor megajándékozunk egy 2000Ft értékű kuponnal, melyet 10000Ft feletti vásárlás esetén tudsz beváltani. Jól hangzik, ugye?"));
            sb.Append(Helper.GetEmailTextRow($"Kupon kód: <b>{coupon.Code}</b>"));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            sb.Append(Helper.GetEmailButton("KÉREM A KUPONT", "https://www.elenora.hu/kosar/folytatas/" + order.Customer.CookieId + "/" + coupon.Code + "?utm_medium=email&utm_source=abandoned-cart&utm_campaign=abandoned-cart-1-2"));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            sb.Append(Helper.GetEmailTextRow("A kosarad tartalma: "));
            sb.Append(GetEmailCartContent(order, cart));
            return sb.ToString();
        }

        private string GetEmailSalutationRow(bool forHtml, Order order)
        {
            var salutationMessage = emailService.GetSalutation(order.BillingAddress?.Name);
            if (forHtml)
            {
                return @$"<tr><td colspan=""2"" align=""center"" style=""font-family:'Montserrat', sans-serif; font-size: 1.2rem;"">{salutationMessage}</td></tr>";
            }
            else
            {
                return salutationMessage;
            }
        }

        private string GetEmailCartContent(Order order, Cart cart = null)
        {
            var sb = new StringBuilder();
            sb.Append(@"<tr><td colspan=""2"" style=""padding-top: 15px""><table border=""0"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%""><tr><th style=""padding-bottom: 15px; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" align=""left"">Termék</th><th style=""padding-bottom: 15px; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" width=""70"">Mennyiség</th><th style=""padding-bottom: 15px; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" align=""right"" width=""30%"">Ár</th></tr>");
            if (cart != null)
            {
                foreach (var cartItem in cart.CartItems)
                {
                    var sizeDisplay = string.Empty;
                    if (cartItem is IBraceletWithSize)
                    {
                        var size = Helper.GetFormattedBraceletSize((cartItem as IBraceletWithSize).BraceletSize, (cartItem as BraceletCartItem)?.BraceletSize2);
                        sizeDisplay = @$"<span style=""font-size: 0.75rem"">{size}</span>";
                    }
                    var quantity = cartItem.Quantity;
                    var price = Helper.GetFormattedMoney((cartItem.ItemPrice + cartItem.CartItemComplementaryProducts.Sum(p => p.ComplementaryProduct.Price)) * quantity);
                    var complementaryProducts = string.Join("", cartItem.CartItemComplementaryProducts.Select(p => @$"<span style=""font-size: 0.75rem;display: block;"">{p.ComplementaryProduct.Name}</span>"));
                    sb.Append(@$"<tr><td style=""padding: 0 0 0 5px; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd""><span style=""font-size: 1rem; font-weight: bold;"">{cartItem.Name}</span><br />{sizeDisplay}{complementaryProducts}</td><td align=""center"" style=""border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">{quantity}</td><td align=""right"" style=""border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">{price} Ft</td></tr>");
                }
                if (promotionService.IsPromotionActive(PromotionEnum.GiftLavaBracelet)) 
                {
                    var promotion = promotionService.GetCurrentOrNextPromotion(PromotionEnum.GiftLavaBracelet);
                    string promotionDescription = "";
                    if (promotion.EndDate.HasValue)
                    {
                        promotionDescription = @$"(Az akció csak {promotion.EndDate.Value:yyyy.MM.dd}-ig tart!)";
                    }
                    sb.Append(@$"<tr><td style=""padding: 0 0 0 5px; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd""><span style=""font-size: 1rem; font-weight: bold;"">Ajándék lávakő karkötő</span><br />{promotionDescription}</td><td align=""center"" style=""border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">1</td><td align=""right"" style=""border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">INGYEN</td></tr>");
                }
            }
            else
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var sizeDisplay = string.Empty;
                    if (orderItem is IBraceletWithSize)
                    {
                        var size = Helper.GetFormattedBraceletSize((orderItem as IBraceletWithSize)?.BraceletSize, (orderItem as BraceletOrderItem)?.BraceletSize2);
                        sizeDisplay = @$"<span style=""font-size: 0.75rem"">{size}</span>";
                    }
                    var quantity = orderItem.Quantity;
                    var price = Helper.GetFormattedMoney(orderItem.UnitPrice * quantity);
                    var complementaryProducts = string.Join("", orderItem.OrderItemComplementaryProducts.Select(p => @$"<span style=""font-size: 0.75rem;display: block;"">{p.ComplementaryProduct.Name}</span>"));
                    sb.Append(@$"<tr><td style=""padding: 0 0 0 5px; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd""><span style=""font-size: 1rem; font-weight: bold;"">{orderItem.Name}</span><br />{sizeDisplay}{complementaryProducts}</td><td align=""center"" style=""border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">{quantity}</td><td align=""right"" style=""border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">{price} Ft</td></tr>");
                }
            }
            sb.Append("</table></td></tr>");

            if (order.Status != OrderStatusEnum.ShippingMethodSelected)
            {
                var paymentMethod = "";
                switch (order.PaymentMethod) 
                {
                    case PaymentMethodEnum.Barion:
                        paymentMethod = "Kártyás fizetés (Barion)";
                        break;
                    case PaymentMethodEnum.BankTransfer:
                        paymentMethod = "Banki átutalás";
                        break;
                    case PaymentMethodEnum.PayAtDelivery:
                        paymentMethod = "Utánvét(fizetés a futárnál)";
                        break;
                }
                sb.Append(@$"<tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Fizetés módja</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" align=""right"">{paymentMethod}</td></tr>");
            }

            if (cart?.Coupon != null || order.CouponId != null)
            {
                var value = "";
                if (cart != null)
                {
                    value = Helper.GetFormattedMoney(cartService.GetCouponValue(cart)) + " Ft";
                }
                else
                {
                    value = Helper.GetFormattedMoney(order.Total - order.OrderItems.Sum(i => i.UnitPrice * i.Quantity) - order.ShippingPrice) + " Ft";
                }
                sb.Append(@$"<tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Kupon</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" align=""right"">{value}</td></tr>");
            }
            return sb.ToString();
        }

        private void SendBarionPaymenFailedEmail(Order order)
        {
            if (order.PaymentRequestEmailsSent > 0) return;
            if (order.Status == OrderStatusEnum.PaymentReceived || order.Status == OrderStatusEnum.OrderPrepared || order.Status == OrderStatusEnum.OrderCompleted) return;
            if (order.PaymentMethod != PaymentMethodEnum.Barion) return;

            var customerActiveOrder = GetCustomerActiveOrder(order.CustomerId);
            if (customerActiveOrder.Id != order.Id && (int)customerActiveOrder.Status >= (int)OrderStatusEnum.ShippingMethodSelected) return;

            var plainTextMessage = GetBarionPaymentFailedEmailContent(order);
            var htmlMessage = GetBarionPaymentFailedEmailContentHtml(order);
            LogOrderAction(order.Id, OrderHistoryActionEnum.EmailSent, "Email: payment-failed - Sikertelen fizetés");
            emailService.SendEmail(order.EmailAddress.Address, order.BillingAddress.Name, "Sikertelen fizetés", plainTextMessage, htmlMessage, "payment-failed");
            emailService.SendEmail("boros.csaba94@gmail.com", "Boros Csaba", "Sikertelen fizetés", plainTextMessage, htmlMessage, "payment-failed");
            order.PaymentRequestEmailsSent++;
            context.SaveChanges();
        }

        private string GetBarionPaymentFailedEmailContent(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetEmailSalutationRow(false, order));
            sb.AppendLine($"A {order.OrderId} számú megrendelés kifizetése sikertelen volt!");
            sb.AppendLine($"A bankkártyás fizetést meg tudod ismét próbálni ezen a linken keresztül: https://www.elenora.hu/rendeles/kartyas-fizetes/{order.CustomerId}-{order.Id}?s=failedemail");
            sb.AppendLine("Ha inkább banki átutalással szeretnél fizetni, akkor a rendelésed összegét az alábbi számlára utald:");
            sb.AppendLine("Név: Boros Csaba EV");
            sb.AppendLine("Bankszámlaszám: HU71 1040 4072 8676 7588 6969 1006");
            sb.AppendLine("Számlavezető bank: K&H");
            sb.AppendLine($"Utalandó összeg: {order.Total}");
            sb.AppendLine("A megjegyzésben kérlek tűntesd fel a rendelésed számát.");
            sb.AppendLine("");
            sb.AppendLine($"Rendelés száma: {order.OrderId}");

            return sb.ToString();
        }

        private string GetBarionPaymentFailedEmailContentHtml(Order order)
        {
            var total = Helper.GetFormattedMoney(order.Total) + " Ft";
            var sb = new StringBuilder();
            sb.Append(GetEmailSalutationRow(true, order));
            sb.Append(Helper.GetEmailTitleRow("Sikertelen fizetés!"));
            sb.Append(Helper.GetEmailTextRow("Rendelésedet megkaptuk,  viszont a fizetés sikertelen volt. Kérlek az alábbi linket használva próbáld újra a kártyás fizetést:<br />"));
            sb.Append(Helper.GetEmailButton("FIZETÉS BANKKÁRTYÁVAL", $"https://www.elenora.hu/rendeles/kartyas-fizetes/{order.CustomerId}-{order.Id}?s=failedemail"));
            sb.Append(Helper.GetEmailTextRow($@"
                    <br />
                    Amennyiben inkább utalással szeretnél fizetni, akkor a rendelésed összegét az alábbi számlára utald:
                    <br />
                    Név: Boros Csaba EV <br />
                    Bankszámlaszám: HU71 1040 4072 8676 7588 6969 1006 <br />
                    Számlavezető bank: K&H<br />
                    Utalandó összeg: {total} <br />
                    A megjegyzésben kérlek tűntesd fel a rendelésed számát. Rendelés száma: {order.OrderId}<br />"));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            sb.Append(GetOrderContentInformation(order));
            return Helper.GetEmailTemplate().Replace("[EMAIL-CONTENT]", sb.ToString());
        }

        private string GetOrderContentInformation(Order order)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<tr><td colspan=""2"" align=""center"" style=""font-size: 1.2rem; font-weight: bold; padding-bottom: 20px;"">Rendelés részletei:</td></tr><tr><td style=""padding-bottom: 15px; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">");
            sb.Append($"Rendelés száma: <b>{order.OrderId}</b>");
            sb.Append(@"</td><td align=""right"" style=""padding-bottom: 15px; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Rendelés dátuma: ");
            sb.Append("<b>" + order.OrderedDate.Value.ToString("yyyy.MM.dd") + "</b></td></tr>");

            sb.Append(GetEmailCartContent(order));

            var total = Helper.GetFormattedMoney(order.Total);
            var shipping = Helper.GetFormattedMoney(order.ShippingPrice);
            var shippingMethod = Helper.GetFormattedShippingMethod(order.ShippingMethod);
            var address = $"{order.BillingAddress.ZipCode} {order.BillingAddress.City}, {order.BillingAddress.AddressLine}";
            sb.Append(@$"<tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Szállítás ({shippingMethod})</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" align=""right"">{shipping} Ft</td></tr><tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Végösszeg</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd; font-weight: bold;"" align=""right"">{total} Ft</td></tr><tr><td colspan=""2"" align=""center"" style=""font-size: 1.2rem; font-weight: bold; padding: 35px  0 5px 0;"">Számlázási adatok:</td></tr><tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Név</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" align=""right"">{order.BillingAddress.Name}</td></tr><tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Cím</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd;"" align=""right"">{address}</td></tr>");
            if (order.ShippingAddress != null)
            {
                var shippingAddress = $"{order.ShippingAddress.ZipCode} {order.ShippingAddress.City}, {order.ShippingAddress.AddressLine}";
                sb.Append(@$"<tr><td colspan=""2"" align=""center"" style=""font-size: 1.2rem; font-weight: bold; padding: 35px  0 5px 0;"">Szállítási adatok:</td></tr><tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Név</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" align=""right"">{order.ShippingAddress.Name}</td></tr><tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Cím</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd;"" align=""right"">{shippingAddress}</td></tr>");
            }
            if (!string.IsNullOrWhiteSpace(order.ShippingPointAddressInformation))
            {
                sb.Append(@$"<tr><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Csomagpont:</td><td style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"" align=""right"">{order.ShippingPointAddressInformation}</td></tr>");
            }
            if (!string.IsNullOrWhiteSpace(order.Remark))
            {
                sb.Append(@$"<tr><td colspan=""2"" style=""padding: 15px 0 15px 0; border-width: 0 0 1px 0; border-style: solid; border-color: #ddd"">Megjegyzés: {order.Remark}</td></tr>");
            }
            return sb.ToString();
        }

        public void ChangePaymentMethod(string orderId, string customerId, PaymentMethodEnum paymentMethod)
        {
            var order = GetCustomerOrder(orderId, customerId);
            var prevShippingMethod = order.ShippingMethod;
            var prevPaymentMethod = order.PaymentMethod;

            if (order.ShippingMethod == ShippingMethodEnum.GLS && 
                order.PaymentMethod == PaymentMethodEnum.PayAtDelivery &&
                paymentMethod != PaymentMethodEnum.PayAtDelivery)
            {
                throw new ArgumentException("Invalid payment method change!");
            }

            order.PaymentMethod = paymentMethod;
            if (paymentMethod == PaymentMethodEnum.PayAtDelivery)
            {
                order.ShippingMethod = ShippingMethodEnum.GLS;
            }
            var totalWithoutShipping = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice) + order.CouponValue;
            order.ShippingPrice = cartService.GetShippingPrice(totalWithoutShipping, order.ShippingMethod, order.PaymentMethod);
            order.Total = totalWithoutShipping + order.ShippingPrice;

            var historyDescription = "Csere:";
            if (prevPaymentMethod != order.PaymentMethod) historyDescription += $" {Helper.GetFormattedPaymentMethod(prevPaymentMethod)} => {Helper.GetFormattedPaymentMethod(order.PaymentMethod)}";
            if (prevShippingMethod != order.ShippingMethod) historyDescription += $" {Helper.GetFormattedShippingMethod(prevShippingMethod)} => {Helper.GetFormattedShippingMethod(order.ShippingMethod)}";
            LogOrderAction(order.Id, OrderHistoryActionEnum.PaymentMethodChanged, historyDescription);
            context.SaveChanges();
        }

        public void LogOrderAction(int orderId, OrderHistoryActionEnum action, string description) 
        {
            var historyItem = new OrderHistory
            {
                OrderId = orderId,
                Date = Helper.Now,
                Action = action,
                Description = description
            };
            context.OrderHistories.Add(historyItem);
            context.SaveChanges();
        }
    }
}
