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
    public class MensagemAutomacaoDatasRepository : RepositoryBase<MENSAGEM_AUTOMACAO_DATAS>, IMensagemAutomacaoDatasRepository
    {
        public List<MENSAGEM_AUTOMACAO_DATAS> GetAllItens()
        {
            return Db.MENSAGEM_AUTOMACAO_DATAS.ToList();
        }

        public MENSAGEM_AUTOMACAO_DATAS GetItemById(Int32 id)
        {
            IQueryable<MENSAGEM_AUTOMACAO_DATAS> query = Db.MENSAGEM_AUTOMACAO_DATAS.Where(p => p.MEAD_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 