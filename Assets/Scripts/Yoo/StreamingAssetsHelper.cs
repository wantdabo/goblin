using System.IO;
using UnityEngine;

namespace Yoo
{
    public class StreamingAssetsDefine
    {
        /// <summary>
        /// 根目录名称（保持和YooAssets资源系统一致）
        /// </summary>
        public const string RootFolderName = "yoo";
    }

#if UNITY_EDITOR
    public sealed class StreamingAssetsHelper
    {
        public static void Init() { }
        public static bool FileExists(string packageName, string fileName, string fileCRC)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, StreamingAssetsDefine.RootFolderName, packageName, fileName);

            return File.Exists(filePath);
        }
    }
#else
    public sealed class StreamingAssetsHelper
    {
        private class PackageQuery
        {
            public readonly Dictionary<string, BuildinFileManifest.Element> Elements = new Dictionary<string, BuildinFileManifest.Element>(1000);
        }

        private static bool _isInit = false;
        private static readonly Dictionary<string, PackageQuery> _packages = new Dictionary<string, PackageQuery>(10);

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (_isInit == false)
            {
                _isInit = true;

                var manifest = Resources.Load<BuildinFileManifest>("BuildinFileManifest");
                if (manifest != null)
                {
                    foreach (var element in manifest.BuildinFiles)
                    {
                        if (_packages.TryGetValue(element.PackageName, out PackageQuery package) == false)
                        {
                            package = new PackageQuery();
                            _packages.Add(element.PackageName, package);
                        }
                        package.Elements.Add(element.FileName, element);
                    }
                }
            }
        }

        /// <summary>
        /// 内置文件查询方法
        /// </summary>
        public static bool FileExists(string packageName, string fileName, string fileCRC32)
        {
            if (_isInit == false)
                Init();

            if (_packages.TryGetValue(packageName, out PackageQuery package) == false)
                return false;

            if (package.Elements.TryGetValue(fileName, out var element) == false)
                return false;

            return true;
        }
    }
#endif
}
