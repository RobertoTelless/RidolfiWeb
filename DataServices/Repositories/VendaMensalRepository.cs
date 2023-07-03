using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using System.Diagnostics;

namespace DataServices.Repositories
{
    public class VendaMensalRepository : RepositoryBase<VENDA_MENSAL>, IVendaMensalRepository
    {
        public VENDA_MENSAL CheckExist(VENDA_MENSAL conta, Int32 idAss)
        {
            IQueryable<VENDA_MENSAL> query = Db.VENDA_MENSAL;
            query = query.Where(p => DbFunctions.TruncateTime(p.VEMA_DT_REFERENCIA).Value.Month == DbFunctions.TruncateTime(conta.VEMA_DT_REFERENCIA).Value.Month & DbFunctions.TruncateTime(p.VEMA_DT_REFERENCIA).Value.Year == DbFunctions.TruncateTime(conta.VEMA_DT_REFERENCIA).Value.Year);
            query = query.Where(p => p.VEMA_IN_TIPO == conta.VEMA_IN_TIPO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public VENDA_MENSAL GetItemById(Int32 id)
        {
            IQueryable<VENDA_MENSAL> query = Db.VENDA_MENSAL;
            query = query.Where(p => p.VEMA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<VENDA_MENSAL> GetAllItens(Int32 idAss)
        {
            IQueryable<VENDA_MENSAL> query = Db.VENDA_MENSAL.Where(p => p.VEMA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VENDA_MENSAL> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<VENDA_MENSAL> query = Db.VENDA_MENSAL;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VENDA_MENSAL> ExecuteFilter(DateTime? dataRef, Int32? tipo, Int32 idAss)
        {
            List<VENDA_MENSAL> lista = new List<VENDA_MENSAL>();
            IQueryable<VENDA_MENSAL> query = Db.VENDA_MENSAL;
            if (dataRef != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.VEMA_DT_REFERENCIA).Value.Month == DbFunctions.TruncateTime(dataRef).Value.Month & DbFunctions.TruncateTime(p.VEMA_DT_REFERENCIA).Value.Year == DbFunctions.TruncateTime(dataRef).Value.Year);
            }
            if (tipo != null)
            {
                query = query.Where(p => p.VEMA_IN_TIPO == tipo);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderByDescending(a => a.VEMA_DT_REFERENCIA);
                lista = query.ToList<VENDA_MENSAL>();
            }
            return lista;
        }
    }
}
