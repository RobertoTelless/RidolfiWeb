using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContatoService : IServiceBase<CONTATO>
    {
        Int32 Create(CONTATO perfil, LOG log);
        Int32 Create(CONTATO perfil);
        Int32 Edit(CONTATO perfil, LOG log);
        Int32 Edit(CONTATO perfil);
        Int32 Delete(CONTATO perfil, LOG log);

        CONTATO CheckExistProt(String prot);
        CONTATO GetItemById(Int32 id);
        List<CONTATO> GetAllItens();
        List<CONTATO> GetAllItensAdm();
        List<CONTATO> ExecuteFilter(Int32? precatorio, Int32? beneficiario, Int32? quali, Int32? quem, String protocolo, DateTime? inicio, DateTime? final, String telefone, String agente, String campanha);

        QUALIFICACAO CheckExistQualificacao(String quali);
        QUEM_DESLIGOU CheckExistDesligamento(String quem);

        List<BENEFICIARIO> GetAllBeneficiarios();
        List<PRECATORIO> GetAllPrecatorios();
        List<QUALIFICACAO> GetAllQualificacao();
        List<QUEM_DESLIGOU> GetAllDesligamento();

        Int32 CreateQualificacao(QUALIFICACAO item);
        Int32 CreateDesligamento(QUEM_DESLIGOU item);
    }
}
