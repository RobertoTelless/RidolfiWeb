using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMRepository : IRepositoryBase<CRM>
    {
        CRM CheckExist(CRM item, Int32 idUsu, Int32 idAss);
        List<CRM> GetByDate(DateTime data, Int32 idAss);
        List<CRM> GetByUser(Int32 user);
        List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss);
        CRM GetItemById(Int32 id);
        List<CRM> GetAllItens(Int32 idAss);
        List<CRM> GetAllItensAdm(Int32 idAss);
        List<CRM> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32? temperatura, Int32? funil, String campanha, Int32 idAss);
    }
}
