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
    public class ClienteRepository : RepositoryBase<CLIENTE>, IClienteRepository
    {
        public CLIENTE CheckExist(CLIENTE cliente, Int32 idAss)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE;
            if (cliente.CLIE_NR_CPF != null)
            {
                query = query.Where(p => p.CLIE_NR_CPF == cliente.CLIE_NR_CPF);
            }
            if (cliente.CLIE_NR_CNPJ != null)
            {
                query = query.Where(p => p.CLIE_NR_CNPJ == cliente.CLIE_NR_CNPJ);
            }
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CLIENTE CheckExistDoctos(String cpf, String cnpj, Int32 idAss)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE;
            if (cpf != null)
            {
                query = query.Where(p => p.CLIE_NR_CPF == cpf);
            }
            if (cnpj != null)
            {
                query = query.Where(p => p.CLIE_NR_CNPJ == cnpj);
            }
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CLIENTE GetByEmail(String email)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE.Where(p => p.CLIE_IN_ATIVO == 1);
            query = query.Where(p => p.CLIE_NM_EMAIL == email);
            query = query.Include(p => p.CLIENTE_ANEXO);
            query = query.Include(p => p.CLIENTE_CONTATO);
            return query.FirstOrDefault();
        }

        public CLIENTE GetItemById(Int32 id)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE;
            query = query.Where(p => p.CLIE_CD_ID == id);
            query = query.Include(p => p.CLIENTE_ANEXO);
            query = query.Include(p => p.CLIENTE_CONTATO);
            query = query.Include(p => p.CLIENTE_ANOTACAO);
            query = query.Include(p => p.CLIENTE_REFERENCIA);
            query = query.Include(p => p.CRM_PEDIDO_VENDA);
            query = query.Include(p => p.CLIENTE_QUADRO_SOCIETARIO);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<CLIENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE.Where(p => p.CLIE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CLIE_NM_NOME);
            return query.ToList();
        }

        public List<CLIENTE> GetAllItensUltimos(Int32 num, Int32 idAss)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE.Where(p => p.CLIE_IN_ATIVO == 1 & p.ASSI_CD_ID == idAss).OrderByDescending(p => p.CLIE_DT_ULTIMO_USO).Take(num);
            return query.ToList();
        }

        public List<CLIENTE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CLIE_NM_NOME);
            return query.ToList();
        }

        public List<CLIENTE> ExecuteFilter(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32? filial, Int32? usu, Int32 idAss)
        {
            List<CLIENTE> lista = new List<CLIENTE>();
            IQueryable<CLIENTE> query = Db.CLIENTE;
            if (id != 0)
            {
                query = query.Where(p => p.CLIE_CD_ID == id);
            }
            if (catId != null & catId > 0)
            {
                query = query.Where(p => p.CACL_CD_ID == catId);
            }
            if (filial != null & filial > 0)
            {
                query = query.Where(p => p.EMPR_CD_ID == filial);
            }
            if (usu != null & usu > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == filial);
            }
            if (!String.IsNullOrEmpty(razao))
            {
                query = query.Where(p => p.CLIE_NM_RAZAO.Contains(razao));
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CLIE_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.CLIE_NR_CPF == cpf);
            }
            if (!String.IsNullOrEmpty(cnpj))
            {
                query = query.Where(p => p.CLIE_NR_CNPJ == cnpj);
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.CLIE_NM_EMAIL.Contains(email));
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.CLIE_NM_CIDADE.Contains(cidade));
            }
            if (uf != null & uf > 0)
            {
                query = query.Where(p => p.UF_CD_ID == uf);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CLIE_NM_NOME);
                lista = query.ToList<CLIENTE>();
            }
            return lista;
        }

        public List<CLIENTE> FiltrarContatos(GRUPO grupo, Int32 idAss)
        {
            List<CLIENTE> lista = new List<CLIENTE>();
            IQueryable<CLIENTE> query = Db.CLIENTE;
            if (grupo.SEXO_CD_ID != null)
            {
                query = query.Where(p => p.SEXO_CD_ID == grupo.SEXO_CD_ID);
            }
            if (grupo.CACL_CD_ID != null)
            {
                query = query.Where(p => p.CACL_CD_ID == grupo.CACL_CD_ID);
            }
            if (!String.IsNullOrEmpty(grupo.GRUP_NM_CIDADE))
            {
                query = query.Where(p => p.CLIE_NM_CIDADE.Contains(grupo.GRUP_NM_CIDADE));
            }
            if (grupo.UF_CD_ID != null)
            {
                query = query.Where(p => p.UF_CD_ID == grupo.UF_CD_ID);
            }
            if (grupo.GRUP_DT_NASCIMENTO != null)
            {
                query = query.Where(p => p.CLIE_DT_NASCIMENTO == grupo.GRUP_DT_NASCIMENTO);
            }
            else
            {
                if (grupo.GRUP_NR_DIA != null)
                {
                    Int32 dia = Convert.ToInt32(grupo.GRUP_NR_DIA);
                    query = query.Where(p => DbFunctions.TruncateTime(p.CLIE_DT_NASCIMENTO).Value.Day == dia);
                }
                if (grupo.GRUP_NR_MES != null)
                {
                    Int32 mes = Convert.ToInt32(grupo.GRUP_NR_MES);
                    query = query.Where(p => DbFunctions.TruncateTime(p.CLIE_DT_NASCIMENTO).Value.Month == mes);
                }
                if (grupo.GRUP_NR_ANO != null)
                {
                    Int32 ano = Convert.ToInt32(grupo.GRUP_NR_ANO);
                    query = query.Where(p => DbFunctions.TruncateTime(p.CLIE_DT_NASCIMENTO).Value.Year == ano);
                }
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CLIE_NM_NOME);
                lista = query.ToList<CLIENTE>();
            }
            return lista;
        }

    }
}
 