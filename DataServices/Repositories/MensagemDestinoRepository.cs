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
    public class MensagemDestinoRepository : RepositoryBase<MENSAGENS_DESTINOS>, IMensagemDestinoRepository
    {
        public List<MENSAGENS_DESTINOS> GetAllItens()
        {
            return Db.MENSAGENS_DESTINOS.ToList();
        }

        public MENSAGENS_DESTINOS GetItemById(Int32 id)
        {
            IQueryable<MENSAGENS_DESTINOS> query = Db.MENSAGENS_DESTINOS.Where(p => p.MEDE_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 