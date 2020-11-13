// <copyright file="FromJsonVisitor.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Protocol.Json.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meta;

    public class FromJsonVisitor : Json.IVisitor
    {
        private readonly ISession session;
        private IMetaPopulation metaPopulation;

        private readonly Stack<Data.IExtent> extents;
        private readonly Stack<Data.IPredicate> predicates;
        private readonly Stack<Data.Result> results;
        private readonly Stack<Data.Fetch> fetches;
        private readonly Stack<Data.Step> steps;
        private readonly Stack<Data.Node> nodes;
        private readonly Stack<Data.Sort> sorts;

        public FromJsonVisitor(ISession session)
        {
            this.session = session;
            this.metaPopulation = this.session.Database.ObjectFactory.MetaPopulation;

            this.extents = new Stack<Data.IExtent>();
            this.predicates = new Stack<Data.IPredicate>();
            this.results = new Stack<Data.Result>();
            this.fetches = new Stack<Data.Fetch>();
            this.steps = new Stack<Data.Step>();
            this.nodes = new Stack<Data.Node>();
            this.sorts = new Stack<Data.Sort>();
        }

        public Data.Pull Pull { get; private set; }

        public void VisitExtent(Extent visited)
        {
            Data.IExtentOperator extentOperator = null;

            switch (visited.Kind)
            {
                case ExtentKind.Extent:
                    if (!visited.ObjectType.HasValue)
                    {
                        throw new Exception("Unknown extent kind " + visited.Kind);
                    }

                    var objectType = (IComposite)this.metaPopulation.Find(visited.ObjectType.Value);
                    var extent = new Data.Extent(objectType);

                    this.extents.Push(extent);

                    if (visited.Predicate != null)
                    {
                        visited.Predicate.Accept(this);
                        extent.Predicate = this.predicates.Pop();
                    }

                    break;

                case ExtentKind.Union:
                    extentOperator = new Data.Union();
                    break;

                case ExtentKind.Except:
                    extentOperator = new Data.Except();
                    break;

                case ExtentKind.Intersect:
                    extentOperator = new Data.Intersect();
                    break;

                default:
                    throw new Exception("Unknown extent kind " + visited.Kind);
            }

            if (extentOperator != null)
            {
                this.extents.Push(extentOperator);

                if (visited.Operands?.Length > 0)
                {
                    var length = visited.Operands.Length;

                    extentOperator.Operands = new Data.IExtent[length];
                    for (var i = 0; i < length; i++)
                    {
                        var operand = visited.Operands[i];
                        operand.Accept(this);
                        extentOperator.Operands[i] = this.extents.Pop();
                    }
                }
            }
        }

        public void VisitFetch(Fetch visited)
        {
            var fetch = new Data.Fetch(this.session.Database.MetaPopulation);

            this.fetches.Push(fetch);

            if (visited.Step != null)
            {
                visited.Step.Accept(this);
                fetch.Step = this.steps.Pop();
            }

            if (visited.Include?.Length > 0)
            {
                fetch.Include = new Data.Node[visited.Include.Length];
                for (var i = 0; i < visited.Include.Length; i++)
                {
                    visited.Include[i].Accept(this);
                    fetch.Include[i] = this.nodes.Pop();
                }
            }
        }

        public void VisitNode(Node visited)
        {
            var propertyType = (IPropertyType)this.metaPopulation.FindAssociationType(visited.AssociationType) ?? this.metaPopulation.FindRoleType(visited.RoleType);
            var node = new Data.Node(propertyType);

            this.nodes.Push(node);

            if (visited.Nodes?.Length > 0)
            {
                foreach (var childNode in visited.Nodes)
                {
                    childNode.Accept(this);
                    node.Add(this.nodes.Pop());
                }
            }
        }

        public void VisitPredicate(Predicate visited)
        {
            switch (visited.Kind)
            {
                case PredicateKind.And:
                    var and = new Data.And
                    {
                        Dependencies = visited.Dependencies,
                    };

                    this.predicates.Push(and);

                    if (visited.Operands?.Length > 0)
                    {
                        var length = visited.Operands.Length;

                        and.Operands = new Data.IPredicate[length];
                        for (var i = 0; i < length; i++)
                        {
                            var operand = visited.Operands[i];
                            operand.Accept(this);
                            and.Operands[i] = this.predicates.Pop();
                        }
                    }

                    break;

                case PredicateKind.Or:
                    var or = new Data.Or
                    {
                        Dependencies = visited.Dependencies
                    };

                    this.predicates.Push(or);

                    if (visited.Operands?.Length > 0)
                    {
                        var length = visited.Operands.Length;

                        or.Operands = new Data.IPredicate[length];
                        for (var i = 0; i < length; i++)
                        {
                            var operand = visited.Operands[i];
                            operand.Accept(this);
                            or.Operands[i] = this.predicates.Pop();
                        }
                    }

                    break;

                case PredicateKind.Not:
                    var not = new Data.Not
                    {
                        Dependencies = visited.Dependencies,
                    };

                    this.predicates.Push(not);

                    if (visited.Operand != null)
                    {
                        visited.Operand.Accept(this);
                        not.Operand = this.predicates.Pop();
                    }

                    break;

                default:
                    var associationType = this.metaPopulation.FindAssociationType(visited.AssociationType);
                    var roleType = this.metaPopulation.FindRoleType(visited.RoleType);
                    var propertyType = (IPropertyType)associationType ?? roleType;

                    switch (visited.Kind)
                    {
                        case PredicateKind.InstanceOf:

                            var instanceOf = new Data.Instanceof(visited.ObjectType != null ? (IComposite)this.session.Database.MetaPopulation.Find(visited.ObjectType.Value) : null)
                            {
                                Dependencies = visited.Dependencies,
                                PropertyType = propertyType,
                            };

                            this.predicates.Push(instanceOf);
                            break;

                        case PredicateKind.Exists:

                            var exists = new Data.Exists
                            {
                                Dependencies = visited.Dependencies,
                                PropertyType = propertyType,
                                Parameter = visited.Parameter,
                            };

                            this.predicates.Push(exists);
                            break;

                        case PredicateKind.Contains:

                            var contains = new Data.Contains
                            {
                                Dependencies = visited.Dependencies,
                                PropertyType = propertyType,
                                Parameter = visited.Parameter,
                                Object = this.session.Instantiate(visited.Object),
                            };

                            this.predicates.Push(contains);
                            break;

                        case PredicateKind.ContainedIn:

                            var containedIn = new Data.ContainedIn(propertyType)
                            {
                                Dependencies = visited.Dependencies,
                                Parameter = visited.Parameter
                            };

                            this.predicates.Push(containedIn);

                            if (visited.Objects != null)
                            {
                                containedIn.Objects = visited.Objects.Select(this.session.Instantiate).ToArray();
                            }
                            else if (visited.Extent != null)
                            {
                                visited.Extent.Accept(this);
                                containedIn.Extent = this.extents.Pop();
                            }

                            break;

                        case PredicateKind.Equals:

                            var equals = new Data.Equals(propertyType)
                            {
                                Dependencies = visited.Dependencies,
                                Parameter = visited.Parameter
                            };

                            this.predicates.Push(equals);

                            if (visited.Object != null)
                            {
                                equals.Object = this.session.Instantiate(visited.Object);
                            }
                            else if (visited.Value != null)
                            {
                                var value = UnitConvert.Parse(((IRoleType)propertyType).ObjectType.Id, visited.Value);
                                equals.Value = value;
                            }

                            break;

                        case PredicateKind.Between:

                            var between = new Data.Between(roleType)
                            {
                                Dependencies = visited.Dependencies,
                                Parameter = visited.Parameter,
                                Values = visited.Values?.Select(v => UnitConvert.Parse(roleType.ObjectType.Id, v)).ToArray(),
                            };

                            this.predicates.Push(between);

                            break;

                        case PredicateKind.GreaterThan:

                            var greaterThan = new Data.GreaterThan(roleType)
                            {
                                Dependencies = visited.Dependencies,
                                Parameter = visited.Parameter,
                                Value = UnitConvert.Parse(roleType.ObjectType.Id, visited.Value),
                            };

                            this.predicates.Push(greaterThan);

                            break;

                        case PredicateKind.LessThan:

                            var lessThan = new Data.LessThan(roleType)
                            {
                                Dependencies = visited.Dependencies,
                                Parameter = visited.Parameter,
                                Value = UnitConvert.Parse(roleType.ObjectType.Id, visited.Value),
                            };

                            this.predicates.Push(lessThan);

                            break;

                        case PredicateKind.Like:

                            var like = new Data.Like(roleType)
                            {
                                Dependencies = visited.Dependencies,
                                Parameter = visited.Parameter,
                                Value = UnitConvert.Parse(roleType.ObjectType.Id, visited.Value)?.ToString(),
                            };

                            this.predicates.Push(like);

                            break;

                        default:
                            throw new Exception("Unknown predicate kind " + visited.Kind);
                    }

                    break;
            }
        }

        public void VisitPull(Pull visited)
        {
            var pull = new Data.Pull
            {
                ExtentRef = visited.ExtentRef,
                ObjectType = visited.ObjectType.HasValue ? (IObjectType)this.session.Database.MetaPopulation.Find(visited.ObjectType.Value) : null,
                Object = visited.Object != null ? this.session.Instantiate(visited.Object) : null,
                Parameters = visited.Parameters,
            };

            if (visited.Extent != null)
            {
                visited.Extent.Accept(this);
                pull.Extent = this.extents.Pop();
            }

            if (visited.Results?.Length > 0)
            {
                var length = visited.Results.Length;

                pull.Results = new Data.Result[length];
                for (var i = 0; i < length; i++)
                {
                    var result = visited.Results[i];
                    result.Accept(this);
                    pull.Results[i] = this.results.Pop();
                }
            }

            this.Pull = pull;
        }

        public void VisitResult(Result visited)
        {
            var result = new Allors.Data.Result
            {
                FetchRef = visited.FetchRef,
                Name = visited.Name,
                Skip = visited.Skip,
                Take = visited.Take,
            };

            if (visited.Fetch != null)
            {
                visited.Fetch.Accept(this);
                result.Fetch = this.fetches.Pop();
            }

            this.results.Push(result);
        }

        public void VisitSort(Sort visited)
        {
            var sort = new Data.Sort
            {
                Descending = visited.Descending,
                RoleType = visited.RoleType != null ? (IRoleType)this.session.Database.ObjectFactory.MetaPopulation.Find(visited.RoleType.Value) : null,
            };

            this.sorts.Push(sort);
        }

        public void VisitStep(Step visited)
        {
            var propertyType = (IPropertyType)this.metaPopulation.FindAssociationType(visited.AssociationType) ?? this.metaPopulation.FindRoleType(visited.RoleType);

            var step = new Allors.Data.Step
            {
                PropertyType = propertyType,
            };

            this.steps.Push(step);

            if (visited.Next != null)
            {
                visited.Next.Accept(this);
                step.Next = this.steps.Pop();
            }

            if (visited.Include?.Length > 0)
            {
                step.Include = new Data.Node[visited.Include.Length];
                for (var i = 0; i < visited.Include.Length; i++)
                {
                    visited.Include[i].Accept(this);
                    step.Include[i] = this.nodes.Pop();
                }
            }
        }
    }
}
