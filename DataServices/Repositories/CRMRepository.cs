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
    public class CRMRepository : RepositoryBase<CRM>, ICRMRepository
    {
        public CRM CheckExist(CRM tarefa, Int32 idUsu, Int32 idAss)
        {
            IQueryable<CRM> query = Db.CRM;
            query = query.Where(p => p.CRM1_NM_NOME == tarefa.CRM1_NM_NOME);
            query = query.Where(p => p.USUA_CD_ID == idUsu);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.CRM1_IN_ATIVO != 2);
            return query.FirstOrDefault();
        }

        public List<CRM> GetByDate(DateTime data, Int32 idAss)
        {
            IQueryable<CRM> query = Db.CRM.Where(p => p.CRM1_IN_ATIVO == 1);
            query = query.Where(p => p.CRM1_DT_CRIACAO == data);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.CRM1_IN_ATIVO != 2);
            return query.ToList();
        }

        public List<CRM> GetByUser(Int32 user)
        {
            IQueryable<CRM> query = Db.CRM.Where(p => p.CRM1_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == user);
            query = query.Where(p => p.CRM1_IN_ATIVO != 2);
            return query.ToList();
        }

        public List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            IQueryable<CRM> query = Db.CRM.Where(p => p.CRM1_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.CRM1_IN_STATUS == tipo);
            query = query.Where(p => p.CRM1_IN_ATIVO != 2);
            return query.ToList();
        }

        public CRM GetItemById(Int32 id)
        {
            IQueryable<CRM> query = Db.CRM;
            query = query.Where(p => p.CRM1_CD_ID == id);
            query = query.Include(p => p.CRM_COMENTARIO);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.CRM_PEDIDO_VENDA);
            query = query.Include(p => p.CRM_ACAO);
            query = query.Include(p => p.CRM_ANEXO);
            query = query.Include(p => p.FUNIL);
            query = query.Include(p => p.FUNIL.FUNIL_ETAPA);
            return query.FirstOrDefault();
        }

        public List<CRM> GetAllItens(Int32 idUsu)
        {
            IQueryable<CRM> query = Db.CRM;
            query = query.Where(p => p.CRM1_IN_ATIVO != 2);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            query = query.Include(p => p.FUNIL);
            query = query.Include(p => p.FUNIL.FUNIL_ETAPA);
            return query.ToList();
        }

        public List<CRM> GetAllItensAdm(Int32 idUsu)
        {
            IQueryable<CRM> query = Db.CRM;
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            query = query.Include(p => p.FUNIL);
            query = query.Include(p => p.FUNIL.FUNIL_ETAPA);
            return query.ToList();
        }

        public List<CRM> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32? temperatura, Int32? funil, String campanha, Int32 idAss)
        {
            List<CRM> lista = new List<CRM>();
            IQueryable<CRM> query = Db.CRM;
            if (status > 0)
            {
                query = query.Where(p => p.CRM1_IN_STATUS == status);
            }
            if (origem != null)
            {
                query = query.Where(p => p.ORIG_CD_ID == origem);
            }
            if (funil != null)
            {
                query = query.Where(p => p.FUNI_CD_ID == funil);
            }
            if (estrela != null)
            {
                query = query.Where(p => p.CRM1_IN_ESTRELA == estrela);
            }
            if (temperatura != null)
            {
                query = query.Where(p => p.CRM1_NR_TEMPERATURA == temperatura);
            }
            if (adic != null)
            {
                if (adic == 1)
                {
                    query = query.Where(p => p.CRM1_IN_ATIVO == 1);
                }
                else if (adic == 2)
                {
                    query = query.Where(p => p.CRM1_IN_ATIVO == 2);
                }
                else if (adic == 3)
                {
                    query = query.Where(p => p.CRM1_IN_ATIVO == 3);
                }
                else if (adic == 4)
                {
                    query = query.Where(p => p.CRM1_IN_ATIVO == 4);
                }
                else if (adic == 5)
                {
                    query = query.Where(p => p.CRM1_IN_ATIVO == 5);
                }
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CRM1_NM_NOME.Contains(nome) || p.CRM1_DS_DESCRICAO.Contains(nome));        
            }
            if (!String.IsNullOrEmpty(campanha))
            {
                query = query.Where(p => p.CRM1_NM_CAMPANHA.Contains(campanha));
            }
            //if (!String.IsNullOrEmpty(busca))
            //{
            //    query = query.Where(p => p.CLIENTE.CLIE_NM_NOME.Contains(busca) || p.CLIENTE.CLIE_NM_RAZAO.Contains(busca) || p.CLIENTE.CLIE_NR_CPF.Contains(busca));
            //}

            if (inicio != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CRM_ACAO.Where(m => m.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA) >= DbFunctions.TruncateTime(inicio));
            }
            if (final != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CRM_ACAO.Where(m => m.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA) <= DbFunctions.TruncateTime(final));
            }
            if (query != null)
            {
                query = query.Where(p => p.CRM1_IN_ATIVO != 2);
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CRM1_DT_CRIACAO);
                lista = query.ToList<CRM>();
            }
            return lista;
        }
    }
}
 