using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Linq
{
    public interface IObjectType { }
    public struct NodeList<T> { }
    public interface ISchema<TQuery, TMutation> { }
    public interface IUnionOf<T1, T2, T3> { }
    struct ID
    {
        string Value;

        ID(string id) { Value = id; }

        public static implicit operator string (ID id) => id.Value;
        public static implicit operator ID (string id) => new ID(id);
    }

    public static class GraphQLExtensions
    {
        public static R Select<T, R>(this T source, Func<T, R> selector) where T: IObjectType => default;
        public static R[] Select<T, R>(this NodeList<T> source, Func<T, R> selector) => default;
    }
}

namespace GraphQL.Linq.Sample
{
    interface Starship: IObjectType
    {
        ID Id { get; }
        string Name { get; }
        double? Length(LengthUnit? unit = LengthUnit.Meter);
    }

    interface Schema : ISchema<Query, Mutation>
    {
    }

    interface Query
    {
        Character Hero(Episode? episode = default);
        Droid Droid(ID id);
        Human Human(ID id);
    }

    interface Mutation
    {
    }

    enum Episode
    {
        Newhope,
        Empire,
        Jedi
    }

    enum LengthUnit { Meter, Inch, Foot }

    interface Character : IObjectType
    {
        ID Id { get; }
        string Name { get; }
        double Height(LengthUnit? unit = LengthUnit.Meter);
        NodeList<Character> Friends { get; }
        Episode?[] AppearsIn { get; }
    }

    interface Human : Character
    {
        NodeList<Starship> Starships { get; }
        int? TotalCredits { get; }
    }
    interface Droid : Character
    {
        string PrimaryFunction { get; }
    }

    struct SearchResult : IUnionOf<Human, Droid, Starship>
    {
        public Human AsHuman() => default;
        public Droid AsDroid() => default;
        public Starship AsStarship() => default;
    }

    class ReviewInput
    {
        int Stars { get; set; }
        string Commentary { get; set; }
    }
}
