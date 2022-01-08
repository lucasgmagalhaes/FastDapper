using DapperOperations.Extensions;

namespace DapperOperations.Mapping
{
    public class MappedEntity<TEntity> : MappedEntity where TEntity : class, new()
    {
        public void Key(Func<TEntity, object> keyMapper)
        {
            var obj = keyMapper.Invoke(new TEntity());
            var props = obj.GetType().GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                ColumnsMap.Add(props[i].Name, props[i].Name.FormatByConvetion());
            }
        }

        public void Key(Func<TEntity, object> keyMapper, params string[] columnName)
        {
            var obj = keyMapper.Invoke(new TEntity());
            var props = obj.GetType().GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                ColumnsMap.Add(props[i].Name, columnName[i].FormatByConvetion());
            }
        }
    }
}
