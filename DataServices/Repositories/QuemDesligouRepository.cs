using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class QuemDesligouRepository : RepositoryBase<QUEM_DESLIGOU>, IQuemDesligouRepository
    {
        public QUEM_DESLIGOU CheckExist(String conta)
        {
            IQueryable<QUEM_DESLIGOU> query = Db.QUEM_DESLIGOU;
            query = query.Where(p => p.QUDE_NM_NOME == conta);
            return query.FirstOrDefault();
        }

        public QUEM_DESLIGOU GetItemById(Int32 id)
        {
            IQueryable<QUEM_DESLIGOU> query = Db.QUEM_DESLIGOU;
            query = query.Where(p => p.QUDE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<QUEM_DESLIGOU> GetAllItens()
        {
            IQueryable<QUEM_DESLIGOU> query = Db.QUEM_DESLIGOU.Where(p => p.QUDE_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<QUEM_DESLIGOU> GetAllItensAdm()
        {
            IQueryable<QUEM_DESLIGOU> query = Db.QUEM_DESLIGOU;
            return query.ToList();
        }
    }
}
