using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.WorkClasses;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMensagemRepository : IRepositoryBase<MENSAGENS>
    {
        MENSAGENS CheckExist(MENSAGENS item, Int32 idAss);
        MENSAGENS GetItemById(Int32 id);
        List<MENSAGENS> GetAllItens(Int32 idAss);
        List<MENSAGENS> GetAllItensAdm(Int32 idAss);
        List<MENSAGENS> ExecuteFilterSMS(DateTime? envio, Int32 cliente, String texto, Int32 idAss);
        List<MENSAGENS> ExecuteFilterEMail(DateTime? envio, Int32 cliente, String texto, Int32 idAss);
        List<MENSAGENS> FilterMensagensNotSend(Int32? mensID, Enumerador.MensagemTipo mensagemTipo);

    }
}
