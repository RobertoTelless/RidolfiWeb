using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PrecatorioEstadoRepository : RepositoryBase<PRECATORIO_ESTADO>, IPrecatorioEstadoRepository
    {
        public PRECATORIO_ESTADO GetItemById(Int32 id)
        {
            IQueryable<PRECATORIO_ESTADO> query = Db.PRECATORIO_ESTADO;
            query = query.Where(p => p.PRES_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PRECATORIO_ESTADO> GetAllItens()
        {
            IQueryable<PRECATORIO_ESTADO> query = Db.PRECATORIO_ESTADO;
            return query.ToList();
        }

        public PRECATORIO_ESTADO CheckExist(PRECATORIO_ESTADO prec)
        {
            IQueryable<PRECATORIO_ESTADO> query = Db.PRECATORIO_ESTADO;
            query = query.Where(p => p.PRES_NM_NOME == prec.PRES_NM_NOME);
            return query.FirstOrDefault();
        }

        public List<PRECATORIO_ESTADO> GetAllItensAdm()
        {
            IQueryable<PRECATORIO_ESTADO> query = Db.PRECATORIO_ESTADO;
            return query.ToList();
        }
    }
}
