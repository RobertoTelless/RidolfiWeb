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
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;

namespace ERP_Condominios_Solution.Classes
{
    public class RecalculoCustos
    {
        private readonly IEmpresaAppService baseApp;
        private readonly IPlataformaEntregaAppService platApp;
        private readonly ITicketAppService tkApp;
        private readonly IAntecipacaoAppService antApp;
        private readonly IMaquinaAppService maqApp;
        private readonly IVendaMensalAppService venApp;
        private readonly IComissaoAppService comApp;

        public RecalculoCustos(IEmpresaAppService baseApps, IPlataformaEntregaAppService platApps, ITicketAppService tkApps, IAntecipacaoAppService antApps, IMaquinaAppService maqApps, IVendaMensalAppService venApps, IComissaoAppService comApps)
        {
            baseApp = baseApps;
            platApp = platApps;
            tkApp = tkApps;
            antApp = antApps;
            maqApp = maqApps;
            venApp = venApps;
            comApp = comApps;
        }

        public Int32 AtualizarCustos(EMPRESA empresa, USUARIO usuario, Int32 idAss)
        {

            try
            {
                // Recupera vendas do mes
                DateTime hoje = DateTime.Today.Date;
                List<VENDA_MENSAL> vendas = venApp.GetAllItens(idAss);
                vendas = vendas.Where(p => p.VEMA_DT_REFERENCIA.Value.Month == hoje.Month & p.VEMA_DT_REFERENCIA.Value.Year == hoje.Year).ToList();
                if (vendas.Count == 0)
                {
                    return 1;
                }

                // Apaga custos variaveis de vendas existentes
                List<EMPRESA_CUSTO_VARIAVEL> custos = baseApp.GetAllCustoVariavel(empresa.EMPR_CD_ID).Where(p => p.EMCV_IN_VENDA == 1).ToList();
                foreach (EMPRESA_CUSTO_VARIAVEL item in custos)
                {
                    item.EMCV_IN_ATIVO = 0;
                    Int32 voltaGG = baseApp.ValidateEditCustoVariavel(item);
                }

                // Inicializa
                Decimal total = vendas.Sum(p => p.VEMA_VL_TOTAL.Value);
                Decimal totalVendedor = vendas.Sum(p => p.VEMA_VL_VENDEDOR.Value);
                Decimal totalGerente = vendas.Sum(p => p.VEMA_VL_GERENTE.Value);
                Decimal totalOutros = vendas.Sum(p => p.VEMA_VL_OUTROS.Value);

                // Percorre vendas do mes
                foreach (VENDA_MENSAL item in vendas)
                {
                    // Calcula percentual
                    Decimal percentual = (item.VEMA_VL_TOTAL.Value * 100) /total;
                    Decimal? sub = 0;
                    Decimal taxa = 0;

                    // Recupera taxa
                    if (item.VEMA_IN_TIPO == 1)
                    {
                        taxa = 0;
                    }
                    if (item.VEMA_IN_TIPO == 2)
                    {
                        MAQUINA maq = maqApp.GetItemById(item.MAQN_CD_ID.Value);
                        taxa = maq.MAQN_PC_DEBITO;
                    }
                    if (item.VEMA_IN_TIPO == 3)
                    {
                        MAQUINA maq = maqApp.GetItemById(item.MAQN_CD_ID.Value);
                        taxa = maq.MAQN_PC_CREDITO;
                    }
                    if (item.VEMA_IN_TIPO == 4)
                    {
                        PLATAFORMA_ENTREGA plen = platApp.GetItemById(item.PLEN_CD_ID.Value);
                        taxa = plen.PLEN_PC_VENDA.Value;
                    }
                    if (item.VEMA_IN_TIPO == 5)
                    {
                        TICKET_ALIMENTACAO tik = tkApp.GetItemById(item.TICK_CD_ID.Value);
                        taxa = tik.TICK_PC_TAXA;
                    }

                    // Calcula
                    sub = (percentual * taxa) / 100;

                    // Monta nome
                    String nome = "Custo Variável - " + hoje.ToShortDateString() + " - ";
                    nome = nome + (item.VEMA_IN_TIPO == 1 ? "Dinheiro" : (item.VEMA_IN_TIPO == 2 ? "Débito" : (item.VEMA_IN_TIPO == 3 ? "Crédito" : (item.VEMA_IN_TIPO == 4 ? "Plataforma de Entrega" : "Ticket Alimentação"))));

                    // Grava custo
                    EMPRESA_CUSTO_VARIAVEL cus = new EMPRESA_CUSTO_VARIAVEL();
                    cus.EMPR_CD_ID = empresa.EMPR_CD_ID;
                    cus.EMCV_IN_ATIVO = 1;
                    cus.EMCV_IN_TIPO = item.VEMA_IN_TIPO;
                    cus.EMCV_IN_VENDA = 1;
                    cus.EMCV_PC_TAXA = taxa;
                    cus.EMCV_PC_PERCENTUAL_VENDA_DECIMAL = percentual;
                    cus.EMCV_NM_NOME = nome;
                    cus.EMCV_VL_VALOR = sub;
                    if (item.VEMA_IN_TIPO == 2 || item.VEMA_IN_TIPO == 3)
                    {
                        cus.MAQN_CD_ID = item.MAQN_CD_ID;
                    }
                    if (item.VEMA_IN_TIPO == 4)
                    {
                        cus.PLEN_CD_ID = item.PLEN_CD_ID;
                    }
                    if (item.VEMA_IN_TIPO == 5)
                    {
                        cus.TICK_CD_ID = item.TICK_CD_ID;
                    }
                    Int32 volta = baseApp.ValidateCreateCustoVariavel(cus, idAss);
                }

                // Acerta custos de vendas na empresa
                EMPRESA emp = new EMPRESA();
                emp.ASSI_CD_ID = empresa.ASSI_CD_ID;
                emp.EMPR_CD_ID = empresa.EMPR_CD_ID;
                emp.EMPR_DT_CADASTRO = empresa.EMPR_DT_CADASTRO;
                emp.EMPR_IN_ATIVO = empresa.EMPR_IN_ATIVO;
                emp.EMPR_IN_CALCULADO = empresa.EMPR_IN_CALCULADO;
                emp.EMPR_IN_OPERA_CARTAO = empresa.EMPR_IN_OPERA_CARTAO;
                emp.EMPR_IN_PAGA_COMISSAO = empresa.EMPR_IN_PAGA_COMISSAO;
                emp.EMPR_NM_BAIRRO = empresa.EMPR_NM_BAIRRO;
                emp.EMPR_NM_CIDADE = empresa.EMPR_NM_CIDADE;
                emp.EMPR_NM_COMPLEMENTO = empresa.EMPR_NM_COMPLEMENTO;
                emp.EMPR_NM_ENDERECO = empresa.EMPR_NM_ENDERECO;
                emp.EMPR_NM_NOME = empresa.EMPR_NM_NOME;
                emp.EMPR_NM_NUMERO = empresa.EMPR_NM_NUMERO;
                emp.EMPR_NM_OUTRA_MAQUINA = empresa.EMPR_NM_OUTRA_MAQUINA;
                emp.EMPR_NM_RAZAO = empresa.EMPR_NM_RAZAO;
                emp.EMPR_NR_CEP = empresa.EMPR_NR_CEP;
                emp.EMPR_NR_CNPJ = empresa.EMPR_NR_CNPJ;
                emp.EMPR_NR_CPF = empresa.EMPR_NR_CPF;
                emp.EMPR_NR_INSCRICAO_ESTADUAL = empresa.EMPR_NR_INSCRICAO_ESTADUAL;
                emp.EMPR_NR_INSCRICAO_MUNICIPAL = empresa.EMPR_NR_INSCRICAO_MUNICIPAL;
                emp.EMPR_PC_ANTECIPACAO = empresa.EMPR_PC_ANTECIPACAO;
                emp.EMPR_PC_CUSTO_ANTECIPACOES = empresa.EMPR_PC_CUSTO_ANTECIPACOES;
                emp.EMPR_PC_CUSTO_VARIAVEL_TOTAL = 0;
                emp.EMPR_PC_CUSTO_VARIAVEL_VENDA = 0;
                emp.EMPR_PC_VENDA_CREDITO = empresa.EMPR_PC_VENDA_CREDITO;
                emp.EMPR_PC_VENDA_DEBITO = empresa.EMPR_PC_VENDA_DEBITO;
                emp.EMPR_PC_VENDA_DINHEIRO = empresa.EMPR_PC_VENDA_DINHEIRO;
                emp.EMPR_VL_COMISSAO_GERENTE = empresa.EMPR_VL_COMISSAO_GERENTE;
                emp.EMPR_VL_COMISSAO_OUTROS = empresa.EMPR_VL_COMISSAO_OUTROS;
                emp.EMPR_VL_COMISSAO_VENDEDOR = empresa.EMPR_VL_COMISSAO_VENDEDOR;
                emp.EMPR_VL_FUNDO_PROPAGANDA = empresa.EMPR_VL_FUNDO_PROPAGANDA;
                emp.EMPR_VL_FUNDO_SEGURANCA = empresa.EMPR_VL_FUNDO_SEGURANCA;
                emp.EMPR_VL_IMPOSTO_MEI = empresa.EMPR_VL_IMPOSTO_MEI;
                emp.EMPR_VL_IMPOSTO_OUTROS = empresa.EMPR_VL_IMPOSTO_OUTROS;
                emp.EMPR_VL_PATRIMONIO_LIQUIDO = empresa.EMPR_VL_PATRIMONIO_LIQUIDO;
                emp.EMPR_VL_ROYALTIES = empresa.EMPR_VL_ROYALTIES;
                emp.EMPR_VL_TAXA_MEDIA = empresa.EMPR_VL_TAXA_MEDIA;
                emp.EMPR_VL_TAXA_MEDIA_DEBITO = empresa.EMPR_VL_TAXA_MEDIA_DEBITO;
                emp.RETR_CD_ID = empresa.RETR_CD_ID;
                emp.TIPE_CD_ID = empresa.TIPE_CD_ID;
                emp.UF_CD_ID = empresa.UF_CD_ID;
                emp.MAQN_CD_ID = empresa.MAQN_CD_ID;

                List<EMPRESA_CUSTO_VARIAVEL> custo = baseApp.GetAllCustoVariavel(empresa.EMPR_CD_ID);
                Decimal? valorVenda = custo.Where(p => p.EMCV_IN_VENDA == 1).Sum(p => p.EMCV_VL_VALOR);
                Decimal? valorOutros = custo.Where(p => p.EMCV_IN_VENDA == 0).Sum(p => p.EMCV_VL_VALOR);
                emp.EMPR_PC_CUSTO_VARIAVEL_VENDA = valorVenda;

                // Acerta custo total
                Decimal? extra = 0;
                extra = valorOutros;
                extra += emp.EMPR_VL_IMPOSTO_MEI;
                extra += emp.EMPR_VL_IMPOSTO_OUTROS;
                extra += emp.EMPR_VL_COMISSAO_GERENTE;
                extra += emp.EMPR_VL_COMISSAO_VENDEDOR;
                extra += emp.EMPR_VL_COMISSAO_OUTROS;
                extra += emp.EMPR_VL_ROYALTIES;
                extra += emp.EMPR_VL_FUNDO_PROPAGANDA;
                extra += emp.EMPR_VL_FUNDO_SEGURANCA;
                extra += valorVenda;
                emp.EMPR_PC_CUSTO_VARIAVEL_TOTAL = extra;

                Decimal? comVend = emp.EMPR_VL_COMISSAO_VENDEDOR;
                Decimal? comGer = emp.EMPR_VL_COMISSAO_GERENTE;
                Decimal? comOutros = emp.EMPR_VL_COMISSAO_OUTROS;

                // Acerta flag da empresa
                emp.EMPR_IN_CALCULADO = 1;
                Int32 volta1 = baseApp.ValidateEdit(emp, emp, usuario);

                // Grava Histórico de custos variaveis
                Int32 existe1 = 0;
                CUSTO_VARIAVEL_HISTORICO variavel = new CUSTO_VARIAVEL_HISTORICO();
                List<CUSTO_VARIAVEL_HISTORICO> ch1 = baseApp.GetAllHistorico(idAss);
                if (ch1.Count > 0)
                {
                    CUSTO_VARIAVEL_HISTORICO xx = new CUSTO_VARIAVEL_HISTORICO();
                    xx = ch1.Where(p => p.CVHI_DT_REFERENCIA.Value.Month == DateTime.Today.Date.Month & p.CVHI_DT_REFERENCIA.Value.Year == DateTime.Today.Date.Year).FirstOrDefault();
                    if (xx != null)
                    {
                        variavel = xx;
                        existe1 = 1;
                    }
                }
                variavel.ASSI_CD_ID = idAss;
                variavel.CVHI_DT_REFERENCIA = DateTime.Today.Date;
                variavel.CVHI_PC_VENDA = valorVenda;
                variavel.CVHI_PC_TOTAL = extra;
                variavel.CVHI_IN_ATIVO = 1;
                if (existe1 == 1)
                {
                    Int32 volta8 = baseApp.ValidateEditCustoVariavelHistorico(variavel);
                }
                else
                {
                    Int32 volta10 = baseApp.ValidateCreateCustoVariavelHistorico(variavel);
                }

                // Calcula taxa média credito
                if (emp.EMPRESA_MAQUINA.Count > 0)
                {
                    List<EMPRESA_MAQUINA> maq1 = emp.EMPRESA_MAQUINA.ToList();
                    Decimal somaC = 0;
                    Decimal somaD = 0;
                    Decimal taxaC = 0;
                    Decimal taxaD = 0;
                    Int32 quant = 0;
                    foreach (EMPRESA_MAQUINA em in maq1)
                    {
                        somaC += em.MAQUINA.MAQN_PC_CREDITO;
                        somaD += em.MAQUINA.MAQN_PC_DEBITO;
                        quant++;
                    }
                    taxaC = somaC / quant;
                    taxaD = somaD / quant;
                    emp.EMPR_VL_TAXA_MEDIA = taxaC;
                    emp.EMPR_VL_TAXA_MEDIA_DEBITO = taxaD;
                    Int32 volta3 = baseApp.ValidateEdit(emp, emp, usuario);
                }

                // Calcula custos em R$
                Decimal? custoVenda = total * (valorVenda / 100);
                Decimal? custoTotal = total * (extra / 100);

                // Grava custos
                Int32 existe3 = 0;
                CUSTO_HISTORICO hist = new CUSTO_HISTORICO();
                List<CUSTO_HISTORICO> ch = baseApp.GetAllCustoHistorico(idAss);
                if (ch.Count > 0)
                {
                    CUSTO_HISTORICO xxx = new CUSTO_HISTORICO();
                    xxx = ch.Where(p => p.CUHI_DT_REFERENCIA.Value.Month == DateTime.Today.Date.Month & p.CUHI_DT_REFERENCIA.Value.Year == DateTime.Today.Date.Year).FirstOrDefault();
                    if (xxx != null)
                    {
                        hist = xxx;
                        existe3 = 1;
                    }
                }
                hist.ASSI_CD_ID = idAss;
                hist.CUHI_DT_REFERENCIA = DateTime.Today.Date;
                hist.CUHI_PC_VARIAVEL = extra;
                hist.CUHI_PC_VENDAS = valorVenda;
                hist.CUHI_VL_CUSTO_VENDA = custoVenda;
                hist.CUHI_VL_CUSTO = custoTotal;
                hist.CUHI_VL_VENDAS = total;
                hist.CUHI_IN_ATIVO = 1;
                if (existe3 == 1)
                {
                    Int32 volta8 = baseApp.ValidateEditCustoHistorico(hist);
                }
                else
                {
                    Int32 volta10 = baseApp.ValidateCreateCustoHistorico(hist);
                }

                // Calcula comissoes
                Decimal? comissaoVendedor = totalVendedor * (comVend / 100);
                Decimal? comissaoGerente = total * (comGer / 100);
                Decimal? comissaoOutros = totalOutros * (comOutros / 100);
                
                // Grava Comissões
                Int32 existe2 = 0;
                COMISSAO com = new COMISSAO();
                List<COMISSAO> coh = comApp.GetAllItens(idAss);
                if (coh.Count > 0)
                {
                    COMISSAO xxx = new COMISSAO();
                    xxx = coh.Where(p => p.COMI_DT_REFERENCIA.Value.Month == DateTime.Today.Date.Month & p.COMI_DT_REFERENCIA.Value.Year == DateTime.Today.Date.Year).FirstOrDefault();
                    if (xxx != null)
                    {
                        com = xxx;
                        existe2 = 1;
                    }
                }

                com.ASSI_CD_ID = idAss;
                com.COMI_DT_REFERENCIA = DateTime.Today.Date;
                com.COMI_PC_GERENTE = emp.EMPR_VL_COMISSAO_GERENTE;
                com.COMI_PC_VENDEDOR = emp.EMPR_VL_COMISSAO_VENDEDOR;
                com.COMI_PC_OUTROS = emp.EMPR_VL_COMISSAO_OUTROS;
                com.COMI_VL_GERENTE = comissaoGerente;
                com.COMI_VL_VALOR = comissaoVendedor;
                com.COMI_VL_OUTROS = comissaoOutros;
                com.COMI_IN_ATIVO = 1;
                if (existe2 == 1)
                {
                    Int32 volta8 = comApp.ValidateEdit(com, com);
                }
                else
                {
                    Int32 volta10 = comApp.ValidateCreate(com);
                }

                // Encerra
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}