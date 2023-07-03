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
    public class TelefoneRepository : RepositoryBase<TELEFONE>, ITelefoneRepository
    {
        public TELEFONE CheckExist(TELEFONE conta, Int32 idAss)
        {
            IQueryable<TELEFONE> query = Db.TELEFONE;
            query = query.Where(p => p.ELE_NM_NOME == conta.TELE_NM_NOME);
            query = query.Where(p => p.TELE_NR_TELEFONE == conta.TELE_NR_TELEFONE);
            query = query.Where(p => p.TELE_NM_CIDADE == conta.TELE_NM_CIDADE);
            return query.FirstOrDefault();
        }

        public TELEFONE GetItemById(Int32 id)
        {
            IQueryable<TELEFONES> query = Db.TELEFONES;
            query = query.Where(p => p.TELE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TELEFONES> GetAllItens(Int32 idAss)
        {
            IQueryable<TELEFONES> query = Db.TELEFONES.Where(p => p.TELE_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<TELEFONES> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TELEFONES> query = Db.TELEFONES;
            return query.ToList();
        }

        public List<TELEFONES> ExecuteFilter(Int32? catId, String nome, String telefone, String cidade, Int32? uf, String celular, String email, Int32 idAss)
        {
            List<TELEFONES> lista = new List<TELEFONES>();
            IQueryable<TELEFONES> query = Db.TELEFONES;
            if (catId != null)
            {
                query = query.Where(p => p.CATE_CD_ID == catId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.TELE_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.TELE_NM_EMAIL.Contains(email));
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.TELE_NM_CIDADE.Contains(cidade));
            }
            if (uf != null)
            {
                query = query.Where(p => p.UF_CD_ID == uf);
            }
            if (!String.IsNullOrEmpty(telefone))
            {
                query = query.Where(p => p.TELE_NR_TELEFONE.Contains(telefone));
            }
            if (!String.IsNullOrEmpty(celular))
            {
                query = query.Where(p => p.TELE_NR_CELULAR.Contains(celular));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.TELE_NM_NOME);
                lista = query.ToList<TELEFONES>();
            }
            return lista;
        }
    }
}
 