using System;
using System.Globalization;

namespace AgendaWinForms
{
    public class Data
    {
        private int dia;
        private int mes;
        private int ano;

        public int Dia { get { return dia; } set { dia = value; } }
        public int Mes { get { return mes; } set { mes = value; } }
        public int Ano { get { return ano; } set { ano = value; } }

        public Data() : this(1, 1, 1900) { }

        public Data(int dia, int mes, int ano)
        {
            setData(dia, mes, ano);
        }

        public void setData(int dia, int mes, int ano)
        {
            DateTime data;
            if (!DateTime.TryParseExact(
                string.Format("{0:00}/{1:00}/{2:0000}", dia, mes, ano),
                "dd/MM/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out data))
                throw new ArgumentException("Data inválida.");

            this.dia = data.Day;
            this.mes = data.Month;
            this.ano = data.Year;
        }

        public DateTime ParaDateTime()
        {
            return new DateTime(ano, mes, dia);
        }

        public override string ToString()
        {
            return string.Format("{0:00}/{1:00}/{2:0000}", dia, mes, ano);
        }
    }
}