using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Features.StringBraceletDesigner
{
    public class StringBraceletOrderItem : OrderItem
    {
        public override string ProductIdString => "string-bracelet";

        public override string Name => BraceletType == BraceletTypeEnum.BraidedBracelet ? "Fonott karkötő" : "Több szálas karkötő";
        public BraceletTypeEnum BraceletType { get; set; }
        public string KnotColor { get; set; }
        public string FlapColor1 { get; set; }
        public string FlapColor2 { get; set; }
        public string StringColor1 { get; set; }
        public string StringColor2 { get; set; }
        public string StringColor3 { get; set; }
    }
}
