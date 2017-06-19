using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
            string SourceConnection = ConfigurationManager.ConnectionStrings["PhotoSource"].ConnectionString;
            string DestinationConnection = ConfigurationManager.ConnectionStrings["PhotoDestination"].ConnectionString;


            var load =  LoadTestImages();




            var result = GetFileFromDB("9AD9FAD8E4C5690F46EEA8195F9A325AF211E039B2C31C206B", @"C:\testImages\Originals\test.jpg");



            //throw new NotImplementedException();
        }


        public static bool GetFileFromDB(string varID, string varPathToNewLocation)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PhotoSource"].ConnectionString))
            using (var sqlQuery = new SqlCommand(@"SELECT image,dataformat FROM [dbo].[OriginalPhotos] WHERE [employeeNo] = @varID", connection))
            {
                sqlQuery.Parameters.AddWithValue("@varID", varID);
                connection.Open();
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult.HasRows)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        var dataFormat = sqlQueryResult["dataFormat"];
                        using (var fs = new FileStream(varPathToNewLocation + "." + dataFormat.ToString(), FileMode.Create, FileAccess.Write))
                            fs.Write(blob, 0, blob.Length);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
        }

        static bool LoadTestImages()
        {

            var SrcFiles = ImageInfo.GetFiles(ConfigurationManager.AppSettings["testfilepath"]).Where(x => x.Jpeg);

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PhotoSource"]
                .ConnectionString))
            {
                using (var command = new SqlCommand("WriteImageToOriginal", connection))
                {

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    foreach (var image in SrcFiles)
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
                                command.Parameters.AddWithValue("@dataformat", image.format);
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