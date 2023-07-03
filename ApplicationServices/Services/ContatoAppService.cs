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
    public class ContatoAppService : AppServiceBase<CONTATO>, IContatoAppService
    {
        private readonly IContatoService _baseService;
        private readonly IConfiguracaoService _confService;

        public ContatoAppService(IContatoService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<CONTATO> GetAllItens()
        {
            List<CONTATO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<CONTATO> GetAllItensAdm()
        {
            List<CONTATO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public CONTATO GetItemById(Int32 id)
        {
            CONTATO item = _baseService.GetItemById(id);
            return item;
        }

        public CONTATO CheckExistProt(String protocolo)
        {
            CONTATO item = _baseService.CheckExistProt(protocolo);
            return item;
        }

        public QUALIFICACAO CheckExistQualificacao(String quali)
        {
            QUALIFICACAO item = _baseService.CheckExistQualificacao(quali);
            return item;
        }

        public QUEM_DESLIGOU CheckExistDesligamento(String quali)
        {
            QUEM_DESLIGOU item = _baseService.CheckExistDesligamento(quali);
            return item;
        }

        public List<BENEFICIARIO> GetAllBeneficiarios()
        {
            List<BENEFICIARIO> lista = _baseService.GetAllBeneficiarios();
            return lista;
        }

        public List<PRECATORIO> GetAllPrecatorios()
        {
            List<PRECATORIO> lista = _baseService.GetAllPrecatorios();
            return lista;
        }

        public List<QUALIFICACAO> GetAllQualificacao()
        {
            List<QUALIFICACAO> lista = _baseService.GetAllQualificacao();
            return lista;
        }

        public List<QUEM_DESLIGOU> GetAllDesligamento()
        {
            List<QUEM_DESLIGOU> lista = _baseService.GetAllDesligamento();
            return lista;
        }

        public Int32 ExecuteFilter(Int32? precatorio, Int32? beneficiario, Int32? quali, Int32? quem, String protocolo, DateTime? inicio, DateTime? final, String telefone,  String agente, String campanha, out List<CONTATO> objeto)
        {
            try
            {
                objeto = new List<CONTATO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(precatorio, beneficiario, quali, quem, protocolo, inicio, final, telefone, agente, campanha);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(CONTATO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.CONT_IN_ATIVO = 1;


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCONT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTATO>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONTATO item, CONTATO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCONT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTATO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CONTATO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONTATO item, CONTATO itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CONTATO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CONT_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCONT",
                    LOG_TX_REGISTRO = item.CONT_NR_PROTOCOLO
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CONTATO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CONT_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCONT",
                    LOG_TX_REGISTRO = item.CONT_NR_PROTOCOLO
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateQualificacao(QUALIFICACAO item)
        {
            try
            {
                item.QUAL_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateQualificacao(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateDesligamento(QUEM_DESLIGOU item)
        {
            try
            {
                item.QUDE_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateDesligamento(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
