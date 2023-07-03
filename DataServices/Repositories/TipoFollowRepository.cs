using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoFollowRepository : RepositoryBase<TIPO_FOLLOW>, ITipoFollowRepository
    {
        public TIPO_FOLLOW CheckExist(TIPO_FOLLOW conta, Int32 idAss)
        {
            IQueryable<TIPO_FOLLOW> query = Db.TIPO_FOLLOW;
            query = query.Where(p => p.TIFL_NM_NOME == conta.TIFL_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_FOLLOW GetItemById(Int32 id)
        {
            IQueryable<TIPO_FOLLOW> query = Db.TIPO_FOLLOW;
            query = query.Where(p => p.TIFL_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_FOLLOW> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_FOLLOW> query = Db.TIPO_FOLLOW.Where(p => p.TIFL_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_FOLLOW> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_FOLLOW> query = Db.TIPO_FOLLOW;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
