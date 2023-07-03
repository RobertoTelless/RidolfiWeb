using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class AssinanteCnpjAppService : AppServiceBase<ASSINANTE_QUADRO_SOCIETARIO>, IAssinanteCnpjAppService
    {
        private readonly IAssinanteCnpjService _baseService;

        public AssinanteCnpjAppService(IAssinanteCnpjService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public ASSINANTE_QUADRO_SOCIETARIO CheckExist(ASSINANTE_QUADRO_SOCIETARIO cqs)
        {
            ASSINANTE_QUADRO_SOCIETARIO item = _baseService.CheckExist(cqs);
            return item;
        }

        public List<ASSINANTE_QUADRO_SOCIETARIO> GetAllItens()
        {
            List<ASSINANTE_QUADRO_SOCIETARIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<ASSINANTE_QUADRO_SOCIETARIO> GetByCliente(ASSINANTE cliente)
        {
            List<ASSINANTE_QUADRO_SOCIETARIO> lista = _baseService.GetByCliente(cliente);
            return lista;
        }

        public Int32 ValidateCreate(ASSINANTE_QUADRO_SOCIETARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.ASQS_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
