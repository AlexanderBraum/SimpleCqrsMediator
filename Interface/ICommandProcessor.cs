using System.Threading.Tasks;

namespace SimpleCqrsMediator.Interface
{
    public interface ICommandProcessor
    {
        Task ProcessAsync(ICommand command);

        Task<TResult> ProcessAsync<TResult>(ICommand<TResult> command);
    }
}
