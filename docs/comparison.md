# Schema

```cs
interface IQuery
{
    int UserCount { get; }
    Node<IPerson> Me { get; }
    NodeArray<IPerson> User(int id);
}

interface IPerson
{
    int Id { get; }
    string Name { get; }
    Node<IPerson> Manager { get; }
}
```

# Simple query

```cs
select new {
    source.UserCount,
}
```

to

```graphql
{
  
}
```



# Arguments

```cs
select new {
    Children = source.Children(first: 10).Select(children => new {
        children.ThirdField
    })
}

select new {
    Children = from children in source.Children(first: 10) select new {
        children.ThirdField
    }
}

```