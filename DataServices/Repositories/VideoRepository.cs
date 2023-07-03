using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class VideoRepository : RepositoryBase<VIDEO>, IVideoRepository
    {
        public VIDEO GetItemById(Int32 id)
        {
            IQueryable<VIDEO> query = Db.VIDEO;
            query = query.Where(p => p.VIDE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<VIDEO> GetAllItens(Int32 idAss)
        {
            IQueryable<VIDEO> query = Db.VIDEO.Where(p => p.VIDE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VIDEO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<VIDEO> query = Db.VIDEO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VIDEO> GetAllItensValidos(Int32 idAss)
        {
            IQueryable<VIDEO> query = Db.VIDEO;
            query = query.Where(p => DbFunctions.TruncateTime(p.VIDE_DT_VALIDADE) >= DbFunctions.TruncateTime(DateTime.Today));
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(p => p.VIDE_DT_EMISSAO);
            return query.ToList();
        }

        public List<VIDEO> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss)
        {
            List<VIDEO> lista = new List<VIDEO>();
            IQueryable<VIDEO> query = Db.VIDEO;
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.VIDE_NM_TITULO.Contains(titulo));
            }
            if (!String.IsNullOrEmpty(autor))
            {
                query = query.Where(p => p.VIDE_NM_AUTOR.Contains(autor));
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.VIDE_DT_EMISSAO) == DbFunctions.TruncateTime(data));
            }
            if (!String.IsNullOrEmpty(texto))
            {
                query = query.Where(p => p.VIDE_NM_DESCRICAO.Contains(texto));
            }
            if (!String.IsNullOrEmpty(link))
            {
                query = query.Where(p => p.VIDE_LK_LINK.Contains(link));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.VIDE_DT_EMISSAO);
                lista = query.ToList<VIDEO>();
            }
            return lista;
        }
    }
}
