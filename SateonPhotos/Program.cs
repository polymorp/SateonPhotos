using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;

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


            LoadTestImages();





            throw new NotImplementedException();
        }


        static void LoadTestImages()
        {

            var SrcFiles = ImageInfo.GetFiles(@"c:\testimages\").Where(x => x.Jpeg);


            //throw new NotImplementedException();
        }

       

    }
}
