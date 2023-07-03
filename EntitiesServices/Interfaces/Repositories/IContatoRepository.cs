using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContatoRepository : IRepositoryBase<CONTATO>
    {
        CONTATO CheckExistProt(String prot);
        CONTATO GetItemById(Int32 id);
        List<CONTATO> GetAllItens();
        List<CONTATO> GetAllItensAdm();
        List<CONTATO> ExecuteFilter(Int32? precatorio, Int32? beneficiario, Int32? quali, Int32? quem, String protocolo, DateTime? inicio, DateTime? final, String telefone,  String agente, String campanha);
    }
}
