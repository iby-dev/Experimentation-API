using System.Collections.Generic;
using Experimentation.Domain.Entities;

namespace Experimentation.Logic.ViewModels
{
    public class ListViewModel<TEntity> where TEntity : class, IEntity
    {
        public IEnumerable<TEntity> Items { get; private set; }

        public ListViewModel(IEnumerable<TEntity> data )
        {
            Items = data;
        }
    }
}