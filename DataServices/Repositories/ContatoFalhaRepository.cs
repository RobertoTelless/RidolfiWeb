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
    public class ContatoFalhaRepository : RepositoryBase<CONTATO_FALHA>, IContatoFalhaRepository
    {
        public CONTATO_FALHA GetItemById(Int32 id)
        {
            IQueryable<CONTATO_FALHA> query = Db.CONTATO_FALHA;
            query = query.Where(p => p.COFA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONTATO_FALHA> GetAllItens(Int32 idAss)
        {
            IQueryable<CONTATO_FALHA> query = Db.CONTATO_FALHA;
            return query.ToList();
        }
    }
}
 