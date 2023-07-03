using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMensagemService : IServiceBase<MENSAGENS>
    {
        Int32 Create(MENSAGENS item, LOG log);
        Int32 Create(MENSAGENS item);
        Int32 Edit(MENSAGENS item, LOG log);
        Int32 Edit(MENSAGENS item);
        Int32 Delete(MENSAGENS item, LOG log);

        MENSAGENS CheckExist(MENSAGENS item, Int32 idAss);
        MENSAGENS GetItemById(Int32 id);
        List<MENSAGENS> GetAllItens(Int32 idAss);
        List<MENSAGENS> GetAllItensAdm(Int32 idAss);
        MENSAGEM_ANEXO GetAnexoById(Int32 id);
        List<MENSAGENS> ExecuteFilterSMS(DateTime? envio, Int32 cliente, String texto, Int32 idAss);
        List<MENSAGENS> ExecuteFilterEMail(DateTime? envio, Int32 cliente, String texto, Int32 idAss);
        List<RESULTADO_ROBOT> GetAllEnviosRobot(Int32 idAss);
        List<RESULTADO_ROBOT> ExecuteFilterRobot(Int32? tipo, DateTime? inicio, DateTime? final, String cliente, String email, String celular, Int32? status, Int32 idAss);

        List<TEMPLATE_SMS> GetAllTemplatesSMS(Int32 idAss);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss);
        //List<POSICAO> GetAllPosicao();

        MENSAGENS_DESTINOS GetDestinoById(Int32 id);
        Int32 EditDestino(MENSAGENS_DESTINOS item);
        Int32 CreateDestino(MENSAGENS_DESTINOS item);
    }
}
