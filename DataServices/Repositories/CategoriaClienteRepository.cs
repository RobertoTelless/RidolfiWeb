using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class CategoriaClienteRepository : RepositoryBase<CATEGORIA_CLIENTE>, ICategoriaClienteRepository
    {
        public CATEGORIA_CLIENTE CheckExist(CATEGORIA_CLIENTE conta, Int32 idAss)
        {
            IQueryable<CATEGORIA_CLIENTE> query = Db.CATEGORIA_CLIENTE;
            query = query.Where(p => p.CACL_NM_NOME == conta.CACL_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CATEGORIA_CLIENTE GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_CLIENTE> query = Db.CATEGORIA_CLIENTE;
            query = query.Where(p => p.CACL_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_CLIENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<CATEGORIA_CLIENTE> query = Db.CATEGORIA_CLIENTE.Where(p => p.CACL_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CATEGORIA_CLIENTE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CATEGORIA_CLIENTE> query = Db.CATEGORIA_CLIENTE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 