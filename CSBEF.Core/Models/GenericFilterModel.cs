using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models {
    public class GenericFilterModel<TDTO>
        where TDTO : class, IDTOModelBase, new () {
            public List<Expression<Func<TDTO, bool>>> WherePredicates { get; } = new List<Expression<Func<TDTO, bool>>> ();
            public List<GenericSortModel> OrderPredicates { get; } = new List<GenericSortModel> ();
            public int Page { get; set; }
            public int PageSize { get; set; }

            public GenericFilterModel () {
                WherePredicates = new List<Expression<Func<TDTO, bool>>> ();
                OrderPredicates = new List<GenericSortModel> ();
                Page = 0;
                PageSize = 0;
            }

            public void AddWherePredicate (Expression<Func<TDTO, bool>> predicate) {
                WherePredicates.Add (predicate);
            }

            public void AddOrderByPredicate (string propertyName, bool descending = false) {
                OrderPredicates.Clear ();
                OrderPredicates.Add (new GenericSortModel {
                    PropertyName = propertyName,
                        Descending = descending
                });
            }

            public GenericFilterModel<TDTO> SetOneWherePredicate (Expression<Func<TDTO, bool>> predicate) {
                AddWherePredicate (predicate);

                return this;
            }

            public GenericFilterModel<TDTO> SetOneWherePredicate (Expression<Func<TDTO, bool>> predicate, int pageSize) {
                AddWherePredicate (predicate);

                Page = 1;
                PageSize = pageSize;

                return this;
            }

            public GenericFilterModel<TDTO> SetOneWherePredicate (Expression<Func<TDTO, bool>> predicate, int page, int pageSize) {
                AddWherePredicate (predicate);

                Page = page;
                PageSize = pageSize;

                return this;
            }

            public GenericFilterModel<TDTO> SetOneWherePredicateWithOrderBy (Expression<Func<TDTO, bool>> predicate, string propertyName, bool descending = false) {
                AddWherePredicate (predicate);
                AddOrderByPredicate (propertyName, descending);

                return this;
            }

            public GenericFilterModel<TDTO> SetOneWherePredicateWithOrderBy (Expression<Func<TDTO, bool>> predicate, string propertyName, int pageSize, bool descending = false) {
                AddWherePredicate (predicate);
                AddOrderByPredicate (propertyName, descending);

                Page = 1;
                PageSize = pageSize;

                return this;
            }

            public GenericFilterModel<TDTO> SetOneWherePredicateWithOrderBy (Expression<Func<TDTO, bool>> predicate, string propertyName, int page, int pageSize, bool descending = false) {
                AddWherePredicate (predicate);
                AddOrderByPredicate (propertyName, descending);

                Page = page;
                PageSize = pageSize;

                return this;
            }
        }
}