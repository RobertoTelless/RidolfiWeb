using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRecursividadeRepository : IRepositoryBase<RECURSIVIDADE>
    {
        RECURSIVIDADE CheckExist(RECURSIVIDADE item, Int32 idAss);
        RECURSIVIDADE GetItemById(Int32 id);
        List<RECURSIVIDADE> GetAllItens(Int32 idAss);
        List<RECURSIVIDADE> GetAllItensAdm(Int32 idAss);
        List<RECURSIVIDADE> ExecuteFilter(Int32? tipoMensagem, String nome, DateTime? dataInico, DateTime? dataFim, String texto, Int32 idAss);
    }
}
