using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class CategoriaCustoFixoRepository : RepositoryBase<CATEGORIA_CUSTO_FIXO>, ICategoriaCustoFixoRepository
    {
        public CATEGORIA_CUSTO_FIXO CheckExist(CATEGORIA_CUSTO_FIXO conta, Int32 idAss)
        {
            IQueryable<CATEGORIA_CUSTO_FIXO> query = Db.CATEGORIA_CUSTO_FIXO;
            query = query.Where(p => p.CACF_NM_NOME == conta.CACF_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CATEGORIA_CUSTO_FIXO GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_CUSTO_FIXO> query = Db.CATEGORIA_CUSTO_FIXO;
            query = query.Where(p => p.CACF_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_CUSTO_FIXO> GetAllItens(Int32 idAss)
        {
            IQueryable<CATEGORIA_CUSTO_FIXO> query = Db.CATEGORIA_CUSTO_FIXO.Where(p => p.CACF_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CATEGORIA_CUSTO_FIXO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CATEGORIA_CUSTO_FIXO> query = Db.CATEGORIA_CUSTO_FIXO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 