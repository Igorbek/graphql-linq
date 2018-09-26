using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Linq.Sample
{
    class SampleQueries
    {
        LinqQueryStaticBuilder<Query> source;

        void Fields()
        {
            var query1 = from q in source select new {
                Hero = from hero in q.Hero() select new
                {
                    hero.Name
                }
            };

            var query2 = from q in source
                         select new
                         {
                             Hero = from hero in q.Hero()
                                    select new
                                    {
                                        hero.Name,
                                        // Queries can have comments
                                        Friends = from friend in hero.Friends
                                                  select new
                                                  {
                                                      friend.Name
                                                  }
                                    }
                         };

            query2 = source.Select(q => new
            {
                Hero = q.Hero().Select(hero => new
                {
                    hero.Name,
                    // Queries can have comments
                    Friends = hero.Friends.Select(friend => new
                    {
                        friend.Name
                    })
                })
            });
        }

        void Arguments()
        {
            var query = from q in source select new {
                Human = from human in q.Human(id: "1000") select new {
                    human.Name,
                    Height = human.Height()
                }
            };

            query = from q in source select new {
                Human = from human in q.Human(id: "1000") select new {
                    human.Name,
                    Height = human.Height(unit: LengthUnit.Foot)
                }
            };
        }

        void Aliases()
        {
            var query = from q in source select new {
                EmpireHero = from hero in q.Hero(Episode.Empire) select new {
                    hero.Name
                },
                JediHero = from hero in q.Hero(episode: Episode.Jedi) select new {
                    hero.Name
                }
            };
        }

        void Fragments()
        {
            var query =
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
                };
        }

        void OperationName()
        {
            var query = from q in source.Operation("HeroNameAndFriends") select new {
                Hero = from hero in q.Hero() select new {
                    hero.Name,
                    Friends = from friend in hero.Friends select new {
                        friend.Name
                    }
                }
            };
        }

        void Variables()
        {
            var query = source.Operation("HeroNameAndFriends",
                (Episode episode) => from q in source select new {
                    Hero = from hero in q.Hero(episode) select new {
                        hero.Name,
                        Friends = from friend in hero.Friends select new {
                            friend.Name
                        }
                    }
                });
        }
    }
}
