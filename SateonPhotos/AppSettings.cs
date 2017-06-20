namespace SateonPhotos
{
    class AppSettingData
    {
        /// <summary>
        /// Main sql connection for image
        /// </summary>
        public string SourceConnString { get; set; }
        
        /// <summary>
        /// Destination sql location for processed/resized images
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

