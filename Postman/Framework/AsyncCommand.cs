using System;
using System.Threading.Tasks;

namespace Postman.Framework
{
    public class AsyncCommand : AsyncCommandBase
    {
        private readonly Func<object, Task> _command;
        //private readonly Func<Task> _command;
        private readonly Predicate<object> _canExecute;//

        public AsyncCommand(Func<object, Task> command, Predicate<object> canExecute = null)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public override Task ExecuteAsync(object parameter)
        {
            //return _command();
            return _command(parameter);
        }
    }
}
