using System.Collections.Generic;

namespace ArdantOffical.Data.ModelVm
{//use for show tables data


    public interface IGenericModelsList
    {

        List<TEntity> GetListAsync<TEntity>()
            where TEntity : class;

    }
    public class GenericModelsLis : IGenericModelsList
    {

        public List<TEntity> GetListAsync<TEntity>() where TEntity : class
        {
            List<TEntity> ListData = new List<TEntity>();
            return ListData;

        }
    }
}




//public interface IGenericModelsList<T> where T:class
//{
//    //List<T> ModelList();
//    IEnumerable<T> ListData { get; set; }


//}
//public class GenericModelsList<T> : IGenericModelsList<T> where T : class
//{
//    public IEnumerable<T> ListData { get; set; } = new List<T>();

//}




