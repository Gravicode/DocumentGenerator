using Microsoft.EntityFrameworkCore;
using DocumentGenerator.Models;
namespace DocumentGenerator.Web.Data
{
    public class DocFunctionService : ICrudDb<DocFunction>
    {
        DocumentGeneratorDb db;

        public DocFunctionService()
        {
            if (db == null) db = new DocumentGeneratorDb();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.DocFunctions.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.DocFunctions.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<DocFunction> FindByKeyword(string Keyword)
        {
            var data = from x in db.DocFunctions
                       where x.FunctionName.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<DocFunction> GetAllData()
        {
            return db.DocFunctions.ToList();
        }
        
        public DocFunction GetDataById(object Id)
        {
            return db.DocFunctions.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(DocFunction data)
        {
            try
            {
                db.DocFunctions.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }
        public bool IsExist(string FuncName)
        {
            return db.DocFunctions.Any(x => x.FunctionName == FuncName);
        }
        public bool UpdateData(DocFunction data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }

        public long GetLastId()
        {
            return db.DocFunctions.Max(x => x.Id);
        }
    }
    /*
    public class DocFunctionService : ICrud<DocFunction>
    {
        //FileGueDB db;
        RAGDb db;
        UserProfileService UserSvc;
        DocumentDB provider;
        public DocFunctionService(DocumentDB provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.db = provider.DB;
            this.provider = provider;
        }

        public void RefreshEntity(DocFunction item)
        {
            try
            {
                //do nothing
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }


        }
        public bool DeleteData(DocFunction item)
        {
            db.DocFunctions.Delete(item);
            provider.Save();
            return true;
        }

        public List<DocFunction> FindByKeyword(string Keyword)
        {
            var data = db.DocFunctions.Where(x => x.FunctionName.Contains(Keyword));
            return data.ToList();
        }

        public List<DocFunction> GetAllData()
        {
            return db.DocFunctions.ToList();
        }

        public DocFunction GetDataById(string Id)
        {
            return db.DocFunctions.Where(x => x.Id == Id).FirstOrDefault();
        }

        public bool IsExist(string FuncName)
        {
            return db.DocFunctions.Any(x => x.FunctionName == FuncName);
        }
        public bool InsertData(DocFunction data)
        {
            try
            {
                db.DocFunctions.Insert(data);
                provider.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }

        public bool UpdateData(DocFunction data)
        {
            try
            {
                db.DocFunctions.Update(data);
                provider.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public DocFunction GetDataById(object Id)
        {
            return db.DocFunctions.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }
    */
}

/*
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Pdf.Content.Objects;
using Redis.OM;
using Redis.OM.Searching;
using DocumentGenerator.Web.Data;
using DocumentGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
namespace DocumentGenerator.Web.Data
{
    public class DocFunctionService : ICrud<DocFunction>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<DocFunction> db;
        UserProfileService UserSvc;

        public DocFunctionService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            db = this.provider.RedisCollection<DocFunction>();
        }

        public void RefreshEntity(DocFunction item)
        {
            try
            {
                //do nothing
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }


        }
        public bool DeleteData(DocFunction item)
        {
            db.Delete(item);
            return true;
        }

        public List<DocFunction> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.FunctionName.Contains(Keyword));
            return data.ToList();
        }

        public List<DocFunction> GetAllData()
        {
            return db.ToList();
        }

        public DocFunction GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }

        public bool IsExist(string FuncName)
        {
            return db.Any(x => x.FunctionName == FuncName);
        }
        public bool InsertData(DocFunction data)
        {
            try
            {
                db.Insert(data);
                //db.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }

        public bool UpdateData(DocFunction data)
        {
            try
            {
                db.Update(data);
                //db.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public DocFunction GetDataById(object Id)
        {
            return db.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }

}

 */ 