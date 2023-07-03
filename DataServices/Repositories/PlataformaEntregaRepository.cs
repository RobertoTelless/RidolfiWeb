
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class PlataformaEntregaRepository : RepositoryBase<PLATAFORMA_ENTREGA>, IPlataformaEntregaRepository
    {
        public PLATAFORMA_ENTREGA CheckExist(PLATAFORMA_ENTREGA tarefa, Int32 idUsu)
        {
            IQueryable<PLATAFORMA_ENTREGA> query = Db.PLATAFORMA_ENTREGA;
            query = query.Where(p => p.PLEN_NM_NOME == tarefa.PLEN_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == tarefa.ASSI_CD_ID);
            return query.FirstOrDefault();
        }

        public PLATAFORMA_ENTREGA GetItemById(Int32 id)
        {
            IQueryable<PLATAFORMA_ENTREGA> query = Db.PLATAFORMA_ENTREGA;
            query = query.Where(p => p.PLEN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PLATAFORMA_ENTREGA> GetAllItens(Int32 idUsu)
        {
            IQueryable<PLATAFORMA_ENTREGA> query = Db.PLATAFORMA_ENTREGA.Where(p => p.PLEN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<PLATAFORMA_ENTREGA> GetAllItensAdm(Int32 idUsu)
        {
            IQueryable<PLATAFORMA_ENTREGA> query = Db.PLATAFORMA_ENTREGA;
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<PLATAFORMA_ENTREGA> ExecuteFilter(String nome, Int32 idAss)
        {
            List<PLATAFORMA_ENTREGA> lista = new List<PLATAFORMA_ENTREGA>();
            IQueryable<PLATAFORMA_ENTREGA> query = Db.PLATAFORMA_ENTREGA;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PLEN_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.PLEN_NM_NOME);
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                lista = query.ToList<PLATAFORMA_ENTREGA>();
            }
            return lista;
        }
    }
}
 