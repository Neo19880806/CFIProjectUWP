using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace CFIProjectUWP.Model
{
    public class FileHelper
    {
        public static async Task saveObjectToXML<T>(String fileName,T saveObject)
        {
            var serializer = new XmlSerializer(typeof(T));
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenStreamForWriteAsync();
            using (stream)
            {
                serializer.Serialize(stream, saveObject);
            }
        }

        public static async Task<T> readObjectFromXML<T>(String fileName)
        {
            T objectFromXML = default(T);
            var serializer = new XmlSerializer(typeof(T));

            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync(fileName);
            var stream = await file.OpenStreamForReadAsync();
            using (stream)
            {
                objectFromXML = (T)serializer.Deserialize(stream);
            }
            return objectFromXML;
        }

        public static async Task<bool> IsFileExist(String fileName)
        {
            bool fileExists = false;
            var allfiles = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var storageFile in allfiles)
            {
                if (storageFile.Name == fileName)
                {
                    fileExists = true;
                }
            }
            return fileExists;
        }
    }
}
