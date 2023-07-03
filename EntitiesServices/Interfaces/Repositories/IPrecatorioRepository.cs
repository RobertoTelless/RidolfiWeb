using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPrecatorioRepository : IRepositoryBase<PRECATORIO>
    {
        PRECATORIO CheckExist(PRECATORIO item);
        PRECATORIO CheckExist(String item);
        PRECATORIO GetItemById(Int32 id);
        List<PRECATORIO> GetAllItens();
        List<PRECATORIO> GetAllItensAdm();
        List<PRECATORIO> ExecuteFilter(Int32? trf, Int32? beneficiario, Int32? advogado, Int32? natureza, Int32? estado, String nome, String ano, Int32? crm, Int32? pesquisa, Decimal? valor1, Decimal? valor2, Int32? situacao);
    }
}
