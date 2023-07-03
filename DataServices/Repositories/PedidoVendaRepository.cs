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
using CrossCutting;

namespace DataServices.Repositories
{
    public class PedidoVendaRepository : RepositoryBase<PEDIDO_VENDA>, IPedidoVendaRepository
    {
        public PEDIDO_VENDA GetItemById(Int32 id)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA;
            query = query.Where(p => p.PEVE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PEDIDO_VENDA> GetAllItens(Int32 idAss)
        {
            IQueryable<PEDIDO_VENDA> query = Db.PEDIDO_VENDA.Where(p => p.PEVE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 