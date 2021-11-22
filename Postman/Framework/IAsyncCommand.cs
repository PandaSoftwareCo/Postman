using System.Threading.Tasks;
using System.Windows.Input;

namespace Postman.Framework
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
