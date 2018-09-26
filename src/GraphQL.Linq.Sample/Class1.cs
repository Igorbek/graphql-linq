using System;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQL.Linq.Sample
{
    class LinqQueryBuilder<T>
    {
        public LinqQueryBuilder<R> Select<R>(Expression<Func<T, R>> selector) => default;
        public T Execute() => default;
    }

    interface ICompiledLinqQuery<T, R>
    {
        string CompiledQuery { get; }
        R ExecuteSync();
    }

    class LinqQueryStaticBuilder<T>
    {
        public LinqQueryStaticBuilder<R> Select<R>(Func<T, R> selector) => default;
        public T Execute() => default;

        public LinqQueryFragmentStaticBuilder<F> Fragment<F>() => default;

        public LinqQueryStaticBuilder<T> Operation(string operationName) => default;

        public LinqQueryStaticBuilder<R> Operation<T1, R>(string operationName, Func<T1, LinqQueryStaticBuilder<R>> builder) => default;

        public static ICompiledLinqQuery<T, R> GetCompiledQuery<R>(Func<LinqQueryStaticBuilder<T>, LinqQueryStaticBuilder<R>> builder) => default;
    }

    class LinqQueryFragmentStaticBuilder<T>
    {
        public Func<T, R> Select<R>(Func<T, R> selector) => default;
    }

    interface ISampleNode
    {
        int Count { get; }
        string Name { get; }
        ISampleNode[] Children(int from = default, int count = default);
    }

    public class Class1
    {
        public void Demo1()
        {
            var request = LinqQueryStaticBuilder<ISampleNode>.GetCompiledQuery(source =>
                from r in source
                select new
                {
                    r.Count,
                    r.Name,
                    Children = (
                        from child in r.Children(count: 10)
                        select new
                        {
                            child.Count,
                            child.Name
                        }
                    ).ToArray()
                }
            );

            int count = request.ExecuteSync().Children[0].Count;
        }
        public void Demo2()
        {
            var request =
                from r in new LinqQueryBuilder<ISampleNode>()
                select new
                {
                    r.Count,
                    r.Name,
                    Children = (
                      from child in r.Children(/* from: */ default, /* count: */ 10)
                      select new
                      {
                          child.Count,
                          child.Name
                      }
                    ).ToArray()
                };

            int count = request.Execute().Children[0].Count;
        }

        public void Demo3()
        {
            var query = new
            {
                count = 0, 
            };
        }
    }
}
