using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PrecatorioRepository : RepositoryBase<PRECATORIO>, IPrecatorioRepository
    {
        public PRECATORIO CheckExist(PRECATORIO conta)
        {
            IQueryable<PRECATORIO> query = Db.PRECATORIO;
            if (conta.PREC_NM_PRECATORIO != null)
            {
                query = query.Where(p => p.PREC_NM_PRECATORIO == conta.PREC_NM_PRECATORIO);
            }
            return query.FirstOrDefault();
        }

        public PRECATORIO CheckExist(String conta)
        {
            IQueryable<PRECATORIO> query = Db.PRECATORIO;
            if (conta != null)
            {
                query = query.Where(p => p.PREC_NM_PRECATORIO == conta);
            }
            return query.FirstOrDefault();
        }

        public PRECATORIO GetItemById(Int32 id)
        {
            IQueryable<PRECATORIO> query = Db.PRECATORIO;
            query = query.Where(p => p.PREC_CD_ID == id);
            query = query.Include(p => p.BENEFICIARIO);
            return query.FirstOrDefault();
        }

        public List<PRECATORIO> GetAllItens()
        {
            IQueryable<PRECATORIO> query = Db.PRECATORIO.Where(p => p.PREC_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<PRECATORIO> GetAllItensAdm()
        {
            IQueryable<PRECATORIO> query = Db.PRECATORIO;
            return query.ToList();
        }

        public List<PRECATORIO> ExecuteFilter(Int32? trf, Int32? beneficiario, Int32? advogado, Int32? natureza, Int32? estado, String nome, String ano, Int32? crm, Int32? pesquisa, Decimal? valor1, Decimal? valor2, Int32? situacao)
        {
            List<PRECATORIO> lista = new List<PRECATORIO>();
            IQueryable<PRECATORIO> query = Db.PRECATORIO;
            if (trf > 0)
            {
                query = query.Where(p => p.TRF1_CD_ID == trf);
            }
            if (beneficiario != null)
            {
                query = query.Where(p => p.BENE_CD_ID == beneficiario);
            }
            if (estado != null)
            {
                query = query.Where(p => p.PRES_CD_ID == estado);
            }
            if (advogado != null)
            {
                query = query.Where(p => p.HONO_CD_ID == advogado);
            }
            if (natureza != null)
            {
                query = query.Where(p => p.NATU_CD_ID == natureza);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PREC_NM_REQUERENTE.Contains(nome));
            }
            if (!String.IsNullOrEmpty(ano))
            {
                query = query.Where(p => p.PREC_NR_ANO.Contains(ano));
            }
            if (crm != null)
            {
                query = query.Where(p => p.PREC_IN_FOI_IMPORTADO_PIPE == crm);
            }
            if (situacao != null)
            {
                query = query.Where(p => p.PREC_IN_SITUACAO_ATUAL == situacao);
            }
            if (pesquisa != null)
            {
                query = query.Where(p => p.PREC_IN_FOI_PESQUISADO == pesquisa);
            }
            if (valor1 != null & valor2 == null)
            {
                query = query.Where(p => p.PREC_VL_BEN_VALOR_REQUISITADO == valor1);
            }
            else if (valor1 != null & valor2 != null)
            {
                query = query.Where(p => p.PREC_VL_BEN_VALOR_REQUISITADO > valor1 & p.PREC_VL_BEN_VALOR_REQUISITADO < valor2);
            }
            else if (valor1 == null & valor2 != null)
            {
                query = query.Where(p => p.PREC_VL_BEN_VALOR_REQUISITADO == valor2);
            }
            if (query != null)
            {
                lista = query.ToList<PRECATORIO>();
            }
            return lista;
        }

    }
}
