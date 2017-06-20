using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Encoder = System.Drawing.Imaging.Encoder;
// ReSharper disable RedundantCast

namespace SateonPhotos
{
    using System;
    using System.Linq;

    /// <summary>
    /// Class contaning method to resize an image and save in JPEG format
    /// </summary>
    public class ImageHandler
    {
        /// <summary>
        /// Method to resize, convert and save the image.
        /// </summary>
        /// <param name="image">Bitmap image.</param>
        /// <param name="maxWidth">resize width.</param>
        /// <param name="maxHeight">resize height.</param>
        /// <param name="quality">quality setting value.</param>
        /// <param name="filePath">file path.</param>      
        public void ResizeAndSavetoDisc(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            //// Get the image's original width and height
            //int originalWidth = image.Width;
            //int originalHeight = image.Height;

            //// To preserve the aspect ratio
            //float ratioX = (float)maxWidth / (float)originalWidth;
            //float ratioY = (float)maxHeight / (float)originalHeight;
            //float ratio = Math.Min(ratioX, ratioY);

            //// New width and height based on aspect ratio
            //int newWidth = (int)(originalWidth * ratio);
            //int newHeight = (int)(originalHeight * ratio);

            //// Convert other formats (including CMYK) to RGB.
            //Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            //// Draws the image in the specified size with quality mode set to HighQuality
            //using (Graphics graphics = Graphics.FromImage(newImage))
            //{
            //    graphics.CompositingQuality = CompositingQuality.HighQuality;
            //    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    graphics.SmoothingMode = SmoothingMode.HighQuality;
            //    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            //}

            try
            {
                Bitmap newImage = ResizeImage(image, maxWidth, maxHeight, quality);
                var x = SaveImageAsJpeg(newImage, filePath, quality);

                //// Get an ImageCodecInfo object that represents the JPEG codec.
                //ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

                //// Create an Encoder object for the Quality parameter.
                //Encoder encoder = Encoder.Quality;

                //// Create an EncoderParameters object. 
                //EncoderParameters encoderParameters = new EncoderParameters(1);

                //// Save the image as a JPEG file with quality level.
                //EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
                //encoderParameters.Param[0] = encoderParameter;
                //newImage.Save(filePath, imageCodecInfo, encoderParameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }


        public static bool SaveImageAsJpeg(Image img, string filePath,int quality = 80)
        {
            try
            {
                Image newImage = img;
          
                // Get an ImageCodecInfo object that represents the JPEG codec.
                ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

                // Create an Encoder object for the Quality parameter.
                Encoder encoder = Encoder.Quality;

                // Create an EncoderParameters object. 
                EncoderParameters encoderParameters = new EncoderParameters(1);

                // Save the image as a JPEG file with quality level.
                EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
                encoderParameters.Param[0] = encoderParameter;
                newImage.Save((filePath.Contains(".jpg") ? filePath : filePath + ".jpg"), imageCodecInfo, encoderParameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return true;
        }


        /// <summary>
        /// Method to resize, convert and return the image.
        /// </summary>
        /// <param name="image">Bitmap image.</param>
        /// <param name="maxWidth">resize width.</param>
        /// <param name="maxHeight">resize height.</param>
        /// <param name="quality">quality setting value.</param>
        /// <returns>Bitmap</returns>
        public Bitmap ResizeImage(Bitmap image, int maxWidth, int maxHeight, int quality)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        /// <summary>
        /// Method to get encoder information for given image format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>image codec info.</returns>
        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}