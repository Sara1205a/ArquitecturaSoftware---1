using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Dominio.Politicas;

public interface IDescuentoConvenio
{
    TipoConvenio Convenio { get; }
    decimal Aplicar(decimal valor);
}

public sealed class DescuentoParticular : IDescuentoConvenio
{
    public TipoConvenio Convenio => TipoConvenio.Particular;
    public decimal Aplicar(decimal valor) => valor; // paga el 100 %
}

public sealed class DescuentoEps : IDescuentoConvenio
{
    public TipoConvenio Convenio => TipoConvenio.Eps;
    public decimal Aplicar(decimal valor) => valor * 0.30m; // EPS cubre 70 %
}

public sealed class DescuentoPrepagada : IDescuentoConvenio
{
    public TipoConvenio Convenio => TipoConvenio.Prepagada;
    public decimal Aplicar(decimal valor) => valor * 0.10m; // Prepagada cubre 90 %
}