using System.Threading.Tasks;

namespace SimpleCqrsMediator.Interface
{

    public interface IQueryProcessor
    {
        Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query);
    }
}
