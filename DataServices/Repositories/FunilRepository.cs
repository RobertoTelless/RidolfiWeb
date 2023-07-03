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

namespace DataServices.Repositories
{
    public class FunilRepository : RepositoryBase<FUNIL>, IFunilRepository
    {
        public FUNIL CheckExist(FUNIL conta, Int32 idAss)
        {
            IQueryable<FUNIL> query = Db.FUNIL;
            query = query.Where(p => p.FUNI_NM_NOME == conta.FUNI_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.FUNI_IN_TIPO == 1);
            return query.FirstOrDefault();
        }

        public FUNIL GetItemById(Int32 id)
        {
            IQueryable<FUNIL> query = Db.FUNIL;
            query = query.Where(p => p.FUNI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public FUNIL GetItemBySigla(String sigla, Int32 idAss)
        {
            IQueryable<FUNIL> query = Db.FUNIL;
            query = query.Where(p => p.FUNI_SG_SIGLA == sigla);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.FUNI_IN_TIPO == 1);
            return query.FirstOrDefault();
        }

        public List<FUNIL> GetAllItens(Int32 idAss)
        {
            IQueryable<FUNIL> query = Db.FUNIL.Where(p => p.FUNI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.FUNI_IN_TIPO == 1);
            return query.ToList();
        }

        public List<FUNIL> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<FUNIL> query = Db.FUNIL;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.FUNI_IN_TIPO == 1);
            return query.ToList();
        }
    }
}
 