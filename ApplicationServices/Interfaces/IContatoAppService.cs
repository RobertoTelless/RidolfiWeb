using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IContatoAppService : IAppServiceBase<CONTATO>
    {
        Int32 ValidateCreate(CONTATO perfil, USUARIO usuario);
        Int32 ValidateEdit(CONTATO perfil, CONTATO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(CONTATO item, CONTATO itemAntes);
        Int32 ValidateDelete(CONTATO perfil, USUARIO usuario);
        Int32 ValidateReativar(CONTATO perfil, USUARIO usuario);

        CONTATO CheckExistProt(String prot);
        CONTATO GetItemById(Int32 id);
        List<CONTATO> GetAllItens();
        List<CONTATO> GetAllItensAdm();

        QUALIFICACAO CheckExistQualificacao(String quali);
        QUEM_DESLIGOU CheckExistDesligamento(String quem);

        List<BENEFICIARIO> GetAllBeneficiarios();
        List<PRECATORIO> GetAllPrecatorios();
        List<QUALIFICACAO> GetAllQualificacao();
        List<QUEM_DESLIGOU> GetAllDesligamento();
        Int32 ExecuteFilter(Int32? precatorio, Int32? beneficiario, Int32? quali, Int32? quem, String protocolo, DateTime? inicio, DateTime? final, String telefone, String agente, String campanha, out List<CONTATO> objeto);

        Int32 ValidateCreateQualificacao(QUALIFICACAO item);
        Int32 ValidateCreateDesligamento(QUEM_DESLIGOU item);
    }
}
