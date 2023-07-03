using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace CrossCutting
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileSystemLibrary
    {
        public static Tuple<Int32, String, Boolean> FileCreate(String path)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                FileInfo fl = new FileInfo(path);
                File.Create(path);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> FileDelete(String path)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                FileInfo fl = new FileInfo(path);
                File.Delete(path);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> FileCopyTo(String path1, String path2)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                FileInfo fi1 = new FileInfo(path1);
                FileInfo fi2 = new FileInfo(path2);
                fi1.CopyTo(path2);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> FileMoveTo(String path1, String path2)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                FileInfo fi1 = new FileInfo(path1);
                FileInfo fi2 = new FileInfo(path2);
                fi1.MoveTo(path2);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> ApendText(String path, String text)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                FileInfo fi = new FileInfo(path);
                StreamWriter sw = fi.AppendText();
                sw.WriteLine(text);
                sw.Close();
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, List<String>, Boolean> FileProperties(String path)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            String linha = String.Empty;
            List<String> lista = new List<String>();
            try
            {
                FileInfo fi = new FileInfo(path);
                linha = "Nome: " + fi.Name;
                lista.Add(linha);
                linha = "Nome Completo: " + fi.FullName;
                lista.Add(linha);
                linha = "Diretório: " + fi.Directory;
                lista.Add(linha);
                linha = "Data Criação: " + fi.CreationTime.ToString();
                lista.Add(linha);
                linha = "Ultimo Acesso: " + fi.LastAccessTime.ToString();
                lista.Add(linha);
                linha = "Tamanho: " + fi.Length.ToString();
                lista.Add(linha);
                linha = "Extensão: " + fi.Extension;
                lista.Add(linha);
                linha = "Última Gravação: " + fi.LastWriteTime.ToString();
                lista.Add(linha);
                linha = "ReadOnly: " + (fi.IsReadOnly == true ? "Sim":"Não");
                lista.Add(linha);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, lista, true);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> FileCheckExist(String path)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            bool exists = false;
            try
            {
                FileInfo fi = new FileInfo(path);
                exists = fi.Exists;
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 2;
            }
            var tupla = Tuple.Create(retorno, mensagem, exists);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> CreateFolder(String folder)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> DeleteFolder(String folder)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.Delete(folder);
                }
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> MoveFolder(String folder, String newFolder)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                if (!Directory.Exists(newFolder))
                {
                     Directory.Move(folder, newFolder);                
                }
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, String, Boolean> SetCreationTimeFile(String file)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            try
            {
                File.SetCreationTime(file, DateTime.Now);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, true);
            return tupla;
        }

        public static Tuple<Int32, String, DateTime> GetCreationTimeFile(String file)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            DateTime dt = DateTime.Now;
            try
            {
                dt = File.GetCreationTime(file);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, dt);
            return tupla;
        }

        public static Tuple<Int32, String, DateTime> GetLastAccessTimeFile(String file)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            DateTime dt = DateTime.Now;
            try
            {
                dt = File.GetLastAccessTime(file);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, dt);
            return tupla;
        }

        public static Tuple<Int32, String, DateTime> GetLastWriteTimeFile(String file)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            DateTime dt = DateTime.Now;
            try
            {
                dt = File.GetLastWriteTime(file);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, dt);
            return tupla;
        }

        public static Tuple<Int32, String, Int32, List<String>> EnumerateFolder(String root)
        {
            Int32 retorno = 0;
            String mensagem = String.Empty;
            Int32 numFolder = 0;
            List<String> list = new List<String>();
            try
            {
                var dirs = from dir in Directory.EnumerateDirectories(root) select dir;
                numFolder = dirs.Count();
                foreach (var dir in dirs)
                {
                    list.Add(dir.Substring(dir.LastIndexOf("\\") + 1));
                }
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                retorno = 1;
            }
            var tupla = Tuple.Create(retorno, mensagem, numFolder, list);
            return tupla;
        }

        public static Document1 FileToByteArray(string fileName)
        {
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(fileName).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            Document1 doc = new Document1();
            doc.DocName = fileName;
            doc.DocContent = fileContent;
            return doc;
        }
    }

    public class Document1
    {
        public int DocId { get; set; }
        public string DocName { get; set; }
        public byte[] DocContent { get; set; }
    }

}
