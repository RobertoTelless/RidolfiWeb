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
    public class TelefoneBenefRepository : RepositoryBase<TELEFONE>, ITelefoneBenefRepository
    {
        public List<TELEFONE> GetAllItens()
        {
            return Db.TELEFONE.ToList();
        }

        public TELEFONE GetItemById(Int32 id)
        {
            IQueryable<TELEFONE> query = Db.TELEFONE.Where(p => p.TELE_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 