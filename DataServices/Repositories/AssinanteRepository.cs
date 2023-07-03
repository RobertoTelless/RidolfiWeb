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
    public class AssinanteRepository : RepositoryBase<ASSINANTE>, IAssinanteRepository
    {
        public ASSINANTE CheckExist(ASSINANTE conta)
        {
            IQueryable<ASSINANTE> query = Db.ASSINANTE;
            if (conta.ASSI_NR_CPF != null)
            {
                query = query.Where(p => p.ASSI_NR_CPF == conta.ASSI_NR_CPF);
            }
            if (conta.ASSI_NR_CNPJ != null)
            {
                query = query.Where(p => p.ASSI_NR_CNPJ == conta.ASSI_NR_CNPJ);
            }
            query = query.Where(p => p.ASSI_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public ASSINANTE GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE> query = Db.ASSINANTE;
            query = query.Where(p => p.ASSI_CD_ID == id);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.ASSINANTE_PLANO);
            return query.FirstOrDefault();
        }

        public List<ASSINANTE> GetAllItens()
        {
            IQueryable<ASSINANTE> query = Db.ASSINANTE.Where(p => p.ASSI_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<ASSINANTE> GetAllItensAdm()
        {
            IQueryable<ASSINANTE> query = Db.ASSINANTE;
            return query.ToList();
        }

        public List<ASSINANTE> ExecuteFilter(Int32? tipo, String nome, String cpf, String cnpj, String cidade, Int32? uf, Int32? status)
        {
            List<ASSINANTE> lista = new List<ASSINANTE>();
            IQueryable<ASSINANTE> query = Db.ASSINANTE;
            if (tipo > 0 & tipo != null)
            {
                query = query.Where(p => p.TIPE_CD_ID == tipo);
            }
            if (status > 0 & status != null)
            {
                query = query.Where(p => p.ASSI_IN_STATUS == status);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.ASSI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.ASSI_NR_CPF == cpf);
            }
            if (!String.IsNullOrEmpty(cnpj))
            {
                query = query.Where(p => p.ASSI_NR_CNPJ == cnpj);
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.ASSI_NM_CIDADE.Contains(cidade));
            }
            if (uf > 0 & uf != null)
            {
                query = query.Where(p => p.UF_CD_ID == uf);
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.ASSI_NM_NOME);
                lista = query.ToList<ASSINANTE>();
            }
            return lista;
        }

        public List<ASSINANTE_PAGAMENTO> ExecuteFilterAtraso(String nome, String cpf, String cnpj, String cidade, Int32? uf)
        {
            List<ASSINANTE_PAGAMENTO> lista = new List<ASSINANTE_PAGAMENTO>();
            IQueryable<ASSINANTE_PAGAMENTO> query = Db.ASSINANTE_PAGAMENTO;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NR_CPF == cpf);
            }
            if (!String.IsNullOrEmpty(cnpj))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NR_CNPJ == cnpj);
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NM_CIDADE.Contains(cidade));
            }
            if (uf > 0 & uf != null)
            {
                query = query.Where(p => p.ASSINANTE.UF_CD_ID == uf);
            }
            if (query != null)
            {
                lista = query.ToList<ASSINANTE_PAGAMENTO>();
            }
            return lista;
        }

        public List<ASSINANTE_PLANO> ExecuteFilterVencidos(String nome, String cpf, String cnpj, String cidade, Int32? uf)
        {
            List<ASSINANTE_PLANO> lista = new List<ASSINANTE_PLANO>();
            IQueryable<ASSINANTE_PLANO> query = Db.ASSINANTE_PLANO;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NR_CPF == cpf);
            }
            if (!String.IsNullOrEmpty(cnpj))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NR_CNPJ == cnpj);
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NM_CIDADE.Contains(cidade));
            }
            if (uf > 0 & uf != null)
            {
                query = query.Where(p => p.ASSINANTE.UF_CD_ID == uf);
            }
            if (query != null)
            {
                lista = query.ToList<ASSINANTE_PLANO>();
            }
            return lista;
        }

        public List<ASSINANTE_PLANO> ExecuteFilterVencer30(String nome, String cpf, String cnpj, String cidade, Int32? uf)
        {
            List<ASSINANTE_PLANO> lista = new List<ASSINANTE_PLANO>();
            IQueryable<ASSINANTE_PLANO> query = Db.ASSINANTE_PLANO;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NR_CPF == cpf);
            }
            if (!String.IsNullOrEmpty(cnpj))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NR_CNPJ == cnpj);
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.ASSINANTE.ASSI_NM_CIDADE.Contains(cidade));
            }
            if (uf > 0 & uf != null)
            {
                query = query.Where(p => p.ASSINANTE.UF_CD_ID == uf);
            }
            if (query != null)
            {
                lista = query.ToList<ASSINANTE_PLANO>();
            }
            return lista;
        }

    }
}
 