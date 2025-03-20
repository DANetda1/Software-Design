using BankHSE.Domain;

namespace BankHSE.Exporters
{
    public interface IExportVisitor
    {
        void Visit(BankAccount account);
        void Visit(Category category);
        void Visit(Operation operation);

        void SaveToFile(string filePath);
    }
}