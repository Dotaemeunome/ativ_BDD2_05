namespace AgendaWinForms
{
    public class Telefone
    {
        public string Tipo { get; set; }
        public string Numero { get; set; }
        public bool Principal { get; set; }

        public Telefone(string tipo, string numero, bool principal)
        {
            Tipo = tipo;
            Numero = numero;
            Principal = principal;
        }

        public override string ToString()
        {
            return Tipo + ": " + Numero + (Principal ? " (principal)" : "");
        }
    }
}