using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Models
{
    public enum ActionEnum
    {
        ViewHomePage = 1,
        ViewCategoryPage = 2,
        ViewProduct = 3,
        Search = 4,
        ViewWishList = 5,
        ViewLandingPage = 6,
        DisplayPopup = 7,
        ViewPage = 8,
        ViewCustomBracelet = 9,
        
        AddToWishlist = 10,
        AddToCart = 11,
        AddCoupon = 12,
        AddToCartCustomBracelet = 13,
        StartBarionPayment = 14,
        ContinueShoppingFromEmail = 15,
        AcceptPopup = 16,

        DeleteFromWishlist = 20,
        DeleteFromCart = 21,
        DeleteCoupon = 22,

        StartEditCustomBracelet = 30,
        SaveEditedCustomBracelet = 31,

        Error = 100
    }
}
