using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager.Models
{
    internal class JsonController<T>
    {
        static public ObservableCollection<T> GetInfo(string fileName)
        {
            ObservableCollection<T> collection = new ObservableCollection<T>();
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Length != 0)
                {
                    try
                    {
                        collection = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<T>>(fs);
                    }
                    catch
                    {
                        MessageBox.Show("Error occured while reading the data!");
                    }
                }
            }
            return collection;
        }

        static public async void LoadInfoAsync<T>(ObservableCollection<T> collection, string fileName)
        {
            string json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, json);
        }
    }
}
