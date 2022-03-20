using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NetCoreRedis.Web.Models;

namespace NetCoreRedis.Web.Core.FakeData
{
    public static class FakeDataGenerator
    {
        public static List<Personal> GetPersonals()
        {
            var jsonFilePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\fake-data.json";
            var jsonString = File.ReadAllText(jsonFilePath);
            var personalList = JsonSerializer.Deserialize<List<Personal>>(jsonString).Take(100).ToList();
            return personalList;
        }

        public static string GetPersonalFullName(int id)
        {
            var personal = GetPersonals().FirstOrDefault(x => x.Id == id);
            var fullName = $"{personal?.FirstName} {personal?.LastName}";
            return fullName;
        }

        public static byte[] GetFileByteArray()
        {
            var filePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\redis.png";
            var fileByteArray = File.ReadAllBytes(filePath);
            return fileByteArray;
        }
    }
}
