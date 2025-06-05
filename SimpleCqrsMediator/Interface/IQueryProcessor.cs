using System.Threading.Tasks;

namespace SimpleCqrsMediator.Interface
{

    public interface IQueryProcessor
    {
        Task<TResult> ProcessAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery;
    }
}
