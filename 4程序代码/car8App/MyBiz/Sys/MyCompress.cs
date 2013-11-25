//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ICSharpCode.SharpZipLib.Zip;
//using ICSharpCode.SharpZipLib.Checksums;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
//using System.IO;
//using MyCmn;

//namespace MyBiz.Sys
//{
//    public class MyCompress
//    {

//        /// <summary>
//        /// 用 Zip 方式打包文件夹.
//        /// </summary>
//        /// <param name="SrcPath"></param>
//        /// <param name="IgnoreFiles"></param>
//        /// <returns>返回false跳过打包,返回true继续打包.</returns>
//        public static byte[] CompressDirectory(string SrcPath, Func<FileInfo, bool> IgnoreFiles)
//        {
//            DirectoryInfo baseInfo = new DirectoryInfo(SrcPath);
//            var fInfo = baseInfo.GetFiles("*.*", SearchOption.AllDirectories);

//            Crc32 crc = new Crc32();
//            MemoryStream mm = new MemoryStream();

//            using (ZipOutputStream s = new ZipOutputStream(mm))
//            {
//                s.SetLevel(3);

//                fInfo.All(o =>
//                {
//                    if (IgnoreFiles != null)
//                    {
//                        if (IgnoreFiles(o) == false) return true;
//                    }

//                    var buffer = File.ReadAllBytes(o.FullName);

//                    //ZipEntry 文件名的格式是  A\B\C.txt
//                    ZipEntry entry = new ZipEntry(o.FullName.Replace(SrcPath, "").TrimStart('\\'));

//                    entry.DateTime = DateTime.Now;

//                    entry.Size = buffer.Length;
//                    crc.Reset();
//                    crc.Update(buffer);
//                    entry.Crc = crc.Value;
//                    s.PutNextEntry(entry);
//                    s.Write(buffer, 0, buffer.Length);
//                    return true;
//                });
//                s.Finish();
//                mm.Seek(0, SeekOrigin.Begin);
//                byte[] data = new byte[mm.Length];
//                mm.Read(data, 0, data.Length);
//                mm.Close();
//                return data;
//            }

//        }




//        /// <summary>
//        ///    解压缩 
//        /// </summary>
//        /// <param name="FilePath">压缩文件路径</param>
//        /// <param name="OutputFolder">解压后的文件的地址</param>
//        public static void DeCompress(Stream FileContent, string OutputFolder)
//        {
//            GodError.Check(OutputFolder.HasValue() == false, "要求 目标文件夹. ");

//            using (ZipInputStream stream = new ZipInputStream(FileContent))
//            {
//                string folderName = OutputFolder;

//                //首先为该文件创建一个解压缩到的目录 
//                DirectoryInfo destFolder = new DirectoryInfo(folderName);
//                if (!destFolder.Exists)
//                    destFolder.Create();

//                ZipEntry ze = null;
//                while ((ze = stream.GetNextEntry()) != null)
//                {
//                    //// 创建路径
//                    //string[] s = ze.Name.Split(Path.DirectorySeparatorChar);
//                    //if (s.Length > 1)
//                    //{
//                    //    Directory.CreateDirectory(folderName + Path.DirectorySeparatorChar + Path.GetDirectoryName(ze.Name));
//                    //}

//                    //要创建的文件
//                    string outfile = folderName + Path.DirectorySeparatorChar + ze.Name;

//                    var fi = new FileInfo(outfile);
//                    if (fi.Exists == false && fi.Directory.Exists == false )
//                    {
//                        fi.Directory.Create();
//                    }

//                    var data = new byte[ze.Size];
//                    stream.Read(data, 0, data.Length);
//                    File.WriteAllBytes(outfile, data);
//                }
//            }
//        }

//        /// <summary>
//        /// 解压到当前路径
//        /// </summary>
//        /// <param name="FilePath"></param>
//        public static void DeCompress(string FilePath, string OutputFolder)
//        {
//            GodError.Check(FilePath.HasValue() == false, "文件名不能为空.");

//            GodError.Check(File.Exists(FilePath) == false, "文件不存在.");

//            DeCompress(new FileStream(FilePath, FileMode.Open), OutputFolder);
//        }
//    }
//}
