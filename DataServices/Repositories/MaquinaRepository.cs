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
    public class MaquinaRepository : RepositoryBase<MAQUINA>, IMaquinaRepository
    {
        public MAQUINA CheckExist(MAQUINA tarefa, Int32 idUsu)
        {
            IQueryable<MAQUINA> query = Db.MAQUINA;
            query = query.Where(p => p.MAQN_NM_NOME == tarefa.MAQN_NM_NOME);
            query = query.Where(p => p.MAQN_NM_PROVEDOR == tarefa.MAQN_NM_PROVEDOR);
            query = query.Where(p => p.ASSI_CD_ID == tarefa.ASSI_CD_ID);
            return query.FirstOrDefault();
        }

        public MAQUINA GetItemById(Int32 id)
        {
            IQueryable<MAQUINA> query = Db.MAQUINA;
            query = query.Where(p => p.MAQN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MAQUINA> GetAllItens(Int32 idUsu)
        {
            IQueryable<MAQUINA> query = Db.MAQUINA.Where(p => p.MAQN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<MAQUINA> GetAllItensAdm(Int32 idUsu)
        {
            IQueryable<MAQUINA> query = Db.MAQUINA;
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<MAQUINA> ExecuteFilter(String provedor, String nome, Int32? idAss)
        {
            List<MAQUINA> lista = new List<MAQUINA>();
            IQueryable<MAQUINA> query = Db.MAQUINA;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.MAQN_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(provedor))
            {
                query = query.Where(p => p.MAQN_NM_PROVEDOR.Contains(provedor));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.MAQN_NM_NOME);
                lista = query.ToList<MAQUINA>();
            }
            return lista;
        }

    }
}
 