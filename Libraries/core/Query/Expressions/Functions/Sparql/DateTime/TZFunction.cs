﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Expressions.Functions.Sparql.DateTime
{
    /// <summary>
    /// Represents the SPARQL TZ() Function
    /// </summary>
    public class TZFunction
        : BaseUnaryExpression
    {
        /// <summary>
        /// Creates a new SPARQL TZ() Function
        /// </summary>
        /// <param name="expr">Argument Expression</param>
        public TZFunction(ISparqlExpression expr)
            : base(expr) { }

        /// <summary>
        /// Gets the Timezone of the Argument Expression as evaluated for the given Binding in the given Context
        /// </summary>
        /// <param name="context">Evaluation Context</param>
        /// <param name="bindingID">Binding ID</param>
        /// <returns></returns>
        public override IValuedNode Evaluate(SparqlEvaluationContext context, int bindingID)
        {
            IValuedNode temp = this._expr.Evaluate(context, bindingID);
            if (temp != null)
            {
                DateTimeOffset dt = temp.AsDateTime();
                //Regex based check to see if the value has a Timezone component
                //If not then the result is a null
                if (!Regex.IsMatch(temp.AsString(), "(Z|[+-]\\d{2}:\\d{2})$")) return new StringNode(null, string.Empty);

                //Now we have a DateTime we can try and return the Timezone
                if (dt.Offset.Equals(TimeSpan.Zero))
                {
                    //If Zero it was specified as Z (which means UTC so zero offset)
                    return new StringNode(null, "Z");
                }
                else
                {
                    //If the Offset is outside the range -14 to 14 this is considered invalid
                    if (dt.Offset.Hours < -14 || dt.Offset.Hours > 14) return null;

                    //Otherwise it has an offset which is a given number of hours (and minutes)
                    return new StringNode(null, dt.Offset.Hours.ToString("00") + ":" + dt.Offset.Minutes.ToString("00"));
                }
            }
            else
            {
                throw new RdfQueryException("Unable to evaluate a Date Time function on a null argument");
            }
        }

        /// <summary>
        /// Gets the Type of this Expression
        /// </summary>
        public override SparqlExpressionType Type
        {
            get
            {
                return SparqlExpressionType.Function;
            }
        }

        /// <summary>
        /// Gets the Functor of this Expression
        /// </summary>
        public override string Functor
        {
            get
            {
                return SparqlSpecsHelper.SparqlKeywordTz;
            }
        }

        /// <summary>
        /// Gets the String representation of this Expression
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SparqlSpecsHelper.SparqlKeywordTz + "(" + this._expr.ToString() + ")";
        }

        /// <summary>
        /// Transforms the Expression using the given Transformer
        /// </summary>
        /// <param name="transformer">Expression Transformer</param>
        /// <returns></returns>
        public override ISparqlExpression Transform(IExpressionTransformer transformer)
        {
            return new TZFunction(transformer.Transform(this._expr));
        }
    }
}