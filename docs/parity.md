# Schema

##  Object types and fields

```graphql
type Character {
  name: String!
  appearsIn: [Episode]!
}
```

```cs
interface Character
{
    string Name { get; }
    NodeList<Episode?> AppearsIn { get; }
}
```

## Arguments

```graphql
type Starship {
  id: ID!
  name: String!
  length(unit: LengthUnit = METER): Float
}
```

```cs
interface Starship {
  ID Id { get; }
  string Name { get; }
  double? Length(LengthUnit? unit = LengthUnit.Meter);
}
```

## The Query and Mutation types

```graphql
schema {
  query: Query
  mutation: Mutation
}

type Query {
  hero(episode: Episode): Character
  droid(id: ID!): Droid
}
```

```cs
interface Schema: ISchema<Query, Mutation>
{
}

interface Query {
  Character? Hero(Episode? episode);
  Droid? Droid(ID id);
}
```

## Scalar types

```graphql
Int
Float
String
Boolean
ID
scalar Date
```

```cs
int
double
string
bool
struct ID { string Value; }
DateTime
```

## Enumeration types

```
enum Episode {
  NEWHOPE
  EMPIRE
  JEDI
}
```

```cs
enum Episode {
  Newhope,
  Empire,
  Jedi
}
```

## Lists and Non-Null

```graphql
nullableNumber: Int
number: Int!
listOfNullableNumbers: [Int]!
listOfNumbers: [Int!]!
nonnullableString: String!
droid: Droid!
listOfDroids: [Droid!]!

nullableListOfNullableNumbers: [Int]
nullableListOfNumbers: [Int!]
nullableString: String
```

```cs
int? NullableNumber;
int Number;
int?[] ListOfNullableNumbers;
int[] ListOfNumbers;
string NonnullableString;
Droid Droid;
NodeList<Droid> ListOfDroids;

//C#8+
int?[]? NullableListOfNullableNumbers;
int[]? NullableListOfNumbers;
string? NullableString;
//C#7-
int?[] NullableListOfNullableNumbers;
int[] NullableListOfNumbers;
string NullableString;
```

## Interfaces

```graphql
interface Character {
  id: ID!
  name: String!
  friends: [Character]
  appearsIn: [Episode]!
}
type Human implements Character {
  id: ID!
  name: String!
  friends: [Character]
  appearsIn: [Episode]!
  starships: [Starship]
  totalCredits: Int
}
type Droid implements Character {
  id: ID!
  name: String!
  friends: [Character]
  appearsIn: [Episode]!
  primaryFunction: String
}
```

```cs
interface Character {
  ID Id { get; }
  string Name { get; }
  NodeList<Character?>? Friends { get; }
  Episode?[] AppearsIn { get; }
}
interface Human: Character {
  NodeList<Starship?>? Starships { get; }
  int? TotalCredits { get; }
}
interface Droid: Character {
  string? PrimaryFunction { get; }
}
```

## Union types

TODO: find a better pattern

```graphql
union SearchResult = Human | Droid | Starship
```

```cs
struct SearchResult: IUnionOf<Human, Droid, Starship>
{
    public Human AsHuman() => default;
    public Droid AsDroid() => default;
    public Starship AsStarship() => default;
}
```

### Input types

```graphql
input ReviewInput {
  stars: Int!
  commentary: String
}
```

```cs
class ReviewInput {
  int Stars { get; set; }
  string? Commentary { get; set; }
}
```

# Queries

## Fields

```graphql
{
  hero {
    name
  }
}
```

```cs
from q in source select new {
    Hero = from hero in q.Hero() select new {
        hero.Name
    }
}
```

```graphql
{
  hero {
    name
    # Queries can have comments!
    friends {
      name
    }
  }
}
```

```cs
from q in source select new {
    Hero = from hero in q.Hero() select new {
        hero.Name,
        // Queries can have comments
        Friends = from friend in hero.Friends select new {
            friend.Name
        }
    }
}

source.Select(q => new {
    Hero = q.Hero().Select(hero => new {
        hero.Name,
        // Queries can have comments
        Friends = hero.Friends.Select(friend => new {
            friend.Name
        })
    })
})
```

### Arguments

```graphql
{
  human(id: "1000") {
    name
    height
  }
}

{
  human(id: "1000") {
    name
    height(unit: FOOT)
  }
}
```

```cs
from q in source select new {
    Human = from human in q.Human(id: "1000") select new {
        human.Name,
        Height = human.Height()
    }
}

from q in source select new {
    Human = from human in q.Human("1000") select new {
        human.Name,
        Height = human.Height(unit: LengthUnit.Foot)
    }
}
```

## Aliases

```graphql
{
  empireHero: hero(episode: EMPIRE) {
    name
  }
  jediHero: hero(episode: JEDI) {
    name
  }
}
```

```cs
from q in source select new {
    EmpireHero = from hero in q.Hero(Episode.Empire) select new {
        hero.Name
    },
    JediHero = from hero in q.Hero(episode: Episode.Jedi) select new {
        hero.Name
    }
}
```

## Fragments

```graphql
{
  leftComparison: hero(episode: EMPIRE) {
    ...comparisonFields
  }
  rightComparison: hero(episode: JEDI) {
    ...comparisonFields
  }
}

fragment comparisonFields on Character {
  name
  appearsIn
  friends {
    name
  }
}
```

```cs
from q in source
let comparisonFields = from f in source.Fragment<Character>() select new {
    f.Name,
    f.AppearsIn,
    Friends = from friend in f.Friends select new {
        friend.Name
    }
}
select new {
    LeftComparison = comparisonFields(q.Hero(Episode.Empire)),
    RightComparison = comparisonFields(q.Hero(Episode.Jedi))
}
```

> **Note** API for getting a fragment may be different. For example a static `QueryBuilder.Fragment<TFragment>()`.

## Operation name

```graphql
query HeroNameAndFriends {
  hero {
    name
    friends {
      name
    }
  }
}
```

```cs
from q in source.Operation("HeroNameAndFriends") select new {
    Hero = from hero in q.Hero() select new {
        hero.Name,
        Friends = from friend in hero.Friends select new {
            friend.Name
        }
    }
}
```

## Variables

```graphql
query HeroNameAndFriends($episode: Episode) {
  hero(episode: $episode) {
    name
    friends {
      name
    }
  }
}
```

```cs
source.Operation("HeroNameAndFriends",
    (Episode episode) => from q in source select new {
        Hero = from hero in q.Hero(episode) select new {
            hero.Name,
            Friends = from friend in hero.Friends select new {
                friend.Name
            }
        }
    }
)
```

### Default values

TODO

```graphql
query HeroNameAndFriends($episode: Episode = JEDI) {
  hero(episode: $episode) {
    name
    friends {
      name
    }
  }
}
```

## Directives

TODO

```
query Hero($episode: Episode, $withFriends: Boolean!) {
  hero(episode: $episode) {
    name
    friends @include(if: $withFriends) {
      name
    }
  }
}
```

## Mutations

TODO

```
mutation CreateReviewForEpisode($ep: Episode!, $review: ReviewInput!) {
  createReview(episode: $ep, review: $review) {
    stars
    commentary
  }
}
```

## Inline Fragments

TODO

```
query HeroForEpisode($ep: Episode!) {
  hero(episode: $ep) {
    name
    ... on Droid {
      primaryFunction
    }
    ... on Human {
      height
    }
  }
}
```

### Meta fields

TODO

```
{
  search(text: "an") {
    __typename
    ... on Human {
      name
    }
    ... on Droid {
      name
    }
    ... on Starship {
      name
    }
  }
}
```

