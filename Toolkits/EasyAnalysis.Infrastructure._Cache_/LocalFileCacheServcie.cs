using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SQLite;
using System.Diagnostics;

namespace EasyAnalysis.Infrastructure.Cache
{
    public class CacheIndex
    {
        public string Url  { get; set; }

        public string Path { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ExpiredOn { get; set; }

        public string Hash { get; set; }
    }

    public class LocalFileCacheServcie : ICacheService
    {
        private bool _isInitialized = false;

        private string _rootFolder;

        private string _sqliteFilePath;

        public ICacheClient CreateClient()
        {
            Initialize();

            return new LocalFileCacheClient(this);
        }

        public void Configure(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        public bool IsCached(Uri uri)
        {
            var hash = Utils.ComputeStringMD5Hash(uri.AbsoluteUri, Encoding.UTF8);

            var selectByHashSql = SqlLibrary.Instance.Require("SELECT_INDEX_BY_HASH");

            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", _sqliteFilePath)))
            {
                var cacheIndex = connection
                                    .Query<CacheIndex>(selectByHashSql, new { @Hash = hash })
                                    .FirstOrDefault();

                if(cacheIndex == null)
                {
                    return false;
                }

                var cacheFilePath = GetCacheFilePhysicalPath(cacheIndex.Path);

                return File.Exists(cacheFilePath);
            }
        }

        public Stream GetCache(Uri uri)
        {
            // compute hash
            var hash = Utils.ComputeStringMD5Hash(uri.AbsoluteUri, Encoding.UTF8);

            var selectByHashSql = SqlLibrary.Instance.Require("SELECT_INDEX_BY_HASH");

            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", _sqliteFilePath)))
            {
                try
                {
                    var cacheIndex = connection
                        .Query<CacheIndex>(selectByHashSql, new { @Hash = hash })
                        .FirstOrDefault();

                    if(cacheIndex == null)
                    {
                        return null;
                    }

                    var cacheFilePath = GetCacheFilePhysicalPath(cacheIndex.Path);

                    if(!File.Exists(cacheFilePath))
                    {
                        return null;
                    }

                    var mem = new MemoryStream();

                    using (var fs = new FileStream(cacheFilePath, FileMode.Open))
                    {
                        fs.CopyTo(mem);

                        mem.Flush();

                        mem.Position = 0;
                    }

                    return mem;
                } catch (Exception ex) {
                    // TODO: log the excpetion here

                    return null;
                }
            }
        }

        public void CacheOrUpdate(Uri uri, Stream content)
        {
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", _sqliteFilePath)))
            {
                try
                {
                    var hash = Utils.ComputeStringMD5Hash(uri.AbsoluteUri, Encoding.UTF8);

                    var selectByHashSql = SqlLibrary.Instance.Require("SELECT_INDEX_BY_HASH");

                    var cacheIndex = connection
                        .Query<CacheIndex>(selectByHashSql, new { @Hash = hash })
                        .FirstOrDefault();

                    if(cacheIndex != null)
                    {
                        var existingCacheFilePath = GetCacheFilePhysicalPath(cacheIndex.Path);

                        if(File.Exists(existingCacheFilePath))
                        {
                            File.Delete(existingCacheFilePath);
                        }

                        // delete history by hash
                        var deleteByHashSql = SqlLibrary.Instance.Require("DELETE_INDEX_BY_HASH");

                        connection.Execute(deleteByHashSql, new { Hash = hash });
                    }

                    var newCacheFilePath = GetCacheFilePhysicalPath(hash + ".cache");

                    if(File.Exists(newCacheFilePath))
                    {
                        File.Delete(newCacheFilePath);
                    }

                    using (var fs = new FileStream(newCacheFilePath, FileMode.CreateNew))
                    {
                        content.CopyTo(fs);

                        fs.Flush();
                    }

                    var insertIntoCacheSql = SqlLibrary.Instance.Require("INSERT_INTO_CACHE_INDEX");

                    connection.Execute(insertIntoCacheSql, new CacheIndex
                    {
                        Url = uri.AbsoluteUri,
                        Path = hash + ".cache",
                        CreatedOn = DateTime.Now,
                        ExpiredOn = DateTime.Now.AddHours(24),
                        Hash = hash
                    });
                }
                catch(Exception ex)
                {
                    // TODO: log the excpetion here

                    throw ex;
                }
            }
        }

        private string GetCacheFilePhysicalPath(string path)
        {
            return Path.Combine(_rootFolder, "_cache_", path);
        }

        private void Initialize()
        {
            if(_isInitialized)
            {
                return;
            }

            _sqliteFilePath = Path.Combine(_rootFolder, "index.sqlite3");

            if (!File.Exists(_sqliteFilePath))
            {
                SQLiteConnection.CreateFile(_sqliteFilePath);

                using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", _sqliteFilePath)))
                {
                    var createTableSql = SqlLibrary.Instance.Require("CREATE_INDEX_TABLE");

                    connection.Execute(createTableSql);
                }
            }

            var cacheFolder = Path.Combine(_rootFolder, "_cache_");

            if(!Directory.Exists(cacheFolder))
            {
                Directory.CreateDirectory(cacheFolder);
            }

            // set up SQLite local cache index
            _isInitialized = true;
        }
    }
}
