using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoTelefoneBaseRepository : RepositoryBase<TIPO_TELEFONE_BASE>, ITipoTelefoneBaseRepository
    {
        public TIPO_TELEFONE_BASE GetItemById(Int32 id)
        {
            IQueryable<TIPO_TELEFONE_BASE> query = Db.TIPO_TELEFONE_BASE;
            query = query.Where(p => p.TITE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_TELEFONE_BASE> GetAllItens()
        {
            IQueryable<TIPO_TELEFONE_BASE> query = Db.TIPO_TELEFONE_BASE;
            return query.ToList();
        }
    }
}
