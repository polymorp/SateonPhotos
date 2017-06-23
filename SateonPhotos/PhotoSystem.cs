using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SateonPhotos
{
    class PhotoSystem
    {
        private AppSettingData appSetting;

        public PhotoSystem()
        {
            appSetting = new AppSettingData
            {
                SourceConnString = ConfigurationManager.ConnectionStrings["PhotoSource"].ConnectionString,
                DestConnnString = ConfigurationManager.ConnectionStrings["PhotoDestination"].ConnectionString,
                ResizedImageFilesystemFolder = ConfigurationManager.AppSettings["processed"],
                SourceImageFilesystemFolder = ConfigurationManager.AppSettings["original"],
                TestImagesFolder = ConfigurationManager.AppSettings["testfilepath"]
            };
        }

        public void ProcessPhotos()
        {
            //var load =  LoadTestImages();

            //todo: get all images without processed version 
            //todo: save original to folder with employee name . extension 
            //todo: resize image and save to file system and db table

            using (var connection = new SqlConnection(appSetting.SourceConnString))
            using (var command = new SqlCommand("sproc_FindUnprocessedImages", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    Image img;
                    string empno;
                    String dataFormat = string.Empty;
                    img = DbHelpers.GetImageOrDefault(rdr, "Image");
                    empno = DbHelpers.GetStringOrDefault(rdr, "identifier").Trim();
                    
                    string ext = new ImageFormatConverter().ConvertToString(img.RawFormat)?.ToLower();

                    var x = ImageHandler.SaveImageAsJpeg(img, appSetting.SourceImageFilesystemFolder + empno + ".jpg");
                    Bitmap resizedBitmap = ImageHandler.ResizeImage(new Bitmap(img), 200, 200, 80);

                    // Resize and save processed to disk
                    //ImageHandler.ResizeAndSavetoDisc( new Bitmap(img), 200, 200, 80, appSetting.ResizedImageFilesystemFolder + empno);
                    ImageHandler.SaveImageAsJpeg(resizedBitmap, appSetting.ResizedImageFilesystemFolder + empno + ".jpg");


                    ImageInfo resultInfo =
                        ImageInfo.readFileDetails(appSetting.ResizedImageFilesystemFolder + empno + ".jpg");


                    SaveFileToDb(resultInfo, "WriteImageToProcessed",empno);

                    Console.WriteLine($"{empno} - {img.Size} {ext}");
                }
            }


#if DEBUG
            //var result = GetFileFromDb("0970E4CA11AEEF8AACEEB60FEF33A429BC9BC44E26B0294B5F",
            //    @"C:\testImages\Originals\test.jpg");

            //Console.WriteLine(result.ToString());

            //Console.Read();
#endif
            //throw new NotImplementedException();
        }

        public bool GetFileFromDb(string identifer, string varPathToNewLocation)
        {
            using (var connection = new SqlConnection(appSetting.SourceConnString))
            using (var sqlQuery =
                new SqlCommand(
                    @"SELECT top 1 employeeNo,image,dataformat FROM [dbo].[OriginalPhotos] WHERE [employeeNo] = @identifer",
                    connection))
            {
                sqlQuery.Parameters.AddWithValue("@identifer", identifer);
                try
                {
                    connection.Open();
                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult.HasRows)
                        {
                            sqlQueryResult.Read();
                            var blob = new Byte[(sqlQueryResult.GetBytes(1, 0, null, 0, int.MaxValue))];
                            sqlQueryResult.GetBytes(1, 0, blob, 0, blob.Length);
                            var dataFormat = sqlQueryResult["dataFormat"];


                            using (var fs = new FileStream(varPathToNewLocation + "." + dataFormat.ToString(),
                                FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(blob, 0, blob.Length);
                            }


                            return true;
                        }
                        else
                        {
                            return false;
                        }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public bool SaveFileToDb(ImageInfo image, string sprocName, string identifier)
        {
            // sanity check inputs


            try
            {
                if (image == null ||  sprocName == null)
                {
                    throw new System.ArgumentException("SaveFileToDb Parameters cannot be null");
                }

                using (var connection = new SqlConnection(appSetting.SourceConnString))
                {
                    using (var command = new SqlCommand(sprocName, connection))
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();

                        var filePath = image.FilePath;
                        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] file2;
                            using (var reader = new BinaryReader(stream))
                            {
                                file2 = reader.ReadBytes((int)stream.Length);
                            }

                            command.Parameters.Clear();

                            command.Parameters.AddWithValue("@employeeNo", identifier);
                            command.Parameters.AddWithValue("@image", file2);
                            command.Parameters.AddWithValue("@height", image.Height);
                            command.Parameters.AddWithValue("@width", image.Width);
                            command.Parameters.AddWithValue("@dataformat", image.Format);
                           // command.Parameters.AddWithValue("@filesize", image.Size);
                            command.Parameters.AddWithValue("@dt", DateTime.UtcNow);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


                return true;
            }

            private bool LoadTestImages()
            {
                var srcFiles = ImageInfo.GetFiles(appSetting.TestImagesFolder).Where(x => x.Jpeg);

                using (var connection = new SqlConnection(appSetting.SourceConnString))
                {
                    using (var command = new SqlCommand("WriteImageToOriginal", connection))
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();

                        foreach (var image in srcFiles)
                        {
                            try
                            {
                                var filePath = image.FilePath;
                                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                {
                                    byte[] file2;
                                    using (var reader = new BinaryReader(stream))
                                    {
                                        file2 = reader.ReadBytes((int) stream.Length);
                                    }

                                    command.Parameters.Clear();

                                    command.Parameters.AddWithValue("@employeeNo", image.Hash);
                                    command.Parameters.AddWithValue("@image", file2);
                                    command.Parameters.AddWithValue("@height", image.Height);
                                    command.Parameters.AddWithValue("@width", image.Width);
                                    command.Parameters.AddWithValue("@dataformat", image.Format);
                                    command.Parameters.AddWithValue("@filename", image.FileName);

                                    command.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                return false;
                            }


                            //throw new NotImplementedException();
                        }
                    }
                }
                return true;
            }
        }
    }