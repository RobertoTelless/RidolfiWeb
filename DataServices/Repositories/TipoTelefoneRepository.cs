using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoTelefoneRepository : RepositoryBase<TIPO_TELEFONE>, ITipoTelefoneRepository
    {
        public TIPO_TELEFONE GetItemById(Int32 id)
        {
            IQueryable<TIPO_TELEFONE> query = Db.TIPO_TELEFONE;
            query = query.Where(p => p.TITE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_TELEFONE> GetAllItens()
        {
            IQueryable<TIPO_TELEFONE> query = Db.TIPO_TELEFONE;
            return query.ToList();
        }
    }
}
