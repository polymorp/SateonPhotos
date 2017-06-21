using System.Drawing;

namespace SateonPhotos
{
    using System;
    using System.Data.SqlClient;
    
    /// <summary>
    /// Database helpers to help the repository layer with common exceptions and code blocks.
    /// </summary>
    /// <remarks>
    /// <list type="table" name="history">
    /// <listheader><term>Revision Details</term><description>Description</description></listheader>
    /// </list>
    /// </remarks>
    internal static class DbHelpers
    {
        /// <summary>
        /// Gets string from the database converting nulls into empty strings.
        /// </summary>
        /// <param name="reader">The sql reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>An empty string or the requested string</returns>
        public static string GetStringOrDefault(SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets integers from the database converting nulls into zeros.
        /// </summary>
        /// <param name="reader">The sql reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Zero or the requested int</returns>
        public static int GetIntOrDefault(SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName))
                ? 0
                : reader.GetInt32(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets dates from the database converting nulls into default dates.
        /// </summary>
        /// <param name="reader">The sql reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Default date or the requested date</returns>
        public static DateTime GetDateTimeOrDefault(SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName))
                ? new DateTime()
                : reader.GetDateTime(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets dates from the database converting nulls into default dates.
        /// </summary>
        /// <param name="reader">The sql reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Default date or the requested date</returns>
        public static bool GetBoolOrDefault(SqlDataReader reader, string columnName)
        {
            return !reader.IsDBNull(reader.GetOrdinal(columnName)) && (reader.GetInt32(reader.GetOrdinal(columnName)) == 1);
        }

        /// <summary>
        /// Gets dates from the database converting nulls into default guids.
        /// </summary>
        /// <param name="reader">The sql reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Default date or the requested date</returns>
        public static Guid GetGuidOrDefault(SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName))
                ? Guid.Empty
                : reader.GetGuid(reader.GetOrdinal(columnName));
        }

        public static Image GetImageOrDefault(SqlDataReader reader, string columnName)
        {

          
            if ( reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                return null;
            }
            else
            {
                byte[] picData = reader[columnName] as byte[];
                return  ImageHandler.ByteArrayToImage(picData);
            }
            
        }
        
    }
}
