namespace Experimentation.Logic.Mapper
{
    public interface IDtoToEntityMapper<in TDto, out TEntity>
    {
        TEntity Map(TDto model);
    }
}