using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;
// ReSharper disable NonReadonlyMemberInGetHashCode



namespace SateonPhotos
{
    public class ImageInfo : IEquatable<ImageInfo>
    {
        private string _filePath;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                try
                {
                    Size = (new FileInfo(_filePath)).Length;
                }
                catch (Exception)
                {
                    // cannot get size but otherwise ignore
                }
            }
        }

        public string FileName
        {
            get
            {
                return Path.GetFileName(_filePath);
            }
        }

        public long Size { get; set; }
        public bool Jpeg { get; set; } = true;
        public bool Error { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Hash { get; set; }

        public string format { get; set; }

        public new string ToString
        {
            get
            {
                var filename = Path.GetFileName(FilePath);

                var fileFormatted =
                    filename != null && filename.Length > 50 ?
                        filename.Substring(0, 23) + "...." + filename.Substring(filename.Length - 23) :
                        filename?.PadLeft(50);

                string info;

                if (!Jpeg)
                    info = "(not a JPEG)";
                else if (Error)
                    info = "Error reading file!";
                else
                    info = $"{Width + "x" + Height,10} {Size,10} bytes";

                return $"{fileFormatted}: {info,27}";
            }
        }

        public static List<ImageInfo> GetFiles(String path)
        {
            var files = new List<ImageInfo>();

            foreach (var file in Directory.EnumerateFiles(path))
            {
                //JpegBitmapDecoder jpeg;

                try
                {
                    
                    var filePath = Path.Combine(path, file);
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        Image img = null;
                        img = Image.FromStream(stream);
                        string ext = new ImageFormatConverter().ConvertToString(img.RawFormat).ToLower();


                        if (ext.ToLower() == "jpeg" || ext.ToLower() == "bmp")
                        {

                            files.Add(new ImageInfo()
                            {
                                FilePath = file,
                                Width = img.Width,
                                Height = img.Height,
                                format = ext,
                                Hash = GetChecksum(filePath)
                            });

                        }
                        //jpeg = new JpegBitmapDecoder(
                        //    stream,
                        //    BitmapCreateOptions.None,
                        //    BitmapCacheOption.None
                        //);

                        //var frame = jpeg.Frames[0];
                        //files.Add(new ImageInfo()
                        //{
                        //    FilePath = file,
                        //    Width = frame.PixelWidth,
                        //    Height = frame.PixelHeight,
                        //    format = ext,
                        //    Hash = GetChecksum(filePath)
                        //});


                    }

                }
                catch (FileFormatException)
                {
                    files.Add(new ImageInfo() { FilePath = file, Jpeg = false, format = "Unknown" , Error = true });
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("error: {0}", ex.Message);
#endif
                    files.Add(new ImageInfo() { FilePath = file, Error = true });
                }
            }

            return files;
        }

        public static bool Similar(ImageInfo a, ImageInfo b)
        {
            return (
                a.Jpeg == b.Jpeg &&
                a.Size == b.Size &&
                a.Width == b.Width &&
                a.Height == b.Height &&
                a.Hash == b.Hash
            );
        }

        public bool Similar(ImageInfo compare)
        {
            return Similar(this, compare);
        }

        private static string GetChecksum(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }


        public bool Equals(ImageInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Size == other.Size && Width == other.Width && Height == other.Height && string.Equals(Hash, other.Hash);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ImageInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Size.GetHashCode();
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                hashCode = (hashCode * 397) ^ (Hash != null ? Hash.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ImageInfo left, ImageInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ImageInfo left, ImageInfo right)
        {
            return !Equals(left, right);
        }
    }
}
