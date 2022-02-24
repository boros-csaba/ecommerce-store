using elenora.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace elenora.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly DataContext context;
        private string imageRootPath;

        private const float whiteSpaceCorrection = 1.02f;
        private const int knotCoverComponentId = 115;

        public ProductImageService(DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public byte[] GetCustomBraceletImage(int cartItemId, int customerId, string imageRootPath, int imageSize)
        {
            this.imageRootPath = imageRootPath;
            var customBraceletCartItem = context.CustomBraceletCartItems
                .Include(c => c.BeadType).ThenInclude(c => c.ComponentImages)
                .Include(c => c.SecondaryBeadType).ThenInclude(c => c.ComponentImages)
                .Include(c => c.Components).ThenInclude(c => c.Component).ThenInclude(c => c.ComponentImages)
                .Where(c => c.Id == cartItemId && c.Cart.CustomerId == customerId)
                .FirstOrDefault();
            if (customBraceletCartItem == null) return Array.Empty<byte>();

            var components = customBraceletCartItem.Components.OrderBy(c => c.Position).Select(c => c.Component).ToList();
            return GenerateCustomBraceletImage(customBraceletCartItem.BraceletSize.Value, customBraceletCartItem.StyleType, components, customBraceletCartItem.BeadType, customBraceletCartItem.SecondaryBeadType, imageSize);
        }

        public byte[] GetCustomOrderedBraceletImage(int orderItemId, int customerId, string imageRootPath, int imageSize)
        {
            this.imageRootPath = imageRootPath;
            var customBraceletCartItem = context.CustomBraceletOrderItems
                .Include(o => o.BeadType)
                .Include(o => o.SecondaryBeadType)
                .Include(o => o.Components).ThenInclude(c => c.Component)
                .Where(o => o.Id == orderItemId && o.Order.CustomerId == customerId)
                .FirstOrDefault();
            if (customBraceletCartItem == null) return Array.Empty<byte>();

            var components = customBraceletCartItem.Components.OrderBy(c => c.Position).Select(c => c.Component).ToList();
            return GenerateCustomBraceletImage(customBraceletCartItem.BraceletSize.Value, customBraceletCartItem.StyleType, components, customBraceletCartItem.BeadType, customBraceletCartItem.SecondaryBeadType, imageSize);
        }

        public byte[] GetCustomBraceletPreviewImage(BraceletSizeEnum braceletSize, CustomBraceletStyleEnum style, int beadTypeId, int secondaryBeadTypeId, int[] componentIds, string imageRootPath, int imageSize)
        {
            try
            {
                this.imageRootPath = imageRootPath;
                var beadType = GetComponentById(beadTypeId);
                var secondaryBeadType = GetComponentById(secondaryBeadTypeId);
                var allComponents = GetComponents(componentIds);
                var components = componentIds.Select(i => allComponents.First(c => c.Id == i)).ToList();
                return GenerateCustomBraceletImage(braceletSize, style, components, beadType, secondaryBeadType, imageSize);
            }
            catch (Exception e)
            {
                throw new Exception($"beadTypeId: {beadTypeId}, secondaryBeadTypeId: {secondaryBeadTypeId}, componentIds: {string.Join(",", componentIds)}", e);
            }
        }

        private byte[] GenerateCustomBraceletImage(BraceletSizeEnum braceletSize, CustomBraceletStyleEnum style, List<Component> components, Component beadType, Component secondaryBeadType, int imageSize)
        {
            float totalLength = 0;
            var componentsToDraw = new List<Component>();
            foreach (var component in components)
            {
                componentsToDraw.Add(component);
                totalLength += component.WidthRatio;
            }

            beadType.WidthRatio = 1;
            beadType.HeightRatio = 1;
            switch (style)
            {
                case CustomBraceletStyleEnum.Simple:
                    while (totalLength + 2 <= GetExpectedBraceletLength(braceletSize))
                    {
                        componentsToDraw.Insert(0, beadType);
                        componentsToDraw.Add(beadType);
                        totalLength += 2;
                    }
                    break;
                case CustomBraceletStyleEnum.Alternating:
                    bool oddBead = true;
                    while (totalLength + 2 <= GetExpectedBraceletLength(braceletSize))
                    {
                        if (oddBead)
                        {
                            componentsToDraw.Insert(0, beadType);
                            componentsToDraw.Add(beadType);
                        }
                        else
                        {
                            componentsToDraw.Insert(0, secondaryBeadType);
                            componentsToDraw.Add(secondaryBeadType);
                        }
                        oddBead = !oddBead;
                        totalLength += 2;
                    }
                    break;
                case CustomBraceletStyleEnum.DoubleAlternating:
                    int alternation = 0;
                    while (totalLength + 2 <= GetExpectedBraceletLength(braceletSize))
                    {
                        if (alternation < 2)
                        {
                            componentsToDraw.Insert(0, beadType);
                            componentsToDraw.Add(beadType);
                        }
                        else
                        {
                            componentsToDraw.Insert(0, secondaryBeadType);
                            componentsToDraw.Add(secondaryBeadType);
                        }
                        alternation++;
                        if (alternation > 3) alternation = 0;
                        totalLength += 2;
                    }
                    break;
                case CustomBraceletStyleEnum.TwoHalves:
                    while (totalLength + 2 <= GetExpectedBraceletLength(braceletSize))
                    {
                        componentsToDraw.Insert(0, beadType);
                        componentsToDraw.Add(secondaryBeadType);
                        totalLength += 2;
                    }
                    break;
            }
            
            var knotCoverComponent = GetComponentById(knotCoverComponentId);
            componentsToDraw.Insert(0, knotCoverComponent);

            return DrawBracelet(componentsToDraw, imageSize);
        }

        private byte[] DrawBracelet(List<Component> components, int imageSize)
        {
            int R = (int)(imageSize / 2 * 0.8);

            var nC = components.Sum(c => c.WidthRatio);
            var nR = nC / (2 * Math.PI);
            var scale = R / nR;

            var bitmap = new Bitmap(imageSize, imageSize, PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                double angle = 180;
                var center = new PointF(imageSize / 2, imageSize / 2);
                var position = new PointF(center.X, center.Y - R);

                Component prevComponent = null;
                foreach (var component in components)
                {
                    if (prevComponent != null)
                    {
                        angle += (double)360 * (component.WidthRatio / 2 + prevComponent.WidthRatio / 2) / nC;
                        position.X = (float)Math.Sin(angle * Math.PI / 180) * R + center.X;
                        position.Y = (float)Math.Cos(angle * Math.PI / 180) * R + center.Y;
                    }
                    DrawComponent(g, component, position, scale, angle, components);
                    prevComponent = component;
                }
            }
            using MemoryStream stream = new MemoryStream();
            var jpgEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            bitmap.Save(stream, jpgEncoder, encoderParameters);
            return stream.ToArray();
        }

        private void DrawComponent(Graphics g, Component component, PointF position, double scale, double angle, List<Component> allComponents)
        {
            var componentImage = GetComponentImage(component, allComponents, scale);
            var componentWidth = component.WidthRatio * scale * whiteSpaceCorrection;
            var componentHeight = component.HeightRatio * componentWidth;

            g.TranslateTransform(position.X, position.Y);
            g.RotateTransform(-(float)angle);
            g.DrawImage(componentImage,
                        (int)(-componentWidth / 2),
                        (int)(-componentHeight / 2),
                        (int)componentWidth,
                        (int)componentHeight);
            g.RotateTransform((float)angle);
            g.TranslateTransform(-position.X, -position.Y);
        }

        private Random random = new Random();
        private Bitmap GetComponentImage(Component component, List<Component> allComponents, double size)
        {
            string imagePath;
            if (component.ComponentImages != null && component.ComponentImages.Any())
            {
                imagePath = string.Empty;
                var occurance = allComponents.Count(c => c.Id == component.Id);
                ComponentImage componentImage = null;
                if (occurance == 1)
                {
                    componentImage = component.ComponentImages.OrderBy(i => i.Frequency).First();
                }
                else
                {
                    var totalWeights = component.ComponentImages.Sum(i => i.Frequency);
                    var randomWeight = random.Next(0, totalWeights);
                    var cumulatedWeight = 0;
                    var selectedIndex = 0;
                    var componentImages = component.ComponentImages.OrderBy(i => i.Frequency).ToList();
                    while (cumulatedWeight <= randomWeight)
                    {
                        componentImage = componentImages[selectedIndex++];
                        cumulatedWeight += componentImage.Frequency;
                    }
                }
                var sizeString = "1024";
                if (size <= 256) sizeString = "256";
                if (size <= 96) sizeString = "96";
                imagePath = $"{imageRootPath}/images/components/{component.IdString}/{component.IdString}-{componentImage.VariationNr}-{sizeString}.png";
            }
            else
            {
                imagePath = imageRootPath + component.ImageUrl.Replace(".webp", ".png");
            }
            imagePath = imagePath.Replace('/', Path.DirectorySeparatorChar)
                            .Replace('\\', Path.DirectorySeparatorChar)
                            .Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", @"{Path.DirectorySeparatorChar}");
            return new Bitmap(imagePath);
        }

        private int GetExpectedBraceletLength(BraceletSizeEnum braceletSize)
        {
            var expectedTotalLength = 22;
            switch (braceletSize)
            {
                case BraceletSizeEnum.XS: expectedTotalLength = 18; break;
                case BraceletSizeEnum.S: expectedTotalLength = 20; break;
                case BraceletSizeEnum.M: expectedTotalLength = 22; break;
                case BraceletSizeEnum.L: expectedTotalLength = 24; break;
                case BraceletSizeEnum.XL: expectedTotalLength = 26; break;
                case BraceletSizeEnum.XXL: expectedTotalLength = 28; break;
            }
            return expectedTotalLength;
        }

        private Component GetComponentById(int componentId)
        {
            return context.Components
                .Include(c => c.ComponentImages)
                .Single(c => c.Id == componentId);
        }

        private List<Component> GetComponents(int[] componentIds)
        {
            return context.Components
                .Include(c => c.ComponentImages)
                .Where(c => componentIds.Contains(c.Id)).ToList();
        }
    }
}
