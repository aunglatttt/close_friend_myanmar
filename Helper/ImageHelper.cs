using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CloseFriendMyanamr.Helper
{
    public class ImageHelper
    {
        public static async Task ReduceImageSizeAsync(IFormFile file, int maxWidth, int maxHeight, string saveFolder, string fileName)
        {
            using (var imageStream = file.OpenReadStream())
            {
                // Load the image from the stream
                using (var originalImage = Image.FromStream(imageStream))
                {
                    // Fix image orientation based on EXIF metadata
                    var correctedImage = FixOrientation(originalImage);

                    // Calculate new dimensions while maintaining aspect ratio
                    var (newWidth, newHeight) = CalculateDimensions(correctedImage.Width, correctedImage.Height, maxWidth, maxHeight);

                    // Create a new bitmap with the calculated dimensions
                    using (var resizedImage = new Bitmap(newWidth, newHeight))
                    {
                        using (var graphics = Graphics.FromImage(resizedImage))
                        {
                            // Set the quality of the resized image
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;

                            // Draw the resized image
                            graphics.DrawImage(correctedImage, 0, 0, newWidth, newHeight);
                        }

                        // Save the resized image with compression
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 87L); // Adjust quality (0-100)

                        var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                        var filePath = Path.Combine(saveFolder, fileName);

                        // Save the image asynchronously
                        await Task.Run(() => resizedImage.Save(filePath, jpegEncoder, encoderParameters));
                    }
                }
            }
        }

        private static Image FixOrientation(Image image)
        {
            // Check if the image has the EXIF orientation property (274)
            if (!image.PropertyIdList.Contains(274))
                return image;

            var orientationProperty = image.GetPropertyItem(274);
            var orientationValue = orientationProperty.Value[0];

            // Rotate or flip based on orientation value
            switch (orientationValue)
            {
                case 2:
                    image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;
                case 5:
                    image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                default:
                    // No rotation needed for orientation value 1
                    break;
            }

            // Remove the EXIF orientation tag
            image.RemovePropertyItem(274);

            // Clone the image to force the rotation to commit
            var rotatedImage = new Bitmap(image);
            image.Dispose();
            return rotatedImage;
        }


        private static (int, int) CalculateDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / originalWidth;
            double ratioY = (double)maxHeight / originalHeight;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            return (newWidth, newHeight);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
