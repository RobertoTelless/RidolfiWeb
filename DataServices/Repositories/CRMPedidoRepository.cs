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
    public class CRMPedidoRepository : RepositoryBase<CRM_PEDIDO_VENDA>, ICRMPedidoRepository
    {
        public List<CRM_PEDIDO_VENDA> GetAllItens(Int32 idUsu)
        {
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA.Where(p => p.CRPV_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            query = query.Where(p => p.CRM1_CD_ID != null);
            return query.ToList();
        }

        public List<CRM_PEDIDO_VENDA> GetAllItensGeral(Int32 idUsu)
        {
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA.Where(p => p.CRPV_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<CRM_PEDIDO_VENDA> GetAllItensVenda(Int32 idUsu)
        {
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA.Where(p => p.CRPV_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            query = query.Where(p => p.CRM1_CD_ID == null);
            return query.ToList();
        }

        public CRM_PEDIDO_VENDA GetItemById(Int32 id)
        {
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA.Where(p => p.CRPV_CD_ID == id);
            query = query.Include(p => p.CRM_PEDIDO_VENDA_ACOMPANHAMENTO);
            query = query.Include(p => p.CRM_PEDIDO_VENDA_ANEXO);
            //query = query.Include(p => p.CRM_PEDIDO_VENDA_ITEM);
            return query.FirstOrDefault();
        }

        public CRM_PEDIDO_VENDA GetItemByNumero(String num, Int32 idAss)
        {
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA.Where(p => p.CRPV_NR_NUMERO == num);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.USUARIO);
            //query = query.Include(p => p.FORMA_ENVIO);
            //query = query.Include(p => p.FORMA_FRETE);
            query = query.Include(p => p.MOTIVO_CANCELAMENTO);
            query = query.Include(p => p.TEMPLATE_PROPOSTA);
            query = query.Include(p => p.CRM_PEDIDO_VENDA_ACOMPANHAMENTO);
            query = query.Include(p => p.CRM_PEDIDO_VENDA_ANEXO);
            //query = query.Include(p => p.CRM_PEDIDO_VENDA_ITEM);
            return query.FirstOrDefault();
        }

        public List<CRM_PEDIDO_VENDA> ExecuteFilter(String busca, Int32? status, DateTime? dataInicio, DateTime? dataFim, Int32? filial, Int32? usuario, Int32 idAss)
        {
            List<CRM_PEDIDO_VENDA> lista = new List<CRM_PEDIDO_VENDA>();
            IQueryable<CRM_PEDIDO_VENDA> query = Db.CRM_PEDIDO_VENDA;
            if (status > 0)
            {
                query = query.Where(p => p.CRPV_IN_STATUS == status);
            }
            if (!String.IsNullOrEmpty(busca))
            {
                query = query.Where(p => p.CLIENTE.CLIE_NM_NOME.Contains(busca) || p.CLIENTE.CLIE_NM_RAZAO.Contains(busca) || p.CLIENTE.CLIE_NR_CPF.Contains(busca));
            }
            if (filial > 0)
            {
                query = query.Where(p => p.FILI_CD_ID == filial);
            }
            if (usuario > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuario);
            }
            if (dataInicio != null & dataFim == null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CRPV_DT_PEDIDO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if (dataInicio == null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CRPV_DT_PEDIDO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (dataInicio != null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CRPV_DT_PEDIDO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.CRPV_DT_PEDIDO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.CRM1_CD_ID == null);
                query = query.Where(p => p.CRPV_IN_ATIVO == 1);
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CRPV_DT_PEDIDO);
                lista = query.ToList<CRM_PEDIDO_VENDA>();
            }
            return lista;
        }

    }
}
 