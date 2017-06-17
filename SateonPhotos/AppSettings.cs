using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SateonPhotos
{
    class AppSettingData
    {
        /// <summary>
        /// Main sql connection for image
        /// </summary>
        public string SourceConnString { get; set; }
        
        /// <summary>
        /// Destination sql lcoation for processed/resized images
        /// </summary>
        public string DestConnnString { get; set; }

        /// <summary>
        /// Folder for temp storage of original size images
        /// </summary>
        public string SourceImageFilesystemFolder { get; set; }

        public string ResizedImageFilesystemFolder { get; set; }

        public string TestImagesFolder { get; set; }    
    }
}

