using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IProductImageService
    {
        public byte[] GetCustomBraceletImage(int cartItemId, int customerId, string webRootPath, int imageSize);
        public byte[] GetCustomOrderedBraceletImage(int orderItemId, int customerId, string imageRootPath, int imageSize);
        public byte[] GetCustomBraceletPreviewImage(BraceletSizeEnum braceletSize, CustomBraceletStyleEnum style, int beadTypeId, int secondaryBeadTypeId, int[] componentIds, string webRootPath, int imageSize);
    }
}
