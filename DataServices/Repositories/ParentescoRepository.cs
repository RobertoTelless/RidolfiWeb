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
    public class ParentescoRepository : RepositoryBase<PARENTESCO>, IParentescoRepository
    {
        public PARENTESCO CheckExist(PARENTESCO conta)
        {
            IQueryable<PARENTESCO> query = Db.PARENTESCO;
            query = query.Where(p => p.PARE_NM_NOME == conta.PARE_NM_NOME);
            return query.FirstOrDefault();
        }

        public PARENTESCO GetItemById(Int32 id)
        {
            IQueryable<PARENTESCO> query = Db.PARENTESCO;
            query = query.Where(p => p.PARE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PARENTESCO> GetAllItens()
        {
            IQueryable<PARENTESCO> query = Db.PARENTESCO.Where(p => p.PARE_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<PARENTESCO> GetAllItensAdm()
        {
            IQueryable<PARENTESCO> query = Db.PARENTESCO;
            return query.ToList();
        }
    }
}
 