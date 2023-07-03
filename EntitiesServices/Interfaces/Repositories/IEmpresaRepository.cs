using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEmpresaRepository : IRepositoryBase<EMPRESA>
    {
        EMPRESA CheckExist(EMPRESA item, Int32 idAss);
        EMPRESA GetItemById(Int32 id);
        EMPRESA GetItemByAssinante(Int32 id);
        List<EMPRESA> GetAllItens(Int32 idAss);
        List<EMPRESA> GetAllItensAdm(Int32 idAss);
        List<EMPRESA> ExecuteFilter(String nome, Int32? idAss);
    }
}

