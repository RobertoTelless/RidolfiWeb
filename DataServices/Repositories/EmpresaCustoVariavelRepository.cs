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
    public class EmpresaCustoVariavelRepository : RepositoryBase<EMPRESA_CUSTO_VARIAVEL>, IEmpresaCustoVariavelRepository
    {
        public EMPRESA_CUSTO_VARIAVEL CheckExistCustoVariavel(EMPRESA_CUSTO_VARIAVEL cliente, Int32 idAss)
        {
            IQueryable<EMPRESA_CUSTO_VARIAVEL> query = Db.EMPRESA_CUSTO_VARIAVEL;
            if (cliente.MAQN_CD_ID != null)
            {
                query = query.Where(p => p.MAQN_CD_ID == cliente.MAQN_CD_ID);
            }
            if (cliente.PLEN_CD_ID != null)
            {
                query = query.Where(p => p.PLEN_CD_ID == cliente.PLEN_CD_ID);
            }
            if (cliente.TICK_CD_ID != null)
            {
                query = query.Where(p => p.TICK_CD_ID == cliente.TICK_CD_ID);
            }
            query = query.Where(p => p.EMCV_NM_NOME == cliente.EMCV_NM_NOME);
            query = query.Where(p => p.EMPR_CD_ID == cliente.EMPR_CD_ID);
            query = query.Where(p => p.EMCV_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public List<EMPRESA_CUSTO_VARIAVEL> GetAllItens(Int32 idUsu)
        {
            IQueryable<EMPRESA_CUSTO_VARIAVEL> query = Db.EMPRESA_CUSTO_VARIAVEL.Where(p => p.EMCV_IN_ATIVO == 1);
            query = query.Where(p => p.EMPR_CD_ID == idUsu);
            return query.ToList();
        }

        public EMPRESA_CUSTO_VARIAVEL GetItemById(Int32 id)
        {
            IQueryable<EMPRESA_CUSTO_VARIAVEL> query = Db.EMPRESA_CUSTO_VARIAVEL.Where(p => p.EMCV_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 