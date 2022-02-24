using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.StringBraceletDesigner
{
    public class StringBraceletViewModel
    {
        public string IdPrefix { get; set; }
        public BraceletTypeEnum BraceletType { get; set; }
        public string KnotColor { get; set; } = "#fff";
        public string Line1Color { get; set; } = "#fff";
        public string Line2Color { get; set; } = "#fff";
        public string Line3Color { get; set; } = "#fff";
        public string Line4Color { get; set; } = "#fff";
        public string Line5Color { get; set; } = "#fff";
        public string Line6Color { get; set; } = "#fff";
        public string Flap1Color { get; set; } = "#fff";
        public string Flap2Color { get; set; } = "#fff";
        public int Width { get; set; } = 600;
        public int Height { get; set; } = 500;
        public string CssClass { get; set; }

        public StringBraceletViewModel()
        {

        }

        public StringBraceletViewModel(StringBraceletCartItem cartItem)
        {
            IdPrefix = cartItem.Id.ToString();
            BraceletType = cartItem.BraceletType;
            KnotColor = cartItem.KnotColor;
            if (BraceletType == BraceletTypeEnum.ManyStringsBracelet)
            {
                if (!string.IsNullOrWhiteSpace(cartItem.StringColor3))
                {
                    Line1Color = cartItem.StringColor3;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor2;
                    Line4Color = cartItem.StringColor2;
                    Line5Color = cartItem.StringColor3;
                    Line6Color = cartItem.StringColor1;
                }
                else if (!string.IsNullOrWhiteSpace(cartItem.StringColor2))
                {
                    Line1Color = cartItem.StringColor2;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor1;
                    Line4Color = cartItem.StringColor2;
                    Line5Color = cartItem.StringColor2;
                    Line6Color = cartItem.StringColor1;
                }
                else
                {
                    Line1Color = cartItem.StringColor1;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor1;
                    Line4Color = cartItem.StringColor1;
                    Line5Color = cartItem.StringColor1;
                    Line6Color = cartItem.StringColor1;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(cartItem.StringColor3))
                {
                    Line1Color = cartItem.StringColor3;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor2;
                }
                else if (!string.IsNullOrWhiteSpace(cartItem.StringColor2))
                {
                    Line1Color = cartItem.StringColor2;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor1;
                }
                else
                {
                    Line1Color = cartItem.StringColor1;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor1;
                }
            }
            Flap1Color = cartItem.FlapColor1;
            Flap2Color = cartItem.FlapColor2;
            if (string.IsNullOrWhiteSpace(Flap2Color)) Flap2Color = Flap1Color;
        }

        public StringBraceletViewModel(StringBraceletOrderItem cartItem)
        {
            IdPrefix = cartItem.Id.ToString();
            BraceletType = cartItem.BraceletType;
            KnotColor = cartItem.KnotColor;
            if (BraceletType == BraceletTypeEnum.ManyStringsBracelet)
            {
                if (!string.IsNullOrWhiteSpace(cartItem.StringColor3))
                {
                    Line1Color = cartItem.StringColor3;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor2;
                    Line4Color = cartItem.StringColor2;
                    Line5Color = cartItem.StringColor3;
                    Line6Color = cartItem.StringColor1;
                }
                else if (!string.IsNullOrWhiteSpace(cartItem.StringColor2))
                {
                    Line1Color = cartItem.StringColor2;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor1;
                    Line4Color = cartItem.StringColor2;
                    Line5Color = cartItem.StringColor2;
                    Line6Color = cartItem.StringColor1;
                }
                else
                {
                    Line1Color = cartItem.StringColor1;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor1;
                    Line4Color = cartItem.StringColor1;
                    Line5Color = cartItem.StringColor1;
                    Line6Color = cartItem.StringColor1;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(cartItem.StringColor3))
                {
                    Line1Color = cartItem.StringColor3;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor2;
                }
                else if (!string.IsNullOrWhiteSpace(cartItem.StringColor2))
                {
                    Line1Color = cartItem.StringColor2;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor1;
                }
                else
                {
                    Line1Color = cartItem.StringColor1;
                    Line2Color = cartItem.StringColor1;
                    Line3Color = cartItem.StringColor1;
                }
            }
            Flap1Color = cartItem.FlapColor1;
            Flap2Color = cartItem.FlapColor2;
            if (string.IsNullOrWhiteSpace(Flap2Color)) Flap2Color = Flap1Color;
        }
    }
}
