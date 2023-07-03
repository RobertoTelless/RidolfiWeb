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
using System.Web.Configuration;

namespace DataServices.Repositories
{
    public class MunicipioRepository : RepositoryBase<MUNICIPIO>, IMunicipioRepository
    {
        public MUNICIPIO CheckExist(MUNICIPIO conta)
        {
            IQueryable<MUNICIPIO> query = Db.MUNICIPIO;
            query = query.Where(p => p.MUNI_NM_NOME == conta.MUNI_NM_NOME);
            query = query.Where(p => p.UF_CD_ID == conta.UF_CD_ID);
            return query.FirstOrDefault();
        }

        public MUNICIPIO GetItemById(Int32 id)
        {
            IQueryable<MUNICIPIO> query = Db.MUNICIPIO;
            query = query.Where(p => p.MUNI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MUNICIPIO> GetAllItens()
        {
            IQueryable<MUNICIPIO> query = Db.MUNICIPIO.Where(p => p.MUNI_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<MUNICIPIO> GetMunicipioByUF(Int32 uf)
        {
            IQueryable<MUNICIPIO> query = Db.MUNICIPIO.Where(p => p.MUNI_IN_ATIVO == 1);
            query = query.Where(p => p.UF_CD_ID == uf);
            return query.ToList();
        }
    }
}
 