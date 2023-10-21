using Microsoft.EntityFrameworkCore;
using DocumentGenerator.Models;
namespace DocumentGenerator.Web.Data
{
    public class DocTemplateService : ICrudDb<DocTemplate>
    {
        DocumentGeneratorDb db;

        public DocTemplateService()
        {
            if (db == null) db = new DocumentGeneratorDb();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.DocTemplates.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.DocTemplates.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<DocTemplate> FindByKeyword(string Keyword)
        {
            var data = from x in db.DocTemplates
                       where x.DocName.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<DocTemplate> GetAllData()
        {
            return db.DocTemplates.ToList();
        }
        public List<DocTemplate> GetAllData(string username)
        {
            return db.DocTemplates.Where(x => x.UserName == username).ToList();
        }
        public DocTemplate GetDataById(object Id)
        {
            return db.DocTemplates.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(DocTemplate data)
        {
            try
            {
                db.DocTemplates.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }

        public bool UpdateData(DocTemplate data)
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
            return db.DocTemplates.Max(x => x.Id);
        }
    }
    /*
    public class DocTemplateService : ICrud<DocTemplate>
    {
        DocumentDB provider;
        RAGDb db;
        UserProfileService UserSvc;

        public DocTemplateService(DocumentDB provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.db = provider.DB;
            this.provider = provider;
        }

        public void RefreshEntity(DocTemplate item)
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
        public bool DeleteData(DocTemplate item)
        {
            db.DocTemplates.Delete(item);
            provider.Save();
            return true;
        }

        public List<DocTemplate> FindByKeyword(string Keyword)
        {
            var data = db.DocTemplates.Where(x => x.DocName.Contains(Keyword));
            return data.ToList();
        }

        public List<DocTemplate> GetAllData()
        {
            return db.DocTemplates.ToList();
        }

        public DocTemplate GetDataById(string Id)
        {
            return db.DocTemplates.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(DocTemplate data)
        {
            try
            {
                db.DocTemplates.Insert(data);
                provider.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }

        public bool UpdateData(DocTemplate data)
        {
            try
            {
                db.DocTemplates.Update(data);
                provider.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public DocTemplate GetDataById(object Id)
        {
            return db.DocTemplates.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }*/

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
    public class DocTemplateService : ICrud<DocTemplate>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<DocTemplate> db;
        UserProfileService UserSvc;

        public DocTemplateService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            db = this.provider.RedisCollection<DocTemplate>();
        }

        public void RefreshEntity(DocTemplate item)
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
        public bool DeleteData(DocTemplate item)
        {
            db.Delete(item);
            return true;
        }

        public List<DocTemplate> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.DocName.Contains(Keyword));
            return data.ToList();
        }

        public List<DocTemplate> GetAllData()
        {
            return db.ToList();
        }

        public DocTemplate GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(DocTemplate data)
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

        public bool UpdateData(DocTemplate data)
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

        public DocTemplate GetDataById(object Id)
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