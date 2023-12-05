using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Trunking.V1;

namespace CrossCutting
{
    public static class UtilitariosGeral
    {
        public static String[] GetListaCores()
        {
            try
            {
                String[] array = new String[20] {"#cd9d6d", "#cdc36d", "#a0cfff", "#fffee4", "#e4fff0", "#e4eeff", "#ffe4ee", "#faffe4", "#cfd4be", "#d4bec4", "#cd9d6d", "#cdc36d", "#a0cfff", "#fffee4", "#e4fff0", "#e4eeff", "#ffe4ee", "#faffe4", "#cfd4be", "#d4bec4"};

                return array;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DateTime? ProximoVencimento(Int32? vencimento, Int32? periodo, DateTime? inicio, DateTime? final)
        {
            try
            {
                DateTime hoje = DateTime.Today.Date;
                Int32 dia = hoje.Day;
                Int32 mes = hoje.Month;
                Int32 ano = hoje.Year;
                Int32 proxMes = 0;
                Int32 proxAno = 0;
                DateTime? proximo = null;
                if (vencimento != null & (vencimento < 1 || vencimento > 31))
                {
                    return null;
                }
                if (inicio == null)
                {
                    return null;
                }
                if (vencimento != null)
                {
                    if (vencimento < dia)
                    {
                        if (mes != 12)
                        {
                            proxMes = mes + 1;
                            proxAno = ano;
                        }
                        else
                        {
                            proxMes = 1;
                            proxAno = ano + 1;
                        }
                        String datax = vencimento.ToString() + "/" + proxMes.ToString() + "/" + proxAno.ToString();
                        proximo = Convert.ToDateTime(datax);
                    }
                    if (vencimento >= dia)
                    {
                        proxMes = mes;
                        proxAno = ano;
                        String datax = vencimento.ToString() + "/" + proxMes.ToString() + "/" + proxAno.ToString();
                        proximo = Convert.ToDateTime(datax);
                    }
                    if (final != null)
                    {
                        if (!InRange(proximo, inicio, final))
                        {
                            return null;
                        }
                    }
                }
                return proximo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DateTime? ProximoVencimentoSemanal(Int32? diaSemana, Int32? periodo, DateTime? inicio, DateTime? final)
        {
            try
            {
                DateTime hoje = DateTime.Today.Date;
                Int32 dia = hoje.Day;
                Int32 mes = hoje.Month;
                Int32 ano = hoje.Year;
                Int32 proxMes = 0;
                Int32 proxAno = 0;
                Int32 dayofweek = Convert.ToInt32(hoje.DayOfWeek);
                DateTime? proximo = null;
                if (diaSemana != null & (diaSemana < 0 || diaSemana > 6))
                {
                    return null;
                }
                if (inicio == null)
                {
                    return null;
                }
                if (diaSemana != null)
                {
                    if (dayofweek == diaSemana)
                    {
                        Int32? dif = diaSemana - dayofweek;
                        proximo = hoje;
                    }
                    else
                    {
                        Int32? dif = diaSemana - dayofweek;
                        proximo = hoje.AddDays(dif.Value);
                        if (dif < 0)
                        {
                            proximo = proximo.Value.AddDays(7);
                        }
                    }
                    if (final != null)
                    {
                        if (!InRange(proximo, inicio, final))
                        {
                            return null;
                        }
                    }
                }
                return proximo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool InRange(this DateTime? dateToCheck, DateTime? startDate, DateTime? endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }

        public static Decimal? ValorAnual(Decimal? valor, DateTime? inicio, DateTime? final, Int32? periodo)
        {
            try
            {
                DateTime hoje = DateTime.Today.Date;
                Int32 dia = hoje.Day;
                Int32 mes = hoje.Month;
                Int32 ano = hoje.Year;
                if (valor == null || valor == 0)
                {
                    return 0;
                }
                if (inicio == null)
                {
                    return 0;
                }
                Int32 meses = 0;
                Decimal? total = 0;

                if (periodo == 30)
                {
                    if (inicio.Value.Year == final.Value.Year)
                    {
                        meses = MonthDifference(final, inicio) + 1;
                        total = valor * meses;
                    }
                    else
                    {
                        if (inicio.Value.Year == DateTime.Today.Date.Year)
                        {
                            DateTime? finalCalc = Convert.ToDateTime("31/12" + "/" + inicio.Value.Year);
                            meses = MonthDifference(finalCalc, inicio) + 1;
                            total = valor * meses;
                        }
                        else if (final.Value.Year == DateTime.Today.Date.Year)
                        {
                            DateTime? finalCalc = null;
                            DateTime? inicioCalc = Convert.ToDateTime("01/01" + "/" + DateTime.Today.Date.Year);
                            if (final.Value.Month != 2)
                            {
                                finalCalc = Convert.ToDateTime("30/" + final.Value.Month + "/" + final.Value.Year);
                            }
                            else
                            {
                                finalCalc = Convert.ToDateTime("28/" + final.Value.Month + "/" + final.Value.Year);
                            }
                            meses = MonthDifference(finalCalc, inicioCalc) + 1;
                            total = valor * meses;
                        }
                        else
                        {
                            DateTime? inicioCalc = Convert.ToDateTime("01/01" + "/" + DateTime.Today.Date.Year);
                            DateTime? finalCalc = Convert.ToDateTime("31/12" + "/" + DateTime.Today.Date.Year);
                            meses = MonthDifference(finalCalc, inicioCalc) + 1;
                            total = valor * meses;
                        }
                    }
                }
                else
                {
                    if (inicio.Value.Year == final.Value.Year)
                    {
                        meses = GetVariousDiff(inicio, final, periodo.Value);
                        total = valor * meses;
                    }
                    else
                    {
                        if (inicio.Value.Year == DateTime.Today.Date.Year)
                        {
                            DateTime? finalCalc = Convert.ToDateTime("31/12" + "/" + inicio.Value.Year);
                            meses = GetVariousDiff(inicio, finalCalc, periodo.Value);
                            total = valor * meses;
                        }
                        else if (final.Value.Year == DateTime.Today.Date.Year)
                        {
                            DateTime? finalCalc = Convert.ToDateTime("01/01" + "/" + final.Value.Year);
                            meses = GetVariousDiff(inicio, finalCalc, periodo.Value);
                            total = valor * meses;
                        }
                        else
                        {
                            DateTime? inicioCalc = Convert.ToDateTime("01/01" + "/" + DateTime.Today.Date.Year);
                            DateTime? finalCalc = Convert.ToDateTime("31/12" + "/" + DateTime.Today.Date.Year);
                            meses = GetVariousDiff(inicioCalc, finalCalc, periodo.Value);
                            total = valor * meses;
                        }
                    }
                }
                return total;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static Int32 MonthDifference(this DateTime? lValue, DateTime? rValue)
        {
            return (lValue.Value.Month - rValue.Value.Month) + 12 * (lValue.Value.Year - rValue.Value.Year);
        }

        public static Int32 GetVariousDiff(DateTime? dtStart, DateTime? dtEnd, Int32 periodo)
        {
            var totalDays = (dtEnd.Value - dtStart.Value).TotalDays;
            var weeks = (Int32)totalDays / periodo;
            var hasRemainder = totalDays % periodo > 0;
            if (hasRemainder)
            {
                if (!(dtStart.Value.DayOfWeek.Equals(DayOfWeek.Saturday) && dtEnd.Value.DayOfWeek.Equals(DayOfWeek.Sunday)))
                {
                    weeks++;
                }
            }
            return weeks;
        }

        public static String DiaSemana(Int32 dia)
        {
            String sem = dia == 1 ? "Domingo" : (dia == 2 ? "2a Feira" : (dia == 3 ? "3a Feira" : (dia == 4 ? "4a Feira" : (dia == 5 ? "5a Feira" : (dia == 6 ? "6a Feira" : "Sábado")))));
            return sem;
        }

        public static String CleanStringGeral(String dirtyString)
        {
            if (dirtyString == null || dirtyString == String.Empty)
            {
                return dirtyString;    
            }
            HashSet<char> removeChars = new HashSet<char>("?&^$#@%*!()+.,:;<>_*|/");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
            {
                if (!removeChars.Contains(c))
                {
                    if (c != '"')
                    {
                        result.Append(c);
                    }
                    else
                    {
                        result.Append(" ");
                    }
                }
                else
                {
                    result.Append(" ");
                }
            }
            result = result.Replace("\r\n", "<br />");
            result = result.Replace("\\r\\n", "<br />");
            result = result.Replace(" p ", " ");
            result = result.Replace(" b ", " ");
            result = result.Replace(" br ", " ");

            return result.ToString();
        }

        public static String CleanStringTexto(String dirtyString)
        {
            if (dirtyString == null || dirtyString == String.Empty)
            {
                return dirtyString;
            }
            HashSet<char> removeChars = new HashSet<char>("^#_|");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
            {
                if (!removeChars.Contains(c))
                {
                    if (c != '"')
                    {
                        result.Append(c);
                    }
                    else
                    {
                        result.Append(" ");
                    }
                }
                else
                {
                    result.Append(" ");
                }
            }
            result = result.Replace("\r\n", "<br />");
            result = result.Replace("\\r\\n", "<br />");
            result = result.Replace(" p ", " ");
            result = result.Replace(" b ", " ");
            result = result.Replace(" br ", " ");

            return result.ToString();
        }

        public static String CleanStringPhone(String dirtyString)
        {
            if (dirtyString == null || dirtyString == String.Empty)
            {
                return dirtyString;
            }
            HashSet<char> removeChars = new HashSet<char>("?&^$#@%*!,.:;<>_*|/");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
            {
                if (!removeChars.Contains(c))
                {
                    if (c != '"')
                    {
                        result.Append(c);
                    }
                    else
                    {
                        result.Append(" ");
                    }
                }
                else
                {
                    result.Append(" ");
                }
            }
            result = result.Replace("\r\n", "<br />");
            result = result.Replace("\\r\\n", "<br />");
            result = result.Replace(" p ", " ");
            result = result.Replace(" b ", " ");
            result = result.Replace(" br ", " ");
            return result.ToString();
        }

        public static String CleanStringDocto(String dirtyString)
        {
            if (dirtyString == null || dirtyString == String.Empty)
            {
                return dirtyString;
            }
            HashSet<char> removeChars = new HashSet<char>("?&^$#@%*!()+,:;<>_*|");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
            {
                if (!removeChars.Contains(c))
                {
                    if (c != '"')
                    {
                        result.Append(c);
                    }
                    else
                    {
                        result.Append(" ");
                    }
                }
                else
                {
                    result.Append(" ");
                }
            }
            result = result.Replace("\r\n", "<br />");
            result = result.Replace("\\r\\n", "<br />");
            result = result.Replace(" p ", " ");
            result = result.Replace(" b ", " ");
            result = result.Replace(" br ", " ");
            return result.ToString();
        }

        public static String CleanStringMail(String dirtyString)
        {
            if (dirtyString == null || dirtyString == String.Empty)
            {
                return dirtyString;
            }
            HashSet<char> removeChars = new HashSet<char>("?&^$#%*!()+,:;<>*|/");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
            {
                if (!removeChars.Contains(c))
                {
                    if (c != '"')
                    {
                        result.Append(c);
                    }
                    else
                    {
                        result.Append(" ");
                    }
                }
                else
                {
                    result.Append(" ");
                }
            }
            result = result.Replace("\r\n", "<br />");
            result = result.Replace("\\r\\n", "<br />");
            result = result.Replace(" p ", " ");
            result = result.Replace(" b ", " ");
            result = result.Replace(" br ", " ");
            return result.ToString();
        }

        public static String CleanStringDate(String dirtyString)
        {
            if (dirtyString == null || dirtyString == String.Empty)
            {
                return dirtyString;
            }
            HashSet<char> removeChars = new HashSet<char>("?&^$#@%*!()+.,:;<>_*|");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
            {
                if (!removeChars.Contains(c))
                {
                    if (c != '"')
                    {
                        result.Append(c);
                    }
                    else
                    {
                        result.Append(" ");
                    }
                }
                else
                {
                    result.Append(" ");
                }
            }
            result = result.Replace("\r\n", "<br />");
            result = result.Replace("\\r\\n", "<br />");
            result = result.Replace(" p ", " ");
            result = result.Replace(" b ", " ");
            result = result.Replace(" br ", " ");
            return result.ToString();
        }

        public static String CleanStringSenha(String dirtyString)
        {
            if (dirtyString == null || dirtyString == String.Empty)
            {
                return dirtyString;
            }
            HashSet<char> removeChars = new HashSet<char>("?^*!()+-.,:;<>|/");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
            {
                if (!removeChars.Contains(c))
                {
                    if (c != '"')
                    {
                        result.Append(c);
                    }
                    else
                    {
                        result.Append(" ");
                    }
                }
                else
                {
                    result.Append(" ");
                }
            }
            result = result.Replace("\r\n", "<br />");
            result = result.Replace("\\r\\n", "<br />");
            result = result.Replace(" p ", " ");
            result = result.Replace(" b ", " ");
            result = result.Replace(" br ", " ");
            return result.ToString();
        }

        public static String CleanStringLink(String dirtyString)
        {
            if (dirtyString == null || dirtyString == String.Empty)
            {
                return dirtyString;
            }
            HashSet<char> removeChars = new HashSet<char>("?&^$#@%*!()+,;<>_*|");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
            {
                if (!removeChars.Contains(c))
                {
                    if (c != '"')
                    {
                        result.Append(c);
                    }
                    else
                    {
                        result.Append(" ");
                    }
                }
                else
                {
                    result.Append(" ");
                }
            }
            result = result.Replace("\r\n", "<br />");
            result = result.Replace("\\r\\n", "<br />");
            result = result.Replace(" p ", " ");
            result = result.Replace(" b ", " ");
            result = result.Replace(" br ", " ");
            return result.ToString();
        }


    }
}
