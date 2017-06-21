using System;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SateonPhotos
{
    class Program
    {
        static void Main(string[] args)
        {


            ProcessPhotos();


        }

        private static void ProcessPhotos()
        {

            //todo:   implement checks on connection strings before we do any work
            string sourceConnection = ConfigurationManager.ConnectionStrings["PhotoSource"].ConnectionString;
            string destinationConnection = ConfigurationManager.ConnectionStrings["PhotoDestination"].ConnectionString;


            //var load =  LoadTestImages();

            //todo: get all images without processed version 
            //todo: save original to folder with employee name . extension 
            //todo: resize image and save to file system and db table

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PhotoSource"].ConnectionString))
            using (var command  = new SqlCommand("sproc_FindUnprocessedImages",connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    Image img = null;
                    string empno = String.Empty;
                    String dataFormat = string.Empty;
                    img = DbHelpers.GetImageOrDefault(rdr, "Image");
                    empno = DbHelpers.GetStringOrDefault(rdr, "employeeNo");
                    string ext = new ImageFormatConverter().ConvertToString(img.RawFormat)?.ToLower();

                    var x = ImageHandler.SaveImageAsJpeg(img, ConfigurationManager.AppSettings["original"] + empno) ;
                    ImageHandler.ResizeAndSavetoDisc( new Bitmap(img), 200, 200, 80,
                        ConfigurationManager.AppSettings["processed"] + empno);

                    Console.WriteLine($"{empno} - {img.Size} {ext}");      
                    
                  }


            }





                var result = GetFileFromDb("0970E4CA11AEEF8AACEEB60FEF33A429BC9BC44E26B0294B5F", @"C:\testImages\Originals\test.jpg");

            Console.WriteLine(result.ToString());
#if DEBUG
            Console.Read();
#endif
            //throw new NotImplementedException();
        }


        public static bool GetFileFromDb(string varId, string varPathToNewLocation)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PhotoSource"].ConnectionString))
            using (var sqlQuery = new SqlCommand(@"SELECT top 1 employeeNo,image,dataformat FROM [dbo].[OriginalPhotos] WHERE [employeeNo] = @varID", connection))
            {
                sqlQuery.Parameters.AddWithValue("@varID", varId);
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


                            using (var fs = new FileStream(varPathToNewLocation + "." + dataFormat.ToString(), FileMode.Create, FileAccess.Write))
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

        static bool LoadTestImages()
        {

            var srcFiles = ImageInfo.GetFiles(ConfigurationManager.AppSettings["testfilepath"]).Where(x => x.Jpeg);

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PhotoSource"]
                .ConnectionString))
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