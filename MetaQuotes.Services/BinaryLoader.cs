using System;
using MetaQuotes.Models;
using Castle.Core.Internal;
using System.IO;
using MetaQuotes.Services.Exceptions;

namespace MetaQuotes.Services
{
    public class BinaryLoader : IBinaryLoader
    {
        public GeoBase LoadDb(string filePath)
        {
            if (filePath.IsNullOrEmpty())
                throw new Exception("Не указан путь к базе");

            if (!File.Exists(filePath))
                throw new Exception("Файла с базой не существует по указанному адресу");

            var db = ReadBinaryFile(filePath);

            return db;
        }

        private GeoBase ReadBinaryFile(string filePath)
        {
            var db = new GeoBase();
            using (BinaryReader br = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                db.Header = br.ReadStruct<Header>();

                if (db.Header.Equals(default(Header)))
                {
                    throw new ConvertBinaryToStructException("Не удалось сконвертировать структуру Header");
                }

                db.Ranges = new IPRange[db.Header.records];
                for (int i = 0; i < db.Header.records; i++)
                {
                    var range = br.ReadStruct<IPRange>();
                    db.Ranges[i] = range;
                }

                br.BaseStream.Seek(db.Header.offset_cities, SeekOrigin.Begin);
                db.Cities = new City[db.Header.records];
                for (int i = 0; i < db.Header.records; i++)
                {
                    var city = br.ReadStruct<City>();
                    db.Cities[i] = city;
                }

                br.BaseStream.Seek(db.Header.offset_locations, SeekOrigin.Begin);
                db.Locations = new Location[db.Header.records];
                for (int i = 0; i < db.Header.records; i++)
                {
                    var location = br.ReadStruct<Location>();
                    db.Locations[i] = location;
                }
            }

            return db;
        }
    }

}
