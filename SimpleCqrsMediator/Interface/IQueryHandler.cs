using System.Threading.Tasks;

namespace SimpleCqrsMediator.Interface
{
    public interface IQueryHandler<TQuery, TResult>
            where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}
