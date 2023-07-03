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
    public class PrecatorioFalhaRepository : RepositoryBase<PRECATORIO_FALHA>, IPrecatorioFalhaRepository
    {
        public PRECATORIO_FALHA GetItemById(Int32 id)
        {
            IQueryable<PRECATORIO_FALHA> query = Db.PRECATORIO_FALHA;
            query = query.Where(p => p.PRFA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PRECATORIO_FALHA> GetAllItens(Int32 idAss)
        {
            IQueryable<PRECATORIO_FALHA> query = Db.PRECATORIO_FALHA;
            return query.ToList();
        }
    }
}
 