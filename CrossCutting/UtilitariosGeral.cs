using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

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


    }
}
