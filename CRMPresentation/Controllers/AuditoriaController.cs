using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution;
using CRMPresentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text;
using System.Net;
using CrossCutting;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using ERP_Condominios_Solution.Classes;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        LOG objeto = new LOG();
        LOG objetoAntes = new LOG();
        List<LOG> listaMaster = new List<LOG>();
        LOG_EXCECAO_NOVO objetoExc = new LOG_EXCECAO_NOVO();
        LOG_EXCECAO_NOVO objetoExcAntes = new LOG_EXCECAO_NOVO();
        List<LOG_EXCECAO_NOVO> listaMasterExc = new List<LOG_EXCECAO_NOVO>();
        String extensao;

        public AuditoriaController(ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps)
        {
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        [HttpGet]
        public ActionResult MontarTelaLog()
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                if ((List<LOG>)Session["ListaLog"] == null)
                {
                    listaMaster = logApp.GetAllItensMesCorrente(idAss);
                    Session["ListaLog"] = listaMaster;
                    Session["FiltroLog"] = null;
                    Session["MensagemLonga"] = 0;
                }
                ViewBag.Listas = (List<LOG>)Session["ListaLog"];
                ViewBag.Logs = ((List<LOG>)Session["ListaLog"]).Count;
                ViewBag.LogsDataCorrente = logApp.GetAllItensDataCorrente(idAss).Count;
                ViewBag.LogsMesCorrente = ((List<LOG>)Session["ListaLog"]).Count;
                List<LOG> listAnt = logApp.GetAllItensMesAnterior(idAss);
                ViewBag.LogsMesAnterior = listAnt.Count;

                // Mensagens
                if ((Int32)Session["MensLog"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensLog"] == 10)
                {
                    String frase = (String)Session["NumLogBkp"] + CRMSys_Base.ResourceManager.GetString("M0320", CultureInfo.CurrentCulture);
                    ModelState.AddModelError("", frase);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AccLOG1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Abre view
                Session["MensLog"] = 0;
                Session["VoltaLog"] = 1;
                objeto = new LOG();
                objeto.LOG_DT_DATA = DateTime.Today.Date;
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroLog()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaLog"] = null;
            Session["FiltroLog"] = null;
            return RedirectToAction("MontarTelaLog");
        }

        public ActionResult VerTodosLog()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = logApp.GetAllItens(idAss);
                Session["ListaLog"] = listaMaster;
                Session["MensagemLonga"] = 1;
                return RedirectToAction("MontarTelaLog");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarLog(LOG item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.LOG_NM_OPERACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.LOG_NM_OPERACAO);

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOG> listaObj = new List<LOG>();
                Session["FiltroLog"] = item;
                Tuple<Int32, List<LOG>, Boolean> volta = logApp.ExecuteFilterTuple(item.USUA_CD_ID, item.LOG_DT_DATA, item.LOG_NM_OPERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLog"] = 1;
                    return RedirectToAction("MontarTelaLog");
                }

                // Sucesso
                listaMaster = volta.Item2;
                Session["ListaLog"] = listaMaster;
                return RedirectToAction("MontarTelaLog");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerLog(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Recupera log
                LOG item = logApp.GetById(id);

                // Prepara JSON
                if (item.LOG_TX_REGISTRO.Substring(0,1) == "{") 
                {
                    // Agenda
                    if (item.LOG_NM_OPERACAO.Substring(3,4) == "AGEN")
                    {
                        AGENDA antes = JsonConvert.DeserializeObject<AGENDA>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            AGENDA antes1 = JsonConvert.DeserializeObject<AGENDA>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Configuracao
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "CONF")
                    {
                        CONFIGURACAO antes = JsonConvert.DeserializeObject<CONFIGURACAO>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            CONFIGURACAO antes1 = JsonConvert.DeserializeObject<CONFIGURACAO>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Empresa
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "EMPR")
                    {
                        EMPRESA antes = JsonConvert.DeserializeObject<EMPRESA>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            EMPRESA antes1 = JsonConvert.DeserializeObject<EMPRESA>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Cliente
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "CLIE")
                    {
                        CLIENTE antes = JsonConvert.DeserializeObject<CLIENTE>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            CLIENTE antes1 = JsonConvert.DeserializeObject<CLIENTE>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Template E-Mail
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "TEEM")
                    {
                        TEMPLATE_EMAIL antes = JsonConvert.DeserializeObject<TEMPLATE_EMAIL>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            TEMPLATE_EMAIL antes1 = JsonConvert.DeserializeObject<TEMPLATE_EMAIL>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Template Proposta
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "TEPR")
                    {
                        TEMPLATE_PROPOSTA antes = JsonConvert.DeserializeObject<TEMPLATE_PROPOSTA>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            TEMPLATE_PROPOSTA antes1 = JsonConvert.DeserializeObject<TEMPLATE_PROPOSTA>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Processos
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "CRM1")
                    {
                        CRM antes = JsonConvert.DeserializeObject<CRM>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            CRM antes1 = JsonConvert.DeserializeObject<CRM>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Ações
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "CRAC")
                    {
                        CRM_ACAO antes = JsonConvert.DeserializeObject<CRM_ACAO>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            CRM_ACAO antes1 = JsonConvert.DeserializeObject<CRM_ACAO>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Propostas
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "CRPV")
                    {
                        CRM_PEDIDO_VENDA antes = JsonConvert.DeserializeObject<CRM_PEDIDO_VENDA>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            CRM_PEDIDO_VENDA antes1 = JsonConvert.DeserializeObject<CRM_PEDIDO_VENDA>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Tarefas
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "TARE")
                    {
                        TAREFA antes = JsonConvert.DeserializeObject<TAREFA>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            TAREFA antes1 = JsonConvert.DeserializeObject<TAREFA>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Usuarios
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "USUA")
                    {
                        USUARIO antes = JsonConvert.DeserializeObject<USUARIO>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            USUARIO antes1 = JsonConvert.DeserializeObject<USUARIO>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }

                    // Agenda
                    if (item.LOG_NM_OPERACAO.Substring(3, 4) == "AGEN")
                    {
                        AGENDA antes = JsonConvert.DeserializeObject<AGENDA>(item.LOG_TX_REGISTRO);
                        String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        String json1 = String.Empty;
                        if (item.LOG_TX_REGISTRO_ANTES != null)
                        {
                            AGENDA antes1 = JsonConvert.DeserializeObject<AGENDA>(item.LOG_TX_REGISTRO_ANTES);
                            json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        }

                        item.LOG_TX_REGISTRO = json;
                        item.LOG_TX_REGISTRO_ANTES = json1;
                    }
                }

                // Prepara view
                LogViewModel vm = Mapper.Map<LOG, LogViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseLog()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaLog"] == 2)
            {
                Session["AbaProduto"] = 12;
                return RedirectToAction("VoltarAnexoProduto", "Produto");
            }
            return RedirectToAction("MontarTelaLog");
        }

        //[HttpGet]
        //public ActionResult GerenciarLog()
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        USUARIO usuario = new USUARIO();
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        if ((USUARIO)Session["UserCredentials"] != null)
        //        {
        //            usuario = (USUARIO)Session["UserCredentials"];

        //            // Verfifica permissão
        //            if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
        //            {
        //                Session["MensCustoFixo"] = 2;
        //                return RedirectToAction("MontarTelaCustoFixo");
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Login", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        // Configuracao
        //        CONFIGURACAO conf = CarregaConfiguracaoGeral();
        //        Double? inicio = Convert.ToDouble(conf.CONF_NR_DIAS_LOG * -1);
        //        Double? final = Convert.ToDouble(conf.CONF_NR_DIAS_FIM_LOG * -1);

        //        // Listas
        //        ViewBag.Usus = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");

        //        // Mensagens
        //        if (Session["MensLog"] != null)
        //        {
        //            if ((Int32)Session["MensLog"] == 2)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0319", CultureInfo.CurrentCulture));
        //            }
        //        }

        //        // Prepara view
        //        GerenciaLogViewModel item = new GerenciaLogViewModel();
        //        item.DATA_INICIO = DateTime.Today.Date.AddDays(inicio.Value);
        //        item.DATA_FINAL = DateTime.Today.Date.AddDays(final.Value);
        //        List<LOG> lista = logApp.GetLogByFaixa(item.DATA_INICIO, item.DATA_FINAL, idAss);
        //        item.LISTA = lista;
        //        item.CONTA = lista.Count;
        //        return View(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Auditoria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //[HttpPost]
        //public ActionResult GerenciarLog(GerenciaLogViewModel vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if ((String)Session["Ativa"] == null)
        //            {
        //                return RedirectToAction("Logout", "ControleAcesso");
        //            }

        //            // Critica
        //            if (vm.DATA_INICIO > vm.DATA_FINAL)
        //            {
        //                Session["MensLog"] = 2;
        //                return RedirectToAction("GerenciarLog", "Log");
        //            }

        //            // Processa
        //            var volta = ProcessaGerenciaLog(vm);

        //            // Retorno
        //            Session["MensLog"] = 10;
        //            List<LOG> listaMaster = new List<LOG>();
        //            Session["ListaLog"] = null;
        //            return RedirectToAction("MontarTelaLog");

        //        }
        //        catch (Exception ex)
        //        {
        //            LogError(ex.Message);
        //            ViewBag.Message = ex.Message;
        //            Session["TipoVolta"] = 2;
        //            Session["VoltaExcecao"] = "Log";
        //            Session["Excecao"] = ex;
        //            Session["ExcecaoTipo"] = ex.GetType().ToString();
        //            GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //            Int32 voltaX = grava.GravarLogExcecao(ex, "Log", "SysPrec", 1, (USUARIO)Session["UserCredentials"]);
        //            return RedirectToAction("TrataExcecao", "BaseAdmin");
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        //public async Task<Int32> ProcessaGerenciaLog(GerenciaLogViewModel vm)
        //{
        //    try
        //    {
        //        Int32 idAss = (Int32)Session["IdAssinante"];
        //        ViewBag.Usus = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");

        //        var volta = await ProcessaGerenciaLogTrata(vm);
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<Int32> ProcessaGerenciaLogTrata(GerenciaLogViewModel vm)
        //{
        //    try
        //    {
        //        Int32 idAss = (Int32)Session["IdAssinante"];
        //        ViewBag.Usus = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");

        //        // Recupera logs
        //        List<LOG> logs = logApp.GetAllItens(idAss).Where(p => p.LOG_DT_DATA > vm.DATA_INICIO & p.LOG_DT_DATA < vm.DATA_FINAL).ToList();

        //        // Processa registros
        //        Int32 num = 0;
        //        foreach (LOG log in logs)
        //        {
        //            // Monta log de backup
        //            LOG_BACKUP linha = new LOG_BACKUP();
        //            linha.LOG_CD_ID = log.LOG_CD_ID;
        //            linha.ASSI_CD_ID = log.ASSI_CD_ID;
        //            linha.USUA_CD_ID = log.USUA_CD_ID;
        //            linha.LOG_DT_DATA = log.LOG_DT_DATA;
        //            linha.LOG_NM_OPERACAO = log.LOG_NM_OPERACAO;
        //            linha.LOG_TX_REGISTRO = log.LOG_TX_REGISTRO;
        //            linha.LOG_TX_REGISTRO_ANTES = log.LOG_TX_REGISTRO_ANTES;
        //            linha.LOG_IN_SISTEMA = log.LOG_IN_SISTEMA;
        //            linha.LOG_IN_ATIVO = log.LOG_IN_ATIVO;

        //            // Grava backup
        //            Int32 volta = logBkpApp.ValidateCreate(linha);

        //            // Remove da base principal
        //            logApp.Remove(log);
        //            num++;
        //        }
        //        Session["NumLogBkp"] = num;
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public CONFIGURACAO CarregaConfiguracaoGeral()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                CONFIGURACAO conf = new CONFIGURACAO();
                if (Session["Configuracao"] == null)
                {
                    conf = confApp.GetAllItems(idAss).FirstOrDefault();
                }
                else
                {
                    if ((Int32)Session["ConfAlterada"] == 1)
                    {
                        conf = confApp.GetAllItems(idAss).FirstOrDefault();
                    }
                    else
                    {
                        conf = (CONFIGURACAO)Session["Configuracao"];
                    }
                }
                Session["ConfAlterada"] = 0;
                Session["Configuracao"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<USUARIO> CarregaUsuario()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> conf = new List<USUARIO>();
                if (Session["Usuarios"] == null)
                {
                    conf = usuApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["UsuarioAlterada"] == 1)
                    {
                        conf = usuApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<USUARIO>)Session["Usuarios"];
                    }
                }
                Session["UsuarioAlterada"] = 0;
                Session["Usuarios"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

    }
}