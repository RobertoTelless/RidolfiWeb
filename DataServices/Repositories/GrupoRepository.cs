using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class GrupoRepository : RepositoryBase<GRUPO>, IGrupoRepository
    {
        public GRUPO CheckExist(GRUPO conta, Int32 idAss)
        {
            IQueryable<GRUPO> query = Db.GRUPO;
            query = query.Where(p => p.GRUP_NM_NOME == conta.GRUP_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.GRUP_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public GRUPO GetItemById(Int32 id)
        {
            IQueryable<GRUPO> query = Db.GRUPO;
            query = query.Where(p => p.GRUP_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<GRUPO> GetAllItens(Int32 idAss)
        {
            IQueryable<GRUPO> query = Db.GRUPO.Where(p => p.GRUP_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<GRUPO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<GRUPO> query = Db.GRUPO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 