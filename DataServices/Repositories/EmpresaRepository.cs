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
    public class EmpresaRepository : RepositoryBase<EMPRESA>, IEmpresaRepository
    {
        public EMPRESA CheckExist(EMPRESA tarefa, Int32 idUsu)
        {
            IQueryable<EMPRESA> query = Db.EMPRESA;
            query = query.Where(p => p.EMPR_NM_NOME == tarefa.EMPR_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == tarefa.ASSI_CD_ID);
            return query.FirstOrDefault();
        }

        public EMPRESA GetItemById(Int32 id)
        {
            IQueryable<EMPRESA> query = Db.EMPRESA;
            query = query.Where(p => p.EMPR_CD_ID == id);
            query = query.Include(p => p.EMPRESA_MAQUINA);
            query = query.Include(p => p.EMPRESA_PLATAFORMA);
            query = query.Include(p => p.EMPRESA_TICKET);
            query = query.Include(p => p.EMPRESA_CUSTO_VARIAVEL);
            query = query.Include(p => p.EMPRESA_ANEXO);
            return query.FirstOrDefault();
        }

        public EMPRESA GetItemByAssinante(Int32 id)
        {
            IQueryable<EMPRESA> query = Db.EMPRESA;
            query = query.Where(p => p.ASSI_CD_ID == id);
            query = query.Include(p => p.EMPRESA_MAQUINA);
            query = query.Include(p => p.EMPRESA_PLATAFORMA);
            query = query.Include(p => p.EMPRESA_TICKET);
            query = query.Include(p => p.EMPRESA_CUSTO_VARIAVEL);
            query = query.Include(p => p.EMPRESA_ANEXO);
            return query.FirstOrDefault();
        }

        public List<EMPRESA> GetAllItens(Int32 idUsu)
        {
            IQueryable<EMPRESA> query = Db.EMPRESA.Where(p => p.EMPR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<EMPRESA> GetAllItensAdm(Int32 idUsu)
        {
            IQueryable<EMPRESA> query = Db.EMPRESA;
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<EMPRESA> ExecuteFilter(String nome, Int32? idUsu)
        {
            List<EMPRESA> lista = new List<EMPRESA>();
            IQueryable<EMPRESA> query = Db.EMPRESA;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.EMPR_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.EMPR_NM_NOME);
                lista = query.ToList<EMPRESA>();
            }
            return lista;
        }

    }
}
 