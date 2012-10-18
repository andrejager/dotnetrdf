/*

Copyright dotNetRDF Project 2009-12
dotnetrdf-develop@lists.sf.net

------------------------------------------------------------------------

This file is part of dotNetRDF.

dotNetRDF is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

dotNetRDF is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with dotNetRDF.  If not, see <http://www.gnu.org/licenses/>.

------------------------------------------------------------------------

dotNetRDF may alternatively be used under the LGPL or MIT License

http://www.gnu.org/licenses/lgpl.html
http://www.opensource.org/licenses/mit-license.php

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF.Nodes;
using VDS.RDF.Query.Operators.Numeric;
using VDS.RDF.Query.Operators.DateTime;

namespace VDS.RDF.Query.Operators
{
    /// <summary>
    /// Registry of SPARQL Operators
    /// </summary>
    public static class SparqlOperators
    {
        private static Dictionary<SparqlOperatorType, List<ISparqlOperator>> _operators = new Dictionary<SparqlOperatorType, List<ISparqlOperator>>();
        private static bool _init = false;

#if SILVERLIGHT
        //Required under Silverlight as we can't just iterate over the enumeration values with Enum.GetValues()
        private static SparqlOperatorType[] _operatorTypes = new[]
                                                                 {
                                                                     SparqlOperatorType.Add, SparqlOperatorType.Subtract,
                                                                     SparqlOperatorType.Multiply, SparqlOperatorType.Divide
                                                                 };
#endif
        /// <summary>
        /// Initializes the Operators registry
        /// </summary>
        private static void Init()
        {
            if (_init) return;
            lock (_operators)
            {
                if (_init) return;

#if !SILVERLIGHT
                //Set up empty registry for each operator type
                foreach (SparqlOperatorType type in Enum.GetValues(typeof(SparqlOperatorType)).OfType<SparqlOperatorType>())
                {
                    _operators.Add(type, new List<ISparqlOperator>());
                }
#else
                foreach(SparqlOperatorType type in _operatorTypes)
                {
                    _operators.Add(type, new List<ISparqlOperator>());
                }
#endif
                //Register default operators
                //Numerics
                _operators[SparqlOperatorType.Add].Add(new AdditionOperator());
                _operators[SparqlOperatorType.Subtract].Add(new SubtractionOperator());
                _operators[SparqlOperatorType.Divide].Add(new DivisionOperator());
                _operators[SparqlOperatorType.Multiply].Add(new MultiplicationOperator());
                //Date Time
                _operators[SparqlOperatorType.Add].Add(new DateTimeAddition());
                _operators[SparqlOperatorType.Subtract].Add(new DateTimeSubtraction());
                //Time Span
                _operators[SparqlOperatorType.Add].Add(new TimeSpanAddition());
                _operators[SparqlOperatorType.Subtract].Add(new TimeSpanSubtraction());

                _init = true;
            }
        }

        /// <summary>
        /// Registers a new operator
        /// </summary>
        /// <param name="op">Operator</param>
        public static void AddOperator(ISparqlOperator op)
        {
            if (!_init) Init();
            lock (_operators)
            {
                _operators[op.Operator].Add(op);
            }
        }

        /// <summary>
        /// Removes the registration of an operator by instance reference
        /// </summary>
        /// <param name="op">Operator Reference</param>
        public static void RemoveOperator(ISparqlOperator op)
        {
            if (!_init) Init();
            lock (_operators)
            {
                _operators[op.Operator].Remove(op);
            }
        }

        /// <summary>
        /// Removes the registration of an operator by instance type of the operator
        /// </summary>
        /// <param name="op">Operator</param>
        public static void RemoveOperatorByType(ISparqlOperator op)
        {
            if (!_init) Init();
            lock (_operators)
            {
                _operators[op.Operator].RemoveAll(o => op.GetType().Equals(o.GetType()));
            }
        }

        /// <summary>
        /// Resets Operator registry to default state
        /// </summary>
        public static void Reset()
        {
            if (_init)
            {
                lock (_operators)
                {
                    _init = false;
                    _operators = new Dictionary<SparqlOperatorType, List<ISparqlOperator>>();
                    Init();
                }
            }
        }

        /// <summary>
        /// Returns whether the given operator is registered
        /// </summary>
        /// <param name="op">Operator</param>
        /// <returns></returns>
        /// <remarks>
        /// Checking is done both by reference and instance type so you can check if an operator is registered even if you don't have the actual reference to the instance that registered
        /// </remarks>
        public static bool IsRegistered(ISparqlOperator op)
        {
            if (!_init) Init();
            lock (_operators)
            {
                return _operators[op.Operator].Contains(op) || _operators[op.Operator].Any(o => op.GetType().Equals(o.GetType()));
            }
        }

        /// <summary>
        /// Gets all registered Operators
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ISparqlOperator> GetOperators()
        {
            if (!_init) Init();
            lock (_operators)
            {
                return (from type in _operators.Keys
                        from op in _operators[type]
                        select op).ToList();
            }
        }

        /// <summary>
        /// Gets all registered operators for the given Operator Type
        /// </summary>
        /// <param name="type">Operator Type</param>
        /// <returns></returns>
        public static IEnumerable<ISparqlOperator> GetOperators(SparqlOperatorType type)
        {
            if (!_init) Init();
            lock (_operators)
            {
                return _operators[type].ToList();
            }
        }

        /// <summary>
        /// Tries to return the operator which applies for the given inputs
        /// </summary>
        /// <param name="type">Operator Type</param>
        /// <param name="op">Operator</param>
        /// <param name="ns">Inputs</param>
        /// <returns></returns>
        public static bool TryGetOperator(SparqlOperatorType type, out ISparqlOperator op, params IValuedNode[] ns)
        {
            if (!_init) Init();

            op = null;
            List<ISparqlOperator> ops;
            lock (_operators)
            {
                if (_operators.TryGetValue(type, out ops))
                {
                    foreach (ISparqlOperator possOp in ops)
                    {
                        if (possOp.IsApplicable(ns))
                        {
                            op = possOp;
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
