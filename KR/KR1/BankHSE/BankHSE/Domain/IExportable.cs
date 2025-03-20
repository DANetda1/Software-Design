using BankHSE.Exporters;

namespace BankHSE.Domain
{
    public interface IExportable
    {
        void Accept(IExportVisitor visitor);
    }
}