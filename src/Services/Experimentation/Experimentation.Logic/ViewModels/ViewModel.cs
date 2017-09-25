using Experimentation.Domain.Entities;

namespace Experimentation.Logic.ViewModels
{
    public class ViewModel<TEntity> where TEntity : class, IEntity
    {
        public TEntity Item { get; private set; }

        public ViewModel(TEntity data)
        {
            Item = data;
        }
    }
}