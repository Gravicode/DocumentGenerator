using ProtoBuf.Grpc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DocumentGenerator.Models
{
    #region helpers  
    public class FileItem : EventArgs
    {
        public int No { get; set; }
        public string Ext { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long Size { get; set; }
        public bool Indexed { get; set; }
    }
    public class RAGItem
    {
        public List<SourceItem> Sources { get; set; } = new();
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class SourceItem
    {
        public string Source { get; set; }
        public string Link { get; set; }
    }
    public class ChatItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public interface ICrud<T> where T : class
    {
        Task<bool> InsertData(T data);
        Task<bool> UpdateData(T data);
        Task<List<T>> GetAllData();
        Task<T> GetDataById(long Id);
        Task<bool> DeleteData(long Id);
        Task<long> GetLastId();
        Task<List<T>> FindByKeyword(string Keyword);
    }

    #endregion

    #region database
    [DataContract]
    public class FormulaData
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string? FormulaName { set; get; }
        [DataMember(Order = 3)]
        public string? Description { set; get; }
        [DataMember(Order = 4)]
        public DateTime? CreatedDate { set; get; }
        [DataMember(Order = 5)]
        public string? FormulaCalculation { set; get; }
        [DataMember(Order = 6)]
        public string? Username { set; get; }

    }

    [DataContract]
    public class KeyValueData
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string KeyData { set; get; }
        [DataMember(Order = 3)]
        public string? ValueData { set; get; }

    }
    [DataContract]
    public class FileStat
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string UID { set; get; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public string StatType { get; set; }

        [DataMember(Order = 5)]
        public string Username { get; set; }

        [DataMember(Order = 6)]
        public DateTime CreatedDate { get; set; }

        [DataMember(Order = 7)]
        public long Size { get; set; }
    }

    [DataContract]
    public class DocTemplate
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string DocName { get; set; }

        [DataMember(Order = 3)]
        public string Desc { get; set; }

        [DataMember(Order = 4)]
        public string DocUrl { get; set; }

        [DataMember(Order = 5)]
        public string UserName { get; set; }
        [DataMember(Order = 6)]
        public List<long> DocFunctions { get; set; } = new();
    }

    public enum FunctionTypes
    {
        Text, Image, RAG
    }

    [DataContract]
    public class DocFunction
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string SkillName { get; set; } = "My Skill";

        string _FuntionName;
        [DataMember(Order = 3)]
        public string FunctionName
        {
            get { return _FuntionName; }
            set
            {
                _FuntionName = value;
                ResultTag = "[$[" + _FuntionName.ToUpper().Trim().Replace(" ", "_") + "]]";
            }
        }

        [DataMember(Order = 4)]
        public string Desc { get; set; } = "Function description";

        [DataMember(Order = 5)]
        public List<string> Parameters { get; set; } = new();

        [DataMember(Order = 6)]
        public int MaxToken { get; set; } = 2000;

        [DataMember(Order = 7)]
        public double Temp { get; set; } = 0.2;

        [DataMember(Order = 8)]
        public double TopP { get; set; } = 0.5;

        [DataMember(Order = 9)]
        public string ResultTag { get; set; } = "[TAG-SAMPLE]";

        [DataMember(Order = 10)]
        public string Prompt { get; set; } = "Write instruction here with parameter like this {{$param1}}";

        [DataMember(Order = 11)]
        public FunctionTypes FunctionType { get; set; } = FunctionTypes.Text;

        [DataMember(Order = 12)]
        public string TagFilter { get; set; } = string.Empty;

    }

    #endregion
    public class GroupedFile
    {
        public string GroupName { get; set; }
        public List<DriveFile> Files { get; set; }
    }
    public class FileTypes
    {
        public const string Image = "Image";
        public const string Audio = "Audio";
        public const string Video = "Video";
        public const string Compressed = "Compressed";
        public const string Word = "Word";
        public const string Excel = "Excel";
        public const string PowerPoint = "PowerPoint";
        public const string Pdf = "Pdf";
        public const string Text = "Text";

        public static string GetFileType(string FileName)
        {
            var ext = Path.GetExtension(FileName.ToLower());
            switch (ext)
            {
                case ".bmp":
                case ".jpg":
                case ".gif":
                case ".png":
                case ".jpeg":
                    return Image;
                    break;
                case ".doc":
                case ".docx":
                    return Word;
                    break;
                case ".xls":
                case ".xlsx":
                    return Excel;
                    break;
                case ".ppt":
                case ".pptx":
                    return PowerPoint;
                    break;
                case ".pdf":
                    return Pdf;
                    break;
                case ".mp3":
                case ".wav":
                case ".flac":
                case ".mid":
                    return Audio;
                    break;
                case ".mp4":
                case ".avi":
                case ".mpeg":
                case ".mpg":
                case ".wmv":
                    return Video;
                    break;
                case ".zip":
                case ".rar":
                case ".tar":
                case ".tar.gz":
                case ".7z":
                    return Compressed;
                    break;
                case ".txt":
                case ".rtf":
                    return Text;
                    break;


            }
            return "Unknown";
        }
    }
    public class DriveFile
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public string Owner { get; set; }
        public bool Favorite { get; set; } = false;
        public string Path { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string FileUrl { get; set; }
        public string ParentFolderUid { set; get; }
        public bool IsDeleted { get; set; } = false;
    }


}