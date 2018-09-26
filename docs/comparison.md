# Schema

```cs
interface IQuery
{
    int UserCount { get; }
    string WelcomeMessage { get; }
    int UserCount(bool disabled = false, bool admins = false);
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
    source.WelcomeMessage
}
```

to

```graphql
{
  userCount
  welcomeMessage
}
```

# Arguments

## no arguments

```cs
select new {
    source.UserCount()
}
```

to

```graphql
{
  userCount
}
```

## named arguments

```cs
select new {
    source.UserCount(admins: true)
}
```

to

```graphql
{
  userCount(admins: true)
}
```

## positional arguments

```cs
select new {
    source.UserCount(true)
}
```

positional arguments are being substituted with corresponding named arguments:

```graphql
{
  userCount(hidden: true)
}
```

# Aliases

```cs
select new {
    OtherName = source.UserCount,
    UserCount = source.UserCount,
    source.WelcomeMessage
}
```

when names have specified explicitly, they are used as aliases, even when the name is the same:

```graphql
{
  otherName: userCount
  userCount: userCount
  welcomeMessage
}
```

# Child nodes

```cs
select new {
    Me = from me in source.Me select new {
        me.Id,
        me.Name
    }
}
```

```graphql
{
  me {
    id
    name
  }
}
```
